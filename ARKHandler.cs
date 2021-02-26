using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace ARK_Invest_Bot
{
    /// <summary>
    /// Listen and process emails, and print them to Discord
    /// </summary>
    public class ARKHandler
    {
        private DiscordSocketClient _client;

        private HttpClient _http;

        public ARKHandler(IServiceProvider services)
        {
            // Initialize the email listener
            _ = new EmailListener(this);

            // Get a connection to our client
            _client = services.GetRequiredService<DiscordSocketClient>();

            // Initialize HTTP Client
            _http = new HttpClient();
        }

        // Process the email and send it to everyone
        public async Task ProcessTrades(string email)
        {
            // Create the image
            ImageGenerator.MakeImage(ReadTrades(email));

            // Create the embed
            var embed = new EmbedBuilder()
                .WithColor(EmbedUtils.ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName($"ARK Trading Information for {DateTime.Now:MM/dd}")
                    .WithIconUrl(EmbedUtils.Logo))
                .WithImageUrl("attachment://ark.png")
                .WithFooter("Via ARK Trade Notifications")
                .Build();

            // Send it to everyone
            foreach (var guildChannel in DataStorage.LoadGuildChannelData(GuildChannels.guildChannelsFile))
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

            // Delete the local image
            File.Delete("ark.png");
        }

        // Convert ARK's email into a list of trade objects
        public static List<Trade> ReadTrades(string email)
        {
            // Scrape the email
            var trades = new List<Trade>();

            var date = DateTime.Now.ToString("MM/dd");

            email = email.CutBeforeAndAfter("</tr>", "</table>");

            do
            {
                email = email.CutBefore("<td>").CutBefore("<td>");

                var fundName = email.CutAfter("</td>");

                email = email.CutBefore("<td>").CutBefore("<td>");
                var direction = email.CutAfter("</td>");

                email = email.CutBefore("<td>");
                var ticker = email.CutAfter("</td>");

                email = email.CutBefore("<td>").CutBefore("<td>").CutBefore("align='right'>");
                var shares = email.CutAfter("</td>");

                email = email.CutBefore("align='right'>");
                var percentOfEtf = email.CutAfter("</td>");

                trades.Add(new Trade
                {
                    Fund = fundName,
                    Date = date,
                    Direction = direction,
                    Ticker = ticker,
                    Shares = shares,
                    PercentOfEtf = percentOfEtf
                });
            } while (email.Contains("<td>"));

            return trades;
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