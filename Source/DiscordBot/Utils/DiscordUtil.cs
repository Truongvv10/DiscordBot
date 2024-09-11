using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XironiteDiscordBot.Exceptions;
using System.Data;
using DiscordBot.Model.Enums;
using NodaTime;
using System.Threading.Channels;

namespace DiscordBot.Utils {
    public static class DiscordUtil {


        public static async Task SendImageEmbedAsync(DiscordChannel channel, string imagePath, string title = "Here is your image!", string description = "This image was uploaded as an attachment.") {
            // Check if the file exists
            if (!File.Exists(imagePath)) {
                await channel.SendMessageAsync("Error: The specified image file does not exist.");
                return;
            }

            // Read the image as a file stream
            using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read)) {
                // Create the embed message
                var message = new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder()
                        .WithTitle(title)
                        .WithDescription(description)
                        .WithImageUrl($"attachment://{Path.GetFileName(imagePath)}")) // Link to the attached image
                    .AddFile(Path.GetFileName(imagePath), fs); // Attach the image with its filename

                // Send the embed with the image attached
                await channel.SendMessageAsync(message);
            }
        }

        public static async Task<DiscordMessage> GetMessageByIdAsync(DiscordChannel channel, ulong messageId) {
            try {
                var message = await channel.GetMessageAsync(messageId);
                return message;
            } catch (Exception ex) {
                throw new Exception($"Error retrieving message with ID {messageId} in channel {channel.Name}: {ex.Message}");
            }
        }

        public static async Task<DiscordRole> GetRolesByIdAsync(DiscordGuild guild, ulong roleId) {
            if (guild.Roles.TryGetValue(roleId, out var role)) {
                var test = role;
                return await Task.FromResult(role);
            } else throw new UtilException($"No roles with id \"{roleId}\" found.");
        }

        public static async Task<DiscordChannel> GetChannelByIdAsync(DiscordGuild guild, ulong channelId) {
            if (guild.Channels.TryGetValue(channelId, out var channel)) {
                return await Task.FromResult(channel);
            } else throw new UtilException($"No channel with id \"{channelId}\" found.");
        }

        public static async Task<string> TranslateTimestamp(DateTime localDateTime, string timeZoneId, TimestampEnum timestampType) {
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
                    _ => 'R',
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
                throw new Exception($"Could not translate {localDateTime} to Discord timestamp.", ex);
            }
        }
    }
}