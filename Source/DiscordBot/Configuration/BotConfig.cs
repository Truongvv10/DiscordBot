using DiscordBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Exceptions;

namespace DiscordBot.Configuration {
    public class BotConfig {

        #region Private Properties
        private string token;
        private string prefix;
        private bool hasAutoReconnect;
        private bool hasEnableMentionPrefix;
        private bool hasEnableDms;
        private bool hasEnableDefaultHelp;
        private bool logUnknownEvents;
        private ulong? logChannel;
        #endregion

        #region Constructors
        public BotConfig() {
            Token = "Your Discord Token";
            Prefix = "!";
            HasAutoReconnect = true;
            HasEnableMentionPrefix = true;
            HasEnableDms = true;
            HasEnableDefaultHelp = true;
            LogUnknownEvents = false;
            logChannel = null;
        }
        public BotConfig(string token, string prefix, bool autoReconnect, bool enableMentionPrefix, bool enableDms, bool enableDefaultHelp, bool logUnknownEvents) {
            Token = token;
            Prefix = prefix;
            HasAutoReconnect = autoReconnect;
            HasEnableMentionPrefix = enableMentionPrefix;
            HasEnableDms = enableDms;
            LogUnknownEvents = logUnknownEvents;
            HasEnableDefaultHelp = enableDefaultHelp;
        }
        #endregion

        #region Getter & Setter
        public string Token {
            get => token;
            set {
                if (string.IsNullOrWhiteSpace(value)) throw new ConfigException($"Config.Token: \"{value}\" can not be null or empty.");
                token = value;
            }
        }

        public string Prefix {
            get => prefix;
            set {
                if (string.IsNullOrWhiteSpace(value)) throw new ConfigException($"Config.Prefix: \"{value}\" can not be null or empty.");
                prefix = value;
            }
        }

        public bool HasAutoReconnect {
            get => hasAutoReconnect;
            set => hasAutoReconnect = value;
        }

        public bool HasEnableMentionPrefix {
            get => hasEnableMentionPrefix;
            set => hasEnableMentionPrefix = value;
        }

        public bool HasEnableDms {
            get => hasEnableDms;
            set => hasEnableDms = value;
        }

        public bool HasEnableDefaultHelp {
            get => hasEnableDefaultHelp;
            set => hasEnableDefaultHelp = value;
        }

        public bool LogUnknownEvents {
            get => logUnknownEvents;
            set => logUnknownEvents = value;
        }

        public ulong? LogChannel {
            get => logChannel;
            set => logChannel = value;
        }
        #endregion
    }
}
