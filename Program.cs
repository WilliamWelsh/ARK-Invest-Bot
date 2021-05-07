using System;
using System.Linq;
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
                await client.LoginAsync(TokenType.Bot, Config.DiscordBotToken);
                await client.SetGameAsync("!ark help", null, ActivityType.Watching);
                await client.StartAsync();

                client.GuildAvailable += OnGuildAvailable;

                client.LeftGuild += OnGuildLeft;

                // Register commands
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                // Set up ARK Handler
                services.GetRequiredService<ARKHandler>();

                // Run forever
                await Task.Delay(Timeout.Infinite);
            }
        }

        // Remove a server from our Guild Channel data if someone removes us
        private Task OnGuildLeft(SocketGuild guild)
        {
            var data = DataStorage.LoadGuildChannelData(Config.GuildChannelsFile);
            var modifiedData = data.ToList();

            if (modifiedData.Any(x => x.GuildID == guild.Id))
            {
                modifiedData.Remove(modifiedData.First(x => x.GuildID == guild.Id));

                DataStorage.SaveGuildChannelData(modifiedData, Config.GuildChannelsFile);
                Console.WriteLine($"Left {guild.Name}. They were subsribed, so I removed them.");
            }
            else
            {
                Console.WriteLine($"Left {guild.Name}. They were not subscribed.");
            }

            return Task.CompletedTask;
        }

        // Send a message when we join a guild
        private Task OnGuildAvailable(SocketGuild guild)
        {
            Console.WriteLine($"Connected to {guild.Name}");
            return Task.CompletedTask;
        }

        // Log
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // Configure services
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