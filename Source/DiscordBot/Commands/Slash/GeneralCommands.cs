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
        private const string NITRO = "NITRO";

        [SlashCommand(TIMESTAMP, "Generate dynamic discord timestamp")]
        [RequirePermission(CommandEnum.TIMESTAMP)]
        public async Task Timestamp(InteractionContext ctx,
            [Option("time-zone", "The time zone you live in.")] TimeZoneEnum timeZone) {
            try {
                // Build the embed message with default values
                var modal = new DiscordInteractionResponseBuilder();
                modal.WithTitle($"TIMESTAMP").WithCustomId(Identity.MODAL_TIMESTAMP)
                    .AddComponents(new TextInputComponent("TIMEZONE", Identity.MODAL_COMP_TIMESTAMP_TIMEZONE, "Europe/Brussels", timeZone.ToString(), true, TextInputStyle.Short))
                    .AddComponents(new TextInputComponent("TIMEZONE", Identity.MODAL_COMP_TIMESTAMP_TIME, "DD/MM/YYYY hh:mm", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), true, TextInputStyle.Short, 16, 16));

                // Create response model
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                
            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{TIMESTAMP}", ex);
            }
        }

        [SlashCommand(NITRO, "Create nitro giveaway")]
        [RequirePermission(CommandEnum.NITRO)]
        public async Task Nitro(InteractionContext ctx,
            [Option("expire", "The expire time (in minutes) of this nitro.")] double expire) {
            try {
                // Build the embed message with default values
                var embed = await CacheData.GetTemplate(ctx.Guild.Id, Identity.TDATA_NITRO);
                var time = await DiscordUtil.TranslateToDynamicTimestamp(DateTime.Now.AddMinutes(expire), "CET", TimestampEnum.RELATIVE);
                embed.Description = embed.Description!.Replace($"{{{Identity.PLACEHOLDER_TIME_EXPIRE}}}", time);

                // Create embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.NITRO, ctx.Interaction, embed, ctx.Channel.Id);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{NITRO}", ex);
            }
        }
    }
}
