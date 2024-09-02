using DiscordBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XironiteDiscordBot.Exceptions;

namespace DiscordBot.Configuration {
    public class NotionConfig {
        #region Private Properties
        private string _notionApiKey;
        private Dictionary<string, string> _databases = new();
        #endregion

        #region Constructors
        public NotionConfig() {

        }
        public NotionConfig(string notionApiKey) {
            _notionApiKey = notionApiKey;
        }
        #endregion

        #region Getter & Setter
        public string NotionApiKey {
            get => _notionApiKey;
            set => _notionApiKey = value;
        }

        public IReadOnlyDictionary<string, string> Databases {
            get => _databases;
            set { foreach (var item in value) AddDatabase(item.Key, item.Value); }
        }
        #endregion

        #region Methods
        public void AddDatabase(string name, string id) {
            if (!_databases.TryAdd(name, id)) {
                throw new ConfigException($"ID: \"{id}\" for \"{name}\" already exist!");
            }
        }
        public void RemoveDatabase(string name) {
            if (_databases.TryGetValue(name, out string dbName)) {
                _databases.Remove(dbName);
            } else throw new ConfigException($"No database with the name \"{name}\" has been found.");
        }
        public void RemoveDatabaseId(string id) {
            if (_databases.Any(x => x.Value == id)) {
                _databases.Remove(_databases.Where(x => x.Value == id).Select(x => x.Key).First());
            } else throw new ConfigException($"No database with the ID \"{id}\" has been found.");
        }
        #endregion
    }
}
