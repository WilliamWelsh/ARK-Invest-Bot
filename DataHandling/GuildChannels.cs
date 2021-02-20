using System.Linq;
using System.Collections.Generic;

namespace ARK_Invest_Bot
{
    public static class GuildChannels
    {
        private static List<GuildChannel> guildChannelData;

        public static string guildChannelsFile = "Resources/guild_channel_data.json";

        // Initializer
        static GuildChannels()
        {
            if (DataStorage.SaveExists(guildChannelsFile))
                guildChannelData = DataStorage.LoadGuildChannelData(guildChannelsFile).ToList();
            else
            {
                guildChannelData = new List<GuildChannel>();
                SaveGuildChannelData();
            }
        }

        public static void SaveGuildChannelData() => DataStorage.SaveGuildChannelData(guildChannelData, guildChannelsFile);

        public static GuildChannel GetGuildChannelData(ulong guildID, ulong channelID) => GetOrCreateGuildChannelData(guildID, channelID);

        private static GuildChannel GetOrCreateGuildChannelData(ulong guildID, ulong channelID)
        {
            guildChannelData = DataStorage.LoadGuildChannelData(guildChannelsFile).ToList();
            var result = from a in guildChannelData
                         where a.GuildID == guildID
                         select a;
            var account = result.FirstOrDefault();
            if (account == null) account = CreateGuildChannelData(guildID, channelID);
            return account;
        }

        private static GuildChannel CreateGuildChannelData(ulong guildID, ulong channelID)
        {
            var newData = new GuildChannel
            {
                GuildID = guildID,
                ChannelID = channelID
            };
            guildChannelData.Add(newData);
            SaveGuildChannelData();
            return newData;
        }
    }
}