using DiscordBot.Exceptions;
using DiscordBot.Model;
using DiscordBot.Model.Enums;
using DiscordBot.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Diagnostics;
using System.Threading.Channels;

namespace XironiteDiscordBot.Commands.Slash {
    public class EmbedCmd : SlashCommand {

        [SlashCommand("embed", "Send an embeded message to the current channel")]
        public async Task UseEmbedCommand(InteractionContext ctx,
            [Option("channel", "The channel where your embeded message will be sent to.")] DiscordChannel channel,
            [Option("hidden", "If only you can see the embeded message being edited.")] bool hidden = false,
            [Option("image", "The main image of your embeded message that will be added.")] DiscordAttachment? image = null,
            [Option("thumbnail", "The thumbnail of your embeded message that will be added.")] DiscordAttachment? thumbnail = null,
            [Option("ping", "The role that will get pinged on sending message.")] DiscordRole? pingrole = null) {

            try {
                // Check if user has permission to use command
                if (!await CheckPermission(ctx, CommandEnum.EMBED)) {
                    await showNoPermissionMessage(ctx);
                    return;
                }

                // Log the command the user is using
                LogCommand(ctx, CommandEnum.EMBED);

                // Build the embed message with default values
                EmbedBuilder embed = new EmbedBuilder() {
                    Title = "This is an example title",
                    Description = "This is the description text.",
                    Image = @"https://i.imgur.com/07DVuUb.gif",
                    Thumbnail = @"https://i.imgur.com/sHL1DQQ.gif",
                    Footer = "Footer sample text",
                    HasTimeStamp = true,
                    ChannelId = channel.Id,
                    Owner = ctx.User.Id
                };
                if (image is not null) embed.Image = image.Url;
                if (thumbnail is not null) embed.Thumbnail = thumbnail.Url;
                if (pingrole is not null) embed.AddPingRole(pingrole.Id);

                // Create the embed message
                await CreateEmbedMessageAsync(ctx, embed, EmbedType.DEFAULT, channel.Id, hidden);

            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw new CommandException($"Embed.UseEmbedCommand: {ex}");
            }
        }

        [SlashCommand("embed-edit", "Edit an existing embeded message from the bot")]
        public async Task UseEmbedEditCommand(InteractionContext ctx,
            [Option("message-id", "The id of the embeded message you want to edit.")] string id) {

            if (!await CheckPermission(ctx, CommandEnum.EMBED_EDIT)) {
                await showNoPermissionMessage(ctx);
                return;
            }
            LogCommand(ctx, CommandEnum.EMBED_EDIT);
            try {
                if (ulong.TryParse(id, out ulong messageid)) {
                    var embed = await CacheData.GetEmbedAsync(ctx.Guild.Id, messageid);
                    await CreateEmbedMessageAsync(ctx, embed, embed.Type, embed.ChannelId, true);
                }
            } catch (Exception ex) {
                throw new CommandException($"Embed.UseEmbedEditCommand: {ex}");
            }
        }
    }
}
