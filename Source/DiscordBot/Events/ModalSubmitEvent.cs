using DSharpPlus.EventArgs;
using DSharpPlus;
using BLL.Exceptions;
using BLL.Enums;
using BLL.Model;
using APP.Utils;
using BLL.Interfaces;
using Microsoft.VisualBasic;

namespace APP.Events {
    public class ModalSubmitEvent {

        #region Fields
        private readonly IDataService dataService;
        private readonly DiscordUtil discordUtil;
        #endregion

        #region Constructors
        public ModalSubmitEvent(IDataService dataService, DiscordUtil discordUtil) {
            this.dataService = dataService;
            this.discordUtil = discordUtil;
        }
        #endregion

        public async Task ModalSubmit(DiscordClient sender, ModalSubmitEventArgs e) {

            // Check if the interaction type is a modal submit
            if (e.Interaction.Type == InteractionType.ModalSubmit) {

                // Check if the custom ID contains "embed modal"
                if (e.Interaction.Data.CustomId.Contains(Identity.MODAL_EMBED))
                    await UseEmbed(e);

                // Check if the custom ID contains "event modal"
                if (e.Interaction.Data.CustomId.Contains(Identity.MODAL_EVENT))
                    await UseEvent(e);

                // Check if the custom ID contains "template modal"
                if (e.Interaction.Data.CustomId.Contains(Identity.MODAL_TEMPLATES)) { };
                //await UseTemplate(e);

                // Check if the custom ID contains "timestamp modal"
                if (e.Interaction.Data.CustomId.Contains(Identity.MODAL_TIMESTAMP))
                    await UseTimestamp(e);

            }
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
                        await discordUtil.ModifyMessageAsync(message.Type, e.Interaction, message, (ulong)message.ChannelId!, message.IsEphemeral);
                        return;

                    case Identity.SELECTION_THUMBNAIL:
                        embed.Thumbnail = data[Identity.MODAL_DATA_THUMBNAIL];
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        await discordUtil.ModifyMessageAsync(message.Type, e.Interaction, message, (ulong)message.ChannelId!, message.IsEphemeral);
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
                        templateUseMessage.Type = message.Type;
                        templateUseMessage.GuildId = message.GuildId;
                        templateUseMessage.MessageId = message.MessageId;
                        templateUseMessage.ChannelId = message.ChannelId;
                        templateUseMessage.IsEphemeral = message.IsEphemeral;
                        templateUseMessage.Childs = message.Childs;
                        await discordUtil.ModifyMessageAsync(templateUseMessage.Type, e.Interaction, templateUseMessage, templateUseMessage.ChannelId, templateUseMessage.IsEphemeral);
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
                var message = await dataService.GetMessageAsync(guildId, messageId);
                var embed = message.Embed!;

                // Check modals
                switch (selection) {

                    case Identity.SELECTION_EVENT_PROPERTIES:

                        string eventName = data[Identity.EVENT_NAME];
                        string timezone = data[Identity.EVENT_TIMEZONE];
                        string start = data[Identity.EVENT_START];
                        string end = data[Identity.EVENT_END];

                        if (DateTime.TryParseExact(start, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDateTime)) {
                            message.SetData(Identity.EVENT_START, start);
                        } else throw new EventException($"Date \"{start}\" has incorrect format");

                        if (DateTime.TryParseExact(end, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDateTime)) {
                            message.SetData(Identity.EVENT_END, end);
                        } else throw new EventException($"Time \"{end}\" has incorrect format");

                        message.SetData(Identity.EVENT_TIMEZONE, timezone);
                        message.SetData(Identity.EVENT_NAME, eventName);
                        embed.Description = await BuildEventDesciption(message);
                        break;

                    case Identity.SELECTION_EVENT_INTRODUCTION:

                        string title = data[Identity.EVENT_TITLE];
                        string intro = data[Identity.EVENT_INTRO];
                        message.SetData(Identity.EVENT_TITLE, title);
                        message.SetData(Identity.EVENT_INTRO, intro);
                        embed.Description = await BuildEventDesciption(message);
                        break;

                    case Identity.SELECTION_EVENT_INFORMATION:

                        string infoTitle = data[Identity.EVENT_INFO_TITLE];
                        string info = data[Identity.EVENT_INFO];
                        message.SetData(Identity.EVENT_INFO_TITLE, infoTitle);
                        message.SetData(Identity.EVENT_INFO, info);
                        embed.Description = await BuildEventDesciption(message);
                        break;

                    case Identity.SELECTION_EVENT_REWARDS:

                        string rewardTitle = data[Identity.EVENT_REWARD_TITLE];
                        string reward = data[Identity.EVENT_REWARD];
                        message.SetData(Identity.EVENT_REWARD_TITLE, rewardTitle);
                        message.SetData(Identity.EVENT_REWARD, reward);
                        embed.Description = await BuildEventDesciption(message);
                        break;

                    case Identity.SELECTION_EVENT_TIMESTAMP:

                        string timeTitle = data[Identity.EVENT_TIME_TITLE];
                        string descStart = data[Identity.EVENT_DESCRIPTION_START];
                        string descEnd = data[Identity.EVENT_DESCRIPTION_END];
                        message.SetData(Identity.EVENT_TIME_TITLE, timeTitle);
                        message.SetData(Identity.EVENT_DESCRIPTION_START, descStart);
                        message.SetData(Identity.EVENT_DESCRIPTION_END, descEnd);
                        embed.Description = await BuildEventDesciption(message);
                        break;

                    case Identity.SELECTION_EVENT_REACTION:

                        string descReaction = data[Identity.EVENT_DESCRIPTION_REACTION];
                        message.SetData(Identity.EVENT_DESCRIPTION_REACTION, descReaction);
                        embed.Description = await BuildEventDesciption(message);
                        break;

                    case Identity.SELECTION_EVENT_CREATION:
                        var components = discordUtil.EventComponent();
                        var channel = await discordUtil.GetChannelByIdAsync(e.Interaction.Guild, (ulong)message.ChannelId!);
                        var hidden = message.IsEphemeral;
                        message = (await dataService.GetTemplateAsync(guildId, "EVENT_CREATE")).Message;
                        message.AddData(Identity.EVENT_NAME, e.Values[Identity.EVENT_NAME]);
                        message.AddData(Identity.EVENT_TIMEZONE, e.Values[Identity.EVENT_TIMEZONE]);
                        message.AddData(Identity.EVENT_START, e.Values[Identity.EVENT_START]);
                        message.AddData(Identity.EVENT_END, e.Values[Identity.EVENT_END]);

                        message.Sender = e.Interaction.User.Id;
                        embed.Footer = embed.Footer!.Replace("{event.host}", e.Interaction.User.Username);
                        embed.FooterUrl = e.Interaction.User.AvatarUrl;
                        embed.Description = await BuildEventDesciption(message);
                        message.IsEphemeral = hidden;
                        message.Type = CommandEnum.EVENTS_EDIT;
                        message.ChannelId = channel.Id;

                        await discordUtil.CreateMessageAsync(CommandEnum.EVENTS_EDIT, e.Interaction, message, channel.Id, hidden);
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

        private async Task UseTemplate(ModalSubmitEventArgs e) {
            try {
                //// Variables
                //var data = e.Values;
                //var selection = e.Interaction.Data.CustomId.Split(";")[1];
                //var messageId = ulong.Parse(e.Interaction.Data.CustomId.Split(";")[2]);
                //var guildId = e.Interaction.Guild.Id;
                //var message = await dataService.GetMessageAsync(guildId, messageId);
                //var embed = message.Embed;

                //// Check modals
                //switch (selection) {

                //    case Identity.SELECTION_TEMPLATE_INPUT:

                //        string templateUseName = data[Identity.TEMPLATE_NAME].ToUpper().Replace(" ", "_");
                //        var templateMessage = await dataService.GetTemplateAsync(e.Interaction.Guild.Id, templateUseName);
                //        var templateUseEmbed = templateMessage.Message.Embed;
                //        var templateMessageOriginal = await dataService.GetMessageAsync(e.Interaction.Guild.Id, messageId);
                //        var templateUseEmbedOriginal = templateMessageOriginal.Embed;

                //        templateUseEmbed.Type = templateUseEmbedOriginal.Type;
                //        templateMessage.ChannelId = templateMessageOriginal.ChannelId;
                //        templateMessage.Sender = templateMessageOriginal.Sender;
                //        templateMessage.IsEphemeral = templateMessageOriginal.IsEphemeral;
                //        templateMessage.Roles = templateMessageOriginal.Roles;
                //        templateUseEmbed.AddData(Identity.TEMPLATE_REPLACE_MESSAGE_ID, messageId);

                //        await DiscordUtil.ModifyMessageAsync(templateUseEmbed.Type, e.Interaction, templateMessage, (ulong)templateMessage.ChannelId!, templateMessage.IsEphemeral);

                //        return;

                //    default:
                //        break;
                //}

                //// Create the response
                //await DiscordUtil.UpdateMessageAsync(e.Interaction, message);

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
                var template = await dataService.GetTemplateAsync(e.Interaction.Guild.Id, Identity.TDATA_TIMESTAMP);
                var message = template.Message;
                var embed = message.Embed;

                // Check modals
                var timeZone = data[Identity.MODAL_DATA_TIMESTAMPS_TIMEZONE];
                var time = data[Identity.MODAL_DATA_TIMESTAMPS_TIME];

                // Check if the time zone exists & time format is correct
                if (!await discordUtil.ExistTimeZone(timeZone))
                    throw new EventException($"Time zone \"{timeZone}\" can not be found.");
                if (!DateTime.TryParseExact(time, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedTime))
                    throw new EventException($"Time \"{time}\" has an incorrect frmat.");

                message.AddData(Placeholder.TIMEZONE, timeZone);
                message.AddData(Placeholder.DATE_START, parsedTime.ToString("dd/MM/yyyy HH:mm"));

                await discordUtil.CreateMessageAsync(CommandEnum.TIMESTAMP, e.Interaction, message, e.Interaction.Channel.Id, message.IsEphemeral);

            } catch (UtilException ex) {
                throw new EventException(ex.Message);

            } catch (Exception ex) {
                throw new EventException($"An error has occured while submitting modal", ex);
            }
        }

        private async Task<string> BuildEventDesciption(Message message) {

            string name = (string)message.Data[Identity.EVENT_NAME] ?? "";
            string start = (string)message.Data[Identity.EVENT_START];
            string end = (string)message.Data[Identity.EVENT_END];
            string time = (string)message.Data[Identity.EVENT_TIMEZONE];

            if (DateTime.TryParseExact(start, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDateTime)) {
                message.SetData(Identity.EVENT_START, start);
            } else throw new EventException($"Date \"{start}\" has incorrect format");

            if (DateTime.TryParseExact(end, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDateTime)) {
                message.SetData(Identity.EVENT_END, end);
            } else throw new EventException($"Time \"{end}\" has incorrect format");

            string startDate = await discordUtil.TranslateToDynamicTimestamp(parsedStartDateTime, time, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
            string endDate = await discordUtil.TranslateToDynamicTimestamp(parsedEndDateTime, time, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
            string startDateRelative = await discordUtil.TranslateToDynamicTimestamp(parsedStartDateTime, time, TimestampEnum.RELATIVE);
            string endDateRelative = await discordUtil.TranslateToDynamicTimestamp(parsedEndDateTime, time, TimestampEnum.RELATIVE);

            Dictionary<string, string> placeholders = new() {
                { Identity.EVENT_NAME, name },
                { Identity.EVENT_START, startDate },
                { Identity.EVENT_END, endDate },
                { Identity.EVENT_START_R, startDateRelative },
                { Identity.EVENT_END_R, endDateRelative }

            };

            string introTitle = (string)message.Data[Identity.EVENT_TITLE];
            string intro = (string)message.Data[Identity.EVENT_INTRO];

            string infoTitle = (string)message.Data[Identity.EVENT_INFO_TITLE];
            string info = (string)message.Data[Identity.EVENT_INFO];

            string rewardTitle = (string)message.Data[Identity.EVENT_REWARD_TITLE];
            string reward = (string)message.Data[Identity.EVENT_REWARD];

            string timeTitle = (string)message.Data[Identity.EVENT_TIME_TITLE];
            string textStart = (string)message.Data[Identity.EVENT_DESCRIPTION_START];
            string textEnd = (string)message.Data[Identity.EVENT_DESCRIPTION_END];

            string textReaction = (string)message.Data[Identity.EVENT_DESCRIPTION_REACTION];

            string result =
                introTitle + "\n" +
                intro + "\n" +
                infoTitle + "\n" +
                info + "\n" +
                rewardTitle + "\n" +
                reward + "\n" +
                timeTitle + "\n" +
                textStart + "\n" +
                textEnd + "\n" + "\n" +
                textReaction;

            foreach (var item in placeholders) {
                result = result.Replace("{" + item.Key + "}", item.Value);
            }

            return result;

        }


    }
}
