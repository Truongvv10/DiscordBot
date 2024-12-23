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
using DSharpPlus.SlashCommands.Attributes;
using System.Threading.Channels;
using System;
using System.Text.RegularExpressions;
using APP.Choices;
using Nager.Country;

namespace APP.Commands.Slash {
    public class GeneralCommands : ApplicationCommandModule {

        #region Fields
        private const string SEND = "SEND";
        private const string TIMESTAMP = "TIMESTAMP";
        private const string NITRO = "NITRO";
        private const string TEMPLATES = "TEMPLATES";
        private const string INTRODUCTION = "INTRODUCTION";
        #endregion

        #region Properties
        public required IDataRepository DataService { private get; set; }
        public required DiscordUtil DiscordUtil { private get; set; }
        #endregion

        [SlashCommand(SEND, "Send a plain message")]
        [RequirePermission(CommandEnum.MESSAGE)]
        public async Task Timestamp(InteractionContext ctx,
            [Option("message", "The time zone you live in.")] string text,
            [Option("image", "The main image of your embeded message that will be added.")] DiscordAttachment? image = null) {
            try {
                // Create response model
                await ctx.DeferAsync(true);
                await ctx.DeleteResponseAsync();

                // Build the embed message with default values
                var message = new Message(text);

                // Send the message
                await DiscordUtil.CreateMessageToChannelAsync(CommandEnum.MESSAGE, ctx.Interaction, message, ctx.Channel);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{SEND}", ex);
            }
        }

        [SlashCommand(TIMESTAMP, "Generate dynamic discord timestamp")]
        [RequirePermission(CommandEnum.NONE)]
        public async Task Timestamp(InteractionContext ctx,
            [Option("time-zone", "The time zone you live in.")] TimeZoneEnum timeZone) {
            try {
                // Build the embed message with default values
                var modal = new DiscordInteractionResponseBuilder();
                modal.WithTitle($"TIMESTAMP").WithCustomId(Identity.MODAL_TIMESTAMP)
                    .AddComponents(new TextInputComponent("TIMEZONE", Identity.MODAL_DATA_TIMESTAMPS_TIMEZONE, "Europe/Brussels", timeZone.ToString(), true, TextInputStyle.Short))
                    .AddComponents(new TextInputComponent("DATE AND TIME", Identity.MODAL_DATA_TIMESTAMPS_TIME, "DD/MM/YYYY hh:mm", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), true, TextInputStyle.Short, 16, 16));

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
                var template = await DataService.GetTemplateAsync(ctx.Guild.Id, TemplateMessage.NITRO);
                var message = template!.Message;
                var date = DateTime.Now.AddMinutes(expire);
                message.SetData(Placeholder.DATE_END, date.ToString("dd/MM/yyyy HH:mm"));

                // Create embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.NITRO, ctx.Interaction, message);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{NITRO}", ex);
            }
        }

        [SlashCommand(TEMPLATES, "View all available templates")]
        [RequirePermission(CommandEnum.TEMPLATES, [Permissions.ManageChannels, Permissions.ManageMessages])]
        public async Task Templates(InteractionContext ctx) {
            try {
                // Build the embed message with default values
                var template = await DataService.GetTemplateAsync(ctx.Guild.Id, TemplateMessage.TEMPLATES);
                var message = template!.Message;

                // Create embed message
                await DiscordUtil.CreateMessageAsync(CommandEnum.TEMPLATES, ctx.Interaction, message, true);

            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{TEMPLATES}", ex);
            }
        }

        [SlashCommand(INTRODUCTION, "Introduce yourself to the server.")]
        [SlashCooldown(9999, 60, SlashCooldownBucketType.Guild)]
        public async Task Introduction(InteractionContext ctx,
            [Autocomplete(typeof(CountryChoiceProvider))][Option("country", "Which country are you from?")] string country,
            [Option("time-zone", "The time zone you live in.")] TimeZoneEnum timeZone,
            [Option("birthday", "Birthday in the format days/month/year.")] string birthday,
            [Option("pronouns", "The pronouns that others should call you with.")] PronounsEnum pronouns,
            [Option("color", "Your favorite colors in hex code. (example: #ffffff)")] string? color = "#0681cd") {
            try {
                // Check if parameters are valid
                if (string.IsNullOrWhiteSpace(country) || !CountryUtil.IsValidCountry(country)) {
                    await DiscordUtil.SendActionMessage(ctx.Interaction, TemplateMessage.ACTION_INVALID, $"Invalid country \"{country}\" was given.");
                    return;
                }
                if (!DateTime.TryParseExact($"{birthday} 00:00", "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out var parsedBirthdayDate)) {
                    await DiscordUtil.SendActionMessage(ctx.Interaction, TemplateMessage.ACTION_INVALID, $"Invalid format with date \"{birthday}\".");
                    return;
                }
                if (!Regex.IsMatch(color!, @"#[a-fA-F0-9]{6}")) {
                    await DiscordUtil.SendActionMessage(ctx.Interaction, TemplateMessage.ACTION_INVALID, $"Invalid color \"{color}\" was given.");
                    return;
                }

                // Check if introduction channel is setup
                var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Settings was not found.");
                var channelId = settings.IntroductionChannel ?? throw new CommandException($"Introduction was not setup yet.");
                var channel = await DiscordUtil.GetChannelByIdAsync(ctx.Guild, channelId) ?? throw new CommandException($"Introduction was not setup yet.");
                var id = $"{Identity.MODAL_INTRODUCTION};{channelId};{CountryUtil.GetCountryCode(country)};{(int)timeZone};{parsedBirthdayDate.ToString("dd/MM/yyyy")};{(int)pronouns};{color}";

                // Create embed message
                var modal = new DiscordInteractionResponseBuilder();
                modal.WithTitle($"INTRODUCTION").WithCustomId(id)
                    .AddComponents(new TextInputComponent(
                        "INTRODUCTION",
                        Identity.MODAL_DATA_INTRODUCTION_TEXT,
                        $"Write about yourself...",
                        $"Hi, my name is {ctx.User.Username}. I'm {DateTime.Now.Year - parsedBirthdayDate.Year} years old and live in {country}.",
                        true, TextInputStyle.Paragraph, 32, 2048));

                // Create response model
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);

            } catch (CommandException ex) {
                await DiscordUtil.SendActionMessage(ctx.Interaction, TemplateMessage.ACTION_FAILED, ex.Message);
            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{INTRODUCTION}", ex);
            }
        }
    }
}
