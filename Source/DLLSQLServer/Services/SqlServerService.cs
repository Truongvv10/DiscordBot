using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using BLL.Services;
using DLLSQLServer.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLSQLServer.Services {
    public class SqlServerService : IDataService {

        #region Fields
        private CacheData cacheData;
        private SqlServerDataContext dataContext;
        #endregion

        #region Constructors
        public SqlServerService(CacheData cacheData, string connectionString) {
            this.cacheData = cacheData;
            dataContext = new SqlServerDataContext(connectionString);
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
            await dataContext.SaveChangesAsync();
            cacheData.AddMessage(message.GuildId, message);
            return message;
        }

        public async Task AddServerAsync(ulong guildId) {
            await dataContext.AddAsync(new Guild() { GuildId = guildId });
            await dataContext.SaveChangesAsync();
        }

        public async Task<Template> AddTemplateAsync(Template template) {
            await dataContext.Templates.AddAsync(template);
            await dataContext.SaveChangesAsync();
            return template;
        }

        public Task<bool> AnyMessageAsync(ulong guildId, ulong messageId) {
            return dataContext.Messages
                .AsNoTracking()
                .AnyAsync(m => m.GuildId == guildId && m.MessageId == messageId);
        }

        public Task<bool> AnyServerAsync(ulong guildId) {
            return dataContext.Servers
                .AsNoTracking()
                .AnyAsync(s => s.GuildId == guildId);
        }

        public Task<bool> AnyTemplateAsync(ulong guildId, string name) {
            return dataContext.Templates
                .AsNoTracking()
                .AnyAsync(t => t.GuildId == guildId && t.Name == name);
        }

        public async Task<List<Guild>> GetAllServersAsync() {
            return await dataContext.Servers
                .Include(s => s.Messages)
                    .ThenInclude(m => m.Embed)
                .ToListAsync();
        }

        public Task<List<Template>> GetAllTemplatesAsync(ulong guildId) {
            return dataContext.Templates
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
                if (result != null) cacheData.Messages.Add((guildId, messageId), result);
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

        public async Task LoadGuildsAsync(IEnumerable<ulong> guildIds) {
            foreach (var id in guildIds) {
                if (!await AnyServerAsync(id)) {
                    await AddServerAsync(id);
                }
            }
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
            await dataContext.SaveChangesAsync();
        }

        public async Task RemoveServerAsync(Guild guild) {
            dataContext.Servers.Remove(guild);
            await dataContext.SaveChangesAsync();
        }

        public async Task RemoveTemplateAsync(Template template) {
            dataContext.Templates.Remove(template);
            await dataContext.SaveChangesAsync();
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
                await dataContext.SaveChangesAsync();
                return fetched;
            } else throw new ServiceException($"Message with id \"{message.MessageId}\" in guild \"{message.GuildId}\" doesn't exists in database.");
        }

        public async Task<Template> UpdateTemplateAsync(Template template) {
            var existingTemplate = await GetTemplateAsync(template.GuildId, template.Name);
            existingTemplate = template;
            await dataContext.SaveChangesAsync();
            return existingTemplate;
        }
        #endregion
    }

}
