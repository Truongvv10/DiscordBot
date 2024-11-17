using BLL.Enums;
using BLL.Services;
using System.Text.RegularExpressions;

namespace APP.Utils {
    public static class Placeholder {

        private const string PREFIX = "";
        private static List<string> placeholders = new List<string>();

        public const string ID = PREFIX + "data.id";
        public const string CUSTOM = PREFIX + "data.custom";
        public const string TIMEZONE = PREFIX + "data.timezone";
        public const string TEMPLATE = PREFIX + "data.template";
        public const string DATE_START = PREFIX + "data.date.start";
        public const string DATE_END = PREFIX + "data.date.end";
        public const string USER_NAME = PREFIX + "data.user.name";
        public const string USER_AVATARURL = PREFIX + "data.user.avatarurl";
        public const string LIST_USERS = PREFIX + "data.list.users";
        public const string LIST_REACTIONS = PREFIX + "data.list.reactions";
        public const string LIST_TEMPLATES = PREFIX + "data.list.templates";
        public const string LIST_TEMPLATES_GUILD = PREFIX + "data.list.templates.guild";


        static Placeholder() {
            placeholders.AddRange(new[] {
                ID, CUSTOM, TIMEZONE, DATE_START, DATE_END, USER_NAME, USER_AVATARURL, LIST_USERS, LIST_REACTIONS, LIST_TEMPLATES, LIST_TEMPLATES_GUILD});
        }

        public static async Task<string> Translate(string input, Dictionary<string, string> data) {
            var replacements = ExtractPlaceholders(input);

            foreach (var placeholder in replacements) {
                string toReplace = $"{{{placeholder}}}";
                string value = Regex.Replace(placeholder, @"(\.\d+)$", "");
                if (data.TryGetValue(value, out var replacement)) {
                    if (Regex.IsMatch(placeholder, @"^data\.date\.start(\.\d+)?$")) {
                        string timezone = data[TIMEZONE] ?? "CET";
                        DateTime date = DateTime.ParseExact(data[DATE_START], "dd/MM/yyyy HH:mm", null);
                        var timestamp = await TranslateTime(date, timezone, placeholder);
                        input = input.Replace(toReplace, timestamp);
                    }
                    if (Regex.IsMatch(placeholder, @"^data\.date\.end(\.\d+)?$")) {
                        string timezone = data[TIMEZONE] ?? "CET";
                        DateTime date = DateTime.ParseExact(data[DATE_END], "dd/MM/yyyy HH:mm", null);
                        var timestamp = await TranslateTime(date, timezone, placeholder);
                        input = input.Replace(toReplace, timestamp);
                    }
                    if (placeholder.Equals(TIMEZONE)) 
                        input = input.Replace(toReplace, data[TIMEZONE]);

                } else if (Regex.IsMatch(placeholder, @"^data\.date\.(start|end)(\.\d+)?$")) {
                    var timestamp = await TranslateTime(DateTime.Now, "CET", placeholder);
                    input = input.Replace(toReplace, timestamp);
                } else if (placeholder == TIMEZONE) {
                    input = input.Replace(toReplace, "CET");
                }

                if (placeholder == LIST_TEMPLATES) {
                    var cache = new CacheData();
                    await cache.LoadTemplates();
                    input = input.Replace(toReplace, cache.Templates.Keys.Aggregate("", (current, next) => current + "`" + next + "`, "));
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

                // Check if the trimmed placeholder exists in the list of known placeholders
                if (placeholders.Contains(trimmedPlaceholder)) {
                    foundPlaceholders.Add(placeholder); // Add the original placeholder with sub-variant if present
                }
            }

            // Return the list of found placeholders
            return foundPlaceholders.ToList();
        }

        private static async Task<string> TranslateTime(DateTime date, string timeZone, string placeholder) {

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

            var result = await DiscordUtil.TranslateToDynamicTimestamp(date, timeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);
            return result;
        }
    }
}
