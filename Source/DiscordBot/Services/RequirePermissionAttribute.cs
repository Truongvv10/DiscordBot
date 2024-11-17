using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using AnsiColor = APP.Utils.AnsiColor;
using BLL.Enums;

namespace APP.Services {
    public class RequirePermissionAttribute : SlashCheckBaseAttribute {

        #region Fields
        private CommandEnum command;
        #endregion

        #region Constructors
        public RequirePermissionAttribute(CommandEnum command) {
            this.command = command;
        }
        #endregion

        #region Properties
        public CommandEnum Command {
            get => command;
            set => command = value;
        }
        #endregion

        #region Methods
        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
            var id = ctx.User.Id;
            var user = await ctx.Guild.GetMemberAsync(id);
            var roles = user.Roles;

            LogCommand(user, ctx.Guild, ctx.Interaction.Channel);

            if (user.Permissions.HasPermission(Permissions.All)) {
                return true;
            } else if (user.Permissions.HasPermission(Permissions.Administrator)) {
                return true;
            } else return false;
        }

        private void LogCommand(DiscordUser user, DiscordGuild guild, DiscordChannel channel) {
            Console.WriteLine(
                $"{AnsiColor.RESET}[{DateTime.Now}] " +
                $"{AnsiColor.BRIGHT_GREEN}{user.Username} issued command: " +
                $"{AnsiColor.YELLOW}/{command.ToString().ToLower()} " +
                $"{AnsiColor.RESET}({guild.Name}) ({channel.Name})");
        }
        #endregion
    }
}
