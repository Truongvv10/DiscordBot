using APP.Enums;
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
        public const string USER_AVATARURL = PREFIX + "data.user.avatarurl";
        public const string CHANNEL_ID = PREFIX + "data.channel.id";
        public const string GUILD_ID = PREFIX + "data.guild.id";
        public const string MESSAGE_ID = PREFIX + "data.message.id";
        public const string LIST_USERS = PREFIX + "data.list.users";
        public const string LIST_REACTIONS = PREFIX + "data.list.reactions";
        public const string LIST_TEMPLATES = PREFIX + "data.list.templates";
        public const string LIST_TEMPLATES_GUILD = PREFIX + "data.list.templates.guild";


        static Placeholder() {
            placeholders.AddRange(new[] {
                ID, CUSTOM, TIMEZONE, DATE_START, DATE_END, URL1, URL2, URL3, URL4, USER_NAME, USER_AVATARURL, CHANNEL_ID, GUILD_ID, MESSAGE_ID, LIST_USERS, LIST_REACTIONS, LIST_TEMPLATES, LIST_TEMPLATES_GUILD});
        }

        public static async Task<string> Translate(string input, Dictionary<string, string> data, DiscordInteraction interaction, IDataRepository dataService) {
            foreach (var placeholder in ExtractPlaceholders(input)) {
                string toReplace = $"{{{placeholder}}}";
                switch (placeholder) {
                    case USER_NAME:
                        input = input.Replace(toReplace, interaction.User.Username);
                        break;
                    case USER_AVATARURL:
                        input = input.Replace(toReplace, interaction.User.AvatarUrl);
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
                    default:
                        if (data.TryGetValue(placeholder, out var replacement)) {
                            if (Regex.IsMatch(placeholder, @"^data\.date\.start(\.\d+)?$")) {
                                string timezone = data[TIMEZONE] ?? "CET";
                                DateTime date = DateTime.ParseExact(data[DATE_START], "dd/MM/yyyy HH:mm", null);
                                var timestamp = await TranslateTime(date, timezone, placeholder);
                                input = input.Replace(toReplace, timestamp);
                            } else if (Regex.IsMatch(placeholder, @"^data\.date\.end(\.\d+)?$")) {
                                string timezone = data[TIMEZONE] ?? "CET";
                                DateTime date = DateTime.ParseExact(data[DATE_END], "dd/MM/yyyy HH:mm", null);
                                var timestamp = await TranslateTime(date, timezone, placeholder);
                                input = input.Replace(toReplace, timestamp);
                            } else if (placeholder.Equals(TIMEZONE)) {
                                input = input.Replace(toReplace, data[TIMEZONE]);
                            } else {
                                input = input.Replace(toReplace, replacement);
                            }

                        } else if (Regex.IsMatch(placeholder, @"^data\.date\.(start|end)(\.\d+)?$")) {
                            var timestamp = await TranslateTime(DateTime.Now, "CET", placeholder);
                            input = input.Replace(toReplace, timestamp);
                        } else if (placeholder == TIMEZONE) {
                            input = input.Replace(toReplace, "CET");
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
                if (placeholders.Contains(trimmedPlaceholder)) {
                    foundPlaceholders.Add(placeholder); // Add the original placeholder with sub-variant if present
                }
            }

            // Return the list of found placeholders
            return foundPlaceholders.OrderBy(x => !x.StartsWith(CUSTOM)).ToList();
        }

        private static async Task<string> TranslateTime(DateTime date, string timeZone, string placeholder) {

            if (Regex.IsMatch(placeholder, @".*\.1$"))
                return await DateTimeUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);

            if (Regex.IsMatch(placeholder, @".*\.2$"))
                return await DateTimeUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);

            if (Regex.IsMatch(placeholder, @".*\.3$"))
                return await DateTimeUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_DATE);

            if (Regex.IsMatch(placeholder, @".*\.4$"))
                return await DateTimeUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.SHORT_DATE);

            if (Regex.IsMatch(placeholder, @".*\.5$"))
                return await DateTimeUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_TIME);

            if (Regex.IsMatch(placeholder, @".*\.6$"))
                return await DateTimeUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.SHORT_TIME);

            if (Regex.IsMatch(placeholder, @".*\.7$"))
                return await DateTimeUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.RELATIVE);

            var result = await DateTimeUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);
            return result;
        }
    }
}
