using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Google.Apis.Gmail.v1.Data;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace ARK_Invest_Bot
{
    /// <summary>
    /// Listen and process emails, and print them to Discord
    /// </summary>
    public class ARKHandler
    {
        private DiscordSocketClient _client;

        private HttpClient _http;

        private TwitterClient _twitterClient;

        private FirefoxDriver _driver;

        public ARKHandler(IServiceProvider services)
        {
            // Get a connection to our client
            _client = services.GetRequiredService<DiscordSocketClient>();

            // Initialize HTTP Client
            _http = new HttpClient();

            // Initialize Twitter Client
            _twitterClient = new TwitterClient(Config.TwitterConsumerKey, Config.TwitterConsumerSecret, Config.TwitterAccessToken, Config.TwitterAccessTokenSecret);

            // Initialize the web driver
            var options = new FirefoxOptions();
            options.AddArgument("--headless");
            _driver = new FirefoxDriver(options);
            _driver.Manage().Window.Size = new Size(825, 2500);

            // Initialize the email listener
            _ = new EmailListener(this);
        }

        // Process the email and send it to everyone
        public async Task ProcessTrades()
        {
            // Take a screenshot of the email
            _driver.Navigate().GoToUrl("file:///C:/Users/Administrator/Desktop/ARK Bot/result.html");
            System.Threading.Thread.Sleep(10000);
            var ss = ((ITakesScreenshot)_driver).GetScreenshot();
            ss.SaveAsFile("ark.png");

            // Send it to Twitter
            var uploadedImage = await _twitterClient.Upload.UploadTweetImageAsync(File.ReadAllBytes("ark.png"));
            var tweetText = "(Click to enlarge)\nARK's Trading Information for Today";
            await _twitterClient.Tweets.PublishTweetAsync(new PublishTweetParameters(tweetText.Length <= 280 ? tweetText : $"(Click to enlarge)\nARK's Trading Information for {DateTime.Now:MM/dd}")
            {
                Medias = { uploadedImage }
            });

            // Send it to Discord
            // Create the embed
            var embed = new EmbedBuilder()
                .WithColor(EmbedUtils.ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName($"ARK's Trading Information for Today")
                    .WithIconUrl(EmbedUtils.Logo))
                .WithImageUrl("attachment://ark.png")
                .WithFooter("Via ARK Trade Notifications")
                .Build();

            // Send it to everyone
            foreach (var guildChannel in DataStorage.LoadGuildChannelData(Config.GuildChannelsFile))
            {
                try
                {
                    var guild = _client.GetGuild(guildChannel.GuildID);
                    var channel = guild.GetTextChannel(guildChannel.ChannelID);

                    if (guild != null && channel != null)
                    {
                        await guild.GetTextChannel(guildChannel.ChannelID).SendFileAsync("ark.png", null, embed: embed);
                        Console.WriteLine($"Successfully posted to {guild.Name}");
                    }
                }
                catch (HttpException e)
                {
                    if (e.HttpCode == HttpStatusCode.Forbidden)
                    {
                        // Oh well
                    }
                }
            }

            // Cleanup
            File.Delete("ark.png");
            File.Delete("result.html");
        }

        // Search for a ticker in ARK's holdings
        public async Task SearchForTicker(ISocketMessageChannel channel, string ticker)
        {
            var result = "";
            using (var response = await _http.GetAsync($"https://cathiesark.com/arkk-holdings-of-{ticker}"))
                result = await response.Content.ReadAsStringAsync();

            result = result.CutBeforeAndAfter("<meta property=\"og:image\" content=\"", "\"/>")
                .Replace("&amp;", "&");

            await channel.SendMessageAsync(result);
        }
    }
}