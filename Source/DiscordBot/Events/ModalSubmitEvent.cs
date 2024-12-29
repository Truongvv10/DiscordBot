using DSharpPlus.EventArgs;
using DSharpPlus;
using BLL.Exceptions;
using BLL.Enums;
using BLL.Model;
using APP.Utils;
using BLL.Interfaces;
using Microsoft.VisualBasic;
using DSharpPlus.Entities;
using System;
using APP.Enums;
using APP.Services;
using Notion.Client;
using static NodaTime.TimeZones.TzdbZone1970Location;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Threading.Channels;

namespace APP.Events {
    public class ModalSubmitEvent {

        #region Fields
        private readonly IDataRepository dataService;
        private readonly DiscordUtil discordUtil;
        #endregion

        #region Constructors
        public ModalSubmitEvent(IDataRepository dataService, DiscordUtil discordUtil) {
            this.dataService = dataService;
            this.discordUtil = discordUtil;
        }
        #endregion

        public async Task ModalSubmit(DiscordClient sender, ModalSubmitEventArgs e) {

            var interactionType = InteractionType.ModalSubmit;
            var modalId = e.Interaction.Data.CustomId;

            // Check if the interaction type is a modal submit
            if (interactionType == InteractionType.ModalSubmit) {

                if (modalId.Contains(Identity.MODAL_EMBED))
                    await UseEmbed(e);

                if (modalId.Contains(Identity.MODAL_EVENT))
                    await UseEvent(e);

                if (modalId.Contains(Identity.MODAL_PLACEHOLDER))
                    await UsePlaceholder(e);

                if (modalId.Contains(Identity.MODAL_TIMESTAMP))
                    await UseTimestamp(e);

                if (modalId.Contains(Identity.MODAL_INTRODUCTION))
                    await UseIntroduction(e);

                if (modalId.Contains(Identity.MODAL_INACTIVITY))
                    await UseInactivity(e);
            }
        }

        private async Task UseInactivity(ModalSubmitEventArgs e) {
            // Saving data to cache
            var message = dataService.GetCacheModalData(e.Interaction.Guild.Id, e.Interaction.User.Id);
            var channel = await discordUtil.GetChannelByIdAsync(e.Interaction.Guild, ulong.Parse(message.Data[Identity.INTERNAL_SEND_CHANNEL]));
            var start = e.Values[Identity.MODAL_DATA_INACTIVITY_START];
            var end = e.Values[Identity.MODAL_DATA_INACTIVITY_END];
            var reason = e.Values[Identity.MODAL_DATA_INACTIVITY_REASON];

            message.AddData($"{Placeholder.CUSTOM}.inactivity.start", start);
            message.AddData($"{Placeholder.CUSTOM}.inactivity.end", end);
            message.AddData($"{Placeholder.CUSTOM}.inactivity.reason", reason);

            var existingMessage = await discordUtil.GetMessageByIdAsync(channel, message.MessageId);
            if (existingMessage != null) {
                var timestamp = DateTimeUtil.ConvertDateTimeToDiscordTimestamp(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), TimeZoneEnum.CET, TimeZoneEnum.CET);
                message.WithContent($"**`Edited on`** {timestamp}");
                await discordUtil.UpdateMessageAsync(e.Interaction, message);
            } else {
                await discordUtil.CreateMessageToChannelAsync(e.Interaction, message, channel);
                await discordUtil.SendActionMessage(e.Interaction, TemplateMessage.ACTION_SUCCESS, $"Successfully created inactivity notice.");
            }
        }

        private async Task UseIntroduction(ModalSubmitEventArgs e) {
            var message = dataService.GetCacheModalData(e.Interaction.Guild.Id, e.Interaction.User.Id);
            var channel = await discordUtil.GetChannelByIdAsync(e.Interaction.Guild, ulong.Parse(message.Data[Identity.INTERNAL_SEND_CHANNEL]));
            await discordUtil.CreateMessageToChannelAsync(e.Interaction, message, channel);
            await discordUtil.SendActionMessage(e.Interaction, TemplateMessage.ACTION_SUCCESS, $"Successfully created introduction.");
        }

        private async Task UseEmbed(ModalSubmitEventArgs e) {
            try {
                // Variables
                var data = e.Values;
                var selection = e.Interaction.Data.CustomId.Split(";")[1];
                var messageId = ulong.Parse(e.Interaction.Data.CustomId.Split(";")[2]);
                var guildId = e.Interaction.Guild.Id;
                var message = await dataService.GetMessageAsync(guildId, messageId);
                var embed = message.Embed;

                // Check modals
                switch (selection) {

                    case Identity.SELECTION_TITLE:
                        embed.Title = data[Identity.MODAL_DATA_TITLE];
                        embed.TitleUrl = data[Identity.MODAL_DATA_TITLE_LINK];
                        message = await dataService.UpdateMessageAsync(message, selection);
                        break;

                    case Identity.SELECTION_DESCRIPTION:
                        embed.Description = data[Identity.MODAL_DATA_DESCRIPTION];
                        message = await dataService.UpdateMessageAsync(message, selection);
                        break;

                    case Identity.SELECTION_CONTENT:
                        message.Content = data[Identity.MODAL_DATA_CONTENT];
                        message = await dataService.UpdateMessageAsync(message, selection);
                        break;

                    case Identity.SELECTION_FOOTER:
                        embed.Footer = data[Identity.MODAL_DATA_FOOTER];
                        embed.FooterUrl = data[Identity.MODAL_DATA_FOOTER_URL];
                        message = await dataService.UpdateMessageAsync(message, selection);
                        break;

                    case Identity.SELECTION_AUTHOR:
                        if (e.Values.First().Value.Contains("self", StringComparison.OrdinalIgnoreCase)) {
                            embed.Author = e.Interaction.User.Username;
                            embed.AuthorLink = e.Interaction.User.DefaultAvatarUrl;
                            embed.AuthorUrl = e.Interaction.User.AvatarUrl;
                        } else {
                            embed.Author = data[Identity.MODAL_DATA_AUTHOR];
                            embed.AuthorLink = data[Identity.MODAL_DATA_AUTHOR_LINK];
                            embed.AuthorUrl = data[Identity.MODAL_DATA_AUTHOR_URL];
                        }
                        message = await dataService.UpdateMessageAsync(message, selection);
                        break;

                    case Identity.SELECTION_COLOR:
                        embed.Color = data[Identity.MODAL_DATA_COLOR];
                        await dataService.UpdateMessageAsync(message, selection);
                        break;

                    case Identity.SELECTION_IMAGE:
                        embed.Image = data[Identity.MODAL_DATA_IMAGE];
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        await discordUtil.ModifyMessageAsync(e.Interaction, message);
                        return;

                    case Identity.SELECTION_THUMBNAIL:
                        embed.Thumbnail = data[Identity.MODAL_DATA_THUMBNAIL];
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        await discordUtil.ModifyMessageAsync(e.Interaction, message);
                        return;

                    case Identity.SELECTION_PINGROLE:
                        List<ulong> pingIds = new();
                        foreach (var item in e.Values.First().Value.Split(",")) {
                            string trimmedItem = item.Trim();

                            // Check if the input is "everyone"
                            if (trimmedItem.Equals("everyone", StringComparison.OrdinalIgnoreCase)) {
                                // Add the @everyone role ID, which is the same as the guild ID
                                pingIds.Add(e.Interaction.Guild.Id);
                            }
                            // Check if the input is a valid ulong (role ID)
                            else if (ulong.TryParse(trimmedItem, out ulong roleid)) {
                                var role = await discordUtil.GetRolesByIdAsync(e.Interaction.Guild, roleid);
                                pingIds.Add(role.Id);
                            }
                        }
                        message.SetRole(pingIds.ToArray());
                        await dataService.UpdateMessageAsync(message, selection);
                        break;

                    case Identity.SELECTION_FIELD_ADD:
                        embed.AddField(e.Values[Identity.MODAL_DATA_FIELD_TITLE], e.Values[Identity.MODAL_DATA_FIELD_TEXT], bool.Parse(e.Values[Identity.MODAL_DATA_FIELD_INLINE]));
                        await dataService.UpdateMessageAsync(message, selection);
                        break;

                    case Identity.SELECTION_FIELD_REMOVE:
                        if (int.TryParse(e.Values[Identity.MODAL_DATA_FIELD_INDEX], out int index)) {
                            if ((index - 1) >= e.Values.Count() - 1) {
                                embed.RemoveFieldAt(index - 1);
                            }
                        }
                        await dataService.UpdateMessageAsync(message, selection);
                        break;

                    case Identity.SELECTION_TEMPLATE_ADD:
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        string newTemplateName = data[Identity.MODAL_DATA_TEMPLATE_ADD].TrimStart().TrimEnd().Replace(" ", "_").ToUpper();
                        if (string.IsNullOrWhiteSpace(newTemplateName)) throw new EventException($"Template \"{newTemplateName}\" can not be empty or null!");
                        var clone = message.DeepClone();
                        clone.GuildId = 0;
                        clone.MessageId = 0;
                        clone.ChannelId = 0;
                        clone.Sender = null;
                        clone.CreationDate = null;
                        clone.IsEphemeral = false;
                        clone.ClearChilds();
                        await dataService.AddTemplateAsync(new Template(guildId, newTemplateName, clone));
                        return;

                    case Identity.SELECTION_TEMPLATE_REMOVE:
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        string oldTemplateName = data[Identity.MODAL_DATA_TEMPLATE_ADD].TrimStart().TrimEnd().Replace(" ", "_").ToUpper();
                        var oldTemplate = await dataService.GetTemplateAsync(guildId, oldTemplateName) 
                            ?? throw new EventException($"There is no template with the name \"{data[Identity.MODAL_DATA_TEMPLATE_ADD]}\"");
                        await dataService.RemoveTemplateAsync(oldTemplate);
                        return;

                    case Identity.SELECTION_TEMPLATE_USE:
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        var templateUseName = data[Identity.MODAL_DATA_TEMPLATE_USE].TrimStart().TrimEnd().Replace(" ", "_").ToUpper();
                        var templateUse = await dataService.GetTemplateAsync(e.Interaction.Guild.Id, templateUseName)
                            ?? throw new EventException($"There is no template with the name \"{data[Identity.MODAL_DATA_TEMPLATE_ADD]}\"");
                        var templateUseMessage = templateUse.Message;
                        templateUseMessage.GuildId = message.GuildId;
                        templateUseMessage.MessageId = message.MessageId;
                        templateUseMessage.ChannelId = message.ChannelId;
                        templateUseMessage.IsEphemeral = message.IsEphemeral;
                        templateUseMessage.Childs = message.Childs;
                        await discordUtil.ModifyMessageAsync(e.Interaction, templateUseMessage);
                        return;

                    default:
                        break;
                }

                // Create the response
                await discordUtil.UpdateMessageAsync(e.Interaction, message);

            } catch (UtilException ex) {
                throw new EventException(ex.Message);

            } catch (Exception ex) {
                 throw new EventException($"An error has occured while submitting modal", ex);
            }

        }

        private async Task UseEvent(ModalSubmitEventArgs e) {
            try {
                // Variables
                var data = e.Values;
                var selection = e.Interaction.Data.CustomId.Split(";")[1];
                var messageId = ulong.Parse(e.Interaction.Data.CustomId.Split(";")[2]);
                var guildId = e.Interaction.Guild.Id;
                var message = await dataService.GetMessageAsync(guildId, messageId) ?? throw new EventException($"Message could not be fetched");

                // Check modals
                switch (selection) {

                    case Identity.BUTTON_EVENT_SETUP:
                        var name = data[Identity.MODAL_DATA_EVENT_NAME];
                        var timezone = data[Identity.MODAL_DATA_EVENT_TIMEZONE];
                        var start = data[Identity.MODAL_DATA_EVENT_START];
                        var end = data[Identity.MODAL_DATA_EVENT_END];

                        if (!DateTimeUtil.ExistInNodaTimeZone(timezone)) throw new EventException($"\"{timezone}\" is not a valid timezone.");
                        if (!DateTimeUtil.IsStringValidDateTime(start)) throw new EventException($"\"{start}\" was not a valid date.");
                        if (!DateTimeUtil.IsStringValidDateTime(end)) throw new EventException($"\"{end}\" was not a valid date.");

                        message.AddData(Placeholder.ID, name);
                        message.AddData(Placeholder.TIMEZONE, timezone);
                        message.AddData(Placeholder.DATE_START, start);
                        message.AddData(Placeholder.DATE_END, end);

                        message.ClearComponents();
                        message.AddButton(ComponentButtons.EMBED);
                        message.AddSelectOption(ComponentSelectOptions.DEFAULT);
                        message.AddSelectOption(ComponentSelectOptions.PLACEHOLDER);

                        break;

                    default:
                        break;
                }

                // Create the response
                message = await dataService.UpdateMessageAsync(message, Identity.SELECTION_PLACEHOLDER);
                await discordUtil.UpdateMessageAsync(e.Interaction, message);

            } catch (UtilException ex) {
                throw new EventException(ex.Message);

            } catch (Exception ex) {
                throw new EventException($"An error has occured while submitting modal", ex);
            }

        }

        private async Task UsePlaceholder(ModalSubmitEventArgs e) {
            try {
                // Variables
                var data = e.Values;
                var selection = e.Interaction.Data.CustomId.Split(";")[1];
                var messageId = ulong.Parse(e.Interaction.Data.CustomId.Split(";")[2]);
                var guildId = e.Interaction.Guild.Id;
                var message = await dataService.GetMessageAsync(guildId, messageId) ?? throw new EventException($"Message could not be fetched");


                if (selection.StartsWith(Identity.SELECTION_PLACEHOLDER_CUSTOM)) {
                    foreach (var kvp in data) {
                        var key = kvp.Key.Replace(Identity.MODAL_DATA_PLACEHOLDER_CUSTOM, "data.custom");
                        if (!string.IsNullOrWhiteSpace(kvp.Value)) message.AddData(key, kvp.Value);
                        else { if (message.Data.ContainsKey(key)) message.RemoveData(key); };
                    }
                }

                // Check modals
                switch (selection) {

                    case Identity.SELECTION_PLACEHOLDER_ID:
                        var name = data[Identity.MODAL_DATA_PLACEHOLDER_ID];
                        message.AddData(Placeholder.ID, name);
                        break;

                    case Identity.SELECTION_PLACEHOLDER_TIME:
                        var timezone = data[Identity.MODAL_DATA_PLACEHOLDER_TIMEZONE];
                        var start = data[Identity.MODAL_DATA_PLACEHOLDER_DATE_START];
                        var end = data[Identity.MODAL_DATA_PLACEHOLDER_DATE_END];

                        if (!DateTimeUtil.ExistInNodaTimeZone(timezone)) throw new EventException($"\"{timezone}\" is not a valid timezone.");
                        if (!DateTimeUtil.IsStringValidDateTime(start)) throw new EventException($"\"{start}\" was not a valid date.");
                        if (!DateTimeUtil.IsStringValidDateTime(end)) throw new EventException($"\"{end}\" was not a valid date.");

                        if (!string.IsNullOrWhiteSpace(timezone)) message.AddData(Placeholder.TIMEZONE, timezone);
                        else { if (message.Data.ContainsKey(Placeholder.TIMEZONE)) message.RemoveData(Placeholder.TIMEZONE); };
                        if (!string.IsNullOrWhiteSpace(start)) message.AddData(Placeholder.DATE_START, start);
                        else { if (message.Data.ContainsKey(Placeholder.DATE_START)) message.RemoveData(Placeholder.DATE_START); };
                        if (!string.IsNullOrWhiteSpace(end)) message.AddData(Placeholder.DATE_END, end);
                        else { if (message.Data.ContainsKey(Placeholder.DATE_END)) message.RemoveData(Placeholder.DATE_END); };

                        break;

                    case Identity.SELECTION_PLACEHOLDER_URLS:
                        var dataUrl1 = data[Identity.MODAL_DATA_PLACEHOLDER_URL1];
                        var dataUrl2 = data[Identity.MODAL_DATA_PLACEHOLDER_URL1];
                        var dataUrl3 = data[Identity.MODAL_DATA_PLACEHOLDER_URL1];
                        var dataUrl4 = data[Identity.MODAL_DATA_PLACEHOLDER_URL1];

                        if (!string.IsNullOrWhiteSpace(dataUrl1)) message.AddData(Placeholder.URL1, dataUrl1);
                        else { if (message.Data.ContainsKey(Placeholder.URL1)) message.RemoveData(Placeholder.URL1); };
                        if (!string.IsNullOrWhiteSpace(dataUrl2)) message.AddData(Placeholder.URL2, dataUrl2);
                        else { if (message.Data.ContainsKey(Placeholder.URL2)) message.RemoveData(Placeholder.URL2); };
                        if (!string.IsNullOrWhiteSpace(dataUrl3)) message.AddData(Placeholder.URL3, dataUrl3);
                        else { if (message.Data.ContainsKey(Placeholder.URL3)) message.RemoveData(Placeholder.URL3); };
                        if (!string.IsNullOrWhiteSpace(dataUrl4)) message.AddData(Placeholder.URL4, dataUrl4);
                        else { if (message.Data.ContainsKey(Placeholder.URL4)) message.RemoveData(Placeholder.URL4); };

                        break;

                    case Identity.SELECTION_PLACEHOLDER_TEXTS:

                        var dataText1 = data[Identity.MODAL_DATA_PLACEHOLDER_TEXT1];
                        var dataText2 = data[Identity.MODAL_DATA_PLACEHOLDER_TEXT2];
                        var dataText3 = data[Identity.MODAL_DATA_PLACEHOLDER_TEXT3];
                        var dataText4 = data[Identity.MODAL_DATA_PLACEHOLDER_TEXT4];

                        if (!string.IsNullOrWhiteSpace(dataText1)) message.AddData(Placeholder.TEXT1, dataText1);
                        else { if (message.Data.ContainsKey(Placeholder.TEXT1)) message.RemoveData(Placeholder.TEXT1); };
                        if (!string.IsNullOrWhiteSpace(dataText2)) message.AddData(Placeholder.TEXT2, dataText2);
                        else { if (message.Data.ContainsKey(Placeholder.TEXT2)) message.RemoveData(Placeholder.TEXT2); };
                        if (!string.IsNullOrWhiteSpace(dataText3)) message.AddData(Placeholder.TEXT3, dataText3);
                        else { if (message.Data.ContainsKey(Placeholder.TEXT3)) message.RemoveData(Placeholder.TEXT3); };
                        if (!string.IsNullOrWhiteSpace(dataText4)) message.AddData(Placeholder.TEXT4, dataText4);
                        else { if (message.Data.ContainsKey(Placeholder.TEXT4)) message.RemoveData(Placeholder.TEXT4); };

                        break;

                    case Identity.SELECTION_PLACEHOLDER_ADD:
                        var dataGroup = data[Identity.MODAL_DATA_PLACEHOLDER_ADD_GROUP].Replace(" ", "_").ToLower();
                        var dataId = data[Identity.MODAL_DATA_PLACEHOLDER_ADD_ID].Replace(" ", "_").ToLower();
                        var dataValue = data[Identity.MODAL_DATA_PLACEHOLDER_ADD_VALUE].ToLower();

                        if (!string.IsNullOrWhiteSpace(dataGroup) && !string.IsNullOrWhiteSpace(dataId) && !string.IsNullOrWhiteSpace(dataValue))
                            message.AddData($"{Placeholder.CUSTOM}.{dataGroup}.{dataId}", dataValue);
                        else throw new EventException($"Group \"{dataGroup}\" and value \"{dataValue}\" can not be null.");

                        break;

                    default:
                        break;
                }

                // Create the response
                message = await dataService.UpdateMessageAsync(message, Identity.SELECTION_PLACEHOLDER);
                await discordUtil.UpdateMessageAsync(e.Interaction, message);

            } catch (UtilException ex) {
                throw new EventException(ex.Message);

            } catch (Exception ex) {
                throw new EventException($"An error has occured while submitting modal", ex);
            }

        }

        private async Task UseTimestamp(ModalSubmitEventArgs e) {
            try {
                // Variables
                var data = e.Values;
                var template = await dataService.GetTemplateAsync(e.Interaction.Guild.Id, TemplateMessage.TIMESTAMP);
                var message = template.Message;

                // Check modals
                var timeZone = data[Identity.MODAL_DATA_TIMESTAMPS_TIMEZONE];
                var time = data[Identity.MODAL_DATA_TIMESTAMPS_TIME];

                // Check if the time zone exists & time format is correct
                if (!DateTimeUtil.ExistInNodaTimeZone(timeZone))
                    throw new EventException($"Time zone \"{timeZone}\" can not be found.");
                if (!DateTime.TryParseExact(time, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedTime))
                    throw new EventException($"Time \"{time}\" has an incorrect format.");

                message.AddData(Placeholder.TIMEZONE, timeZone);
                message.AddData(Placeholder.DATE_START, parsedTime.ToString("dd/MM/yyyy HH:mm"));

                await discordUtil.CreateMessageAsync(e.Interaction, message);

            } catch (UtilException ex) {
                throw new EventException(ex.Message);

            } catch (Exception ex) {
                throw new EventException($"An error has occured while submitting modal", ex);
            }
        }

    }
}
