using DSharpPlus.EventArgs;
using DSharpPlus;
using DSharpPlus.Entities;
using APP.Utils;
using BLL.Interfaces;
using BLL.Services;
using BLL.Model;
using BLL.Enums;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using BLL.Exceptions;
using System.Runtime.InteropServices;

namespace APP.Events {
    public class ButtonClickEvent {

        #region Fields
        private readonly IDataService dataService;
        public readonly DiscordUtil discordUtil;
        #endregion

        #region Constructors
        public ButtonClickEvent(IDataService dataService, DiscordUtil discordUtil) {
            this.dataService = dataService;
            this.discordUtil = discordUtil;
        }
        #endregion

        #region Methods
        public async Task ButtonClick(DiscordClient sender, ComponentInteractionCreateEventArgs e) {
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains(Identity.BUTTON_EMBED)) await UseEmbed(sender, e);
            }
            //if (e.Interaction.Data.ComponentType == ComponentType.Button) {
            //    if (e.Id.Contains(Identity.BUTTON_TEMPLATES)) await UseTemplate(e);
            //}
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains(Identity.BUTTON_NITRO)) await UseNitro(e);
            }
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains(Identity.BUTTON_EVENT)) await UseEvent(e);
            }
        }

        public async Task UseEmbed(DiscordClient discordClient, ComponentInteractionCreateEventArgs e) {
            try {
                // Get the message
                var messageId = e.Message.Id;
                var guildId = e.Guild.Id;
                var message = await dataService.GetMessageAsync(e.Guild.Id, messageId) ?? throw new Exception($"nawinwinr");

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders(e.Interaction, dataService);
                var embed = translated.Embed;

                // Roles to ping
                string pingRoles = await discordUtil.AddPingRolesToContentAsync(translated, e.Guild);

                switch (e.Id) {

                    case Identity.BUTTON_CHANNEL:
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        var channel = await discordUtil.GetChannelByIdAsync(e.Guild, message.ChannelId!);
                        var response = discordUtil.ResolveImageAttachment(translated);
                        if (!string.IsNullOrWhiteSpace(pingRoles)) response.WithContent(pingRoles);
                        var sentMessage = await channel.SendMessageAsync(response);
                        message.AddChild(sentMessage.Id, sentMessage.Channel.Id);
                        var sentMessageClone = message.DeepClone();
                        sentMessageClone.MessageId = sentMessage.Id;
                        sentMessageClone.ChannelId = sentMessage.Channel.Id;
                        sentMessageClone.ClearChilds();
                        await dataService.AddMessageAsync(sentMessageClone);
                        break;

                    case Identity.BUTTON_TEMPLATE:
                        var modal = new DiscordInteractionResponseBuilder();
                        modal.WithTitle("TEMPLATE SELECT").WithCustomId($"{Identity.MODAL_EMBED};{Identity.SELECTION_TEMPLATE_USE};{messageId}");
                        modal.AddComponents(new TextInputComponent("TEMPLATE NAME", Identity.MODAL_DATA_TEMPLATE_USE, "/templates to view the available ones", null, true, TextInputStyle.Short, 1, 32));
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                        break;

                    case Identity.BUTTON_UPDATE:
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        foreach (var m in message.Childs) {
                            var childChannel = await discordClient.GetChannelAsync(m.Value);
                            if (childChannel != null) {
                                var oldMessage = await childChannel.GetMessageAsync(m.Key);
                                if (oldMessage != null) {
                                    var newResponse = discordUtil.ResolveImageAttachment(translated);
                                    newResponse.WithContent(pingRoles);
                                    await oldMessage.ModifyAsync(newResponse);
                                }
                            }
                        }
                        break;

                    case Identity.BUTTON_CANCEL:
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        var deleteMessage = await e.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                        await deleteMessage.DeleteAsync();
                        await dataService.RemoveMessageAsync(message);
                        break;

                    default:
                        break;
                }

            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw new EventException("An error occured using the button for embed", ex);
            }
        }

        public async Task UseNitro(ComponentInteractionCreateEventArgs e) {
            try {

                // Get the message
                var messageId = e.Message.Id;
                var guildId = e.Guild.Id;
                var message = await dataService.GetMessageAsync(e.Guild.Id, messageId);
                var interaction = e.Interaction;

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders(e.Interaction, dataService);
                var embed = translated.Embed;

                // Initialize the modal
                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build())
                    .AsEphemeral(false);

                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                var nitroMessage = new DiscordFollowupMessageBuilder()
                    .WithContent("https://www.icegif.com/wp-content/uploads/2023/01/icegif-162.gif")
                    .AsEphemeral(true);
                await e.Interaction.CreateFollowupMessageAsync(nitroMessage.AsEphemeral(true));

            } catch (Exception ex) {
                throw new EventException("An error occured using the button for nitro", ex);
            }
        }

        public async Task UseEvent(ComponentInteractionCreateEventArgs e) {
            try {

                // Get the message
                var messageId = e.Message.Id;
                var guildId = e.Guild.Id;
                var message = await dataService.GetMessageAsync(e.Guild.Id, messageId);
                var interaction = e.Interaction;

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders(e.Interaction, dataService);
                var embed = translated.Embed;

                // Initialize the modal
                string datePlaceholder = "DD/MM/YYYY hh:mm";
                var modal = new DiscordInteractionResponseBuilder();
                modal.WithTitle($"SETUP EVENT").WithCustomId($"{Identity.MODAL_EVENT};{Identity.BUTTON_EVENT_SETUP};{messageId}");
                modal.AddComponents(new TextInputComponent("EVENT NAME", Identity.MODAL_DATA_EVENT_NAME, "Hide and Seek", null, true, TextInputStyle.Short));
                modal.AddComponents(new TextInputComponent("EVENT TIMEZONE", Identity.MODAL_DATA_EVENT_TIMEZONE, "CET | BST | GMT", message.Data[Placeholder.TIMEZONE], true, TextInputStyle.Short));
                modal.AddComponents(new TextInputComponent("EVENT START", Identity.MODAL_DATA_EVENT_START, datePlaceholder, message.Data[Placeholder.DATE_START], true, TextInputStyle.Short));
                modal.AddComponents(new TextInputComponent("EVENT END", Identity.MODAL_DATA_EVENT_END, datePlaceholder, message.Data[Placeholder.DATE_END], true, TextInputStyle.Short));
                await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);

            } catch (Exception ex) {
                throw new EventException("An error occured using the button for event", ex);
            }
        }
        #endregion

    }
}
