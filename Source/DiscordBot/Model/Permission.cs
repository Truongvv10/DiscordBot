using DiscordBot.Model.Enums;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XironiteDiscordBot.Exceptions;

namespace DiscordBot.Model {
    public class Permission {

        #region Private Properties
        private string cmd;
        private bool allowAdministrator;
        private bool allowEveryone;
        private bool allowAddReaction;
        private Dictionary<ulong, DiscordUser> allowedUsers = new();
        private Dictionary<ulong, DiscordRole> allowedRoles = new();
        private Dictionary<ulong, DiscordChannel> allowedChannels = new();
        #endregion

        #region Constructors
        public Permission() {
            allowAdministrator = true;
        }
        public Permission(CommandEnum command) {
            cmd = command.ToString();
            allowAdministrator = true;
        }
        public Permission(CommandEnum command, bool allowAdministrator) {
            cmd = command.ToString();
            this.allowAdministrator = allowAdministrator;
        }
        public Permission(CommandEnum command, bool allowAdministrator, bool allowEveryone) {
            cmd = command.ToString();
            this.allowAdministrator = allowAdministrator;
            this.allowEveryone = allowEveryone;
        }
        #endregion

        #region Getter & Setter
        public string Cmd {
            get => cmd;
            set => cmd = value;
        }
        public bool AllowAdministrator {
            get => allowAdministrator;
            set => allowAdministrator = value;
        }
        public bool AllowEveryone {
            get => allowEveryone;
            set => allowEveryone = value;
        }
        public bool AllowAddReaction {
            get => allowAddReaction;
            set => allowAddReaction = value;
        }
        public IReadOnlyDictionary<ulong, DiscordUser> AllowedUsers {
            get => allowedUsers;
            set { foreach (var user in value) AddUser(user.Value); }
        }
        public IReadOnlyDictionary<ulong, DiscordRole> AllowedRoles {
            get => allowedRoles;
            set { foreach (var role in value) AddRole(role.Value); }
        }
        public IReadOnlyDictionary<ulong, DiscordChannel> AllowedChannels {
            get => allowedChannels;
            set { foreach (var channel in value) AddChannel(channel.Value); }
        }
        #endregion

        #region Methods
        public bool AddUser(DiscordUser user) {
            if (allowedUsers.TryAdd(user.Id, user)) return true; return false;
        }
        public bool AddRole(DiscordRole role) {
            if (allowedRoles.TryAdd(role.Id, role)) return true; return false;
        }
        public bool AddChannel(DiscordChannel channel) {
            if (allowedChannels.TryAdd(channel.Id, channel)) return true; return false;
        }
        #endregion

    }
}
