using DiscordBot.Exceptions;
using DiscordBot.Model.Enums;
using DiscordBot.Services;
using DiscordBot.Utils;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands.Slash {
    public class GeneralCommands : ApplicationCommandModule {

        private const string TIMESTAMP = "TIMESTAMP";

        [SlashCommand(TIMESTAMP, "Generate dynamic discord timestamp")]
        [RequirePermission(CommandEnum.TIMESTAMP)]
        public async Task UseTimestampCommand(InteractionContext ctx,
            [Option("time-zone", "The time zone you live in.")] TimeZoneEnum timeZone) {
            try {
                // Build the embed message with default values
                var modal = new DiscordInteractionResponseBuilder();
                modal.WithTitle($"TIMESTAMP").WithCustomId(Identity.MODAL_TIMESTAMP)
                    .AddComponents(new TextInputComponent("TIMEZONE", Identity.MODAL_COMP_TIMESTAMP_TIMEZONE, "Europe/Brussels", timeZone.ToString(), true, TextInputStyle.Short))
                    .AddComponents(new TextInputComponent("TIMEZONE", Identity.MODAL_COMP_TIMESTAMP_TIME, "DD/MM/YYYY hh:mm", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), true, TextInputStyle.Short, 16, 16));

                // Create response model
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);


                //break;

                //if (DateTime.TryParseExact(inputDateTime, "d/M/yyyy H:m", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime)) {

                //    string date1 = await DiscordUtil.TranslateToDynamicTimestamp(parsedDateTime, timeZone, TimestampEnum.SHORT_DATE);
                //    string date2 = await DiscordUtil.TranslateToDynamicTimestamp(parsedDateTime, timeZone, TimestampEnum.SHORT_TIME);
                //    string date3 = await DiscordUtil.TranslateToDynamicTimestamp(parsedDateTime, timeZone, TimestampEnum.LONG_DATE);
                //    string date4 = await DiscordUtil.TranslateToDynamicTimestamp(parsedDateTime, timeZone, TimestampEnum.LONG_TIME);
                //    string date5 = await DiscordUtil.TranslateToDynamicTimestamp(parsedDateTime, timeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                //    string date6 = await DiscordUtil.TranslateToDynamicTimestamp(parsedDateTime, timeZone, TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME);
                //    string date7 = await DiscordUtil.TranslateToDynamicTimestamp(parsedDateTime, timeZone, TimestampEnum.RELATIVE);

                //    EmbedBuilder embed = new EmbedBuilder()
                //        .WithAuthor($"Dynamic Discord Time ({timeZone})", "https://cdn-icons-png.flaticon.com/512/2972/2972531.png", "https://cdn-icons-png.flaticon.com/512/2972/2972531.png")
                //        .WithDescription("These are [**Dynamic Discord Timestamps**](https://r.3v.fi/discord-timestamps/), automatically adjusting based on the time zone you have provided.")
                //        .AddField("Relative", $"{date7}\n```m\n{date7}```", true)
                //        .AddField("Short Date", $"{date1}\n```m\n{date1}```", true)
                //        .AddField("Short Time", $"{date2}\n```m\n{date2}```", true)
                //        .AddField("Long Date", $"{date3}\n```m\n{date3}```", true)
                //        .AddField("Long Time", $"{date4}\n```m\n{date4}```", true)
                //        .AddField("Long Date & Long Time", $"{date5}\n```m\n{date5}```", true)
                //        .AddField("Long Date, Day of Week & Short Time", $"{date6}\n```m\n{date6}```", true);

                //    var response = new DiscordInteractionResponseBuilder()
                //        .AddEmbed(embed.Build())
                //        .AsEphemeral(true);

                //    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);

                
            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: {ex}");
            }
        }
    }
}
