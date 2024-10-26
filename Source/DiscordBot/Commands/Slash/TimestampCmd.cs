using DiscordBot.Model;
using DiscordBot.Model.Enums;
using DiscordBot.Services;
using DiscordBot.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DiscordBot.Commands;

namespace DiscordBot.Commands.Slash {
    public class TimestampCmd : SlashCommand {

        [SlashCommand("timestamp", "Generate dynamic discord timestamp")]
        [RequirePermission(CommandEnum.TIMESTAMP)]
        public async Task UseTimestampCommand(InteractionContext ctx,
            [Option("date", "The date with format: \"dd/mm/yyyy\".")] string date,
            [Option("time", "The time with format: \"hh/mm\". (24 hour clock)")] string time,
            [Option("time-zone", "The time zone you live in.")] string timeZone) {

            LogCommand(ctx, CommandEnum.TIMESTAMP);

            try {
                string inputDateTime = $"{date.Trim()} {time.Trim()}";
                if (DateTime.TryParseExact(inputDateTime, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime)) {

                    string date1 = await DiscordUtil.TranslateTimestamp(parsedDateTime, timeZone, TimestampEnum.SHORT_DATE);
                    string date2 = await DiscordUtil.TranslateTimestamp(parsedDateTime, timeZone, TimestampEnum.SHORT_TIME);
                    string date3 = await DiscordUtil.TranslateTimestamp(parsedDateTime, timeZone, TimestampEnum.LONG_DATE);
                    string date4 = await DiscordUtil.TranslateTimestamp(parsedDateTime, timeZone, TimestampEnum.LONG_TIME);
                    string date5 = await DiscordUtil.TranslateTimestamp(parsedDateTime, timeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                    string date6 = await DiscordUtil.TranslateTimestamp(parsedDateTime, timeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);
                    string date7 = await DiscordUtil.TranslateTimestamp(parsedDateTime, timeZone, TimestampEnum.RELATIVE);

                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor($"Dynamic Discord Time ({timeZone})", "https://cdn-icons-png.flaticon.com/512/2972/2972531.png", "https://cdn-icons-png.flaticon.com/512/2972/2972531.png")
                        .WithDescription("These are [**Dynamic Discord Timestamps**](https://r.3v.fi/discord-timestamps/), automatically adjusting based on the time zone you have provided.")
                        .AddField("Relative", $"{date7}\n```m\n{date7}```", true)
                        .AddField("Short Date", $"{date1}\n```m\n{date1}```", true)
                        .AddField("Short Time", $"{date2}\n```m\n{date2}```", true)
                        .AddField("Long Date", $"{date3}\n```m\n{date3}```", true)
                        .AddField("Long Time", $"{date4}\n```m\n{date4}```", true)
                        .AddField("Long Date & Long Time", $"{date5}\n```m\n{date5}```", true)
                        .AddField("Long Date, Day of Week & Short Time", $"{date6}\n```m\n{date6}```", true);

                    var response = new DiscordInteractionResponseBuilder()
                        .AddEmbed(embed.Build())
                        .AsEphemeral(true);

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);

                }
            } catch (Exception ex) {

                throw;
            }

        }
    }
}
