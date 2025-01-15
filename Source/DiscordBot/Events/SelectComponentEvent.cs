using APP.Utils;
using BLL.Enums;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System.Diagnostics;

namespace APP.Events {
    public class SelectComponentEvent {

        #region Fields
        private readonly IDataRepository dataService;
        private readonly DiscordUtil discordUtil;
        #endregion

        #region Constructors
        public SelectComponentEvent(IDataRepository dataService, DiscordUtil discordUtil) {
            this.dataService = dataService;
            this.discordUtil = discordUtil;
        }
        #endregion

        #region Events
        public async Task ComponentSelect(DiscordClient sender, ComponentInteractionCreateEventArgs e) {
            if (e.Interaction.Data.ComponentType == ComponentType.StringSelect) {
                if (e.Id == Identity.SELECTION_EMBED) await HandleEmbedInteraction(sender, e);
                if (e.Id == Identity.SELECTION_PLACEHOLDER) await PlaceholderInteraction(sender, e);
            }
        }
        #endregion

        #region Methods
        public async Task HandleEmbedInteraction(DiscordClient client, ComponentInteractionCreateEventArgs e) {

            var messageId = e.Message.Id;
            var message = await dataService.GetMessageAsync(e.Guild.Id, messageId);
            var embed = message.Embed;

            try {
                const string text = "Write something...";
                var exampleUrl = @"https://example.com/";
                var options = e.Values;
                var modal = new DiscordInteractionResponseBuilder();

                foreach (var option in options) {
                    switch (option) {
                        case Identity.SELECTION_CONTENT:
                            modal.WithTitle($"EDITING PLAIN CONTENT").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("MESSAGE CONTENT", Identity.MODAL_DATA_CONTENT, text, message.Content, false, TextInputStyle.Paragraph));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_TITLE:
                            modal.WithTitle($"EDITING TITLE").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("EMBED TITLE", Identity.MODAL_DATA_TITLE, text, embed.Title, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("EMBED TITLE LINK", Identity.MODAL_DATA_TITLE_LINK, exampleUrl, embed.TitleUrl, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_DESCRIPTION:
                            modal.WithTitle($"EDITING DESCRIPTION").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("EMBED DESCRIPTION", Identity.MODAL_DATA_DESCRIPTION, text, embed.Description, false, TextInputStyle.Paragraph));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_FOOTER:
                            modal.WithTitle($"EDITING FOOTER").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("FOOTER TEXT", Identity.MODAL_DATA_FOOTER, text, embed.Footer, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("FOOTER IMAGE", Identity.MODAL_DATA_FOOTER_URL, exampleUrl, embed.FooterUrl, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_AUTHOR:
                            modal.WithTitle($"EDITING AUTHOR").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("AUTHOR TEXT", Identity.MODAL_DATA_AUTHOR, text + " or self", embed.Author, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("AUTHOR LINK", Identity.MODAL_DATA_AUTHOR_LINK, exampleUrl, embed.AuthorLink, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("AUTHOR IMAGE", Identity.MODAL_DATA_AUTHOR_URL, exampleUrl, embed.AuthorUrl, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_COLOR:
                            modal.WithTitle($"EDITING COLOR").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("EMBED COLOR", Identity.MODAL_DATA_COLOR, text, embed.Color, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_IMAGE:
                            modal.WithTitle($"EDITING IMAGE").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("EMBED IMAGE", Identity.MODAL_DATA_IMAGE, exampleUrl, embed.Image, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_THUMBNAIL:
                            modal.WithTitle($"EDITING THUMBNAIL").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("EMBED THUMBNAIL", Identity.MODAL_DATA_THUMBNAIL, exampleUrl, embed.Thumbnail, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_PINGS:
                            string roleId = "836964332595707955, 1265749062183813242, everyone";
                            string userId = "836964332595707955, 1265749062183813242";
                            modal.WithTitle($"EDITING PINGED ROLES").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent($"ROLE PINGS", Identity.MODAL_DATA_PINGROLE, roleId, string.Join(", ", message.Roles), false, TextInputStyle.Paragraph));
                            modal.AddComponents(new TextInputComponent($"USER PINGS", Identity.MODAL_DATA_PINGUSER, userId, string.Join(", ", message.Users), false, TextInputStyle.Paragraph));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_TIMESTAMP:
                            embed.HasTimeStamp = !embed.HasTimeStamp;
                            await discordUtil.UpdateMessageAsync(e.Interaction, message);
                            break;
                        case Identity.SELECTION_FIELD_ADD:
                            modal.WithTitle($"ADDING FIELD TEXT").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TITLE", Identity.MODAL_DATA_FIELD_TITLE, text, null, true, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("TEXT", Identity.MODAL_DATA_FIELD_TEXT, text, null, true, TextInputStyle.Paragraph));
                            modal.AddComponents(new TextInputComponent("INLINE", Identity.MODAL_DATA_FIELD_INLINE, "True or False", "True", true, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_FIELD_REMOVE:
                            if (embed.Fields.Count() != 0) {
                                modal.WithTitle($"REMOVING FIELD").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                                modal.AddComponents(new TextInputComponent("EMBED FIELD INDEX", Identity.MODAL_DATA_FIELD_INDEX, $"Number from 1 to {embed.Fields.Count()}", null, true, TextInputStyle.Short, 1, 2));
                                await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            }
                            break;

                        case Identity.SELECTION_TEMPLATE_ADD:
                            modal.WithTitle("SAVE TEMPLATE").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TEMPLATE NAME", Identity.MODAL_DATA_TEMPLATE_ADD, text, null, true, TextInputStyle.Short, 3, 24));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        case Identity.SELECTION_TEMPLATE_REMOVE:
                            modal.WithTitle("REMOVE TEMPLATE").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TEMPLATE NAME", Identity.MODAL_DATA_TEMPLATE_ADD, text, null, true, TextInputStyle.Short, 3, 24));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        case Identity.SELECTION_TEMPLATE_LIST:
                            var template = await dataService.GetTemplateAsync(e.Guild.Id, TemplateMessage.TEMPLATES);
                            var templateMessage = template!.Message;
                            await discordUtil.CreateMessageAsync(e.Interaction, templateMessage);
                            break;

                        case Identity.SELECTION_TEMPLATE_USE:
                            modal.WithTitle("SELECT TEMPLATE").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TEMPLATE NAME", Identity.MODAL_DATA_TEMPLATE_USE, "/templates to view the available ones", null, true, TextInputStyle.Short, 3, 24));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        default:
                            await SendNotAFeatureYet(e.Interaction);
                            break;
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task PlaceholderInteraction(DiscordClient discordClient, ComponentInteractionCreateEventArgs e) {
            try {

                var messageId = e.Message.Id;
                var option = e.Values.First();
                var message = await dataService.GetMessageAsync(e.Guild.Id, messageId) ?? throw new EventException($"Message could not be found");
                var data = message.Data;
                var modal = new DiscordInteractionResponseBuilder();
                string placeholderText = "Write something...";
                string placeholerUrl = @"https://example.com/";
                string placeholderDate = "DD/MM/YYYY hh:mm";

                if (option.StartsWith(Identity.SELECTION_PLACEHOLDER_CUSTOM)) {
                    var group = option.Replace(Identity.SELECTION_PLACEHOLDER_CUSTOM, "");
                    var values = data.Where(x => x.Key.StartsWith(Placeholder.CUSTOM + group)).ToDictionary(x => x.Key, y => y.Value);
                    modal.WithTitle($"PLACEHOLDER {group.Replace(".", "").ToUpper()}").WithCustomId($"{Identity.MODAL_PLACEHOLDER};{option};{messageId}");
                    foreach (var item in values) {
                        var dataId = Placeholder.CUSTOM + group + item.Key.Substring(item.Key.LastIndexOf("."));
                        var modelId = Identity.MODAL_DATA_PLACEHOLDER_CUSTOM + group + item.Key.Substring(item.Key.LastIndexOf("."));
                        modal.AddComponents(new TextInputComponent(item.Key, modelId, null, data[dataId], false, TextInputStyle.Paragraph, 0, 1024));
                    }
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                    return;
                }

                switch (option) {

                    case Identity.SELECTION_PLACEHOLDER_ID:

                        var dataMessageId = data.ContainsKey(Placeholder.ID) ? data[Placeholder.ID] : "";

                        modal.WithTitle("PLACEHOLDER NAME").WithCustomId($"{Identity.MODAL_PLACEHOLDER};{option};{messageId}");
                        modal.AddComponents(new TextInputComponent(Placeholder.ID, Identity.MODAL_DATA_PLACEHOLDER_ID, string.IsNullOrWhiteSpace(dataMessageId) ? placeholderText : $"{dataMessageId} (current)", dataMessageId, false, TextInputStyle.Short, 0, 32));
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                        break;

                    case Identity.SELECTION_PLACEHOLDER_TIME:

                        var dataTimeZone = data.ContainsKey(Placeholder.TIMEZONE) ? data[Placeholder.TIMEZONE] : "";
                        var dataDateStart = data.ContainsKey(Placeholder.DATE_START) ? data[Placeholder.DATE_START] : DateTimeUtil.RoundDateTime(DateTime.Now).ToString("dd/MM/yyyy HH:mm");
                        var dataDateEnd = data.ContainsKey(Placeholder.DATE_END) ? data[Placeholder.DATE_END] : DateTimeUtil.RoundDateTime(DateTime.Now).ToString("dd/MM/yyyy HH:mm");

                        modal.WithTitle("PLACEHOLDER DATES").WithCustomId($"{Identity.MODAL_PLACEHOLDER};{option};{messageId}");
                        modal.AddComponents(new TextInputComponent(Placeholder.TIMEZONE, Identity.MODAL_DATA_PLACEHOLDER_TIMEZONE, $"Examples: CET, BST, GMT...", dataTimeZone, false, TextInputStyle.Short));
                        modal.AddComponents(new TextInputComponent(Placeholder.DATE_START, Identity.MODAL_DATA_PLACEHOLDER_DATE_START, placeholderDate, dataDateStart, false, TextInputStyle.Short, 16, 16));
                        modal.AddComponents(new TextInputComponent(Placeholder.DATE_END, Identity.MODAL_DATA_PLACEHOLDER_DATE_END, placeholderDate, dataDateEnd, false, TextInputStyle.Short, 16, 16));
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                        break;

                    case Identity.SELECTION_PLACEHOLDER_URLS:

                        var dataUrl1 = data.ContainsKey(Placeholder.URL1) ? data[Placeholder.URL1] : "";
                        var dataUrl2 = data.ContainsKey(Placeholder.URL2) ? data[Placeholder.URL2] : "";
                        var dataUrl3 = data.ContainsKey(Placeholder.URL3) ? data[Placeholder.URL3] : "";
                        var dataUrl4 = data.ContainsKey(Placeholder.URL4) ? data[Placeholder.URL4] : "";

                        modal.WithTitle("PLACEHOLDER URLS").WithCustomId($"{Identity.MODAL_PLACEHOLDER};{option};{messageId}");
                        modal.AddComponents(new TextInputComponent(Placeholder.URL1, Identity.MODAL_DATA_PLACEHOLDER_URL1, placeholerUrl, dataUrl1, false, TextInputStyle.Short));
                        modal.AddComponents(new TextInputComponent(Placeholder.URL2, Identity.MODAL_DATA_PLACEHOLDER_URL2, placeholerUrl, dataUrl2, false, TextInputStyle.Short));
                        modal.AddComponents(new TextInputComponent(Placeholder.URL3, Identity.MODAL_DATA_PLACEHOLDER_URL3, placeholerUrl, dataUrl3, false, TextInputStyle.Short));
                        modal.AddComponents(new TextInputComponent(Placeholder.URL4, Identity.MODAL_DATA_PLACEHOLDER_URL4, placeholerUrl, dataUrl4, false, TextInputStyle.Short));
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                        break;

                    case Identity.SELECTION_PLACEHOLDER_TEXTS:

                        var dataText1 = data.ContainsKey(Placeholder.TEXT1) ? data[Placeholder.TEXT1] : "";
                        var dataText2 = data.ContainsKey(Placeholder.TEXT2) ? data[Placeholder.TEXT2] : "";
                        var dataText3 = data.ContainsKey(Placeholder.TEXT3) ? data[Placeholder.TEXT3] : "";
                        var dataText4 = data.ContainsKey(Placeholder.TEXT4) ? data[Placeholder.TEXT4] : "";

                        modal.WithTitle("PLACEHOLDER TEXTS").WithCustomId($"{Identity.MODAL_PLACEHOLDER};{option};{messageId}");
                        modal.AddComponents(new TextInputComponent(Placeholder.TEXT1, Identity.MODAL_DATA_PLACEHOLDER_TEXT1, placeholderText, dataText1, false, TextInputStyle.Short));
                        modal.AddComponents(new TextInputComponent(Placeholder.TEXT2, Identity.MODAL_DATA_PLACEHOLDER_TEXT2, placeholderText, dataText2, false, TextInputStyle.Short));
                        modal.AddComponents(new TextInputComponent(Placeholder.TEXT3, Identity.MODAL_DATA_PLACEHOLDER_TEXT3, placeholderText, dataText3, false, TextInputStyle.Short));
                        modal.AddComponents(new TextInputComponent(Placeholder.TEXT4, Identity.MODAL_DATA_PLACEHOLDER_TEXT4, placeholderText, dataText4, false, TextInputStyle.Short));
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                        break;

                    case Identity.SELECTION_PLACEHOLDER_ADD:

                        modal.WithTitle("ADD PLACEHOLDER").WithCustomId($"{Identity.MODAL_PLACEHOLDER};{option};{messageId}");
                        modal.AddComponents(new TextInputComponent($"GROUP", Identity.MODAL_DATA_PLACEHOLDER_ADD_GROUP, placeholderText, null, true, TextInputStyle.Short));
                        modal.AddComponents(new TextInputComponent($"ID", Identity.MODAL_DATA_PLACEHOLDER_ADD_ID, placeholderText, null, true, TextInputStyle.Short));
                        modal.AddComponents(new TextInputComponent($"VALUE", Identity.MODAL_DATA_PLACEHOLDER_ADD_VALUE, placeholderText, null, true, TextInputStyle.Paragraph));
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                        break;

                    default:

                        await SendNotAFeatureYet(e.Interaction);
                        break;
                }


            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private async Task SendNotAFeatureYet(DiscordInteraction interaction) {
            var embedMessage = new DiscordEmbedBuilder()
                .WithAuthor("Feature doesn't work yet!", null, "https://cdn-icons-png.flaticon.com/512/2581/2581801.png")
                .WithColor(new DiscordColor("#d82b40"));

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(embedMessage)
                .AsEphemeral(true));
        }
        #endregion
    }
}
