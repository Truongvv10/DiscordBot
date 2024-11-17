using APP.Services;
using BLL.Enums;
using DSharpPlus.SlashCommands;

namespace APP.Commands.Slash {
    public class ChangelogCmd : ApplicationCommandModule {

        [SlashCommand("changelog", "Setup, edit and create changelogs.")]
        [RequirePermission(CommandEnum.CHANGELOG)]
        public async Task UseChangelogCommand(InteractionContext ctx) {
        }
    }

}
