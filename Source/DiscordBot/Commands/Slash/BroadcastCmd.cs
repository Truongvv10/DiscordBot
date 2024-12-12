using APP.Attributes;
using APP.Services;
using BLL.Enums;
using DSharpPlus.SlashCommands;

namespace APP.Commands.Slash {
    public class BroadcastCmd : ApplicationCommandModule {


        [SlashCommand("broadcast", "Send an embeded message to the current channel")]
        [RequirePermission(CommandEnum.BROADCAST, true)]
        public async Task UseBroadcastCommand(InteractionContext ctx) {
        }

    }
}
