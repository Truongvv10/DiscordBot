﻿using APP.Enums;
using Azure.Core;
using BLL.Enums;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using BLL.Services;
using DSharpPlus.Entities;
using NodaTime;
using System.Text.RegularExpressions;

namespace APP.Utils {
    public static class Placeholder {

        private const string PREFIX = "";
        private static List<string> placeholders = new List<string>();

        public const string ID = PREFIX + "data.id";
        public const string CUSTOM = PREFIX + "data.custom";
        public const string TIMEZONE = PREFIX + "data.timezone";
        public const string DATE_START = PREFIX + "data.date.start";
        public const string URL1 = PREFIX + "data.url.1";
        public const string URL2 = PREFIX + "data.url.2";
        public const string URL3 = PREFIX + "data.url.3";
        public const string URL4 = PREFIX + "data.url.4";
        public const string TEXT1 = PREFIX + "data.text.1";
        public const string TEXT2 = PREFIX + "data.text.2";
        public const string TEXT3 = PREFIX + "data.text.3";
        public const string TEXT4 = PREFIX + "data.text.4";
        public const string DATE_END = PREFIX + "data.date.end";
        public const string USER_NAME = PREFIX + "data.user.name";
        public const string USER_USERNAME = PREFIX + "data.user.username";
        public const string USER_MENTION = PREFIX + "data.user.mention";
        public const string USER_AVATARURL = PREFIX + "data.user.avatarurl";
        public const string CHANNEL_ID = PREFIX + "data.channel.id";
        public const string GUILD_ID = PREFIX + "data.guild.id";
        public const string MESSAGE_ID = PREFIX + "data.message.id";
        public const string LIST_USERS = PREFIX + "data.list.users";
        public const string LIST_USERS_MENTION = PREFIX + "data.list.users.mention";
        public const string LIST_ROLES = PREFIX + "data.list.roles";
        public const string LIST_ROLES_MENTION = PREFIX + "data.list.roles.mention";
        public const string LIST_REACTIONS = PREFIX + "data.list.reactions";
        public const string LIST_TEMPLATES = PREFIX + "data.list.templates";
        public const string LIST_TEMPLATES_GUILD = PREFIX + "data.list.templates.guild";


        static Placeholder() {
            placeholders.AddRange(new[] {
                ID, CUSTOM, TIMEZONE, DATE_START, DATE_END, 
                URL1, URL2, URL3, URL4, 
                TEXT1, TEXT2, TEXT3, TEXT4, 
                USER_NAME, USER_AVATARURL, USER_MENTION, 
                CHANNEL_ID, GUILD_ID, MESSAGE_ID, 
                LIST_USERS, LIST_USERS_MENTION,
                LIST_ROLES, LIST_ROLES_MENTION,
                LIST_REACTIONS, LIST_TEMPLATES, LIST_TEMPLATES_GUILD});
        }

        public static async Task<string> Translate(string input, Message message, DiscordInteraction interaction, IDataRepository dataService) {
            var data = message.Data;
            var extractedPlaceholders = ExtractPlaceholders(input);
            foreach (var placeholder in extractedPlaceholders) {
                string toReplace = $"{{{placeholder}}}";
                switch (placeholder) {
                    case USER_NAME:
                        input = input.Replace(toReplace, interaction.User.Username);
                        break;
                    case USER_AVATARURL:
                        input = input.Replace(toReplace, interaction.User.AvatarUrl);
                        break;
                    case USER_MENTION:
                        input = input.Replace(toReplace, interaction.User.Mention);
                        break;
                    case LIST_TEMPLATES:
                        var cache = new CacheData();
                        await cache.LoadTemplates();
                        input = input.Replace(toReplace, cache.Templates.Keys.Aggregate("", (current, next) => current + "`" + next + "`, "));
                        break;
                    case LIST_TEMPLATES_GUILD:
                        var templates = await dataService.GetAllTemplatesAsync(interaction.Guild.Id);
                        string templateList = templates.Count() == 0 ? "`No templates saved yet...`" : templates.Select(x => x.Name).Aggregate("", (current, next) => current + "`" + next + "`, ");
                        input = input.Replace(toReplace, templateList);
                        break;
                    case MESSAGE_ID:
                        input = input.Replace(toReplace, interaction.Id.ToString());
                        break;
                    case CHANNEL_ID:
                        input = input.Replace(toReplace, interaction.Channel.Id.ToString());
                        break;
                    case GUILD_ID:
                        input = input.Replace(toReplace, interaction.Guild.Id.ToString());
                        break;
                    case LIST_USERS:
                        if (message.Users.Count() == 0) {
                            input = input.Replace(toReplace, "No one...");
                        } else {
                            var userTasks = message.Users.Select(async x => await interaction.Guild.GetMemberAsync(x));
                            var userResults = await Task.WhenAll(userTasks); // Run the tasks in parallel and wait for completion
                            var users = userResults.Select(x => x.Username)
                                                   .Aggregate("", (current, next) => string.IsNullOrEmpty(current) ? next : current + ", " + next);
                            input = input.Replace(toReplace, users);
                        }
                        break;
                    case LIST_USERS_MENTION:
                        if (message.Users.Count() == 0) {
                            input = input.Replace(toReplace, "No one...");
                        } else {
                            var userTasks = message.Users.Select(async x => await interaction.Guild.GetMemberAsync(x));
                            var userResults = await Task.WhenAll(userTasks); // Run the tasks in parallel and wait for completion
                            var users = userResults.Select(x => x.Mention)
                                                   .Aggregate("", (current, next) => string.IsNullOrEmpty(current) ? next : current + ", " + next);
                            input = input.Replace(toReplace, users);
                        }
                        break;
                    case LIST_ROLES:
                        if (message.Roles.Count() == 0) {
                            input = input.Replace(toReplace, "No roles found...");
                        } else {
                            var roleResults = message.Roles.Select(x => interaction.Guild.GetRole(x));
                            var roles = roleResults.Select(x => x.Name)
                                                   .Aggregate("", (current, next) => string.IsNullOrEmpty(current) ? next : current + ", " + next);
                            input = input.Replace(toReplace, roles);
                        }
                        break;
                    case LIST_ROLES_MENTION:
                        if (message.Roles.Count() == 0) {
                            input = input.Replace(toReplace, "No roles found...");
                        } else {
                            var roleResults = message.Roles.Select(x => interaction.Guild.GetRole(x));
                            var roles = roleResults.Select(x => x.Mention)
                                                   .Aggregate("", (current, next) => string.IsNullOrEmpty(current) ? next : current + ", " + next);
                            input = input.Replace(toReplace, roles);
                        }
                        break;
                    default:
                        if (data.TryGetValue(placeholder, out var replacement)) {
                            input = input.Replace(toReplace, replacement);
                        } else if (Regex.IsMatch(placeholder, @"^data\.date\.(start|end)(\.\d+)?$")) {
                            string timezone = data.ContainsKey(TIMEZONE) ? data[TIMEZONE] : TimeZoneEnum.CET.ToString();
                            string date = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            if (placeholder.Contains(DATE_START)) date = data[DATE_START];
                            if (placeholder.Contains(DATE_END)) date = data[DATE_END];
                            var timestamp = TranslateTime(placeholder, date, timezone, TimeZoneEnum.CET.ToString());
                            input = input.Replace(toReplace, timestamp);
                        } else if (placeholder == TIMEZONE) {
                            input = input.Replace(toReplace, TimeZoneEnum.CET.ToString());
                        }
                        break;
                }
            }
            return input;
        }

        private static List<string> ExtractPlaceholders(string input) {
            // Extract all placeholders from the input string using regex
            var matches = Regex.Matches(input, @"\{(.*?)\}");
            var foundPlaceholders = new HashSet<string>();

            // Iterate through the matches and add valid group values
            foreach (Match match in matches) {
                string placeholder = match.Groups[1].Value; // This gets the value inside the brackets

                // Trim numeric sub-variants (e.g., ".1", ".2")
                string trimmedPlaceholder = Regex.Replace(placeholder, @"(\.\d+)$", "");

                // Trim custom sub-variants (e.g., "custom.1", "custom.2", "custom.2.test")
                trimmedPlaceholder = Regex.Replace(trimmedPlaceholder, @"custom\.((\w+\.\w+)|\w+)$", "custom");

                // Check if the trimmed placeholder exists in the list of known placeholders
                if (placeholders.Contains(trimmedPlaceholder) || placeholder.Contains(placeholder)) {
                    foundPlaceholders.Add(placeholder); // Add the original placeholder with sub-variant if present
                }
            }

            // Return the list of found placeholders
            return foundPlaceholders.OrderBy(x => !x.StartsWith(CUSTOM)).ToList();
        }

        private static string TranslateTime(string placeholder, string date, string sourceTimeZoneId, string targetTimeZoneId) {

            var sourceTimeZone = DateTimeUtil.CheckTimeZone(sourceTimeZoneId);
            var targetTimeZone = DateTimeUtil.CheckTimeZone(targetTimeZoneId);

            if (Regex.IsMatch(placeholder, @".*\.1$"))
                return DateTimeUtil.ConvertDateTimeToDiscordTimestamp(date, sourceTimeZone, targetTimeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);

            if (Regex.IsMatch(placeholder, @".*\.2$"))
                return DateTimeUtil.ConvertDateTimeToDiscordTimestamp(date, sourceTimeZone, targetTimeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);

            if (Regex.IsMatch(placeholder, @".*\.3$"))
                return DateTimeUtil.ConvertDateTimeToDiscordTimestamp(date, sourceTimeZone, targetTimeZone, TimestampEnum.LONG_DATE);

            if (Regex.IsMatch(placeholder, @".*\.4$"))
                return DateTimeUtil.ConvertDateTimeToDiscordTimestamp(date, sourceTimeZone, targetTimeZone, TimestampEnum.SHORT_DATE);

            if (Regex.IsMatch(placeholder, @".*\.5$"))
                return DateTimeUtil.ConvertDateTimeToDiscordTimestamp(date, sourceTimeZone, targetTimeZone, TimestampEnum.LONG_TIME);

            if (Regex.IsMatch(placeholder, @".*\.6$"))
                return DateTimeUtil.ConvertDateTimeToDiscordTimestamp(date, sourceTimeZone, targetTimeZone, TimestampEnum.SHORT_TIME);

            if (Regex.IsMatch(placeholder, @".*\.7$"))
                return DateTimeUtil.ConvertDateTimeToDiscordTimestamp(date, sourceTimeZone, targetTimeZone, TimestampEnum.RELATIVE);

            var result = DateTimeUtil.ConvertDateTimeToDiscordTimestamp(date, sourceTimeZone, targetTimeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);
            return result;
        }
    }
}
