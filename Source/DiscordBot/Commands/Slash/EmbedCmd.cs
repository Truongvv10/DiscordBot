using APP.Attributes;
using APP.Services;
using APP.Utils;
using BLL.Enums;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using BLL.Services;
using DSharpPlus;
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
        private const string EMBED_TEMPLATES = "TEMPLATES";
        #endregion

        #region Properties
        public required IDataRepository DataService { private get; set; }
        public required DiscordUtil DiscordUtil { private get; set; }
        #endregion

        #region Command: /embed create
        [SlashCommand(EMBED_CREATE, "Send an editable embeded message to the current channel")]
        [RequirePermission(CommandEnum.EMBED, [Permissions.ManageChannels, Permissions.ManageMessages])]
        public async Task Create(InteractionContext ctx,
            [Option("sent_channel", "The channel where your embeded message will be sent to.")] DiscordChannel channel,
            [Option("hidden", "If only you can see this embeded message, default is false")] bool hidden = false,
            [Option("image", "The main image of your embeded message that will be added.")] DiscordAttachment? image = null,
            [Option("thumbnail", "The thumbnail of your embeded message that will be added.")] DiscordAttachment? thumbnail = null,
            [Option("ping", "The server role that will get pinged on sending message.")] DiscordRole? pingrole = null) {
            try {

                // Build the embed message with default values
                var template = await DataService.GetTemplateAsync(ctx.Guild.Id, TemplateMessage.EMBED);
                var message = template!.Message;
                message.AddData(Identity.INTERNAL_SEND_CHANNEL, channel.Id.ToString());
                message.Type = CommandEnum.EMBED_CREATE;

                // Add the optional values
                if (image is not null) message.Embed!.Image = image.Url;
                if (thumbnail is not null) message.Embed!.Thumbnail = thumbnail.Url;
                if (pingrole is not null) message.AddRole(pingrole.Id);

                // Create the embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.EMBED_CREATE, ctx.Interaction, message, hidden);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{EMBED} {EMBED_CREATE}", ex);
            }
        }
        #endregion

        #region Command: /embed edit
        [SlashCommand(EMBED_EDIT, "Edit an existing embeded message from the bot")]
        [RequirePermission(CommandEnum.EMBED_EDIT, [Permissions.ManageChannels, Permissions.ManageMessages])]
        public async Task Edit(InteractionContext ctx,
            [Option("message-id", "The id of the embeded message you want to edit.")] string id) {
            try {
                // Check if the id is a valid ulong
                if (ulong.TryParse(id, out ulong messageid)) {
                    var message = await DataService.GetMessageAsync(ctx.Guild.Id, messageid) ?? throw new CommandException($"Message with id \"{id}\" in guild \"{ctx.Guild.Id}\" was not found.");
                    var copy = message.DeepClone();
                    copy.Type = CommandEnum.EMBED_EDIT;
                    await DiscordUtil.CreateMessageAsync(CommandEnum.EMBED_EDIT, ctx.Interaction, copy, false);
                }
            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{EMBED} {EMBED_EDIT}", ex);
            }
        }
        #endregion

        #region Command: /embed templates
        [SlashCommand(EMBED_TEMPLATES, "View all available templates")]
        [RequirePermission(CommandEnum.EMBED_TEMPLATES, [Permissions.ManageChannels, Permissions.ManageMessages])]
        public async Task Templates(InteractionContext ctx) {
            try {
                // Build the embed message with default values
                var template = await DataService.GetTemplateAsync(ctx.Guild.Id, TemplateMessage.TEMPLATES);
                var message = template!.Message;

                // Create embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.TEMPLATES, ctx.Interaction, message, true);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{EMBED_TEMPLATES}", ex);
            }
        }
        #endregion
    }
}
