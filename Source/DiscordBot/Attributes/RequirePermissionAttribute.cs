using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using AnsiColor = APP.Utils.AnsiColor;
using BLL.Enums;
using DSharpPlus.CommandsNext.Attributes;

namespace APP.Attributes {
    public class RequirePermissionAttribute : SlashCheckBaseAttribute {

        #region Fields
        private CommandEnum command;
        private bool checkChannelPermission = false;
        private bool everyoneCanUse = false;
        #endregion

        #region Constructors
        public RequirePermissionAttribute(CommandEnum command, bool everyoneCanUse = false, bool checkChannelPermission = false) {
            this.command = command;
            this.everyoneCanUse = everyoneCanUse;
            this.checkChannelPermission = checkChannelPermission;
        }
        #endregion

        #region Properties
        public CommandEnum Command {
            get => command;
            set => command = value;
        }
        public bool CheckChannelPermission {
            get => checkChannelPermission;
            set => checkChannelPermission = value;
        }
        #endregion

        #region Methods
        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
            var user = await ctx.Guild.GetMemberAsync(ctx.User.Id);
            var userInChannel = user.PermissionsIn(ctx.Channel);
            var roles = user.Roles;

            LogCommand(user, ctx.Guild, ctx.Interaction.Channel);

            if (everyoneCanUse) return true;
            if (user.Permissions.HasPermission(Permissions.All)) return true;
            if (user.Permissions.HasPermission(Permissions.Administrator)) return true;

            if (checkChannelPermission) {
                if (userInChannel.HasPermission(Permissions.ManageChannels)) return true;
                if (userInChannel.HasPermission(Permissions.ManageMessages)) return true;
            }

            return false;
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
