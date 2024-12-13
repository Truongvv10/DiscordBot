using APP.Attributes;
using APP.Services;
using BLL.Enums;
using BLL.Model;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace APP.Commands.Slash {
    public class BroadcastCmd : ApplicationCommandModule {


        [SlashCommand("broadcast", "Send an embeded message to the current channel")]
        [RequirePermission(CommandEnum.BROADCAST, [Permissions.ManageChannels, Permissions.ManageMessages])]
        public async Task UseBroadcastCommand(InteractionContext ctx) {
        }

    }
}
