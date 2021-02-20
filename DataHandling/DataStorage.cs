using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ARK_Invest_Bot
{
    public static class DataStorage
    {
        public static void SaveGuildChannelData(IEnumerable<GuildChannel> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<GuildChannel> LoadGuildChannelData(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<GuildChannel>>(json);
        }

        public static bool SaveExists(string filePath) => File.Exists(filePath);
    }
}