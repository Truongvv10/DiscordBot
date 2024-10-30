using DiscordBot.Exceptions;
using DiscordBot.Model.Enums;
using DiscordBot.Model;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DiscordBot.Commands;
using DiscordBot.Utils;
using DSharpPlus;
using System.Diagnostics;
using NodaTime;
using DiscordBot.Services;
using System.Reflection;

namespace DiscordBot.Commands.Slash {

    [SlashCommandGroup("event", "Commands for creating events")]
    public class EventCmd : ApplicationCommandModule {

        private const string EVENT = "EVENT";
        private const string EVENT_CREATE = "CREATE";
        private const string EVENT_POST_CREATE = "POST_CREATE";
        private const string EVENT_EDIT = "EDIT";
        private const string EVENT_REMINDER = "REMINDER";
        private const string EVENT_WINNERS = "WINNERS";

        [SlashCommand(EVENT_CREATE, "Send an embeded message to the current channel")]
        [RequirePermission(CommandEnum.EVENTS)]
        public async Task Create(InteractionContext ctx,
            [Option("name", "The name of the event that will be used as title.")] string name,
            [Option("time_zone", "The timezone of the date & time will be calculated to.")] TimeZoneEnum timeZone,
            [Option("sent_channel", "The channel where your event will be sent to.")] DiscordChannel channel,
            [Option("hidden", "If only you can see this embeded message, default is false")] bool hidden = false,
            [Option("image", "The main image of your event message that will be added.")] DiscordAttachment? image = null,
            [Option("thumbnail", "The thumbnail of your event message that will be added.")] DiscordAttachment? thumbnail = null,
            [Option("ping", "The server roles that are pinged on sending event message.")] DiscordRole? pingrole = null) {
            try {
                // Build the embed message with default values
                var template = $"{EVENT}_{EVENT_POST_CREATE}";
                var embed = await CacheData.GetTemplate(ctx.Guild.Id, template);

                // Set the values
                embed.Author = name.Replace(" Event", "").Replace("Event", "") + " Event";
                embed.ChannelId = channel.Id;
                embed.Owner = ctx.User.Id;
                embed.IsEphemeral = hidden;
                embed.Type = CommandEnum.EVENTS_CREATE;

                // Set the custom data
                embed.AddCustomData(Identity.EVENT_NAME, name);
                embed.AddCustomData(Identity.EVENT_TIMEZONE, timeZone.ToString());
                embed.AddCustomData(Identity.EVENT_START, DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm"));
                embed.AddCustomData(Identity.EVENT_END, DateTime.Now.AddDays(1).AddHours(1).ToString("dd/MM/yyyy HH:mm"));

                // Add the optional values
                if (image is not null) embed.Image = image.Url;
                if (thumbnail is not null) embed.Thumbnail = thumbnail.Url;
                if (pingrole is not null) embed.AddPingRole(pingrole.Id);

                // Create the embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.EVENTS_CREATE, ctx.Interaction, embed, channel.Id, hidden);

            } catch (Exception ex) {
                throw new CommandException($"Embed.UseEmbedCommand: {ex}");
            }
        }
    }
}


 