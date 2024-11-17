using BLL.Exceptions;
using BLL.Model;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services {
    public class CacheData {
        #region Properties
        private string folder = Path.Combine(Environment.CurrentDirectory, "Saves");
        private Dictionary<(ulong, ulong), Message> messages = new();
        private Dictionary<string, Message> templates = new();
        private List<string> timeZones = new();
        #endregion

        #region Constructors
        public CacheData() { }
        #endregion

        #region Getter & Setter
        public Dictionary<(ulong, ulong), Message> Messages {
            get => messages;
            set => messages = value;
        }
        public IReadOnlyDictionary<string, Message> Templates {
            get => templates;
        }
        public IReadOnlyCollection<string> TimeZones {
            get => timeZones;
        }
        #endregion

        #region Methods
        public async Task LoadTemplates() {
            try {
                var file = Path.Combine(folder, "Templates.json");
                if (File.Exists(file)) {
                    using (StreamReader sr = new StreamReader(file)) {
                        var data = await sr.ReadToEndAsync();
                        var json = JsonConvert.DeserializeObject<Dictionary<string, Message>>(data);
                        foreach (var template in json) {
                            templates.Add(template.Key, template.Value);
                        }
                    }
                }
            } catch (Exception ex) {
                throw new UtilException($"There has been an error loading all templates.", ex);
            }
        }
        public void LoadTimeZones() {
            foreach (var zone in DateTimeZoneProviders.Tzdb.GetAllZones()) {
                timeZones.Add(zone.Id);
            }
        }
        #endregion

        #region Messages
        public bool AnyMessage(ulong guildId, ulong messageId) {
            return messages.ContainsKey((guildId, messageId));
        }
        public void AddMessage(ulong guildId, Message message) {
            if (message != null && message.MessageId != null) {
                if (!AnyMessage(guildId, (ulong)message.MessageId)) {
                    messages.Add((guildId, (ulong)message.MessageId), message);
                } else throw new UtilException($"Message with id \"{message.MessageId}\" in guild \"{guildId}\" already exists in cache.");
            } else throw new UtilException($"Message or id can not be null");
        }
        public Message GetMessage(ulong guildId, ulong messageId) {
            if (AnyMessage(guildId, messageId)) {
                return messages[(guildId, messageId)];
            } else throw new UtilException($"Message with id \"{messageId}\" in guild \"{guildId}\" doesn't exists in cache.");
        }
        public Message UpdateMessage(ulong guildId, Message message) {
            if (message != null && message.MessageId != null) {
                if (AnyMessage(guildId, (ulong)message.MessageId)) {
                    messages[(guildId, (ulong)message.MessageId)] = message;
                    return messages[(guildId, (ulong)message.MessageId)];
                } else throw new UtilException($"Message with id \"{message.MessageId}\" in guild \"{guildId}\" doesn't exists in cache.");
            } else throw new UtilException($"Message or id can not be null");
        }
        public void DeleteMessage(ulong guildId, ulong messageId) {
            if (AnyMessage(guildId, messageId)) {
                messages.Remove((guildId, messageId));
            } else throw new UtilException($"Message with id \"{messageId}\" in guild \"{guildId}\" doesn't exists in cache.");
        }
        #endregion

        #region Templates
        public bool AnyTemplate(string name) {
            return templates.ContainsKey(name);
        }
        public Template GetTemplate(string name) {
            if (AnyTemplate(name)) {
                var message = templates[name].DeepClone();
                return new Template(name, message);
            } else throw new UtilException($"Template with name \"{name}\" doesn't exists in cache.");
        }
        #endregion
    }
}