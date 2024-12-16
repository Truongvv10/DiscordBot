using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Model {
    public class Settings {

        #region Fields
        private ulong? welcomeChannel;
        private ulong? introductionChannel;
        private ulong? logChannel;
        private ulong? changelogChannel;
        private ulong? inactivityChannel;
        private ulong? punishmentChannel;
        private ulong? strikeChannel;
        private ulong? verifyChannel;
        #endregion

        #region Constructors
        public Settings() { }
        public Settings(ulong guildId) {
            GuildId = guildId;
        }
        #endregion

        #region Properties
        [Column("guild_id", TypeName = "decimal(20, 0)")]
        [JsonProperty("guild")]
        public ulong GuildId { get; set; }

        [Column("welcome_channel", TypeName = "decimal(20, 0)")]
        [JsonProperty("welcome_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? WelcomeChannel {
            get => welcomeChannel;
            set => welcomeChannel = value;
        }

        [Column("introduction_channel", TypeName = "decimal(20, 0)")]
        [JsonProperty("introduction_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? IntroductionChannel {
            get => introductionChannel;
            set => introductionChannel = value;
        }

        [Column("log_channel", TypeName = "decimal(20, 0)")]
        [JsonProperty("log_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? LogChannel {
            get => logChannel;
            set => logChannel = value;
        }

        [Column("changelog_channel", TypeName = "decimal(20, 0)")]
        [JsonProperty("changelog_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ChangelogChannel {
            get => changelogChannel;
            set => changelogChannel = value;
        }

        [Column("inactivity_channel", TypeName = "decimal(20, 0)")]
        [JsonProperty("inactivity_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? InactivityChannel {
            get => inactivityChannel;
            set => inactivityChannel = value;
        }

        [Column("punishment_channel", TypeName = "decimal(20, 0)")]
        [JsonProperty("punishment_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? PunishmentChannel {
            get => punishmentChannel;
            set => punishmentChannel = value;
        }

        [Column("strike_channel", TypeName = "decimal(20, 0)")]
        [JsonProperty("strike_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? StrikeChannel {
            get => strikeChannel;
            set => strikeChannel = value;
        }

        [Column("verify_channel", TypeName = "decimal(20, 0)")]
        [JsonProperty("verify_channel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? VerifyChannel {
            get => verifyChannel;
            set => verifyChannel = value;
        }
        #endregion
    }
}
