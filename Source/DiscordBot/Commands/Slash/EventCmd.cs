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
using DiscordBot.Services;

namespace DiscordBot.Commands.Slash {
    public class EventCmd : SlashCommand {
        [SlashCommand("event-create", "Send an embeded message to the current channel")]
        [RequirePermission(CommandEnum.EVENTS)]
        public async Task UseEventCommand(InteractionContext ctx,
            [Option("starting_in", "The total minutes remaining until the event begins.")] long starting,
            [Option("timezone", "The timezone of the date & time will be calculated to.")] string timeZone,
            [Option("channel", "The channel where your embeded message will be sent to.")] DiscordChannel channel,
            [Option("image", "The main image of your embeded message that will be added.")] DiscordAttachment? image = null,
            [Option("thumbnail", "The thumbnail of your embeded message that will be added.")] DiscordAttachment? thumbnail = null,
            [Option("ping", "The role that will get pinged on sending message.")] DiscordRole? pingrole = null) {

            LogCommand(ctx, CommandEnum.EVENTS);

            if (!CacheData.Timezones.Contains(timeZone, StringComparer.OrdinalIgnoreCase)) {
                return;
            }
            try {

                DateTime date = DateTime.Now.AddMinutes(starting);
                string startDate = await DiscordUtil.TranslateTimestamp(date, timeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                string endDate = await DiscordUtil.TranslateTimestamp(date.AddHours(1.0), timeZone, TimestampEnum.LONG_DATE_AND_SHORT_TIME);
                string startDateRelative = await DiscordUtil.TranslateTimestamp(date, timeZone, TimestampEnum.RELATIVE);

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

                string title = "# [replace] Event";
                string intro = "Hey **everyone**! We'll be hosting **[replace]**!";
                string infoTitle = "## 🔸 Game Info";
                string info = "[replace]";
                string rewardTitle = "## 🔸 Top 5 leaderboard rewards";
                string reward =
                    "**``1st``** 🥇 500 event points\n" +
                    "**``2nd``** 🥈 350 event points \n" +
                    "**``3rd``** 🥉 250 event points \n" +
                    "**``4th``**    150 event points\n" +
                    "**``5th``**    100 event points\n" +
                    "**``All``**    5 event points per game\n" +
                    "\n" +
                    "Earn event points by participating in and winning events. Exchange them for in-game cosmetics, items, and perks using the **`/events`** command. Points are shared across all servers, but each server may have a different shop selection.\n";
                string timeTitle = "## 🔸 When will it start?";

                embed.AddField("Start Date:", $"{startDate} ({startDateRelative})", false);
                embed.AddField("End Date:", endDate, false);
                embed.AddField("React with a `✅` if you're coming to the event!", "**React with a `❌` if you're going to miss out...**", false);

                if (image is not null) embed.WithImage(image.Url);
                if (thumbnail is not null) embed.WithThumbnail(thumbnail.Url);
                if (pingrole is not null) embed.AddPingRole(pingrole.Id);

                embed.AddCustomSaveMessage("Event", "N/A");
                embed.AddCustomSaveMessage(Identity.EVENT_TITLE, title);
                embed.AddCustomSaveMessage(Identity.EVENT_INTRO, intro);
                embed.AddCustomSaveMessage(Identity.EVENT_INFO_TITLE, infoTitle);
                embed.AddCustomSaveMessage(Identity.EVENT_INFO, info);
                embed.AddCustomSaveMessage(Identity.EVENT_REWARD_TITLE, rewardTitle);
                embed.AddCustomSaveMessage(Identity.EVENT_REWARD, reward);
                embed.AddCustomSaveMessage(Identity.EVENT_TIME_TITLE, timeTitle);
                embed.AddCustomSaveMessage(Identity.EVENT_TIMEZONE, timeZone);
                embed.AddCustomSaveMessage(Identity.EVENT_START, date.ToString("dd/MM/yyyy HH:mm"));
                embed.AddCustomSaveMessage(Identity.EVENT_END, date.AddHours(1).ToString("dd/MM/yyyy HH:mm"));
                await CreateEmbedMessageAsync(ctx, embed, EmbedType.EVENT, channel.Id, false);

            } catch (Exception ex) {
                throw new CommandException($"Embed.UseEmbedCommand: {ex}");
            }
        }
    }
}

internal class HoursCP : IChoiceProvider {
    public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider() {
        var choices = new List<DiscordApplicationCommandOptionChoice>();
        for (int i = 0; i < 24; i++) {
            choices.Add(new DiscordApplicationCommandOptionChoice(i.ToString(), i.ToString()));
        }
        return choices;
    }
}

internal class MinutesCP : IChoiceProvider {
    public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider() {
        var choices = new List<DiscordApplicationCommandOptionChoice>();
        for (int i = 0; i <= 55; i += 5) {
            choices.Add(new DiscordApplicationCommandOptionChoice(i.ToString(), $"{i.ToString()}m"));
        }
        return choices;
    }
}


