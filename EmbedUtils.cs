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

        public static async Task PrintSuccess(this ISocketMessageChannel channel, string message) =>
            await channel.SendMessageAsync(null, false, new EmbedBuilder()
                .WithColor(ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(Logo)
                    .WithName("Success"))
                .WithDescription(message)
                .Build());

        public static async Task PrintError(this ISocketMessageChannel channel, string message) =>
            await channel.SendMessageAsync(null, false, new EmbedBuilder()
                .WithColor(ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(Logo)
                    .WithName("Error"))
                .WithDescription(message)
                .Build());
    }
}