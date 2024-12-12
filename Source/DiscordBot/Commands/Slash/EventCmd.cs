using BLL.Enums;
using BLL.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using APP.Utils;
using APP.Enums;
using APP.Services;
using BLL.Interfaces;
using BLL.Model;
using DSharpPlus;
using System;

namespace APP.Commands.Slash {

    [SlashCommandGroup("event", "Commands for creating events")]
    public class EventCmd : ApplicationCommandModule {

        #region Fields
        private const string EVENT = "EVENT";
        private const string EVENT_CREATE = "CREATE";
        private const string EVENT_POST_CREATE = "POST_CREATE";
        private const string EVENT_EDIT = "EDIT";
        private const string EVENT_REMINDER = "REMINDER";
        private const string EVENT_WINNERS = "WINNERS";
        #endregion

        #region Properties
        public required IDataService DataService { private get; set; }
        public required DiscordUtil DiscordUtil { private get; set; }
        #endregion

        [SlashCommand(EVENT_CREATE, "Send an embeded message to the current channel")]
        [RequirePermission(CommandEnum.EVENTS)]
        public async Task Create(InteractionContext ctx,
            [Option("time_zone", "The timezone of the date & time will be calculated to.")] TimeZoneEnum timeZone,
            [Option("sent_channel", "The channel where your event will be sent to.")] DiscordChannel channel,
            [Option("hidden", "If only you can see this embeded message, default is false")] bool hidden = false,
            [Option("image", "The main image of your event message that will be added.")] DiscordAttachment? image = null,
            [Option("thumbnail", "The thumbnail of your event message that will be added.")] DiscordAttachment? thumbnail = null,
            [Option("ping", "The server roles that are pinged on sending event message.")] DiscordRole? pingrole = null) {
            try {
                // Build the embed message with default values
                var template = await DataService.GetTemplateAsync(ctx.Guild.Id, $"{EVENT}_{EVENT_CREATE}");
                var message = template!.Message;
                message.ChannelId = channel.Id;
                message.Sender = ctx.User.Id;
                message.Type = CommandEnum.EVENTS_CREATE;

                // Set the values
                var dateTime = DateTime.Now;
                int roundedMinutes = (dateTime.Minute < 15) ? 0 : (dateTime.Minute < 45) ? 30 : 0;
                dateTime = dateTime.AddMinutes(roundedMinutes - dateTime.Minute).AddHours(dateTime.Minute >= 45 ? 1 : 0);
                message.AddData(Placeholder.TIMEZONE, timeZone.ToString());
                message.AddData(Placeholder.DATE_START, dateTime.ToString("dd/MM/yyyy HH:mm"));
                message.AddData(Placeholder.DATE_END, dateTime.AddHours(1).ToString("dd/MM/yyyy HH:mm"));

                // Add the optional values
                if (image is not null) message.Embed!.Image = image.Url;
                if (thumbnail is not null) message.Embed!.Thumbnail = thumbnail.Url;
                if (pingrole is not null) message.AddRole(pingrole.Id);

                // Create the embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.EVENTS_SETUP, ctx.Interaction, message, channel.Id, hidden);

                //// Build the embed message with default values
                //var template = await DataService.GetTemplateAsync(ctx.Guild.Id, $"{EVENT}_{EVENT_POST_CREATE}");
                //var message = template!.Message;
                //message.ChannelId = channel.Id;
                //message.Sender = ctx.User.Id;
                //message.IsEphemeral = hidden;
                //message.Type = CommandEnum.EVENTS_CREATE;

                //// Set the values
                //message.Embed.Author = name.Replace(" Event", "").Replace("Event", "") + " Event";
                //message.ChannelId = channel.Id;
                //message.Sender = ctx.User.Id;

                //// Set the custom data
                //embed.AddCustomData(Identity.EVENT_NAME, name);
                //embed.AddCustomData(Identity.EVENT_TIMEZONE, timeZone.ToString());
                //embed.AddCustomData(Identity.EVENT_START, DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm"));
                //embed.AddCustomData(Identity.EVENT_END, DateTime.Now.AddDays(1).AddHours(1).ToString("dd/MM/yyyy HH:mm"));

                //// Add the optional values
                //if (image is not null) embed.Image = image.Url;
                //if (thumbnail is not null) embed.Thumbnail = thumbnail.Url;
                //if (pingrole is not null) embed.AddPingRole(pingrole.Id);

                //// Create the embed message
                //await DiscordUtil.CreateMessageAsync(CommandEnum.EVENTS_CREATE, ctx.Interaction, embed, channel.Id, hidden);

            } catch (Exception ex) {
                throw new CommandException($"Embed.UseEmbedCommand: {ex}");
            }
        }
    }
}


