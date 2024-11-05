using BLL.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Model {
    public class Server {

        #region Fields
        private ulong guildId;
        private Config config;
        private ICollection<Message>? messages;
        #endregion

        #region Constructors
        public Server() {
            config = new Config();
        }
        public Server(ulong guildId) : this() {
            this.guildId = guildId;
        }
        public Server(ulong guildId, Config config) : this(guildId) {
            this.config = config;
        }
        #endregion

        #region Properties
        [Key]
        [Column("guild_id", TypeName = "decimal(20, 0)")]
        public ulong GuildId {
            get => guildId;
            set => guildId = value;
        }

        public Config Config {
            get => config;
            set {
                if (value == null) throw new DomainException("Server.Config: value can not be null.");
                config = value;
            }
        }

        public ICollection<Message>? Messages {
            get => messages;
            set => messages = value;
        }
        #endregion
    }
}
