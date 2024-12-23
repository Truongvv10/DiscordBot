using BLL.Enums;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using DLL.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Repositories {
    public class DataRepository : IDataRepository {
        #region Fields
        protected ICacheData cacheData;
        protected DataContextAbstract dataContext;
        #endregion

        #region Constructors
        protected DataRepository(ICacheData cacheData, DataContextAbstract dataContext) {
            this.cacheData = cacheData;
            this.dataContext = dataContext;
        }
        #endregion

        #region Methods
        public async Task<Message> AddMessageAsync(Message message) {
            if (message == null)
                throw new ServiceException($"Message can not be null.");
            if (message.GuildId == 0)
                throw new ServiceException($"Guild Id can not be null.");
            if (message.MessageId == 0)
                throw new ServiceException($"Message Id can not be null.");
            await dataContext.Messages.AddAsync(message);
            await CtxSaveAndClear();
            cacheData.AddMessage(message.GuildId, message);
            return message;
        }

        public async Task AddServerAsync(ulong guildId) {
            await dataContext.AddAsync(new Guild(guildId));
            await CtxSaveAndClear();
        }

        public async Task<Settings> AddSettingsAsync(Settings settings) {
            if (settings == null)
                throw new ServiceException($"Settings can not be null.");
            if (settings.GuildId == 0)
                throw new ServiceException($"Guild Id can not be null.");
            await dataContext.Settings.AddAsync(settings);
            await CtxSaveAndClear();
            cacheData.AddSettings(settings.GuildId, settings);
            return settings;
        }

        public async Task<Template> AddTemplateAsync(Template template) {
            await dataContext.Templates.AddAsync(template);
            await CtxSaveAndClear();
            return template;
        }

        public async Task<bool> AnyMessageAsync(ulong guildId, ulong messageId) {
            return await dataContext.Messages
                .AsNoTracking()
                .AnyAsync(m => m.GuildId == guildId && m.MessageId == messageId);
        }

        public async Task<bool> AnyServerAsync(ulong guildId) {
            return await dataContext.Servers
                .AsNoTracking()
                .AnyAsync(s => s.GuildId == guildId);
        }

        public async Task<bool> AnySettingsAsync(ulong guildId) {
            return await dataContext.Settings
                .AsNoTracking()
                .AnyAsync(s => s.GuildId == guildId);
        }

        public async Task<bool> AnyTemplateAsync(ulong guildId, string name) {
            return await dataContext.Templates
                .AsNoTracking()
                .AnyAsync(t => t.GuildId == guildId && t.Name == name);
        }

        public async Task CtxSaveAndClear() {
            await dataContext.SaveChangesAsync();
            dataContext.ChangeTracker.Clear();
        }

        public async Task<List<Guild>> GetAllServersAsync() {
            return await dataContext.Servers
                .ToListAsync();
        }

        public async Task<List<Template>> GetAllTemplatesAsync(ulong guildId) {
            return await dataContext.Templates
                .Where(t => t.GuildId == guildId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Message?> GetMessageAsync(ulong guildId, ulong messageId) {
            if (cacheData.Messages.TryGetValue((guildId, messageId), out var message)) {
                return message;
            } else {
                var result = await dataContext.Messages
                    .Where(m => m.GuildId == guildId && m.MessageId == messageId)
                    .Include(m => m.Embed)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (result != null) cacheData.AddMessage(guildId, result);
                return result;
            }
        }

        public async Task<Guild?> GetServerAsync(ulong guildId, bool includeMessage = true) {
            IQueryable<Guild> query = dataContext.Servers.Where(s => s.GuildId == guildId);
            if (includeMessage) query = query.Include(m => m.Messages).ThenInclude(m => m.Embed);
            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync();

        }

        public async Task<Settings?> GetSettingsAsync(ulong guildId) {
            if (cacheData.Settings.TryGetValue(guildId, out var settings)) {
                return settings;
            } else {
                var result = await dataContext.Settings
                    .Where(s => s.GuildId == guildId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (result != null) cacheData.AddSettings(guildId, result);
                return result;
            }
        }

        public async Task<Template?> GetTemplateAsync(ulong guildId, string name) {
            var template = await dataContext.Templates
                .Where(t => t.GuildId == guildId && t.Name == name)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (template == null) {
                return cacheData.GetTemplate(name);
            } else {
                return new Template(template.GuildId, template.Name, template.Message.DeepClone());
            }
        }

        public async Task<Template?> GetTemplateAsync(ulong guildId, TemplateMessage name) {
            var template = await dataContext.Templates
                .Where(t => t.GuildId == guildId && t.Name == name.ToString())
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (template == null) {
                return cacheData.GetTemplate(name.ToString());
            } else {
                return new Template(template.GuildId, template.Name, template.Message.DeepClone());
            }
        }

        public async Task LoadGuildsAsync(IEnumerable<ulong> guildIds) {
            foreach (var id in guildIds) {
                if (!await AnyServerAsync(id)) {
                    await AddServerAsync(id);
                }
            }
        }

        public Task LoadSettingsFromGuildsAsync() {
            throw new NotImplementedException();
        }

        public async Task LoadTemplatesAsync() {
            await cacheData.LoadTemplates();
        }

        public async Task LoadTimeZonesAsync() {
            await Task.Run(() => cacheData.LoadTimeZones());
        }

        public async Task RemoveMessageAsync(Message message) {
            var tracked = dataContext.Messages.Local.FirstOrDefault(m => m.MessageId == message.MessageId && m.GuildId == m.GuildId);
            if (tracked != null) dataContext.Entry(tracked).State = EntityState.Detached;
            dataContext.Messages.Remove(message);
            cacheData.DeleteMessage(message.GuildId, message.MessageId);
            await CtxSaveAndClear();
        }

        public async Task RemoveServerAsync(Guild guild) {
            dataContext.Servers.Remove(guild);
            await CtxSaveAndClear();
        }

        public async Task RemoveSettingsAsync(Settings settings) {
            dataContext.Settings.Remove(settings);
            cacheData.DeleteSettings(settings.GuildId);
            await CtxSaveAndClear();
        }

        public async Task RemoveTemplateAsync(Template template) {
            dataContext.Templates.Remove(template);
            await CtxSaveAndClear();
        }

        public async Task<Message> UpdateMessageAsync(Message message) {
            if (message == null)
                throw new ServiceException($"Message can not be null.");
            if (message.GuildId == 0)
                throw new ServiceException($"Guild Id can not be null.");
            if (message.MessageId == 0)
                throw new ServiceException($"Message Id can not be null.");

            await RemoveMessageAsync(message);
            cacheData.DeleteMessage(message.GuildId, message.MessageId);
            await AddMessageAsync(message);
            return message;
        }

        public async Task<Message> UpdateMessageAsync(Message message, string component) {
            if (message == null)
                throw new ServiceException($"Message can not be null.");
            if (message.GuildId == 0)
                throw new ServiceException($"Guild Id can not be null.");
            if (message.MessageId == 0)
                throw new ServiceException($"Message Id can not be null.");

            if (cacheData.AnyMessage(message.GuildId, message.MessageId))
                cacheData.UpdateMessage(message.GuildId, message);
            else cacheData.AddMessage(message.GuildId, message);

            var fetched = await GetMessageAsync(message.GuildId, message.MessageId);
            if (fetched != null) {
                switch (component) {

                    case Identity.SELECTION_TITLE:
                        fetched.Embed.Title = message.Embed.Title;
                        fetched.Embed.TitleUrl = message.Embed.TitleUrl;
                        dataContext.Entry(fetched.Embed).Property(e => e.Title).IsModified = true;
                        dataContext.Entry(fetched.Embed).Property(e => e.TitleUrl).IsModified = true;
                        break;

                    case Identity.SELECTION_DESCRIPTION:
                        fetched.Embed.Description = message.Embed.Description;
                        dataContext.Entry(fetched.Embed).Property(e => e.Description).IsModified = true;
                        break;

                    case Identity.SELECTION_CONTENT:
                        fetched.Content = message.Content;
                        dataContext.Entry(fetched).Property(m => m.Content).IsModified = true;
                        break;

                    case Identity.SELECTION_FOOTER:
                        fetched.Embed.Footer = message.Embed.Footer;
                        fetched.Embed.FooterUrl = message.Embed.FooterUrl;
                        dataContext.Entry(fetched.Embed).Property(e => e.Footer).IsModified = true;
                        dataContext.Entry(fetched.Embed).Property(e => e.FooterUrl).IsModified = true;
                        break;

                    case Identity.SELECTION_AUTHOR:
                        fetched.Embed.Author = message.Embed.Author;
                        fetched.Embed.AuthorLink = message.Embed.AuthorLink;
                        fetched.Embed.AuthorUrl = message.Embed.AuthorUrl;
                        dataContext.Entry(fetched.Embed).Property(e => e.Author).IsModified = true;
                        dataContext.Entry(fetched.Embed).Property(e => e.AuthorLink).IsModified = true;
                        dataContext.Entry(fetched.Embed).Property(e => e.AuthorUrl).IsModified = true;
                        break;

                    case Identity.SELECTION_COLOR:
                        fetched.Embed.Color = message.Embed.Color;
                        dataContext.Entry(fetched.Embed).Property(e => e.Color).IsModified = true;
                        break;

                    case Identity.SELECTION_IMAGE:
                        fetched.Embed.Image = message.Embed.Image;
                        dataContext.Entry(fetched.Embed).Property(e => e.Image).IsModified = true;
                        break;

                    case Identity.SELECTION_THUMBNAIL:
                        fetched.Embed.Thumbnail = message.Embed.Thumbnail;
                        dataContext.Entry(fetched.Embed).Property(e => e.Thumbnail).IsModified = true;
                        break;

                    case Identity.SELECTION_PINGROLE:
                        fetched.Roles = message.Roles;
                        dataContext.Entry(fetched).Property(e => e.Roles).IsModified = true;
                        break;

                    case Identity.SELECTION_FIELD_ADD:
                        fetched.Embed.SetFields(message.Embed.Fields.ToList());
                        dataContext.Entry(fetched.Embed).Property(e => e.Fields).IsModified = true;
                        break;

                    case Identity.SELECTION_FIELD_REMOVE:
                        fetched.Embed.SetFields(message.Embed.Fields.ToList());
                        dataContext.Entry(fetched.Embed).Property(e => e.Fields).IsModified = true;
                        break;

                    case Identity.SELECTION_PLACEHOLDER:
                        fetched.Data = message.Data;
                        dataContext.Entry(fetched).Property(m => m.Data).IsModified = true;
                        break;

                    default:
                        return message;
                }
                await CtxSaveAndClear();
                return fetched;
            } else throw new ServiceException($"Message with id \"{message.MessageId}\" in guild \"{message.GuildId}\" doesn't exists in database.");
        }

        public async Task<Settings> UpdateSettingsAsync(Settings settings) {
            if (settings == null)
                throw new ServiceException($"Settings can not be null.");
            if (settings.GuildId == 0)
                throw new ServiceException($"Guild Id can not be null.");
            var existingSettings = await GetSettingsAsync(settings.GuildId);
            existingSettings = settings;
            dataContext.Update(existingSettings);
            cacheData.UpdateSettings(settings.GuildId, existingSettings);
            await CtxSaveAndClear();
            return existingSettings;
        }

        public async Task<Template> UpdateTemplateAsync(Template template) {
            var existingTemplate = await GetTemplateAsync(template.GuildId, template.Name);
            existingTemplate = template;
            await CtxSaveAndClear();
            return existingTemplate;
        }
        #endregion
    }
}
