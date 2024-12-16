using BLL.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Model {
    public class Guild {

        #region Fields
        private ulong guildId;
        private Settings settings;
        private ICollection<Message>? messages;
        private ICollection<Template>? templates;
        #endregion

        #region Constructors
        public Guild() {
        }
        public Guild(ulong guildId) : this() {
            this.guildId = guildId;
            settings = new Settings(guildId);
        }
        #endregion

        #region Properties
        [Key]
        [Column("guild_id", TypeName = "decimal(20, 0)")]
        public ulong GuildId {
            get => guildId;
            set => guildId = value;
        }

        public Settings Settings {
            get => settings;
            set {
                if (value is null) throw new DomainException("Settings can not be null.");
                else settings = value;
            }
        }

        public ICollection<Message>? Messages {
            get => messages;
            set => messages = value;
        }

        public ICollection<Template>? Templates {
            get => templates;
            set => templates = value;
        }
        #endregion
    }
}
