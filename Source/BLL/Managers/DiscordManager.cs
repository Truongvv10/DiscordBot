using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Managers {
    public class DiscordManager {
        #region Properties
        private IRepository repository;
        private string configFolder = Path.Combine(Environment.CurrentDirectory, "Saves", "Config.json");
        #endregion

        #region Constructors

        public DiscordManager(IRepository repository) {
            this.repository = repository;
        }
        #endregion

        #region Method
        public async Task<Config> GetDiscordBotConfig() {
            try {
                using (StreamReader sr = new StreamReader(configFolder)) {
                    string text = await sr.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<Config>(text)!;
                }

            } catch (Exception ex) {
                throw new ManagerException($"JsonReaderUtil.ReadConfigFile: \"Config.json\"", ex);
            }
        }

        // Server
        public async Task AddServerAsync(ulong guildId) {
            if (!await repository.ExistServerAsync(guildId)) {
                await repository.AddServerAsync(guildId);
            } else throw new ManagerException($"Guild with id \"{guildId}\" already exists.");
        }
        public async Task<Server> GetServerAsync(ulong guildId) {
            if (await repository.ExistServerAsync(guildId)) {
                return await repository.GetServerAsync(guildId);
            } else throw new ManagerException($"Guild with id \"{guildId}\" was not present.");
        }
        public async Task DeleteServerAsync(ulong guildId) {
            if (await repository.ExistServerAsync(guildId)) {
                await repository.RemoveServerAsync(guildId);
            } else throw new ManagerException($"Guild with id \"{guildId}\" was not present.");
        }

        // Messages
        public async Task AddMessageAsync(ulong guildId, Message message) {
            if (message != null) {
                if (await repository.ExistServerAsync(guildId)) {
                    if (!await repository.ExistMessageAsync(message.MessageId)) {
                        await repository.AddMessageAsync(guildId, message);
                    } else throw new RepositoryException($"Message with id \"{message.MessageId}\" already exists.");
                } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
            } else throw new RepositoryException($"The given message can not be null.");
        }
        public async Task<Message> GetMessageAsync(ulong guildId, ulong messageId) {
            if (await repository.ExistServerAsync(guildId)) {
                if (await repository.ExistMessageAsync(messageId)) {
                    return await repository.GetMessageAsync(guildId, messageId);
                } else throw new RepositoryException($"Message with id \"{messageId}\" was not present.");
            } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
        }
        public async Task DeleteMessageAsync(ulong guildId, ulong messageId) {
            if (await repository.ExistServerAsync(guildId)) {
                if (await repository.ExistMessageAsync(messageId)) {
                    await repository.RemoveMessageAsync(guildId, messageId);
                } else throw new RepositoryException($"Message with id \"{messageId}\" was not present.");
            } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
        }
        public async Task<Message> UpdateMessageAsync(ulong guildId, Message message) {
            if (await repository.ExistServerAsync(guildId)) {
                if (await repository.ExistMessageAsync(message.MessageId)) {
                    return await repository.SetMessageAsync(guildId, message);
                } else throw new RepositoryException($"Message with id \"{message.MessageId}\" was not present.");
            } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
        }
        #endregion
    }
}
