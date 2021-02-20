using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace ARK_Invest_Bot
{
    internal class Program
    {
        private static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;

                // Login
                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("discordToken"));
                await client.SetGameAsync("!ark", null, ActivityType.Watching);
                await client.StartAsync();

                // Register commands
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                // Set up ARK Handler
                services.GetRequiredService<ARKHandler>();

                // Run forever
                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<ARKHandler>()
                .BuildServiceProvider();
        }
    }
}