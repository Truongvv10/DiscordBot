using BLL.Enums;
using BLL.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using APP.Enums;
using APP.Utils;
using APP.Services;
using BLL.Model;
using BLL.Services;
using BLL.Interfaces;
using APP.Attributes;

namespace APP.Commands.Slash {
    public class GeneralCommands : ApplicationCommandModule {

        #region Fields
        private const string TIMESTAMP = "TIMESTAMP";
        private const string NITRO = "NITRO";
        private const string TEMPLATES = "TEMPLATES";
        #endregion

        #region Properties
        public required IDataRepository DataService { private get; set; }
        public required DiscordUtil DiscordUtil { private get; set; }
        #endregion

        [SlashCommand(TIMESTAMP, "Generate dynamic discord timestamp")]
        [RequirePermission(CommandEnum.NONE)]
        public async Task Timestamp(InteractionContext ctx,
            [Option("time-zone", "The time zone you live in.")] TimeZoneEnum timeZone) {
            try {
                // Build the embed message with default values
                var modal = new DiscordInteractionResponseBuilder();
                modal.WithTitle($"TIMESTAMP").WithCustomId(Identity.MODAL_TIMESTAMP)
                    .AddComponents(new TextInputComponent("TIMEZONE", Identity.MODAL_DATA_TIMESTAMPS_TIMEZONE, "Europe/Brussels", timeZone.ToString(), true, TextInputStyle.Short))
                    .AddComponents(new TextInputComponent("TIMEZONE", Identity.MODAL_DATA_TIMESTAMPS_TIME, "DD/MM/YYYY hh:mm", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), true, TextInputStyle.Short, 16, 16));

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
                var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_NITRO);
                var message = template!.Message;
                var date = DateTime.Now.AddMinutes(expire);
                message.SetData(Placeholder.DATE_END, date.ToString("dd/MM/yyyy HH:mm"));

                // Create embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.NITRO, ctx.Interaction, message, ctx.Channel.Id);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{NITRO}", ex);
            }
        }

        [SlashCommand(TEMPLATES, "View all available templates")]
        [RequirePermission(CommandEnum.TEMPLATES, [Permissions.ManageChannels, Permissions.ManageMessages])]
        public async Task Templates(InteractionContext ctx) {
            try {
                // Build the embed message with default values
                var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_TEMPLATES);
                var message = template!.Message;

                // Create embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.TEMPLATES, ctx.Interaction, message, ctx.Channel.Id, true);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{TEMPLATES}", ex);
            }
        }
    }
}
