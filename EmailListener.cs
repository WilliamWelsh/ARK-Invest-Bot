using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;
using System.Timers;

namespace ARK_Invest_Bot
{
    /// <summary>
    /// Listen for ARK's daily emails
    /// </summary>
    public class EmailListener
    {
        private GmailService GmailService;

        private ARKHandler _arkHandler;

        public EmailListener(ARKHandler arkHandler)
        {
            _arkHandler = arkHandler;

            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                var credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,

                    // We need the scope to trash email, and read email
                    new[] { GmailService.Scope.MailGoogleCom },

                    "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
            }

            // Create Gmail API service.
            GmailService = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "ARK Invest Bot",
            });

            // Set up a timer that will automatically check emails every 2 minutes
            var levelTimer = new System.Timers.Timer
            {
                Interval = 120000,
                AutoReset = true,
                Enabled = true
            };
            levelTimer.Elapsed += OnTimerElapsedAsync;
        }

        // Call email checker when the timer elapses
        private async void OnTimerElapsedAsync(object sender, ElapsedEventArgs e) => await CheckForArkEmail();

        // Decode email body
        public static string Base64UrlDecode(string input)
        {
            return string.IsNullOrWhiteSpace(input) ?
                "Message body was not returned from Google" :
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(input.Replace("-", "+").Replace("_", "/")));
        }

        // Read ARK email
        public async Task CheckForArkEmail()
        {
            Console.WriteLine($"[{DateTime.Now:H:mm:ss}] Checking email...");

            // Define parameters of request.
            var request = GmailService.Users.Messages.List("me");
            request.Q = "from:ark@ark-funds.com in:inbox";

            // Get
            var messages = request.Execute().Messages;

            // Null = 0 messages
            if (messages == null || messages.Count == 0)
                return;

            // Get the message
            var message = GmailService.Users.Messages.Get("me", messages.ElementAt(0).Id).Execute();

            // If it's in the trash bin, ignore it
            if (message.LabelIds.Any(label => label == "TRASH"))
                return;

            // Get the body
            var result = Base64UrlDecode(message.Payload.Body.Data);

            // Trash the message
            GmailService.Users.Messages.Trash("me", messages.ElementAt(0).Id).Execute();

            // Process the email's body and send it to everyone
            await _arkHandler.ProcessTrades(result);
        }
    }
}