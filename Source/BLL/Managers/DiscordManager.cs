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
        private IDataRepository repository;
        private string configFolder = Path.Combine(Environment.CurrentDirectory, "Saves", "Config.json");
        #endregion

        #region Constructors
        public DiscordManager() { }
        #endregion

        //#region Method
        // Main Config
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

        //// Server
        //public async Task AddServerAsync(ulong guildId) {
        //    if (!await repository.AnyServerAsync(guildId)) {
        //        await repository.AddServerAsync(guildId);
        //    } else throw new ManagerException($"Guild with id \"{guildId}\" already exists.");
        //}
        //public async Task<Guild> GetServerAsync(ulong guildId, bool includeMessage = true) {
        //    if (await repository.AnyServerAsync(guildId)) {
        //        return await repository.GetServerAsync(guildId, includeMessage);
        //    } else throw new ManagerException($"Guild with id \"{guildId}\" was not present.");
        //}
        //public async Task DeleteServerAsync(Guild guild) {
        //    if (await repository.AnyServerAsync(guild.GuildId)) {
        //        await repository.RemoveServerAsync(guild);
        //    } else throw new ManagerException($"Guild with id \"{guild.GuildId}\" was not present.");
        //}
        //public async Task<bool> ExistsServer(ulong guildId) {
        //    return await repository.AnyServerAsync(guildId);
        //}

        //// Messages
        //public async Task AddMessageAsync(Message message) {
        //    if (message != null && message.MessageId != null) {
        //        if (await repository.AnyServerAsync(message.GuildId)) {
        //            if (!await repository.AnyMessageAsync(message.GuildId, (ulong)message.MessageId!)) {
        //                await repository.AddMessageAsync(message);
        //            } else throw new RepositoryException($"Message with id \"{message.MessageId}\" already exists.");
        //        } else throw new RepositoryException($"Guild with id \"{message.GuildId}\" was not present.");
        //    } else throw new RepositoryException($"The given message can not be null.");
        //}
        //public async Task<Message> GetMessageAsync(ulong guildId, int id) {
        //    if (await repository.AnyServerAsync(guildId)) {
        //        if (await repository.ExistMessageAsync(id)) {
        //            return await repository.GetMessageAsync(id);
        //        } else throw new RepositoryException($"Message with id \"{id}\" was not present.");
        //    } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
        //}
        //public async Task<Message> GetMessageAsync(ulong guildId, ulong? messageId) {
        //    if (messageId != null) {
        //        if (await repository.AnyServerAsync(guildId)) {
        //            if (await repository.AnyMessageAsync(guildId, (ulong)messageId)) {
        //                return await repository.GetMessageAsync(guildId, (ulong)messageId);
        //            } else throw new RepositoryException($"Message with id \"{messageId}\" was not present.");
        //        } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
        //    } else throw new RepositoryException($"Message Id can not be null.");
        //}
        //public async Task DeleteMessageAsync(ulong guildId, int id) {
        //    if (await repository.AnyServerAsync(guildId)) {
        //        if (await repository.ExistMessageAsync(id)) {
        //            await repository.RemoveMessageAsync(id);
        //        } else throw new RepositoryException($"Message with id \"{id}\" was not present.");
        //    } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
        //}
        //public async Task DeleteMessageAsync(ulong guildId, Message message) {
        //    if (message.MessageId != null) {
        //        if (await repository.AnyServerAsync(guildId)) {
        //            if (await repository.AnyMessageAsync(guildId, (ulong)message.MessageId)) {
        //                await repository.RemoveMessageAsync(message);
        //            } else throw new RepositoryException($"Message with id \"{(ulong)message.MessageId}\" was not present.");
        //        } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
        //    }
        //}
        //public async Task<Message> UpdateMessageAsync(Message message) {
        //    if (await repository.AnyServerAsync(message.GuildId)) {
        //        if (await repository.ExistMessageAsync(message.Id)) {
        //            return await repository.SetMessageAsync(message);
        //        } else throw new RepositoryException($"Message with id \"{message.MessageId}\" was not present.");
        //    } else throw new RepositoryException($"Guild with id \"{message.GuildId}\" was not present.");
        //}
        //// Templates
        //public async Task AddTemplateAsync(Template template) {
        //    if (template != null) {
        //        if (await repository.AnyServerAsync(template.GuildId)) {
        //            if (!await repository.AnyTemplateAsync(template.GuildId, template.Name)) {
        //                await repository.AddTemplateAsync(template);
        //            } else throw new RepositoryException($"Template with name \"{template.Name}\" already exists.");
        //        } else throw new RepositoryException($"Guild with id \"{template.GuildId}\" was not present.");
        //    } else throw new RepositoryException($"The given template can not be null.");
        //}
        //public async Task<Template> GetTemplateAsync(ulong guildId, string name) {
        //    if (await repository.AnyServerAsync(guildId)) {
        //        if (await repository.AnyTemplateAsync(guildId, name)) {
        //            return await repository.GetTemplateAsync(guildId, name);
        //        } else throw new RepositoryException($"Template with name \"{name}\" was not present.");
        //    } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
        //}
        //public async Task DeleteTemplateAsync(ulong guildId, string name) {
        //    if (await repository.AnyServerAsync(guildId)) {
        //        if (await repository.AnyTemplateAsync(guildId, name)) {
        //            await repository.RemoveTemplateAsync(guildId, name);
        //        } else throw new RepositoryException($"Template with name \"{name}\" was not present.");
        //    } else throw new RepositoryException($"Guild with id \"{guildId}\" was not present.");
        //}
        //public async Task<Template> UpdateTemplateAsync(Template template) {
        //    if (await repository.AnyServerAsync(template.GuildId)) {
        //        if (await repository.AnyTemplateAsync(template.GuildId, template.Name)) {
        //            return await repository.SetTemplateAsync(template);
        //        } else throw new RepositoryException($"Template with id \"{template.Id}\" was not present.");
        //    } else throw new RepositoryException($"Guild with id \"{template.GuildId}\" was not present.");
        //}
        //public async Task<bool> AnyTemplateAsync(ulong guildId, string name) {
        //    return await repository.AnyTemplateAsync(guildId, name);
        //}
        //#endregion
    }
}
