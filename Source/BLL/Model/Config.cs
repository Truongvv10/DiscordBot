using BLL.Exceptions;
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
        private string prefix;
        private bool hasAutoReconnect;
        private bool hasEnableMentionPrefix;
        private bool hasEnableDms;
        private bool hasEnableDefaultHelp;
        private bool logUnknownEvents;
        private ulong? logChannel;
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
        }
        public Config(string token, string prefix = "!", bool autoReconnect = true, bool enableMentionPrefix = true, bool enableDms = false, bool enableDefaultHelp = false, bool logUnknownEvents = false) {
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
        [Key]
        [Column("ServerId")]
        public ulong ServerId {
            get;
            set;
        }

        [Column("BotToken")]
        public string? Token {
            get => token;
            set => token = value;
        }

        [MaxLength(1)]
        [Column("CommandPrefix")]
        public string Prefix {
            get => prefix;
            set {
                if (string.IsNullOrWhiteSpace(value)) throw new ConfigException($"Config.Prefix: \"{value}\" can not be null or empty.");
                prefix = value;
            }
        }

        [Column("Reconnect")]
        public bool HasAutoReconnect {
            get => hasAutoReconnect;
            set => hasAutoReconnect = value;
        }

        [Column("MentionPrefix")]
        public bool HasEnableMentionPrefix {
            get => hasEnableMentionPrefix;
            set => hasEnableMentionPrefix = value;
        }

        [Column("Dms")]
        public bool HasEnableDms {
            get => hasEnableDms;
            set => hasEnableDms = value;
        }

        [Column("DefaultHelp")]
        public bool HasEnableDefaultHelp {
            get => hasEnableDefaultHelp;
            set => hasEnableDefaultHelp = value;
        }

        [Column("LogUnknownEvents")]
        public bool LogUnknownEvents {
            get => logUnknownEvents;
            set => logUnknownEvents = value;
        }

        [Column("LogChannel")]
        public ulong? LogChannel {
            get => logChannel;
            set => logChannel = value;
        }
    }
    #endregion
}
