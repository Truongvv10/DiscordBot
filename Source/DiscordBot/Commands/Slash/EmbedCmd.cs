using DiscordBot.Exceptions;
using DiscordBot.Model;
using DiscordBot.Model.Enums;
using DiscordBot.Services;
using DiscordBot.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Channels;

namespace DiscordBot.Commands.Slash {

    [SlashCommandGroup("embed", "Commands for sending embeded messages")]
    public class EmbedCmd : SlashCommand {

        private const string EMBED = "EMBED";
        private const string EMBED_CREATE = "CREATE";
        private const string EMBED_EDIT = "EDIT";

        [SlashCommand(EMBED_CREATE, "Send an editable embeded message to the current channel")]
        [RequirePermission(CommandEnum.EMBED)]
        public async Task Create(InteractionContext ctx,
            [Option("sent_channel", "The channel where your embeded message will be sent to.")] DiscordChannel channel,
            [Option("hidden", "If only you can see this embeded message, default is false")] bool hidden = false,
            [Option("image", "The main image of your embeded message that will be added.")] DiscordAttachment? image = null,
            [Option("thumbnail", "The thumbnail of your embeded message that will be added.")] DiscordAttachment? thumbnail = null,
            [Option("ping", "The server role that will get pinged on sending message.")] DiscordRole? pingrole = null) {
            try {

                // Build the embed message with default values
                var template = $"{EMBED}_{EMBED_CREATE}";
                var embed = await CacheData.GetTemplate(ctx.Guild.Id, template);
                embed.ChannelId = channel.Id;
                embed.Owner = ctx.User.Id;
                embed.Author = ctx.User.Username;
                embed.Type = CommandEnum.EMBED_CREATE;

                // Add the optional values
                if (image is not null) embed.Image = image.Url;
                if (thumbnail is not null) embed.Thumbnail = thumbnail.Url;
                if (pingrole is not null) embed.AddPingRole(pingrole.Id);

                // Create the embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.EMBED_CREATE, ctx.Interaction, embed, channel.Id, hidden);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{EMBED} {EMBED_CREATE}", ex);
            }
        }

        [SlashCommand(EMBED_EDIT, "Edit an existing embeded message from the bot")]
        [RequirePermission(CommandEnum.EMBED_EDIT)]
        public async Task Edit(InteractionContext ctx, 
            [Option("message-id", "The id of the embeded message you want to edit.")] string id,
            [Option("hidden", "If only you can see this embeded message, default is true")] bool hidden = true) {

            try {

                // Check if the id is a valid ulong
                if (ulong.TryParse(id, out ulong messageid)) {
                    var embed = CacheData.GetEmbed(ctx.Guild.Id, messageid);
                    await DiscordUtil.CreateMessageAsync(embed.Type, ctx.Interaction, embed, (ulong)embed.ChannelId!, hidden);
                }

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{EMBED} {EMBED_EDIT}", ex);
            }
        }
    }
}
