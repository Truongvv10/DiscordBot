using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XironiteDiscordBot.Exceptions;

namespace XironiteDiscordBot.Model {
    public class Config {

        #region Private Properties
        private string token;
        private string prefix;
        private bool hasAutoReconnect;
        private bool hasEnableMentionPrefix;
        private bool hasEnableDms;
        private bool hasEnableDefaultHelp;
        private ulong? logChannel;
        #endregion

        #region Constructors
        public Config() {
            Token = "Your Discord Token";
            Prefix = "!";
            HasAutoReconnect = true;
            HasEnableMentionPrefix = true;
            HasEnableDms = true;
            HasEnableDefaultHelp = true;
            logChannel = null;
        }
        public Config(string token, string prefix, bool autoReconnect, bool enableMentionPrefix, bool enableDms, bool enableDefaultHelp) {
            Token = token;
            Prefix = prefix;
            HasAutoReconnect = autoReconnect;
            HasEnableMentionPrefix = enableMentionPrefix;
            HasEnableDms = enableDms;
            HasEnableDefaultHelp = enableDefaultHelp;
        }
        #endregion

        #region Getter & Setter
        public string Token {
            get => token;
            set {
                if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"Config.Token: \"{value}\" can not be null or empty.");
                token = value;
            }
        }

        public string Prefix {
            get => prefix;
            set {
                if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"Config.Prefix: \"{value}\" can not be null or empty.");
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

        public ulong? LogChannel {
            get => logChannel;
            set => logChannel = value;
        }
        #endregion
    }
}
