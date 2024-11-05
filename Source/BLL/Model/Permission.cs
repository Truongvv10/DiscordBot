using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Model {
    public class Permission {
        #region Private Properties
        private bool allowAdministrator;
        private bool allowEveryone;
        private bool allowAddReaction;
        private HashSet<ulong> allowedUsers = new();
        private HashSet<ulong> allowedRoles = new();
        private HashSet<ulong> allowedChannels = new();
        private CommandEnum action;
        #endregion

        #region Constructors
        public Permission() {
            allowAdministrator = true;
            allowEveryone = false;
            allowAddReaction = true;
        }
        public Permission(CommandEnum action) : this() {
            this.action = action;
            allowAdministrator = true;
        }
        public Permission(CommandEnum action, bool allowAdministrator = true, bool allowEveryone = false, bool allowAddReaction = true) : this(action) {
            this.action = action;
            this.allowAdministrator = allowAdministrator;
            this.allowEveryone = allowEveryone;
            this.allowAddReaction = allowAddReaction;
        }
        #endregion

        #region Getter & Setter
        public CommandEnum Action {
            get => action;
            set => action = value;
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
            if (AllowedChannels.Contains(channelId)) {
                return false;
            } else {
                allowedUsers.Add(channelId);
                return true;
            }
        }
        #endregion
    }
}
