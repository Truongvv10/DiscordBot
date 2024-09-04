using DiscordBot.Exceptions;
using DiscordBot.Model;
using DiscordBot.Model.Enums;
using DSharpPlus.SlashCommands;
using System.Threading.Channels;
using XironiteDiscordBot.Commands;
using static System.Net.Mime.MediaTypeNames;

namespace DiscordBot.Commands.Slash {
    public class PermissionCmd : SlashCommand {

        [SlashCommand("permissions", "Show an overview of permissions")]
        public async Task UsePermissionEditCommand(InteractionContext ctx) {

            try {
                // Check if user has permission to use command
                if (!await CheckPermission(ctx, CommandEnum.PERMISSIONS)) {
                    await showNoPermissionMessage(ctx);
                    return;
                }

                // Build the embed message with default values
                EmbedBuilder embed = new EmbedBuilder() {
                    Title = "This is an example title",
                    Description = "This is the description text.",
                    Image = @"https://i.imgur.com/07DVuUb.gif",
                    Thumbnail = @"https://i.imgur.com/sHL1DQQ.gif",
                    Footer = "Footer sample text",
                    HasTimeStamp = true,
                    Owner = ctx.User.Id
                };

                // Create the embed message
                await CreateEmbedMessageAsync(ctx, embed, EmbedType.PERMISSION, ctx.Interaction.Channel.Id, false);

            } catch (Exception ex) {
                throw new CommandException($"Embed.UseEmbedCommand: {ex}");
            }


        }

    }
}
