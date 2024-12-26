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
        private const string INACTIVITY = "INACTIVITY";
        #endregion

        #region Properties
        public required IDataRepository DataService { private get; set; }
        public required DiscordUtil DiscordUtil { private get; set; }
        #endregion

        #region Command: Send
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
        #endregion

        #region Command: Timestamp
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
        #endregion

        #region Command: Nitro
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
        #endregion

        #region Command: Templates
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
        #endregion

        #region Command: Introduction
        [SlashCommand(INTRODUCTION, "Introduce yourself to the server.")]
        [SlashCooldown(1, 60, SlashCooldownBucketType.Guild)]
        public async Task Introduction(InteractionContext ctx,
            [Autocomplete(typeof(CountryChoiceProvider))][Option("country", "Which country are you from?")] string country,
            [Option("time-zone", "The time zone you live in.")] TimeZoneEnum timeZone,
            [Option("birthday", "Birthday in the format days/month/year.")] string birthday,
            [Option("pronouns", "The pronouns that others should call you with.")] PronounsEnum pronouns,
            [Option("text", "An introduction about yourself. (not required)")] string? text = null,
            [Option("color", "Your favorite colors in hex code. (example: #ffffff)")] string? color = "#0681cd") {
            try {
                // Check if parameters are valid
                if (string.IsNullOrWhiteSpace(country) || !CountryUtil.IsValidCountry(country)) {
                    await DiscordUtil.SendActionMessage(ctx.Interaction, TemplateMessage.ACTION_INVALID, $"Invalid country \"{country}\" was given.", $"Please use the tabbed countries.");
                    return;
                }
                if (!DateTime.TryParseExact($"{birthday} 00:00", "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out var parsedBirthdayDate)) {
                    await DiscordUtil.SendActionMessage(ctx.Interaction, TemplateMessage.ACTION_INVALID, $"Invalid format with date \"{birthday}\".", "Please use the format \"day/month/year\".");
                    return;
                }
                if (!Regex.IsMatch(color!, @"#[a-fA-F0-9]{6}")) {
                    await DiscordUtil.SendActionMessage(ctx.Interaction, TemplateMessage.ACTION_INVALID, $"Invalid color \"{color}\" was given.", "Please use the format \"#hexcolor\".");
                    return;
                }
                if (string.IsNullOrWhiteSpace(text)) {
                    text = $"Hi, my name is {ctx.User.Username}. I'm {DateTime.Now.Year - parsedBirthdayDate.Year} years old and live in {country}.";
                }

                // Check if introduction channel is setup
                var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Settings was not found.");
                var channelId = settings.IntroductionChannel ?? throw new CommandException($"Introduction was not setup yet.");
                var channel = await DiscordUtil.GetChannelByIdAsync(ctx.Guild, channelId) ?? throw new CommandException($"Introduction was not setup yet.");

                // Create embed message
                var modal = new DiscordInteractionResponseBuilder();
                modal.WithTitle($"INTRODUCTION")
                    .WithCustomId(Identity.MODAL_INTRODUCTION)
                    .AddComponents(new TextInputComponent("INTRODUCTION", Identity.MODAL_DATA_INTRODUCTION_TEXT, "Write about yourself...", text, true, TextInputStyle.Paragraph, 0, 2048));

                // Create response model
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);

                // Saving data to cache
                var template = await DataService.GetTemplateAsync(ctx.Interaction.Guild.Id, TemplateMessage.INTRODUCTION);
                var message = template!.Message;
                var embed = message.Embed;

                message.AddData($"{Placeholder.CUSTOM}.introduction.country", country);
                message.AddData($"{Placeholder.CUSTOM}.introduction.birthday", birthday);
                message.AddData($"{Placeholder.CUSTOM}.introduction.pronouns", pronouns.GetEnumChoiceName());
                message.AddData($"{Placeholder.CUSTOM}.introduction.text", text);
                message.AddData(Identity.INTERNAL_SEND_CHANNEL, channel.Id.ToString());

                embed.AddField("Country", $"{{{Placeholder.CUSTOM}.introduction.country}}");
                embed.AddField("Birthday", $"{{{Placeholder.CUSTOM}.introduction.birthday}}");
                embed.AddField("Pronouns", $"{{{Placeholder.CUSTOM}.introduction.pronouns}}");
                embed.AddField("Introduction", $"{{{Placeholder.CUSTOM}.introduction.text}}", false);
                embed.WithAuthor(ctx.Interaction.User.Username, ctx.Interaction.User.AvatarUrl);
                embed.WithColor(color!);
                DataService.AddCacheModalData(ctx.Guild.Id, ctx.User.Id, message);

            } catch (CommandException ex) {
                await DiscordUtil.SendActionMessage(ctx.Interaction, TemplateMessage.ACTION_FAILED, ex.Message);
            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{INTRODUCTION}", ex);
            }
        }
        #endregion

        #region Command: Inactivity
        [SlashCommand(INACTIVITY, "Give an inactivity notice.")]
        public async Task Inactivity(InteractionContext ctx) {
            try {
                // Check if introduction channel is setup
                var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Settings was not found.");
                var channelId = settings.InactivityChannel ?? throw new CommandException($"Inactivity channel was not setup yet.");
                var channel = await DiscordUtil.GetChannelByIdAsync(ctx.Guild, channelId) ?? throw new CommandException($"Inactivity channel was not setup yet.");

                // Create modal
                var date = DateTimeUtil.RoundDateTime(DateTime.Now);
                var modal = new DiscordInteractionResponseBuilder();
                modal.WithTitle($"{INACTIVITY} NOTICE")
                    .WithCustomId(Identity.MODAL_INACTIVITY)
                    .AddComponents(new TextInputComponent("START DATE", Identity.MODAL_DATA_INACTIVITY_START, "day/month/yea.", date.ToString("dd/MM/yyyy"), true))
                    .AddComponents(new TextInputComponent("END DATE", Identity.MODAL_DATA_INACTIVITY_END, "day/month/year", date.ToString("dd/MM/yyyy"), true))
                    .AddComponents(new TextInputComponent("REASON", Identity.MODAL_DATA_INACTIVITY_REASON, "Reason of inactivity...", null, true, TextInputStyle.Paragraph, 16, 2048));

                // Create response model
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);

                // Saving modal data to cache
                var template = await DataService.GetTemplateAsync(ctx.Interaction.Guild.Id, TemplateMessage.INACTIVITY);
                var message = template!.Message;
                var embed = message.Embed;

                embed.ReplaceFieldAt(0, "User", $"```{ctx.Interaction.User.Username}```");
                embed.WithFooter(ctx.User.Username, ctx.User.AvatarUrl);
                message.Type = CommandEnum.INACTIVITY;
                message.AddData(Identity.INTERNAL_SEND_CHANNEL, channel.Id.ToString());

                DataService.AddCacheModalData(ctx.Guild.Id, ctx.User.Id, message);
            } catch (CommandException ex) {
                await DiscordUtil.SendActionMessage(ctx.Interaction, TemplateMessage.ACTION_FAILED, ex.Message);
            } catch (Exception ex) {
                throw new CommandException($"An error occured using the command: /{INTRODUCTION}", ex);
            }
        }
        #endregion
    }
}
