using DiscordBot.Model.Enums;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Utils {
    public static class Placeholder {

        private const string PREFIX = "";
        private static List<string> placeholders = new List<string>();

        public const string ID = PREFIX + "data.id";
        public const string CUSTOM = PREFIX + "data.custom";
        public const string TIMEZONE = PREFIX + "data.timezone";
        public const string DATE_START = PREFIX + "data.date.start";
        public const string DATE_END = PREFIX + "data.date.end";
        public const string USER_NAME = PREFIX + "data.user.name";
        public const string USER_AVATARURL = PREFIX + "data.user.avatarurl";
        public const string LIST_USERS = PREFIX + "data.list.users";
        public const string LIST_REACTIONS = PREFIX + "data.list.reactions";

        static Placeholder() {
            placeholders.AddRange(new[] {
                ID, DATE_START, DATE_END, USER_NAME, USER_AVATARURL, LIST_USERS, LIST_REACTIONS});
        }

        public static async Task<DiscordEmbed> Translate(EmbedBuilder embed) {
            var clone = embed.DeepClone2();

            if (clone.CustomData != null) {
                clone.Content = embed.Content ?? await Translate(embed.Content!, (Dictionary<string, string>)clone.CustomData);
                clone.Description = embed.Description ?? await Translate(embed.Description!, (Dictionary<string, string>)clone.CustomData);
            }

            return clone.Build();
        }

        public static async Task<string> Translate(string input, Dictionary<string, string> data) {
            var replacements = ExtractPlaceholders(input);

            foreach (var placeholder in replacements) {
                string toReplace = $"{{{placeholder}}}";
                if (data.TryGetValue(Regex.Replace(placeholder, @"(\.\d+)$", ""), out var replacement)) {
                    replacement = input.Replace(toReplace, await TranslateTime(replacement, data[TIMEZONE], placeholder));
                    input = input.Replace(toReplace, replacement);
                } else {
                    input = input.Replace(toReplace, "{placeholder.not.found}");
                }
            }

            return input;
        }

        private static List<string> ExtractPlaceholders(string input) {
            // Extract all placeholders from the input string using regex
            var matches = Regex.Matches(input, @"\{(.*?)\}");
            var foundPlaceholders = new List<string>();

            // Iterate through the matches and add valid group values
            foreach (Match match in matches) {
                string placeholder = match.Groups[1].Value; // This gets the value inside the brackets

                // Trim numeric sub-variants (e.g., ".1", ".2")
                string trimmedPlaceholder = Regex.Replace(placeholder, @"(\.\d+)$", "");

                // Check if the trimmed placeholder exists in the list of known placeholders
                if (placeholders.Contains(trimmedPlaceholder)) {
                    foundPlaceholders.Add(placeholder); // Add the original placeholder with sub-variant if present
                }
            }

            // Return the list of found placeholders
            return foundPlaceholders;
        }

        private static async Task<string> TranslateTime(string input, string timeZone, string placeholder) {
            if (placeholder.Contains(DATE_START) || placeholder.Contains(DATE_END)) {
                if (DateTime.TryParseExact(input, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime date)) {

                    if (Regex.IsMatch(placeholder, @".*\.1$"))
                        return await DiscordUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);

                    if (Regex.IsMatch(placeholder, @".*\.2$"))
                        return await DiscordUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);

                    if (Regex.IsMatch(placeholder, @".*\.3$"))
                        return await DiscordUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_DATE);

                    if (Regex.IsMatch(placeholder, @".*\.4$"))
                        return await DiscordUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.SHORT_DATE);

                    if (Regex.IsMatch(placeholder, @".*\.5$"))
                        return await DiscordUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_TIME);

                    if (Regex.IsMatch(placeholder, @".*\.6$"))
                        return await DiscordUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.SHORT_TIME);

                    if (Regex.IsMatch(placeholder, @".*\.7$"))
                        return await DiscordUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.RELATIVE);

                    return await DiscordUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);
                } else return input;
            } else return input;
        }
    }
}
