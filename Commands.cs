using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;

namespace ARK_Invest_Bot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public ARKHandler ARKHandler { get; set; }

        [Alias("ark ping")]
        [Command("ping")]
        public async Task PingAsync() => await Context.Message.ReplyAsync("Pong!");

        [Command("ark subscribe")]
        public async Task Subscribe()
        {
            try
            {
                var user = (SocketGuildUser)Context.User;
                if (user.GuildPermissions.Administrator)
                {
                    GuildChannels.GetGuildChannelData(Context.Guild.Id, Context.Channel.Id);
                    await Context.Channel.PrintSuccess(
                        $"{((ITextChannel)Context.Channel).Mention} is subscribed to ARK's trade notifications. One server may only be subscribed on one channel. You can unsubscribe by doing `!ark unsubscribe`");
                }
                else
                {
                    await Context.Channel.PrintError("Sorry, you must be an administrator to subscribe to ARK.");
                }
            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Forbidden)
                    await Context.User.PrintNoPermissionsMessage();
            }
        }

        [Command("ark unsubscribe")]
        public async Task Unsubscribe()
        {
            try
            {
                var user = (SocketGuildUser)Context.User;
                if (user.GuildPermissions.Administrator)
                {
                    var data = DataStorage.LoadGuildChannelData(Config.GuildChannelsFile);
                    var modifiedData = data.ToList();

                    if (modifiedData.Any(x => x.GuildID == Context.Guild.Id))
                    {
                        modifiedData.Remove(modifiedData.First(x => x.GuildID == Context.Guild.Id));

                        DataStorage.SaveGuildChannelData(modifiedData, Config.GuildChannelsFile);
                        await Context.Channel.PrintSuccess(
                            $"This server is no longer subscribed to ARK notifications. You can subscribe again by doing `!ark subscribe`");
                    }
                    else
                    {
                        await Context.Channel.PrintError("This server is not subscribed to ARK notifications. You can subscribe by doing `!ark subscribe`");
                    }
                }
                else
                {
                    await Context.Channel.PrintError("Sorry, you must be an administrator to unsubscribe to ARK.");
                }
            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Forbidden)
                    await Context.User.PrintNoPermissionsMessage();
            }
        }

        [Alias("ark help")]
        [Command("ark")]
        public async Task PrintHelp() => await Context.Channel.PrintHelp();

        [Command("ark info")]
        public async Task PrintInfo() =>
            await Context.Channel.SendMessageAsync(null, false, new EmbedBuilder()
                .WithColor(EmbedUtils.ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(EmbedUtils.Logo)
                    .WithName("ARK Invest Bot Info"))
                .WithFields(new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder()
                        .WithName("Commands")
                        .WithValue("!ark help"),
                    new EmbedFieldBuilder()
                        .WithName("Library")
                        .WithValue("Discord.Net v2.3.0-dev-20210121.1 (API v6)"),
                    new EmbedFieldBuilder()
                        .WithName("Servers")
                        .WithValue(Context.Client.Guilds.Count),
                    new EmbedFieldBuilder()
                        .WithName("Developer")
                        .WithValue("Reverse#0069"),
                    new EmbedFieldBuilder()
                        .WithName("Links")
                        .WithValue("[Invite](https://discord.com/oauth2/authorize?client_id=811803089853874226&permissions=68608&scope=bot) | [GitHub](https://github.com/WilliamWelsh/ARK-Invest-Bot) | [Support Server](https://discord.com/invite/gzhdfGC2as)")
                })
                .Build());

        [Command("ark announce")]
        public async Task Announce([Remainder] string message)
        {
            // This is ONLY FOR ME!! In case I want to announce something
            // Maybe ARK resent their email and messed something up, or a new feature
            if (Context.User.Id != 354458973572956160)
                return;

            // Get the guild channel data and start sending this to everyone
            var guildChannels = DataStorage.LoadGuildChannelData(Config.GuildChannelsFile);

            // Create the embed
            var embed = new EmbedBuilder()
                .WithColor(EmbedUtils.ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName("Announcement")
                    .WithIconUrl(EmbedUtils.Logo))
                .WithDescription(message)
                .Build();

            // Send it to everyone
            foreach (var guildChannel in guildChannels)
            {
                try
                {
                    var guild = Context.Client.GetGuild(guildChannel.GuildID);
                    await guild.GetTextChannel(guildChannel.ChannelID).SendMessageAsync(null, false, embed);
                    Console.WriteLine($"Successfully posted to {guild.Name}");
                }
                catch (HttpException e)
                {
                    if (e.HttpCode == HttpStatusCode.Forbidden)
                    {
                        // Oh well
                    }
                }
            }
        }
    }
}