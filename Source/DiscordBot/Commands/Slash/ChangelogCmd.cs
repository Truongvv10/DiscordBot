using DiscordBot.Exceptions;
using DiscordBot.Model;
using DiscordBot.Model.Enums;
using DiscordBot.Utils;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using XironiteDiscordBot.Commands;
using static System.Net.Mime.MediaTypeNames;

namespace DiscordBot.Commands.Slash {
    public class ChangelogCmd : SlashCommand {
        [SlashCommand("changelog", "Setup/Edit/Create changelogs for your server.")]
        public async Task UseChangelogCommand(InteractionContext ctx) {
            await DiscordUtil.SendImageEmbedAsync(ctx.Channel, @"T:\Projects\Discord\Github\DiscordBot\Source\DiscordBot\Saves\Images\image.png");
            //    try {

            //        // Check if user has permission to use command
            //        if (!await CheckPermission(ctx, CommandEnum.CHANGELOG)) {
            //            await showNoPermissionMessage(ctx);
            //            return;
            //        }

            //        // Log the command the user is using
            //        LogCommand(ctx, CommandEnum.CHANGELOG);

            //        // Build the embed message with default values
            //        EmbedBuilder embed = new EmbedBuilder() {
            //            Title = "Changelog Editor",
            //            Description = "Choose which embed you want to edit.",
            //            ChannelId = ctx.Channel.Id,
            //            Owner = ctx.User.Id
            //        };

            //        // Create the embed message
            //        await CreateEmbedMessageAsync(ctx, embed, EmbedType.CHANGELOG, ctx.Channel.Id, false);

            //    } catch (Exception ex) {
            //        throw new CommandException($"Embed.UseChangelogCommand: {ex}");
            //    }
            //}
        }
    }
}
