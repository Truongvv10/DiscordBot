using DSharpPlus.EventArgs;
using DSharpPlus;
using DSharpPlus.Entities;
using DiscordBot.Model.Enums;
using DiscordBot.Utils;
using DiscordBot.Exceptions;
using System.Diagnostics;
using AnsiColor = DiscordBot.Utils.AnsiColor;
using System.Formats.Asn1;
using System;
using System.Xml.Linq;

namespace DiscordBot.Listeners {
    public class ModalSubmitEvent {

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
                if (e.Interaction.Data.CustomId.Contains(Identity.MODAL_TEMPLATE))
                    await UseTemplate(e);

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
                var embed = CacheData.GetEmbed(guildId, messageId);

                // Check modals
                switch (selection) {

                    case Identity.SELECTION_TITLE:
                        embed.Title = data[Identity.MODAL_COMP_TITLE];
                        embed.TitleUrl = data[Identity.MODAL_COMP_TITLE_LINK];
                        break;

                    case Identity.SELECTION_DESCRIPTION:
                        embed.Description = data[Identity.MODAL_COMP_DESCRIPTION];
                        break;

                    case Identity.SELECTION_CONTENT:
                        embed.Content = data[Identity.MODAL_COMP_CONTENT];
                        break;

                    case Identity.SELECTION_FOOTER:
                        embed.Footer = data[Identity.MODAL_COMP_FOOTER];
                        embed.FooterUrl = data[Identity.MODAL_COMP_FOOTER_URL];
                        break;

                    case Identity.SELECTION_AUTHOR:
                        if (e.Values.First().Value.Contains("self", StringComparison.OrdinalIgnoreCase)) {
                            embed.Author = e.Interaction.User.Username;
                            embed.AuthorLink = e.Interaction.User.DefaultAvatarUrl;
                            embed.AuthorUrl = e.Interaction.User.AvatarUrl;
                        } else {
                            embed.Author = data[Identity.MODAL_COMP_AUTHOR];
                            embed.AuthorLink = data[Identity.MODAL_COMP_AUTHOR_LINK];
                            embed.AuthorUrl = data[Identity.MODAL_COMP_AUTHOR_URL];
                        }
                        break;

                    case Identity.SELECTION_COLOR:
                        embed.Color = data[Identity.MODAL_COMP_COLOR];
                        break;

                    case Identity.SELECTION_IMAGE:
                        embed.Image = data[Identity.MODAL_COMP_IMAGE];
                        await DiscordUtil.ModifyMessageAsync(embed.Type, e.Interaction, embed, (ulong)embed.ChannelId!, embed.IsEphemeral);
                        return;

                    case Identity.SELECTION_THUMBNAIL:
                        embed.Thumbnail = data[Identity.MODAL_COMP_THUMBNAIL];
                        await DiscordUtil.ModifyMessageAsync(embed.Type, e.Interaction, embed, (ulong)embed.ChannelId!, embed.IsEphemeral);
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
                                var role = await DiscordUtil.GetRolesByIdAsync(e.Interaction.Guild, roleid);
                                pingIds.Add(role.Id);
                            }
                        }

                        embed.SetPingRoles(pingIds.ToArray());
                        break;

                    case Identity.SELECTION_FIELD_ADD:
                        embed.AddField(e.Values[Identity.MODAL_COMP_FIELD_TITLE], e.Values[Identity.MODAL_COMP_FIELD_TEXT], bool.Parse(e.Values[Identity.MODAL_COMP_FIELD_INLINE]));
                        break;

                    case Identity.SELECTION_FIELD_REMOVE:
                        if (int.TryParse(e.Values[Identity.MODAL_COMP_FIELD_INDEX], out int index)) {
                            if (e.Values.Count() - 1 >= index) {
                                embed.RemoveFieldAt(index);
                            }
                        }
                        break;

                    case Identity.SELECTION_TEMPLATE_ADD:
                        string name = data[Identity.MODAL_COMP_TEMPLATE_ADD];
                        if (!string.IsNullOrWhiteSpace(name)) {
                            var clone = embed.DeepClone2();
                            await CacheData.SaveTemplate(guildId, name.ToUpper().Replace(" ", "_"), clone);
                        } else throw new ListenerException($"Template \"{name}\" can not be empty or null!");

                        var templateAddEmbed = await CacheData.GetTemplate(e.Interaction.Guild.Id, Identity.TDATA_CREATE);
                        templateAddEmbed.Description = templateAddEmbed.Description.Replace($"{{{Identity.TEMPLATE_NAME}}}", name);
                        await DiscordUtil.CreateMessageAsync(CommandEnum.TEMPLATE_CREATE, e.Interaction, templateAddEmbed, e.Interaction.Channel.Id, templateAddEmbed.IsEphemeral);
                        return;

                    default:
                        break;
                }

                // Create the response
                await DiscordUtil.UpdateMessageAsync(e.Interaction, embed);

            } catch (UtilException ex) {
                Console.WriteLine(ex);
                throw new ListenerException(ex.Message);

            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw new ListenerException($"An error has occured while submitting modal", ex);
            }

        }

        private async Task UseEvent(ModalSubmitEventArgs e) {
            try {
                // Variables
                var data = e.Values;
                var selection = e.Interaction.Data.CustomId.Split(";")[1];
                var messageId = ulong.Parse(e.Interaction.Data.CustomId.Split(";")[2]);
                var guildId = e.Interaction.Guild.Id;
                var embed = CacheData.GetEmbed(guildId, messageId);

                // Check modals
                switch (selection) {

                    case Identity.SELECTION_EVENT_PROPERTIES:

                        string eventName = data[Identity.EVENT_NAME];
                        string timezone = data[Identity.EVENT_TIMEZONE];
                        string start = data[Identity.EVENT_START];
                        string end = data[Identity.EVENT_END];

                        if (DateTime.TryParseExact(start, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDateTime)) {
                            embed.SetCustomData(Identity.EVENT_START, start);
                        } else throw new ListenerException($"Date \"{start}\" has incorrect format");

                        if (DateTime.TryParseExact(end, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDateTime)) {
                            embed.SetCustomData(Identity.EVENT_END, end);
                        } else throw new ListenerException($"Time \"{end}\" has incorrect format");

                        embed.SetCustomData(Identity.EVENT_TIMEZONE, timezone);
                        embed.SetCustomData(Identity.EVENT_NAME, eventName);
                        embed.Description = await BuildEventDesciption(embed);
                        break;

                    case Identity.SELECTION_EVENT_INTRODUCTION:

                        string title = data[Identity.EVENT_TITLE];
                        string intro = data[Identity.EVENT_INTRO];
                        embed.SetCustomData(Identity.EVENT_TITLE, title);
                        embed.SetCustomData(Identity.EVENT_INTRO, intro);
                        embed.Description = await BuildEventDesciption(embed);
                        break;

                    case Identity.SELECTION_EVENT_INFORMATION:

                        string infoTitle = data[Identity.EVENT_INFO_TITLE];
                        string info = data[Identity.EVENT_INFO];
                        embed.SetCustomData(Identity.EVENT_INFO_TITLE, infoTitle);
                        embed.SetCustomData(Identity.EVENT_INFO, info);
                        embed.Description = await BuildEventDesciption(embed);
                        break;

                    case Identity.SELECTION_EVENT_REWARDS:

                        string rewardTitle = data[Identity.EVENT_REWARD_TITLE];
                        string reward = data[Identity.EVENT_REWARD];
                        embed.SetCustomData(Identity.EVENT_REWARD_TITLE, rewardTitle);
                        embed.SetCustomData(Identity.EVENT_REWARD, reward);
                        embed.Description = await BuildEventDesciption(embed);
                        break;

                    case Identity.SELECTION_EVENT_TIMESTAMP:

                        string timeTitle = data[Identity.EVENT_TIME_TITLE];
                        string descStart = data[Identity.EVENT_DESCRIPTION_START];
                        string descEnd = data[Identity.EVENT_DESCRIPTION_END];
                        embed.SetCustomData(Identity.EVENT_TIME_TITLE, timeTitle);
                        embed.SetCustomData(Identity.EVENT_DESCRIPTION_START, descStart);
                        embed.SetCustomData(Identity.EVENT_DESCRIPTION_END, descEnd);
                        embed.Description = await BuildEventDesciption(embed);
                        break;

                    case Identity.SELECTION_EVENT_REACTION:

                        string descReaction = data[Identity.EVENT_DESCRIPTION_REACTION];
                        embed.SetCustomData(Identity.EVENT_DESCRIPTION_REACTION, descReaction);
                        embed.Description = await BuildEventDesciption(embed);
                        break;

                    case Identity.SELECTION_EVENT_CREATION:
                        var components = DiscordUtil.EventComponent();
                        var channel = await DiscordUtil.GetChannelByIdAsync(e.Interaction.Guild, (ulong)embed.ChannelId!);
                        var hidden = embed.IsEphemeral;
                        embed = await CacheData.GetTemplate(e.Interaction.Guild.Id, "EVENT_CREATE");
                        embed.AddCustomData(Identity.EVENT_NAME, e.Values[Identity.EVENT_NAME]);
                        embed.AddCustomData(Identity.EVENT_TIMEZONE, e.Values[Identity.EVENT_TIMEZONE]);
                        embed.AddCustomData(Identity.EVENT_START, e.Values[Identity.EVENT_START]);
                        embed.AddCustomData(Identity.EVENT_END, e.Values[Identity.EVENT_END]);

                        embed.Owner = e.Interaction.User.Id;
                        embed.Footer = embed.Footer!.Replace("{event.host}", e.Interaction.User.Username);
                        embed.FooterUrl = e.Interaction.User.AvatarUrl;
                        embed.Description = await BuildEventDesciption(embed);
                        embed.IsEphemeral = hidden;
                        embed.Type = CommandEnum.EVENTS_EDIT;
                        embed.ChannelId = channel.Id;

                        await DiscordUtil.CreateMessageAsync(CommandEnum.EVENTS_EDIT, e.Interaction, embed, channel.Id, hidden);
                        return;

                    default:
                        break;
                }

                // Create the response
                await DiscordUtil.UpdateMessageAsync(e.Interaction, embed);

            } catch (UtilException ex) {
                throw new ListenerException(ex.Message);

            } catch (Exception ex) {
                throw new ListenerException($"An error has occured while submitting modal", ex);
            }
        }

        private async Task UseTemplate(ModalSubmitEventArgs e) {
            try {
                // Variables
                var data = e.Values;
                var selection = e.Interaction.Data.CustomId.Split(";")[1];
                var messageId = ulong.Parse(e.Interaction.Data.CustomId.Split(";")[2]);
                var guildId = e.Interaction.Guild.Id;
                var embed = CacheData.GetEmbed(guildId, messageId);

                // Check modals
                switch (selection) {

                    case Identity.SELECTION_TEMPLATE_INPUT:

                        string templateUseName = data[Identity.TEMPLATE_NAME].ToUpper().Replace(" ", "_");
                        var templateUseEmbed = await CacheData.GetTemplate(e.Interaction.Guild.Id, templateUseName);
                        var templateUseEmbedOriginal = CacheData.GetEmbed(e.Interaction.Guild.Id, messageId);

                        templateUseEmbed.Type = templateUseEmbedOriginal.Type;
                        templateUseEmbed.ChannelId = templateUseEmbedOriginal.ChannelId;
                        templateUseEmbed.Owner = templateUseEmbedOriginal.Owner;
                        templateUseEmbed.IsEphemeral = templateUseEmbedOriginal.IsEphemeral;
                        templateUseEmbed.PingRoles = templateUseEmbedOriginal.PingRoles;
                        templateUseEmbed.AddCustomData(Identity.TEMPLATE_REPLACE_MESSAGE_ID, messageId);

                        await DiscordUtil.ModifyMessageAsync(templateUseEmbed.Type, e.Interaction, templateUseEmbed, (ulong)templateUseEmbed.ChannelId!, templateUseEmbed.IsEphemeral);

                        return;

                    default:
                        break;
                }

                // Create the response
                await DiscordUtil.UpdateMessageAsync(e.Interaction, embed);

            } catch (UtilException ex) {
                throw new ListenerException(ex.Message);

            } catch (Exception ex) {
                throw new ListenerException($"An error has occured while submitting modal", ex);
            }
        }

        private async Task UseTimestamp(ModalSubmitEventArgs e) {
            try {
                // Variables
                var data = e.Values;
                var embed = await CacheData.GetTemplate(e.Interaction.Guild.Id, Identity.TDATA_TEMPLATE);

                // Check modals
                var timeZone = data[Identity.MODAL_COMP_TIMESTAMP_TIMEZONE];
                var time = data[Identity.MODAL_COMP_TIMESTAMP_TIME];

                // Check if the time zone exists & time format is correct
                if (!await DiscordUtil.ExistTimeZone(timeZone))
                    throw new ListenerException($"Time zone \"{timeZone}\" can not be found.");
                if (!DateTime.TryParseExact(time, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedTime))
                    throw new ListenerException($"Time \"{time}\" has an incorrect frmat.");

                // Translate the time to the selected time zone
                string date1 = await DiscordUtil.TranslateToDynamicTimestamp(parsedTime, timeZone, TimestampEnum.SHORT_DATE);
                string date2 = await DiscordUtil.TranslateToDynamicTimestamp(parsedTime, timeZone, TimestampEnum.SHORT_TIME);
                string date3 = await DiscordUtil.TranslateToDynamicTimestamp(parsedTime, timeZone, TimestampEnum.LONG_DATE);
                string date4 = await DiscordUtil.TranslateToDynamicTimestamp(parsedTime, timeZone, TimestampEnum.LONG_TIME);
                string date5 = await DiscordUtil.TranslateToDynamicTimestamp(parsedTime, timeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                string date6 = await DiscordUtil.TranslateToDynamicTimestamp(parsedTime, timeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);
                string date7 = await DiscordUtil.TranslateToDynamicTimestamp(parsedTime, timeZone, TimestampEnum.RELATIVE);

                Dictionary<string, string> placeholders = new() {
                    { Identity.TIMESTAMP_TIMEZONE, timeZone },
                    { Identity.TIMESTAMP_SHORT_DATE, date1 },
                    { Identity.TIMESTAMP_SHORT_TIME, date2 },
                    { Identity.TIMESTAMP_LONG_DATE, date3 },
                    { Identity.TIMESTAMP_LONG_TIME, date4 },
                    { Identity.TIMESTAMP_LONG_DATE_SHORT_TIME, date5 },
                    { Identity.TIMESTAMP_LONG_DATE_DAY_OF_WEEK_SHORT_TIME, date6 },
                    { Identity.TIMESTAMP_RELATIVE_TIME, date7 }};

                var copy = embed.DeepClone2();
                embed.ClearFields();
                embed.Author = placeholders.Aggregate(embed.Author, (current, p) => current.Replace("{" + p.Key + "}", p.Value));

                foreach (var item in copy.Fields) {
                    // Apply all placeholders to field title and value in one pass
                    string one = placeholders.Aggregate(item.Item1, (current, p) => current.Replace("{" + p.Key + "}", p.Value));
                    string two = placeholders.Aggregate(item.Item2, (current, p) => current.Replace("{" + p.Key + "}", p.Value));

                    // Add updated fields back to embed
                    embed.AddField(one, two, item.Item3);
                }

                await DiscordUtil.CreateMessageAsync(CommandEnum.TIMESTAMP, e.Interaction, embed, e.Interaction.Channel.Id, embed.IsEphemeral);

            } catch (UtilException ex) {
                throw new ListenerException(ex.Message);

            } catch (Exception ex) {
                throw new ListenerException($"An error has occured while submitting modal", ex);
            }
        }

        private async Task<string> BuildEventDesciption(EmbedBuilder embed) {

            string name = (string)embed.CustomData[Identity.EVENT_NAME] ?? "";
            string start = (string)embed.CustomData[Identity.EVENT_START];
            string end = (string)embed.CustomData[Identity.EVENT_END];
            string time = (string)embed.CustomData[Identity.EVENT_TIMEZONE];

            if (DateTime.TryParseExact(start, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDateTime)) {
                embed.SetCustomData(Identity.EVENT_START, start);
            } else throw new ListenerException($"Date \"{start}\" has incorrect format");

            if (DateTime.TryParseExact(end, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDateTime)) {
                embed.SetCustomData(Identity.EVENT_END, end);
            } else throw new ListenerException($"Time \"{end}\" has incorrect format");

            string startDate = await DiscordUtil.TranslateToDynamicTimestamp(parsedStartDateTime, time, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
            string endDate = await DiscordUtil.TranslateToDynamicTimestamp(parsedEndDateTime, time, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
            string startDateRelative = await DiscordUtil.TranslateToDynamicTimestamp(parsedStartDateTime, time, TimestampEnum.RELATIVE);
            string endDateRelative = await DiscordUtil.TranslateToDynamicTimestamp(parsedEndDateTime, time, TimestampEnum.RELATIVE);

            Dictionary<string, string> placeholders = new() {
                { Identity.EVENT_NAME, name },
                { Identity.EVENT_START, startDate },
                { Identity.EVENT_END, endDate },
                { Identity.EVENT_START_R, startDateRelative },
                { Identity.EVENT_END_R, endDateRelative },
                { Identity.REACTION_1, "✅" },
                { Identity.REACTION_2, "❌" }

            };

            string introTitle = (string)embed.CustomData[Identity.EVENT_TITLE];
            string intro = (string)embed.CustomData[Identity.EVENT_INTRO];

            string infoTitle = (string)embed.CustomData[Identity.EVENT_INFO_TITLE];
            string info = (string)embed.CustomData[Identity.EVENT_INFO];

            string rewardTitle = (string)embed.CustomData[Identity.EVENT_REWARD_TITLE];
            string reward = (string)embed.CustomData[Identity.EVENT_REWARD];

            string timeTitle = (string)embed.CustomData[Identity.EVENT_TIME_TITLE];
            string textStart = (string)embed.CustomData[Identity.EVENT_DESCRIPTION_START];
            string textEnd = (string)embed.CustomData[Identity.EVENT_DESCRIPTION_END];

            string textReaction = (string)embed.CustomData[Identity.EVENT_DESCRIPTION_REACTION];

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
