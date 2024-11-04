using Domain.Exceptions;

namespace Domain.Model {
    public class Server {

        #region Fields
        private ulong guildId;
        private Config config;
        private Permission permission;
        private Dictionary<ulong, Message> messages = new();
        #endregion

        #region Constructors
        public Server() {
        }
        #endregion

        #region Properties
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
        public Permission Permission {
            get => permission;
            set {
                if (value == null) throw new DomainException("Server.Permission: value can not be null.");
                permission = value;
            }
        }
        public IReadOnlyDictionary<ulong, Message> Messages {
            get => messages;
            set {
                if (value == null) throw new DomainException("Server.Messages: value can not be null.");
                foreach (var item in value) AddMessage(item.Value);
            }
        }
        #endregion

        #region Methods
        public void AddMessage(Message message) {
            if (message != null && message.MessageId != null) {
                var id = (ulong)message.MessageId;
                if (messages.ContainsKey(id)) {
                    messages[id] = message;
                } else {
                    messages.Add(id, message);
                }
            }
        }
        #endregion




    }
}
