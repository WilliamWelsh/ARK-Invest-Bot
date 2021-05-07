using System;
using System.IO;
using Newtonsoft.Json;

namespace ARK_Invest_Bot
{
    public static class Config
    {
        // Token to log into Discord
        public static string DiscordBotToken;

        // Twitter Tokens
        public static string TwitterConsumerKey;
        public static string TwitterConsumerSecret;
        public static string TwitterAccessToken;
        public static string TwitterAccessTokenSecret;

        // Guild data file
        public static string GuildChannelsFile = "Resources/guild_channel_data.json";

        static Config()
        {
            // Create the resources folder if it doesn't exist
            if (!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");

            // Get our config (lazy)
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("Resources/config.json"));

            // Discord token
            DiscordBotToken = config.DiscordBotToken;

            // Twitter tokens
            TwitterConsumerKey = config.TwitterConsumerKey;
            TwitterConsumerSecret = config.TwitterConsumerSecret;
            TwitterAccessToken = config.TwitterAccessToken;
            TwitterAccessTokenSecret = config.TwitterAccessTokenSecret;
        }
    }
}