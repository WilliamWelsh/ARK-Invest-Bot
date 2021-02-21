using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace ARK_Invest_Bot
{
    public static class EmbedUtils
    {
        public static string Logo = "https://cdn.discordapp.com/avatars/811803089853874226/f09d3ba474b1956ee3768d9ff5b6a564.png?size=64";
        public static Color ARKColor = new Color(131, 100, 255);

        // Print a success message
        public static async Task PrintSuccess(this ISocketMessageChannel channel, string message) =>
            await channel.SendMessageAsync(null, false, new EmbedBuilder()
                .WithColor(ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(Logo)
                    .WithName("Success"))
                .WithDescription(message)
                .Build());

        // Print an error message
        public static async Task PrintError(this ISocketMessageChannel channel, string message) =>
            await channel.SendMessageAsync(null, false, new EmbedBuilder()
                .WithColor(ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(Logo)
                    .WithName("Error"))
                .WithDescription(message)
                .Build());

        // Print available commands
        public static async Task PrintHelp(this ISocketMessageChannel channel) =>
            await channel.SendMessageAsync(null, false, new EmbedBuilder()
                .WithColor(ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(Logo)
                    .WithName("ARK Commands"))
                .WithFields(new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder()
                        .WithName("Subscribe a channel to notifications")
                        .WithValue("!ark subscribe"),
                    new EmbedFieldBuilder()
                        .WithName("Unsubscribe a channel from notifications")
                        .WithValue("!ark unsubscribe"),
                    new EmbedFieldBuilder()
                        .WithName("Suggested Role Color")
                        .WithValue("#8364ff"),
                    new EmbedFieldBuilder()
                        .WithName("Links")
                        .WithValue("[Invite](https://discord.com/oauth2/authorize?client_id=811803089853874226&permissions=68608&scope=bot) | [GitHub](https://github.com/WilliamWelsh/ARK-Invest-Bot) | [Support Server](https://discord.com/invite/gzhdfGC2as)")
                })
                .Build());
    }
}