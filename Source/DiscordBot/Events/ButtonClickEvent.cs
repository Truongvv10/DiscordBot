﻿using DSharpPlus.EventArgs;
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
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.CommandsNext.Attributes;

namespace APP.Events {
    public class ButtonClickEvent {

        #region Fields
        private readonly IDataRepository dataService;
        public readonly DiscordUtil discordUtil;
        #endregion

        #region Constructors
        public ButtonClickEvent(IDataRepository dataService, DiscordUtil discordUtil) {
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
                if (e.Id.Contains(Identity.BUTTON_NITRO)) await UseNitro(e);
            }
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains(Identity.BUTTON_EVENT)) await UseEvent(e);
            }
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains(Identity.BUTTON_INACTIVITY)) await UseInactivity(e);
            }
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains(Identity.BUTTON_INTRODUCTION)) await UseIntroduction(e);
            }
        }

        private async Task UseInactivity(ComponentInteractionCreateEventArgs e) {
            try {
                // Get the message
                var message = await dataService.GetMessageAsync(e.Guild.Id, e.Message.Id) ?? throw new Exception($"Message does not exists");
                var embed = message.Embed;
                var user = e.Interaction.User;

                // Translate the placeholders
                switch (e.Id) {
                    case Identity.BUTTON_INACTIVITY_SEEN:
                        if (message.Users.Contains(user.Id))
                            message.RemoveUser(user.Id);
                        else message.AddUser(user.Id);
                        await discordUtil.UpdateMessageAsync(e.Interaction, message);
                        break;
                    case Identity.BUTTON_INACTIVITY_EDIT:
                        if (message.Sender == e.User.Id) {
                            var modal = new DiscordInteractionResponseBuilder();
                            modal.WithTitle($"INACTIVE NOTICE")
                                .WithCustomId(Identity.MODAL_INACTIVITY)
                                .AddComponents(new TextInputComponent("START DATE", Identity.MODAL_DATA_INACTIVITY_START, "day/month/year", message.Data[$"{Placeholder.CUSTOM}.inactivity.start"], true))
                                .AddComponents(new TextInputComponent("END DATE", Identity.MODAL_DATA_INACTIVITY_END, "day/month/year", message.Data[$"{Placeholder.CUSTOM}.inactivity.end"], true))
                                .AddComponents(new TextInputComponent("REASON", Identity.MODAL_DATA_INACTIVITY_REASON, "Reason of inactivity...", message.Data[$"{Placeholder.CUSTOM}.inactivity.reason"], true, TextInputStyle.Paragraph, 0, 2048));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            dataService.AddCacheModalData(e.Guild.Id, e.User.Id, message);
                        } else {
                            await discordUtil.SendActionMessage(e.Interaction, TemplateMessage.ACTION_FAILED, "This is not your inactivity notice.");
                        }
                        break;
                    default:
                        break;
                }
            } catch (Exception ex) {
                throw new EventException("An error occured using the button for inactivity", ex);
            }
        }

        private async Task UseIntroduction(ComponentInteractionCreateEventArgs e) {
            try {
                // Get the message
                var message = await dataService.GetMessageAsync(e.Guild.Id, e.Message.Id) ?? throw new Exception($"Message does not exists");
                var embed = message.Embed;
                var user = e.Interaction.User;
                var member = await e.Guild.GetMemberAsync(user.Id);
                var button = e.Id;

                // Interaction response
                if (message.Sender == e.User.Id || (member.Permissions.HasPermission(Permissions.All) || member.Permissions.HasPermission(Permissions.Administrator))) {
                    switch (button) {
                        case Identity.BUTTON_INTRODUCTION_REMOVE:
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                            var deleteMessage = await e.Interaction.GetOriginalResponseAsync();
                            await deleteMessage.DeleteAsync();
                            await dataService.RemoveMessageAsync(message);
                            break;
                        case Identity.BUTTON_INTRODUCTION_EDIT:
                            var modal = new DiscordInteractionResponseBuilder();
                            modal.WithTitle($"INTRODUCTION NOTICE")
                                .WithCustomId($"{Identity.MODAL_INTRODUCTION};{button}")
                                .AddComponents(new TextInputComponent("COUNTRY", Identity.MODAL_DATA_INTRODUCTION_COUNTRY, null, message.Data[$"{Placeholder.CUSTOM}.introduction.country"], true, TextInputStyle.Short, 0, 128))
                                .AddComponents(new TextInputComponent("BIRTHDAY", Identity.MODAL_DATA_INTRODUCTION_BIRTHDAY, null, message.Data[$"{Placeholder.CUSTOM}.introduction.birthday"].Substring(0, 10), true, TextInputStyle.Short, 10, 10))
                                .AddComponents(new TextInputComponent("PRONOUNS", Identity.MODAL_DATA_INTRODUCTION_PRONOUNS, null, message.Data[$"{Placeholder.CUSTOM}.introduction.pronouns"], true, TextInputStyle.Short, 0, 32))
                                .AddComponents(new TextInputComponent("TEXT", Identity.MODAL_DATA_INTRODUCTION_TEXT, null, message.Data[$"{Placeholder.CUSTOM}.introduction.text"], true, TextInputStyle.Paragraph, 0, 2048));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            dataService.AddCacheModalData(e.Guild.Id, user.Id, message);
                            break;
                        default:
                            break;
                    }
                } else {
                    await discordUtil.SendActionMessage(e.Interaction, TemplateMessage.ACTION_FAILED, "This is not your introduction.");
                }
            } catch (Exception ex) {
                throw new EventException("An error occured using the button for introduction", ex);
            }
        }

        public async Task UseEmbed(DiscordClient discordClient, ComponentInteractionCreateEventArgs e) {
            try {
                // Get the message
                var messageId = e.Message.Id;
                var guildId = e.Guild.Id;
                var message = await dataService.GetMessageAsync(e.Guild.Id, messageId) ?? throw new Exception($"Message does not exists");

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders(e.Interaction, dataService);
                var embed = translated.Embed;

                switch (e.Id) {

                    case Identity.BUTTON_CHANNEL:
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        var channel = await discordUtil.GetChannelByIdAsync(e.Guild, message.ChannelId!);
                        var response = discordUtil.ResolveImageAttachment(translated);
                        if (message.Data.ContainsKey(Identity.INTERNAL_SEND_CHANNEL)) {
                            channel = await discordUtil.GetChannelByIdAsync(e.Guild, ulong.Parse(message.Data[Identity.INTERNAL_SEND_CHANNEL]));
                        }
                        if (message.Users.Count() > 0) {
                            var mentions = message.Users.Select(x => new UserMention(x)).ToList();
                            response.AddMentions(mentions.Cast<IMention>());
                        }
                        if (message.Roles.Count() > 0) {
                            var roles = message.Roles.Select(x => new RoleMention(x)).ToList();
                            response.AddMentions(roles.Cast<IMention>());
                        }
                        if (!string.IsNullOrWhiteSpace(translated.Content)) response.WithContent(translated.Content);
                        var sentMessage = await channel.SendMessageAsync(response);
                        message.AddChild(sentMessage.Id, sentMessage.Channel.Id);
                        var sentMessageClone = message.DeepClone();
                        sentMessageClone.MessageId = sentMessage.Id;
                        sentMessageClone.ChannelId = sentMessage.Channel.Id;
                        sentMessageClone.Sender = e.User.Id;
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
                                    if (!string.IsNullOrWhiteSpace(translated.Content)) newResponse.WithContent(translated.Content);
                                    var newMessage = await oldMessage.ModifyAsync(newResponse);
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
                if (e.Id == Identity.BUTTON_CANCEL) {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    var deleteMessage = await e.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                    await deleteMessage.DeleteAsync();
                    return;
                }
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
