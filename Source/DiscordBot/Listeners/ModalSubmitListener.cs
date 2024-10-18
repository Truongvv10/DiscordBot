using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.VisualBasic.FileIO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using DiscordBot.Model.Enums;
using DSharpPlus.SlashCommands;
using DiscordBot.Utils;
using XironiteDiscordBot.Exceptions;
using DiscordBot.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiscordBot.Listeners {
    public class ModalSubmitListener {
        public async Task HandleEmbedCommand(DiscordClient discordClient, ModalSubmitEventArgs e) {

			try {
                var options = e.Values;
                var selection = e.Interaction.Data.CustomId.Split(";")[1];
                var messageId = ulong.Parse(e.Interaction.Data.CustomId.Split(";")[2]);
                var guildId = e.Interaction.Guild.Id;
                var embed = CacheData.GetEmbed(guildId, messageId);
                var message = await DiscordUtil.GetMessageByIdAsync(e.Interaction.Channel, messageId);

                switch (selection) {

                    case Identity.SELECTION_TITLE:
                        embed.Title = e.Values[Identity.MODAL_TITLE];
                        embed.TitleUrl = e.Values[Identity.MODAL_TITLE_LINK];
                        break;

                    case Identity.SELECTION_DESCRIPTION:
                        embed.Description = e.Values[Identity.MODAL_DESCRIPTION];
                        break;

                    case Identity.SELECTION_CONTENT:
                        embed.Content = e.Values[Identity.MODAL_CONTENT];
                        break;

                    case Identity.SELECTION_FOOTER:
                        embed.Footer = e.Values[Identity.MODAL_FOOTER];
                        embed.FooterUrl = e.Values[Identity.MODAL_FOOTER_URL];
                        break;

                    case Identity.SELECTION_AUTHOR:
                        if (e.Values.First().Value.Contains("self", StringComparison.OrdinalIgnoreCase)) {
                            embed.Author = e.Interaction.User.Username;
                            embed.AuthorLink = e.Interaction.User.DefaultAvatarUrl;
                            embed.AuthorUrl = e.Interaction.User.AvatarUrl;
                        } else {
                            embed.Author = e.Values[Identity.MODAL_AUTHOR];
                            embed.AuthorLink = e.Values[Identity.MODAL_AUTHOR_LINK];
                            embed.AuthorUrl = e.Values[Identity.MODAL_AUTHOR_URL];
                        }
                        break;

                    case Identity.SELECTION_COLOR:
                        embed.Color = e.Values[Identity.MODAL_COLOR];
                        break;

                    case Identity.SELECTION_IMAGE:
                        embed.Image = e.Values[Identity.MODAL_IMAGE];
                        break;

                    case Identity.SELECTION_THUMBNAIL:
                        embed.Thumbnail = e.Values[Identity.MODAL_THUMBNAIL];
                        break;

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
                        embed.AddField(e.Values[Identity.MODAL_FIELD_TITLE], e.Values[Identity.MODAL_FIELD_TEXT], bool.Parse(e.Values[Identity.MODAL_FIELD_INLINE]));
                        break;

                    case Identity.SELECTION_FIELD_REMOVE:
                        if (int.TryParse(e.Values[Identity.MODAL_FIELD_INDEX], out int index)) {
                            if (e.Values.Count() - 1 >= index) {
                                embed.RemoveFieldAt(index);
                            }
                        }
                        break;

                    case Identity.SELECTION_TEMPLATE_ADD:
                        string name = e.Values[Identity.MODAL_TEMPLATE_ADD];
                        if (!string.IsNullOrWhiteSpace(name)) {
                            var clone = embed.DeepClone();
                            await CacheData.SaveTemplate(guildId, name.ToUpper().Replace(" ", "_"), clone);
                        } else throw new ListenerException($"Template \"{name}\" can not be empty or null!");
                        break;

                    case Identity.SELECTION_EVENT_INTRODUCTION:

                        string title = e.Values[Identity.EVENT_TITLE];
                        string intro = e.Values[Identity.EVENT_INTRO];
                        embed.SetCustomSaveMessage(Identity.EVENT_TITLE, title);
                        embed.SetCustomSaveMessage(Identity.EVENT_INTRO, intro);
                        embed.Description = BuildEventDesciption(embed);
                        break;

                    case Identity.SELECTION_EVENT_INFORMATION:

                        string infoTitle = e.Values[Identity.EVENT_INFO_TITLE];
                        string info = e.Values[Identity.EVENT_INFO];
                        embed.SetCustomSaveMessage(Identity.EVENT_INFO_TITLE, infoTitle);
                        embed.SetCustomSaveMessage(Identity.EVENT_INFO, info);
                        embed.Description = BuildEventDesciption(embed);
                        break;

                    case Identity.SELECTION_EVENT_REWARDS:

                        string rewardTitle = e.Values[Identity.EVENT_REWARD_TITLE];
                        string reward = e.Values[Identity.EVENT_REWARD];
                        embed.SetCustomSaveMessage(Identity.EVENT_REWARD_TITLE, rewardTitle);
                        embed.SetCustomSaveMessage(Identity.EVENT_REWARD, reward);
                        embed.Description = BuildEventDesciption(embed);
                        break;

                    case Identity.SELECTION_EVENT_TIMESTAMP:

                        string timeTitle = e.Values[Identity.EVENT_TIME_TITLE];
                        string timezone = e.Values[Identity.EVENT_TIMEZONE];
                        string start = e.Values[Identity.EVENT_START];
                        string end = e.Values[Identity.EVENT_END];

                        if (CacheData.Timezones.Contains(timezone, StringComparer.OrdinalIgnoreCase)) {
                            embed.SetCustomSaveMessage(Identity.EVENT_TIMEZONE, timezone);
                        } else throw new ListenerException($"Time Zone \"{timezone}\" does not exist");

                        if (DateTime.TryParseExact(start, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDateTime)) {
                            embed.SetCustomSaveMessage(Identity.EVENT_START, start);
                        } else throw new ListenerException($"Date \"{start}\" has incorrect format");

                        if (DateTime.TryParseExact(end, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDateTime)) {
                            embed.SetCustomSaveMessage(Identity.EVENT_END, end);
                        } else throw new ListenerException($"Time \"{end}\" has incorrect format");

                        string startDate = await DiscordUtil.TranslateTimestamp(parsedStartDateTime, (embed.CustomSaves[Identity.EVENT_TIMEZONE] as string)!, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                        string endDate = await DiscordUtil.TranslateTimestamp(parsedEndDateTime, (embed.CustomSaves[Identity.EVENT_TIMEZONE] as string)!, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                        string startDateRelative = await DiscordUtil.TranslateTimestamp(parsedStartDateTime, (embed.CustomSaves[Identity.EVENT_TIMEZONE] as string)!, TimestampEnum.RELATIVE);

                        embed.ClearFields();
                        embed.AddField("Start Date:", $"{startDate} ({startDateRelative})", false);
                        embed.AddField("End Date:", endDate, false);
                        embed.AddField("React with a `✅` if you're coming to the event!", "**React with a `❌` if you're going to miss out...**", false);
                        break;

                    default:
                        break;
                }

                // Create a list of action rows (as you may have multiple rows of components)
                var actionRows = new List<DiscordActionRowComponent>();

                // Ensure you're working with the correct component type
                foreach (var row in message.Components) {
                    // Ensure it's an action row
                    if (row is DiscordActionRowComponent actionRow) {
                        actionRows.Add(actionRow); // Add the entire action row
                    } else {
                        // Handle the case where the component is not in an action row (optional)
                        throw new InvalidOperationException("Component is not an action row.");
                    }
                }

                // If you have multiple action rows, pass them all to the response
                var response = new DiscordInteractionResponseBuilder()
                    .WithContent(await CreatePingRoles(embed, e.Interaction.Guild) + " " + embed.Content)
                    .AddEmbed(embed.Build())
                    .AddComponents(actionRows);  // Add all action rows to the response

                // Send the response
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, response);
                await JsonData.SaveEmbedsAsync(guildId);

            } catch (UtilException ex) {
                Console.WriteLine(ex);
            } catch (Exception ex) {
                throw new ListenerException(ex.Message);
			}

        }

        private string BuildEventDesciption(EmbedBuilder embed) {

            string title = (string)embed.CustomSaves[Identity.EVENT_TITLE];
            string intro = (string)embed.CustomSaves[Identity.EVENT_INTRO];
            string infoTitle = (string)embed.CustomSaves[Identity.EVENT_INFO_TITLE];
            string info = (string)embed.CustomSaves[Identity.EVENT_INFO];
            string rewardTitle = (string)embed.CustomSaves[Identity.EVENT_REWARD_TITLE];
            string reward = (string)embed.CustomSaves[Identity.EVENT_REWARD];
            string timeTitle = (string)embed.CustomSaves[Identity.EVENT_TIME_TITLE];

            return 
                title + "\n" + 
                intro + "\n" + 
                infoTitle + "\n" + 
                info + "\n" + 
                rewardTitle + "\n" + 
                reward + "\n" + 
                timeTitle + "\n";

        }

        private async Task<string> CreatePingRoles(EmbedBuilder embed, DiscordGuild guild) {

            string pingRoles = string.Empty;

            if (embed.PingRoles.Count > 0) {

                // List of roles to be kept
                List<DiscordRole> roles = new();

                // Translate each id in to roles
                foreach (var roleId in embed.PingRoles) {
                    roles.Add(await DiscordUtil.GetRolesByIdAsync(guild, roleId));
                }

                // Extract the mention property from each role.
                foreach (var role in roles) {
                    if (role.Name == "@everyone") {
                        pingRoles += "@everyone";
                    } else pingRoles += role.Mention + " ";
                }

            }

            return pingRoles;
        }
    }
}
