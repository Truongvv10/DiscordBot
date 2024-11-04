using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model {
    public class Permission {
        #region Private Properties
        private string cmd;
        private bool allowAdministrator;
        private bool allowEveryone;
        private bool allowAddReaction;
        private HashSet<ulong> allowedUsers = new();
        private HashSet<ulong> allowedRoles = new();
        private HashSet<ulong> allowedChannels = new();
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
        public HashSet<ulong> AllowedUsers {
            get => allowedUsers;
            set { foreach (var id in value) AddUser(id); }
        }
        public HashSet<ulong> AllowedRoles {
            get => allowedRoles;
            set { foreach (var id in value) AddRole(id); }
        }
        public HashSet<ulong> AllowedChannels {
            get => allowedChannels;
            set { foreach (var id in value) AddChannel(id); }
        }
        #endregion

        #region Methods
        public bool AddUser(ulong userId) {
            if (allowedUsers.Contains(userId)) {
                return false;
            } else {
                allowedUsers.Add(userId);
                return true;
            }
        }
        public bool AddRole(ulong roleId) {
            if (allowedRoles.Contains(roleId)) {
                return false;
            } else {
                allowedUsers.Add(roleId);
                return true;
            }
        }
        public bool AddChannel(ulong channelId) {
            if (allowedch.Contains(channelId)) {
                return false;
            } else {
                allowedUsers.Add(channelId);
                return true;
            }
        }
        #endregion
    }
}
