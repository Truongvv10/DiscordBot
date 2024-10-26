using DiscordBot.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Exceptions;

namespace DiscordBot.Manager
{
    public class ConfigManager {

        #region Properties
        private string configFolder = Path.Combine(Environment.CurrentDirectory, "Saves", "Config.json");
        #endregion

        #region Constructors

        public ConfigManager() {
        }
        #endregion


        #region Method
        public async Task<BotConfig> GetDiscordBotConfig() {
            try {
                using (StreamReader sr = new StreamReader(configFolder)) {
                    string text = await sr.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<BotConfig>(text)!;
                }

            } catch (Exception ex) {
                throw new ManagerException($"JsonReaderUtil.ReadConfigFile: \"Config.json\"", ex);
            }
        }
        #endregion
    }
}
