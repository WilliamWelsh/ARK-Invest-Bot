﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace ARK_Invest_Bot
{
    public class ARKHandler
    {
        private DiscordSocketClient _client;

        public ARKHandler(IServiceProvider services)
        {
            // Initialize the email listener
            _ = new EmailListener(this);

            // Get a connection to our client
            _client = services.GetRequiredService<DiscordSocketClient>();
        }

        // Process the email and send it to everyone
        public async Task ProcessTrades(string email)
        {
            // Create the image
            ImageGenerator.MakeImage(ReadTrades(email));

            // Get the guild channel data and start sending this to everyone
            var guildChannels = DataStorage.LoadGuildChannelData(GuildChannels.guildChannelsFile);

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
            foreach (var guildChannel in guildChannels)
            {
                await _client.GetGuild(guildChannel.GuildID).GetTextChannel(guildChannel.ChannelID).SendFileAsync("ark.png", null, embed: embed);
            }

            // Delete the local image
            File.Delete("ark.png");
        }

        public List<Trade> ReadTrades(string email)
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
    }
}