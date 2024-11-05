using BLL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces {
    public interface IRepository {
        public Task AddServerAsync(ulong guildId);
        public Task<Server> GetServerAsync(ulong guildId);
        public Task RemoveServerAsync(ulong guildId);
        public Task<bool> ExistServerAsync(ulong guildId);
        public Task AddMessageAsync(ulong guildId, Message message);
        public Task<Message> GetMessageAsync(ulong guildId, ulong message);
        public Task<Message> SetMessageAsync(ulong guildId, Message message);
        public Task RemoveMessageAsync(ulong guildId, ulong messageId);
        public Task<bool> ExistMessageAsync(ulong messageId);
    }
}
