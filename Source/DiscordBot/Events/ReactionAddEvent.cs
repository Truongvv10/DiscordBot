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
        private readonly IDataService dataService;
        #endregion

        #region Constructors
        public ReactionAddEvent(IDataService dataService) {
            this.dataService = dataService;
        }
        #endregion

        public async Task ReactionAdd(DiscordClient sender, MessageReactionAddEventArgs args) {
        }
    }
}
