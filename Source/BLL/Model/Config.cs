using BLL.Enums;
using BLL.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Model {
    public class Config {

        #region Private Properties
        private string? token;
        private DatabaseSaveType databaseType;
        private string? connectionString;
        private string prefix;
        private bool hasAutoReconnect;
        private bool hasEnableMentionPrefix;
        private bool hasEnableDms;
        private bool hasEnableDefaultHelp;
        private bool logUnknownEvents;
        private ulong? logChannel;
        private ulong? changelogChannel;
        #endregion

        #region Constructors
        public Config() {
            Token = null;
            Prefix = "!";
            HasAutoReconnect = true;
            HasEnableMentionPrefix = true;
            HasEnableDms = true;
            HasEnableDefaultHelp = true;
            LogUnknownEvents = false;
            logChannel = null;
            changelogChannel = null;
        }
        public Config(string token, DatabaseSaveType databaseType, string prefix = "!", bool autoReconnect = true, bool enableMentionPrefix = true, bool enableDms = false, bool enableDefaultHelp = false, bool logUnknownEvents = false) {
            this.token = token;
            this.databaseType = databaseType;
            this.prefix = prefix;
            HasAutoReconnect = autoReconnect;
            HasEnableMentionPrefix = enableMentionPrefix;
            HasEnableDms = enableDms;
            LogUnknownEvents = logUnknownEvents;
            HasEnableDefaultHelp = enableDefaultHelp;
        }
        #endregion

        #region Getter & Setter
        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public string? Token {
            get => token;
            set => token = value;
        }

        [JsonProperty("database", NullValueHandling = NullValueHandling.Ignore)]
        public DatabaseSaveType DatabaseType {
            get => databaseType;
            set => databaseType = value;
        }

        [JsonProperty("connection", NullValueHandling = NullValueHandling.Ignore)]
        public string? ConnectionString {
            get => connectionString;
            set => connectionString = value;
        }

        public string Prefix {
            get => prefix;
            set {
                if (string.IsNullOrWhiteSpace(value)) throw new ConfigException($"Config.Prefix: \"{value}\" can not be null or empty.");
                prefix = value;
            }
        }

        [JsonProperty("has_reconnect", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasAutoReconnect {
            get => hasAutoReconnect;
            set => hasAutoReconnect = value;
        }

        [JsonProperty("has_prefix", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasEnableMentionPrefix {
            get => hasEnableMentionPrefix;
            set => hasEnableMentionPrefix = value;
        }

        [JsonProperty("has_dms", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasEnableDms {
            get => hasEnableDms;
            set => hasEnableDms = value;
        }

        [JsonProperty("has_default_help", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasEnableDefaultHelp {
            get => hasEnableDefaultHelp;
            set => hasEnableDefaultHelp = value;
        }

        [JsonProperty("log_unknown_events", NullValueHandling = NullValueHandling.Ignore)]
        public bool LogUnknownEvents {
            get => logUnknownEvents;
            set => logUnknownEvents = value;
        }

        [JsonProperty("log_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? LogChannel {
            get => logChannel;
            set => logChannel = value;
        }

        [JsonProperty("changelog_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ChangelogChannel {
            get => changelogChannel;
            set => changelogChannel = value;
        }
    }
    #endregion
}
