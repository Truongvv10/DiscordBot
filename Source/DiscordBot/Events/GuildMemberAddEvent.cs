using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.Utils;
using BLL.Interfaces;

namespace APP.Events {
    public class GuildMemberAddEvent {

        #region Fields
        private readonly IDataRepository dataService;
        #endregion

        #region Constructors
        public GuildMemberAddEvent(IDataRepository dataService) {
            this.dataService = dataService;
        }
        #endregion

        public async Task GuildMemberAdd(DiscordClient sender, GuildMemberAddEventArgs args) {
        }
    }
}
