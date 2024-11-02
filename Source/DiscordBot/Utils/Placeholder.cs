using DiscordBot.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Utils {
    public static class Placeholder {

        private static List<string> placeholders = new List<string>();
        private const string PREFIX = "";

        public const string ID = PREFIX + "plchldr.id";
        public const string TIMEZONE = PREFIX + "plchldr.timezone";
        public const string DATE_START = PREFIX + "plchldr.date.start";
        public const string DATE_END = PREFIX + "plchldr.date.end";
        public const string USER_NAME = PREFIX + "plchldr.user.name";
        public const string USER_AVATARURL = PREFIX + "plchldr.user.avatarurl";
        public const string LIST_USERS = PREFIX + "plchldr.list.users";
        public const string LIST_REACTIONS = PREFIX + "plchldr.list.reactions";

        static Placeholder() {
            placeholders.AddRange(new[] { 
                ID, TIMEZONE, DATE_START, DATE_END, USER_NAME, USER_AVATARURL, LIST_USERS, LIST_REACTIONS });
        }

        public static async Task<string> Translate(string text, Dictionary<string, object> data) {
            foreach (var placeholder in placeholders) {
                if (data.TryGetValue(placeholder, out var value) && value is string result) {
                    text = await Replace(text, placeholder, result);
                }
            }
            return text;
        }


        private static async Task<string> Replace(string text, string placeholder, string value) {
            return text.Replace("{" + placeholder + "}", await ParseTime(value));
        }

        private static async Task<string> ParseTime(string value) {
            if (DateTime.TryParseExact(value, "d/M/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime date)) {
                if (Regex.IsMatch(value, @"plchldr\.date\.(start|end)\.1"))
                    return await DiscordUtil.TranslateToDynamicTimestamp(date, "CET", TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);
                if (Regex.IsMatch(value, @"plchldr\.date\.(start|end)\.2"))
                    return await DiscordUtil.TranslateToDynamicTimestamp(date, "CET", TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                if (Regex.IsMatch(value, @"plchldr\.date\.(start|end)\.3"))
                    return await DiscordUtil.TranslateToDynamicTimestamp(date, "CET", TimestampEnum.LONG_DATE);
                if (Regex.IsMatch(value, @"plchldr\.date\.(start|end)\.4"))
                    return await DiscordUtil.TranslateToDynamicTimestamp(date, "CET", TimestampEnum.SHORT_DATE);
                if (Regex.IsMatch(value, @"plchldr\.date\.(start|end)\.5"))
                    return await DiscordUtil.TranslateToDynamicTimestamp(date, "CET", TimestampEnum.LONG_TIME);
                if (Regex.IsMatch(value, @"plchldr\.date\.(start|end)\.6"))
                    return await DiscordUtil.TranslateToDynamicTimestamp(date, "CET", TimestampEnum.SHORT_TIME);
                if (Regex.IsMatch(value, @"plchldr\.date\.(start|end)\.7"))
                    return await DiscordUtil.TranslateToDynamicTimestamp(date, "CET", TimestampEnum.RELATIVE);
                return await Task.FromResult(value);
            } else return await Task.FromResult(value);
        }   

    }
}
