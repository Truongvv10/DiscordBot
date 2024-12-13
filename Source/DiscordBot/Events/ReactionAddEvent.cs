using APP.Utils;
using BLL.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Events {
    public class ReactionAddEvent {

        #region Fields
        private readonly IDataRepository dataService;
        #endregion

        #region Constructors
        public ReactionAddEvent(IDataRepository dataService) {
            this.dataService = dataService;
        }
        #endregion

        public async Task ReactionAdd(DiscordClient sender, MessageReactionAddEventArgs args) {
        }
    }
}
