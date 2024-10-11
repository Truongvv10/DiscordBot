using DiscordBot.Exceptions;
using DiscordBot.Model.Enums;
using DiscordBot.Model;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using XironiteDiscordBot.Commands;
using DiscordBot.Utils;
using DSharpPlus;
using System.Diagnostics;
using NodaTime;

namespace DiscordBot.Commands.Slash {
    public class EventCmd : SlashCommand {
        [SlashCommand("event-create", "Send an embeded message to the current channel")]
        public async Task UseEventCommand(InteractionContext ctx,
            [Option("day", "The date of the event with format: ''dd/mm/yyyy''.")] double day,
            [Option("month", "The time of the event with format: ''hh:mm''.")] YearMonth month,
            [Option("date", "The date of the event with format: ''dd/mm/yyyy''.")] string date,
            [Option("time", "The time of the event with format: ''hh:mm''.")] string time,
            [Option("timezone", "The timezone of the date & time will be calculated to.")] string timeZone,
            [Option("channel", "The channel where your embeded message will be sent to.")] DiscordChannel channel,
            [Option("image", "The main image of your embeded message that will be added.")] DiscordAttachment? image = null,
            [Option("thumbnail", "The thumbnail of your embeded message that will be added.")] DiscordAttachment? thumbnail = null,
            [Option("ping", "The role that will get pinged on sending message.")] DiscordRole? pingrole = null) {

            if (!await CheckPermission(ctx, CommandEnum.EVENTS)) {
                await showNoPermissionMessage(ctx);
                return;
            }
            LogCommand(ctx, CommandEnum.EVENTS);

            if (!CacheData.Timezones.Contains(timeZone, StringComparer.OrdinalIgnoreCase)) {
                return;
            }
            try {
                string dateTime = $"{date.Trim()} {time.Trim()}";
                string startDate = string.Empty;
                string endDate = string.Empty;
                string startDateRelative = string.Empty;

                if (DateTime.TryParseExact(dateTime, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime)) {
                    startDate = await DiscordUtil.TranslateTimestamp(parsedDateTime, timeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                    endDate = await DiscordUtil.TranslateTimestamp(parsedDateTime.AddHours(1.0), timeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                    startDateRelative = await DiscordUtil.TranslateTimestamp(parsedDateTime, timeZone, TimestampEnum.RELATIVE);
                }

                EmbedBuilder embed = new EmbedBuilder() {
                    Description =
                        "# [replace] Event\n" +
                        "Hey **everyone**! We'll be hosting **[replace]**!\n" +
                        "## 🔸 Game Info\n" +
                        "[replace]\n" +
                        "## 🔸 Top 5 leaderboard rewards\n" +
                        "**``1st``** 🥇 500 event points\n" +
                        "**``2nd``** 🥈 350 event points \n" +
                        "**``3rd``** 🥉 250 event points \n" +
                        "**``4th``**    150 event points\n" +
                        "**``5th``**    100 event points\n" +
                        "**``All``**    5 event points per game\n" +
                        "\n" +
                        "Earn event points by participating in and winning events. Exchange them for in-game cosmetics, items, and perks using the `/events` command. Points are shared across all servers, but each server may have a different shop selection.\n" +
                        "## 🔸 When will it start?",
                    Image = @"https://i.imgur.com/07DVuUb.gif",
                    HasTimeStamp = true,
                    ChannelId = channel.Id,
                    Owner = ctx.User.Id,
                    Footer = $"Your host {ctx.User.Username}",
                    FooterUrl = ctx.User.AvatarUrl
                };

                embed.AddField("Start Date:", $"{startDate} ({startDateRelative})", false);
                embed.AddField("End Date:", endDate, false);
                embed.AddField("React with a `✅` if you're coming to the event!", "**React with a `❌` if you're going to miss out...**", false);

                if (image is not null) embed.WithImage(image.Url);
                if (thumbnail is not null) embed.WithThumbnail(thumbnail.Url);
                if (pingrole is not null) embed.AddPingRole(pingrole.Id);
                embed.AddCustomSaveMessage("Event", "N/A");
                embed.AddCustomSaveMessage(Identity.EVENT_TIMEZONE, timeZone);
                embed.AddCustomSaveMessage(Identity.EVENT_START, parsedDateTime.ToString("dd/MM/yyyy HH:mm"));
                embed.AddCustomSaveMessage(Identity.EVENT_END, parsedDateTime.AddHours(1).ToString("dd/MM/yyyy HH:mm"));
                await CreateEmbedMessageAsync(ctx, embed, EmbedType.EVENT, channel.Id, false);
            } catch (Exception ex) {
                throw new CommandException($"Embed.UseEmbedCommand: {ex}");
            }
        }
    }
}
