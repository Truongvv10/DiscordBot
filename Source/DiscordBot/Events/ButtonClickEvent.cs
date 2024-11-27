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
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains(Identity.BUTTON_TEMPLATES)) await UseTemplate(e);
            }
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains(Identity.BUTTON_NITRO)) await UseNitro(e);
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

                //if (e.Id == "embedButtonEventPostCreate") {
                //    modal.WithTitle("EVENT CREATION").WithCustomId($"{Identity.MODAL_EVENT};{Identity.SELECTION_EVENT_CREATION};{messageId}");
                //    modal.AddComponents(new TextInputComponent("EVENT NAME", Identity.EVENT_NAME, "Write something...", message.Data[Identity.EVENT_NAME] as string, true, TextInputStyle.Short, 4, 32));
                //    modal.AddComponents(new TextInputComponent("TIME ZONE", Identity.EVENT_TIMEZONE, "Europe/Brussels", message.Data[Identity.EVENT_TIMEZONE] as string, true, TextInputStyle.Short));
                //    modal.AddComponents(new TextInputComponent("START DATE", Identity.EVENT_START, "DD/MM/YYYY hh:mm", message.Data[Identity.EVENT_START] as string, true, TextInputStyle.Short));
                //    modal.AddComponents(new TextInputComponent("END DATE", Identity.EVENT_END, "DD/MM/YYYY hh:mm", message.Data[Identity.EVENT_END] as string, true, TextInputStyle.Short));
                //    await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                //}

                //if (e.Id == "embedButtonTemplateUse") {
                //    modal.WithTitle("USE TEMPLATE").WithCustomId($"{Identity.MODAL_TEMPLATES};{Identity.SELECTION_TEMPLATE_INPUT};{message.Data[Identity.TEMPLATE_REPLACE_MESSAGE_ID]}");
                //    modal.AddComponents(new TextInputComponent("TEMPLATE", Identity.TEMPLATE_NAME, "The template you want to use", null, true, TextInputStyle.Short, 1, 64));
                //    await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                //}

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
                var modal = new DiscordInteractionResponseBuilder();
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

        public async Task UseTemplate(ComponentInteractionCreateEventArgs e) {
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
                var modal = new DiscordInteractionResponseBuilder();
                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build())
                    .AsEphemeral(false);


                switch (e.Id) {

                    case Identity.BUTTON_TEMPLATES_ADD:
                        modal.WithTitle("TEMPLATE SAVE").WithCustomId($"{Identity.MODAL_TEMPLATES};{Identity.SELECTION_EVENT_CREATION};{messageId}");
                        modal.AddComponents(new TextInputComponent("TEMPLATE NAME", Identity.MODAL_DATA_TEMPLATES_ADD_NAME, "Unique name id for saved template", null, true, TextInputStyle.Short, 1, 32));
                        modal.AddComponents(new TextInputComponent("MESSAGE ID", Identity.MODAL_DATA_TEMPLATES_ADD_MESSAGE, "Id of the message that will be saved", null, true, TextInputStyle.Short, 1, 32));
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                        break;

                    case Identity.BUTTON_TEMPLATES_SELECT:
                        modal.WithTitle("TEMPLATE SELECT").WithCustomId($"{Identity.MODAL_TEMPLATES};{Identity.SELECTION_EVENT_CREATION};{messageId}");
                        modal.AddComponents(new TextInputComponent("TEMPLATE NAME", Identity.MODAL_DATA_TEMPLATES_ADD_NAME, "/templates to view the available ones", null, true, TextInputStyle.Short, 1, 32));
                        modal.AddComponents(new TextInputComponent("MESSAGE ID", Identity.MODAL_DATA_TEMPLATES_ADD_MESSAGE, "Message that will using this template", null, true, TextInputStyle.Short, 1, 32));
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                        break;

                    case Identity.BUTTON_TEMPLATES_DELETE:
                        modal.WithTitle("TEMPLATE DELETE").WithCustomId($"{Identity.MODAL_TEMPLATES};{Identity.SELECTION_EVENT_CREATION};{messageId}");
                        modal.AddComponents(new TextInputComponent("TEMPLATE NAME", Identity.MODAL_DATA_TEMPLATES_REMOVE_NAME, "/templates to view the available ones", null, true, TextInputStyle.Short, 1, 32));
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                        break;

                    case Identity.BUTTON_TEMPLATES_CANCEL:
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        var sentMessage = await e.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                        await sentMessage.DeleteAsync();
                        await dataService.RemoveMessageAsync(message);
                        break;

                    default:
                        break;
                }

            } catch (Exception ex) {
                throw new EventException("An error occured using the button for templates", ex);
            }
        }

        private async Task TemplateButtonClick(DiscordInteraction interaction, Message message) {
            var guildId = interaction.Guild.Id;
            var templates = await dataService.GetTemplateAsync(guildId, Identity.TDATA_USE);
            if (templates != null) {
                var template = templates.Message;
                template.MessageId = message.MessageId;
                template.GuildId = message.GuildId;
                template.ChannelId = message.ChannelId;
                template.Sender = message.Sender;
                template.IsEphemeral = message.IsEphemeral;
                template.AddData(Placeholder.TEMPLATE, JsonConvert.SerializeObject(message));
                message = template;
            }
            await discordUtil.ModifyMessageAsync(CommandEnum.TEMPLATE_USE, interaction, message, interaction.Channel.Id);
        }

        #endregion

        //private async Task<bool> CheckPermission(ComponentInteractionCreateEventArgs e, CommandEnum cmd, ulong ownerid) {

        //    var permission = await CacheData.GetPermission(e.Guild.Id, cmd);
        //    var userid = e.User.Id;
        //    var user = await e.Guild.GetMemberAsync(userid);
        //    var roles = user.Roles;

        //    if (user.Permissions.HasPermission(Permissions.All)) {
        //        return true;
        //    } else if (permission.AllowAdministrator) {
        //        return true;
        //    } else if (e.User.Id == ownerid) {
        //        return true;
        //    } else {
        //        return false;
        //    }
        //}

    }
}
