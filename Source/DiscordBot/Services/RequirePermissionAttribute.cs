using DiscordBot.Model;
using DiscordBot.Model.Enums;
using DiscordBot.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Exceptions;
using AnsiColor = DiscordBot.Utils.AnsiColor;

namespace DiscordBot.Services {
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
            var permission = await CacheData.GetPermission(ctx.Guild.Id, Command);
            var roles = user.Roles;

            LogCommand(user, ctx.Guild, ctx.Interaction.Channel);

            if (user.Permissions.HasPermission(Permissions.All)) {
                return true;
            } else if (user.Permissions.HasPermission(Permissions.Administrator)) {
                return true;
            } else return false;

            //if (permission.AllowEveryone) {
            //    return true;
            //} else if (user.Permissions.HasPermission(Permissions.All)) {
            //    return true;
            //} else if (permission.AllowAdministrator) {
            //    return true;
            //} else if (permission.AllowedUsers.Any(x => x.Value == user)) {
            //    return true;
            //} else if (permission.AllowedRoles.Values.ToList().Intersect(roles).Any()) {
            //    return true;
            //} else {
            //    return false;
            //}
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
