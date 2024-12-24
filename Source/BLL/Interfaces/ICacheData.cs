using BLL.Exceptions;
using BLL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces {
    public interface ICacheData {

        // Fields
        public IReadOnlyDictionary<(ulong, ulong), Message> Messages { get; }
        public IReadOnlyDictionary<ulong, Settings> Settings { get; }
        public IReadOnlyDictionary<string, Message> Templates { get; }
        public IReadOnlyCollection<string> TimeZones { get; }

        // Loading Data
        public Task LoadTemplates();
        public void LoadTimeZones();

        // Template Data
        public bool AnyTemplate(string name);
        public Template GetTemplate(string name);

        // Settings Data
        public bool AnySettings(ulong guildId);
        public void AddSettings(ulong guildId, Settings settings);
        public Settings GetSettings(ulong guildId);
        public Settings UpdateSettings(ulong guildId, Settings settings);
        public void DeleteSettings(ulong guildId);

        // Modal Data
        public bool AddModalData(ulong guildId, ulong userId, Message message);
        public bool SetModalData(ulong guildId, ulong userId, Message message);
        public Message GetModalData(ulong guildId, ulong userId);

        // Message Data
        public bool AnyMessage(ulong guildId, ulong messageId);
        public void AddMessage(ulong guildId, Message message);
        public Message GetMessage(ulong guildId, ulong messageId);
        public Message UpdateMessage(ulong guildId, Message message);
        public void DeleteMessage(ulong guildId, ulong messageId);

    }
}
