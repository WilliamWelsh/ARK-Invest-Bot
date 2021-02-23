using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace ARK_Invest_Bot
{
    public static class EmbedUtils
    {
        // ARK Invest Logo (Bot's profile picture)
        public static string Logo = "https://cdn.discordapp.com/avatars/811803089853874226/f09d3ba474b1956ee3768d9ff5b6a564.png?size=64";

        // ARK's Purple-ish Color
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
                        .WithName("Bot Information")
                        .WithValue("!ark info"),
                    new EmbedFieldBuilder()
                        .WithName("Links")
                        .WithValue("[Invite](https://discord.com/api/oauth2/authorize?client_id=811803089853874226&permissions=117760&scope=bot) | [GitHub](https://github.com/WilliamWelsh/ARK-Invest-Bot) | [Support Server](https://discord.com/invite/gzhdfGC2as)")
                })
                .Build());

        // Print the no permissions message
        public static async Task PrintNoPermissionsMessage(this SocketUser user) =>
            await user.SendMessageAsync(null, false, new EmbedBuilder()
                .WithColor(ARKColor)
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(Logo)
                    .WithName("ARK Commands"))
                .WithDescription("I don't have permissions to send messages in that channel. Please give me these following permissions. If you need more support, join the support server here: https://discord.com/invite/gzhdfGC2as")
                .WithFields(new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder()
                        .WithName("Permissions Needed")
                        .WithValue("View Channel\nSend Messages\nEmbed Links\nAttach Files\nRead Message History")
                })
                .Build());
    }
}