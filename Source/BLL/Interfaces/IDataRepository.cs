using BLL.Enums;
using BLL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces {
    public interface IDataRepository {
        // Saving
        public Task CtxSaveAndClear();
        // Cache
        public Message GetCacheModalData(ulong guildId, ulong userId);
        public bool AddCacheModalData(ulong guildId, ulong userId, Message message);
        public bool SetCacheModalData(ulong guildId, ulong userId, Message message);

        // Loading Data
        public Task LoadTemplatesAsync();
        public Task LoadTimeZonesAsync();
        public Task LoadSettingsFromGuildsAsync();
        public Task LoadGuildsAsync(IEnumerable<ulong> guildIds);
        // Server
        public Task AddServerAsync(ulong guildId);
        public Task<Guild?> GetServerAsync(ulong guildId, bool includeMessage = true);
        public Task<List<Guild>> GetAllServersAsync();
        public Task RemoveServerAsync(Guild guild);
        public Task<bool> AnyServerAsync(ulong guildId);
        // Settings
        public Task<Settings> AddSettingsAsync(Settings settings);
        public Task<Settings?> GetSettingsAsync(ulong guildId);
        public Task<Settings> UpdateSettingsAsync(Settings settings);
        public Task RemoveSettingsAsync(Settings settings);
        public Task<bool> AnySettingsAsync(ulong guildId);
        // Message
        public Task<Message> AddMessageAsync(Message message);
        public Task<Message?> GetMessageAsync(ulong guildId, ulong messageId);
        public Task<Message> UpdateMessageAsync(Message message);
        public Task<Message> UpdateMessageAsync(Message message, string component);
        public Task RemoveMessageAsync(Message message);
        public Task<bool> AnyMessageAsync(ulong guildId, ulong messageId);
        // Template
        public Task<Template> AddTemplateAsync(Template template);
        public Task<Template?> GetTemplateAsync(ulong guildId, TemplateMessage name);
        public Task<Template?> GetTemplateAsync(ulong guildId, string name);
        public Task<List<Template>> GetAllTemplatesAsync(ulong guildId);
        public Task<Template> UpdateTemplateAsync(Template template);
        public Task RemoveTemplateAsync(Template template);
        public Task<bool> AnyTemplateAsync(ulong guildId, string name);
    }
}
