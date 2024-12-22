using APP.Enums;
using BLL.Enums;
using BLL.Exceptions;
using NodaTime;
using NodaTime.Text;
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

        public static bool IsStringValidDateTime(string date) {
            string[] formats = { 
                "dd/MM/yyyy HH:mm", 
                "dd-MM-yyyy HH:mm", 
                "dd.MM.yyyy HH:mm",
                "dd/MM/yyyy",
                "dd-MM-yyyy",
                "dd.MM.yyyy"};
            foreach (var format in formats) {
                if (DateTime.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.None, out _)) return true;
            }
            return false;
        }

        public static bool ExistInNodaTimeZone(string timeZoneId) {
            timeZoneId = CheckTimeZone(timeZoneId);
            var test = DateTimeZoneProviders.Tzdb.GetZoneOrNull(CheckTimeZone(timeZoneId)) is null ? false : true;
            return test;
        }

        public static async Task<string> TranslateToDynamicTimestamp(DateTime localDateTime, string timeZoneId, TimestampEnum timestampType) {
            try {
                // Translate custom timezones to Discord timezones
                timeZoneId = CheckTimeZone(timeZoneId);

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

        public static string ConvertDateTimeToDiscordTimestamp(string dateTimeString, string sourceTimeZoneId, string targetTimeZoneId, TimestampEnum timestampType = TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME, string format = "dd/MM/yyyy HH:mm") {
            try {

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

                // Parse the input date-time string
                var sourceTimeZoneIdInput = CheckTimeZone(sourceTimeZoneId);
                var targetTimeZoneIdInput = CheckTimeZone(targetTimeZoneId);
                var pattern = LocalDateTimePattern.CreateWithInvariantCulture(format);
                var parseResult = pattern.Parse(dateTimeString);

                if (!parseResult.Success) {
                    throw new FormatException($"Invalid date-time format: {dateTimeString}");
                }

                LocalDateTime localDateTime = parseResult.Value;

                // Get the source and target time zones
                var sourceTimeZone = DateTimeZoneProviders.Tzdb[sourceTimeZoneIdInput];
                var targetTimeZone = DateTimeZoneProviders.Tzdb[targetTimeZoneIdInput];

                // Create a ZonedDateTime in the source time zone
                var sourceZonedDateTime = localDateTime.InZoneStrictly(sourceTimeZone);
                var targetZonedDateTime = sourceZonedDateTime.WithZone(targetTimeZone);
                Instant instantFromZonedDateTime = targetZonedDateTime.ToInstant();
                long unixTimestamp = instantFromZonedDateTime.ToUnixTimeSeconds();
                string discordTimestamp = $"<t:{unixTimestamp}:{timestampTypeChar}>";
                return discordTimestamp;
            } catch (Exception ex) {
                return $"Error: {CheckTimeZone(targetTimeZoneId)} {ex.Message}";
            }
        }

        public static string ConvertDateTimeToDiscordTimestamp(string dateTimeString, TimeZoneEnum sourceTimeZoneId, TimeZoneEnum targetTimeZoneId, TimestampEnum timestampType = TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME, string format = "dd/MM/yyyy HH:mm") {
            try {

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

                // Parse the input date-time string
                var sourceTimeZoneIdInput = CheckTimeZone(sourceTimeZoneId);
                var targetTimeZoneIdInput = CheckTimeZone(targetTimeZoneId);
                var pattern = LocalDateTimePattern.CreateWithInvariantCulture(format);
                var parseResult = pattern.Parse(dateTimeString);

                if (!parseResult.Success) {
                    throw new FormatException($"Invalid date-time format: {dateTimeString}");
                }

                LocalDateTime localDateTime = parseResult.Value;

                // Get the source and target time zones
                var sourceTimeZone = DateTimeZoneProviders.Tzdb[sourceTimeZoneIdInput];
                var targetTimeZone = DateTimeZoneProviders.Tzdb[targetTimeZoneIdInput];

                // Create a ZonedDateTime in the source time zone
                var sourceZonedDateTime = localDateTime.InZoneStrictly(sourceTimeZone);
                var targetZonedDateTime = sourceZonedDateTime.WithZone(targetTimeZone);
                Instant instantFromZonedDateTime = targetZonedDateTime.ToInstant();
                long unixTimestamp = instantFromZonedDateTime.ToUnixTimeSeconds();
                string discordTimestamp = $"<t:{unixTimestamp}:{timestampTypeChar}>";
                return discordTimestamp;
            } catch (Exception ex) {
                return $"Error: {CheckTimeZone(targetTimeZoneId)} {ex.Message}";
            }
        }

        public static string ConvertDateTimeToTimeZone(string dateTimeString, TimeZoneEnum sourceTimeZoneId, TimeZoneEnum targetTimeZoneId, string format = "dd/MM/yyyy HH:mm") {
            try {
                // Parse the input date-time string
                var sourceTimeZoneIdInput = CheckTimeZone(sourceTimeZoneId);
                var targetTimeZoneIdInput = CheckTimeZone(targetTimeZoneId);
                var pattern = LocalDateTimePattern.CreateWithInvariantCulture(format);
                var parseResult = pattern.Parse(dateTimeString);

                if (!parseResult.Success) {
                    throw new FormatException($"Invalid date-time format: {dateTimeString}");
                }

                LocalDateTime localDateTime = parseResult.Value;

                // Get the source and target time zones
                var sourceTimeZone = DateTimeZoneProviders.Tzdb[sourceTimeZoneIdInput];
                var targetTimeZone = DateTimeZoneProviders.Tzdb[targetTimeZoneIdInput];

                // Create a ZonedDateTime in the source time zone
                var sourceZonedDateTime = localDateTime.InZoneStrictly(sourceTimeZone);

                // Convert to the target time zone
                var targetZonedDateTime = sourceZonedDateTime.WithZone(targetTimeZone);

                // Return the converted date-time as a formatted string
                return targetZonedDateTime.ToString(format, null);
            } catch (Exception ex) {
                return $"Error: {CheckTimeZone(targetTimeZoneId)} {ex.Message}";
            }
        }

        public static string CheckTimeZone(string timeZone) {

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

            if (copy == TimeZoneEnum.BST_BANGLADESH.ToString())
                return "Etc/GMT+6";

            if (copy == TimeZoneEnum.WIB.ToString())
                return "Etc/GMT+7";

            if (copy == TimeZoneEnum.JST.ToString())
                return "Japan";

            if (copy == TimeZoneEnum.KST.ToString())
                return "Japan";

            if (copy == TimeZoneEnum.AEST.ToString())
                return "Etc/GMT+10";

            if (copy == TimeZoneEnum.NZST.ToString())
                return "Etc/GMT+12";

            if (copy == TimeZoneEnum.NZDT.ToString())
                return "Pacific/Tongatapu";

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

        public static string CheckTimeZone(TimeZoneEnum timeZone) {
            switch (timeZone) {
                case TimeZoneEnum.BST:
                    return "Europe/London";
                case TimeZoneEnum.MSK:
                    return "Europe/Moscow";
                case TimeZoneEnum.GST:
                    return "Etc/GMT+4";
                case TimeZoneEnum.PKT:
                    return "Etc/GMT+5";
                case TimeZoneEnum.IST:
                    return "Etc/GMT+5";
                case TimeZoneEnum.BST_BANGLADESH:
                    return "Etc/GMT+6";
                case TimeZoneEnum.WIB:
                    return "Etc/GMT+7";
                case TimeZoneEnum.JST:
                    return "Japan";
                case TimeZoneEnum.KST:
                    return "Japan";
                case TimeZoneEnum.AEST:
                    return "Etc/GMT+10";
                case TimeZoneEnum.NZST:
                    return "Etc/GMT+12";
                case TimeZoneEnum.NZDT:
                    return "Pacific/Tongatapu";
                case TimeZoneEnum.HST:
                    return "US/Hawaii";
                case TimeZoneEnum.AKST:
                    return "US/Alaska";
                case TimeZoneEnum.PST:
                    return "US/Pacific";
                case TimeZoneEnum.MST:
                    return "US/Mountain";
                case TimeZoneEnum.CST:
                    return "US/Central";
                case TimeZoneEnum.AST:
                    return "Canada/Atlantic";
                default:
                    return timeZone.ToString();
            }
        }

    }
}
