using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
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

        [Command("ark unsubscribe")]
        public async Task Unsubscribe()
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

        [Command("ark")]
        public async Task PrintHelp()
        {
        }
    }
}