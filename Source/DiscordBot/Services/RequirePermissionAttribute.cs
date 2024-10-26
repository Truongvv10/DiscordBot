using DiscordBot.Model.Enums;
using DiscordBot.Utils;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services {
    public class RequirePermissionAttribute : SlashCheckBaseAttribute {

        public CommandEnum Command;

        public RequirePermissionAttribute(CommandEnum command) {
            this.Command = command;
        }

        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
            var id = ctx.User.Id;
            var user = await ctx.Guild.GetMemberAsync(id);
            var permission = await CacheData.GetPermission(ctx.Guild.Id, Command);
            var roles = user.Roles;

            if (permission.AllowEveryone) {
                return true;
            } else if (user.Permissions.HasPermission(Permissions.All)) {
                return true;
            } else if (permission.AllowAdministrator) {
                return true;
            } else if (permission.AllowedUsers.Any(x => x.Value == user)) {
                return true;
            } else if (permission.AllowedRoles.Values.ToList().Intersect(roles).Any()) {
                return true;
            } else {
                return false;
            }
        }
    }
}
