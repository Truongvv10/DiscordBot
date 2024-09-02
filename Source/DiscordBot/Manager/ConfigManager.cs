using DiscordBot.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XironiteDiscordBot.Exceptions;

namespace XironiteDiscordBot.Manager
{
    public class ConfigManager {

        #region Properties
        private string configFolder = Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory)!.FullName)!.FullName)!.FullName + @"\Saves\Config.json";
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
