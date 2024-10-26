using DiscordBot.Exceptions;
using DiscordBot.Model.Enums;
using DiscordBot.Model;
using DiscordBot.Utils;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XironiteDiscordBot.Commands;
using DiscordBot.Services;
using DSharpPlus.SlashCommands.Attributes;

namespace DiscordBot.Commands.Slash {
    public class BroadcastCmd : SlashCommand {


        [SlashCommand("broadcast", "Send an embeded message to the current channel")]
        [RequirePermission(CommandEnum.BROADCAST)]
        public async Task UseBroadcastCommand(InteractionContext ctx,
            [Option("channel", "The channel where your embeded message will be sent to.")] DiscordChannel channel,
            [Option("image", "The main image of your embeded message that will be added.")] DiscordAttachment? image = null,
            [Option("thumbnail", "The thumbnail of your embeded message that will be added.")] DiscordAttachment? thumbnail = null,
            [Option("ping", "The role that will get pinged on sending message.")] DiscordRole? pingrole = null) {

            LogCommand(ctx, CommandEnum.BROADCAST);

            try {
                EmbedBuilder embed = new EmbedBuilder() {
                    Author = ctx.User.Username,
                    AuthorLink = ctx.User.AvatarUrl,
                    AuthorUrl = ctx.User.AvatarUrl,
                    FooterUrl = ctx.Client.CurrentUser.AvatarUrl,
                    ChannelId = channel.Id,
                    Description = "Sample description.",
                    HasTimeStamp = true,
                };
                if (image is not null) embed.Image = image.Url;
                if (thumbnail is not null) embed.Thumbnail = thumbnail.Url;
                if (pingrole is not null) embed.AddPingRole(pingrole.Id);
                await CreateEmbedMessageAsync(ctx, embed, EmbedType.BROADCAST, channel.Id, true);
            } catch (Exception ex) {
                throw new CommandException($"Embed.UseEmbedCommand: {ex}");
            }
        }

    }
}
