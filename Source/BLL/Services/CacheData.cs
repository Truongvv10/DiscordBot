using BLL.Exceptions;
using BLL.Interfaces;
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
    public class CacheData : ICacheData {
        #region Properties
        private string folder = Path.Combine(Environment.CurrentDirectory, "Saves");
        private Dictionary<(ulong, ulong), Message> messages = new();
        private Dictionary<ulong, Settings> settings = new();
        private Dictionary<string, Message> templates = new();
        private Dictionary<(ulong, ulong), Message> modalData = new();
        private List<string> timeZones = new();
        #endregion

        #region Constructors
        public CacheData() { }
        #endregion

        #region Getter & Setter
        public IReadOnlyDictionary<(ulong, ulong), Message> Messages {
            get => messages;
        }
        public IReadOnlyDictionary<ulong, Settings> Settings {
            get => settings;
        }
        public IReadOnlyDictionary<string, Message> Templates {
            get => templates;
        }
        public IReadOnlyDictionary<(ulong, ulong), Message> ModalData {
            get => modalData;
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
                if (!AnyMessage(guildId, message.MessageId)) {
                    messages.Add((guildId, message.MessageId), message);
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
                if (AnyMessage(guildId, message.MessageId)) {
                    messages[(guildId, message.MessageId)] = message;
                    return messages[(guildId, message.MessageId)];
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

        #region Modal Data
        public Message GetModalData(ulong guildId, ulong userId) {
            if (modalData.ContainsKey((guildId, userId))) {
                return modalData[(guildId, userId)];
            } else throw new UtilException($"Modal data on guild \"{guildId}\" and user \"{userId}\" doesn't exists in cache.");
        }
        public bool AddModalData(ulong guildId, ulong userId, Message message) {
            if (!modalData.ContainsKey((guildId, userId))) {
                modalData.Add((guildId, userId), message);
                return true;
            } else {
                return SetModalData(guildId, userId, message);
            }
        }
        public bool SetModalData(ulong guildId, ulong userId, Message message) {
            if (modalData.ContainsKey((guildId, userId))) {
                modalData[(guildId, userId)] = message;
                return true;
            } else {
                return AddModalData(guildId, userId, message);
            }
        }
        #endregion

        #region Settings
        public bool AnySettings(ulong guildId) {
            return settings.ContainsKey(guildId);
        }

        public void AddSettings(ulong guildId, Settings settings) {
            if (settings != null) {
                if (!AnySettings(guildId)) {
                    this.settings.Add(guildId, settings);
                } else throw new UtilException($"Settings for guild \"{guildId}\" already exists in cache.");
            } else throw new UtilException($"Settings can not be null");
        }

        public Settings GetSettings(ulong guildId) {
            if (AnySettings(guildId)) {
                return settings[guildId];
            } else throw new UtilException($"Settings for guild \"{guildId}\" doesn't exists in cache.");
        }

        public Settings UpdateSettings(ulong guildId, Settings settings) {
            if (settings != null) {
                if (AnySettings(guildId)) {
                    this.settings[guildId] = settings;
                    return this.settings[guildId];
                } else throw new UtilException($"Settings for guild \"{guildId}\" doesn't exists in cache.");
            } else throw new UtilException($"Settings can not be null");
        }

        public void DeleteSettings(ulong guildId) {
            if (AnySettings(guildId)) {
                settings.Remove(guildId);
            } else throw new UtilException($"Settings for guild \"{guildId}\" doesn't exists in cache.");
        }
        #endregion
    }
}