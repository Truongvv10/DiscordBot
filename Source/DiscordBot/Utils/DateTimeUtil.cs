using APP.Enums;
using BLL.Enums;
using BLL.Exceptions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Utils {
    public static class DateTimeUtil {

        public static DateTime RoundDateTime(DateTime dateTime) {
            int roundedMinutes = (dateTime.Minute < 15) ? 0 : (dateTime.Minute < 45) ? 30 : 0;
            var result = dateTime.AddMinutes(roundedMinutes - dateTime.Minute).AddHours(dateTime.Minute >= 45 ? 1 : 0);
            return result;
        }

        public static bool IsStringValidDate(string date) {
            bool result = DateTime.TryParseExact(date, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out _);
            return result;
        }

        public static bool ExistInNodaTimeZone(string timeZoneId) {
            timeZoneId = TranslateCustomTimezone(timeZoneId);
            var test = DateTimeZoneProviders.Tzdb.GetZoneOrNull(TranslateCustomTimezone(timeZoneId)) is null ? false : true;
            return test;
        }

        public static async Task<string> TranslateToDynamicTimestamp(DateTime localDateTime, string timeZoneId, TimestampEnum timestampType) {
            try {
                // Translate custom timezones to Discord timezones
                timeZoneId = TranslateCustomTimezone(timeZoneId);

                // Map TimestampEnum to Discord format characters
                char timestampTypeChar = timestampType switch {
                    TimestampEnum.SHORT_TIME => 't',
                    TimestampEnum.LONG_TIME => 'T',
                    TimestampEnum.SHORT_DATE => 'd',
                    TimestampEnum.LONG_DATE => 'D',
                    TimestampEnum.LONG_DATE_AND_SHORT_TIME => 'f',
                    TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME => 'F',
                    TimestampEnum.RELATIVE => 'R',
                    _ => 'F',
                };

                // Convert the DateTime to NodaTime LocalDateTime
                LocalDateTime localDateTimeValue = LocalDateTime.FromDateTime(localDateTime);

                // Get the time zone
                DateTimeZone timeZone = DateTimeZoneProviders.Tzdb[timeZoneId];

                // Convert LocalDateTime to ZonedDateTime in the specified time zone
                ZonedDateTime zonedDateTime = timeZone.AtLeniently(localDateTimeValue);

                // Convert ZonedDateTime to Instant
                Instant instantFromZonedDateTime = zonedDateTime.ToInstant();

                // Convert to Unix Timestamp
                long unixTimestamp = instantFromZonedDateTime.ToUnixTimeSeconds();

                // Format the timestamp for Discord
                string discordTimestamp = $"<t:{unixTimestamp}:{timestampTypeChar}>";

                return await Task.FromResult(discordTimestamp);

            } catch (Exception ex) {
                throw new UtilException($"Could not translate {localDateTime} to Discord timestamp.", ex);
            }
        }

        private static string TranslateCustomTimezone(string timeZone) {

            var copy = timeZone.ToUpper();

            if (copy == TimeZoneEnum.BST.ToString())
                return "Europe/London";

            if (copy == TimeZoneEnum.MSK.ToString())
                return "Europe/Moscow";

            if (copy == TimeZoneEnum.GST.ToString())
                return "Etc/GMT+4";

            if (copy == TimeZoneEnum.PKT.ToString())
                return "Etc/GMT+5";

            if (copy == TimeZoneEnum.IST.ToString())
                return "Etc/GMT+5";

            if (copy == TimeZoneEnum.BST.ToString())
                return "Etc/GMT+6";

            if (copy == TimeZoneEnum.WIB.ToString())
                return "Etc/GMT+7";

            if (copy == TimeZoneEnum.JST.ToString())
                return "Japan";

            if (copy == TimeZoneEnum.BST.ToString())
                return "Japan";

            if (copy == TimeZoneEnum.AEST.ToString())
                return "Etc/GMT+10";

            if (copy == TimeZoneEnum.NZST.ToString())
                return "Etc/GMT+12";

            if (copy == TimeZoneEnum.HST.ToString())
                return "US/Hawaii";

            if (copy == TimeZoneEnum.AKST.ToString())
                return "US/Alaska";

            if (copy == TimeZoneEnum.PST.ToString())
                return "US/Pacific";

            if (copy == TimeZoneEnum.MST.ToString())
                return "US/Mountain";

            if (copy == TimeZoneEnum.CST.ToString())
                return "US/Central";

            if (copy == TimeZoneEnum.AST.ToString())
                return "Canada/Atlantic";

            return timeZone;
        }

    }
}
