﻿using APP.Attributes;
using APP.Services;
using BLL.Enums;
using DSharpPlus.SlashCommands;

namespace APP.Commands.Slash {
    public class PermissionCmd : ApplicationCommandModule {

        [SlashCommand("permissions", "Show an overview of permissions")]
        [RequirePermission(CommandEnum.PERMISSIONS)]
        public async Task UsePermissionEditCommand(InteractionContext ctx) {

            //try {

            //    // Build the embed message with default values
            //    EmbedBuilder embed = new EmbedBuilder() {
            //        Title = "Permissions Editor",
            //        Owner = ctx.User.Id
            //    };

            //    foreach (var perms in CacheData.Permissions[ctx.Guild.Id]) {
            //        string desc =
            //            $"```ansi\n" +
            //            $"{AnsiColor.RESET}Everyone : {AnsiColor.GREEN}{perms.Value.AllowEveryone}\n" +
            //            $"{AnsiColor.RESET}Admins   : {AnsiColor.GREEN}{perms.Value.AllowAdministrator}\n" +
            //            $"{AnsiColor.RESET}Reactions: {AnsiColor.GREEN}{perms.Value.AllowAddReaction}\n" +
            //            $"{AnsiColor.RESET}Users    : {AnsiColor.YELLOW}{perms.Value.AllowedUsers.Count()}\n" +
            //            $"{AnsiColor.RESET}Roles    : {AnsiColor.YELLOW}{perms.Value.AllowedRoles.Count()}\n" +
            //            $"{AnsiColor.RESET}Channels : {AnsiColor.MAGENTA}{perms.Value.AllowedChannels.Count()}\n" +
            //            $"```";
            //        embed.AddField(perms.Value.Cmd, desc);
            //    }

            //    // Create the embed message
            //    await CreateEmbedMessageAsync(ctx, embed, EmbedType.PERMISSION, ctx.Interaction.Channel.Id, false);

            //} catch (Exception ex) {
            //    throw new CommandException($"Embed.UseEmbedCommand: {ex}");
            //}


        }

    }
}
