﻿using APP.Services;
using APP.Utils;
using BLL.Enums;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using BLL.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using Notion.Client;

namespace APP.Commands.Slash {

    [SlashCommandGroup("embed", "Commands for sending embeded messages")]
    public class EmbedCmd : ApplicationCommandModule {

        #region Fields
        private const string EMBED = "EMBED";
        private const string EMBED_CREATE = "CREATE";
        private const string EMBED_EDIT = "EDIT";
        #endregion

        #region Properties
        public required IDataService DataService { private get; set; }
        #endregion

        #region Command: /embed
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
                var template = await DataService.GetTemplateAsync(ctx.Guild.Id, $"{EMBED}_{EMBED_CREATE}");
                var message = template!.Message;
                message.ChannelId = channel.Id;
                message.Sender = ctx.User.Id;
                message.Type = CommandEnum.EMBED_CREATE;

                // Add the optional values
                if (image is not null) message.Embed!.Image = image.Url;
                if (thumbnail is not null) message.Embed!.Thumbnail = thumbnail.Url;
                if (pingrole is not null) message.AddRole(pingrole.Id);

                // Create the embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.EMBED_CREATE, ctx.Interaction, message, channel.Id, hidden);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{EMBED} {EMBED_CREATE}", ex);
            }
        }
        #endregion

        #region Command: /embed edit
        [SlashCommand(EMBED_EDIT, "Edit an existing embeded message from the bot")]
        [RequirePermission(CommandEnum.EMBED_EDIT)]
        public async Task Edit(InteractionContext ctx,
            [Option("message-id", "The id of the embeded message you want to edit.")] string id,
            [Option("hidden", "If only you can see this embeded message, default is true")] bool hidden = true) {
            try {
                // Check if the id is a valid ulong
                if (ulong.TryParse(id, out ulong messageid)) {
                    var message = await DataService.GetMessageAsync(ctx.Guild.Id, messageid);
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, (ulong)message.ChannelId!, hidden);
                }

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{EMBED} {EMBED_EDIT}", ex);
            }
        }
        #endregion
    }
}
