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
        [Alias("ark ping")]
        [Command("ping")]
        public Task PingAsync() => ReplyAsync("Pong!");

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
                    await Context.User.SendMessageAsync("I don't have permissions to send messages in that channel. Please give me that permission. If you need more support, join the support server here: https://discord.com/invite/gzhdfGC2as");
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
                    var data = DataStorage.LoadGuildChannelData(GuildChannels.guildChannelsFile);
                    var modifiedData = data.ToList();

                    if (modifiedData.Any(x => x.GuildID == Context.Guild.Id))
                    {
                        modifiedData.Remove(modifiedData.First(x => x.GuildID == Context.Guild.Id));

                        DataStorage.SaveGuildChannelData(modifiedData, GuildChannels.guildChannelsFile);
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
                    await Context.User.SendMessageAsync("I don't have permissions to send messages in that channel. Please give me that permission. If you need more support, join the support server here: https://discord.com/invite/gzhdfGC2as");
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
                        .WithName("Library")
                        .WithValue("Discord.Net"),
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
    }
}