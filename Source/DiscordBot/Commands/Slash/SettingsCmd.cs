using APP.Attributes;
using APP.Utils;
using BLL.Enums;
using BLL.Exceptions;
using BLL.Interfaces;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Model;
using System.Threading.Channels;

namespace APP.Commands.Slash {

    [SlashCommandGroup("settings", "Commands editing settings of your guild")]
    public class SettingsCmd : ApplicationCommandModule {

        #region Fields
        // Command
        private const string SETTINGS = "SETTINGS";
        // Command $1
        private const string SETTINGS_SET = "SET";
        private const string SETTINGS_REMOVE = "REMOVE";
        private const string SETTINGS_ENABLE = "ENABLE";
        private const string SETTINGS_DISABLE = "DISABLE";
        // Command Set $1
        private const string SETTINGS_WELCOME = "WELCOME";
        private const string SETTINGS_INTRODUCTION = "INTRODUCTION";
        private const string SETTINGS_LOG = "LOG";
        private const string SETTINGS_CHANGELOG = "CHANGELOG";
        private const string SETTINGS_INACTIVITY = "INACTIVITY";
        private const string SETTINGS_PUNISHMENT = "PUNISHMENT";
        private const string SETTINGS_STRIKE = "STRIKE";
        private const string SETTINGS_VERIFY = "VERIFY";
        #endregion


        [SlashCommandGroup(SETTINGS_SET, "Set your settings.")]
        public class SettingsSetCmd : ApplicationCommandModule {

            #region Properties
            public required IDataRepository DataService { private get; set; }
            public required DiscordUtil DiscordUtil { private get; set; }
            #endregion

            #region Command: /settings set welcome channel
            [SlashCommand(SETTINGS_WELCOME, "Set the welcome channel.")]
            [RequirePermission(CommandEnum.SETTINGS_SET_WELCOME)]
            public async Task SetWelcomeChannel(InteractionContext ctx, [Option("channel", "Set the channel.")] DiscordChannel channel) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_SET_WELCOME;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"Edited **`welcome channel`** to {channel.Mention}.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.WelcomeChannel = channel.Id;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, channel.Id, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_SET} {SETTINGS_WELCOME}", ex);
                }
            }
            #endregion

            #region Settings introduction channel
            [SlashCommand(SETTINGS_INTRODUCTION, "Set the introduction channel.")]
            [RequirePermission(CommandEnum.SETTINGS_SET_INTRODUCTION)]
            public async Task SetIntroductionChannel(InteractionContext ctx, [Option("channel", "Set the channel.")] DiscordChannel channel) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_SET_INTRODUCTION;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"Edited **`introduction channel`** to {channel.Mention}.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.IntroductionChannel = channel.Id;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, channel.Id, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_SET} {SETTINGS_INTRODUCTION}", ex);
                }
            }
            #endregion

            #region Settings log channel
            [SlashCommand(SETTINGS_LOG, "Set the log channel.")]
            [RequirePermission(CommandEnum.SETTINGS_SET_LOG)]
            public async Task SetLogChannel(InteractionContext ctx, [Option("channel", "Set the channel.")] DiscordChannel channel) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_SET_LOG;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"Edited **`log channel`** to {channel.Mention}.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.LogChannel = channel.Id;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, channel.Id, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_SET} {SETTINGS_LOG}", ex);
                }
            }
            #endregion

            #region Settings changelog channel
            [SlashCommand(SETTINGS_CHANGELOG, "Set the changelog channel.")]
            [RequirePermission(CommandEnum.SETTINGS_SET_CHANEGLOG)]
            public async Task SetChangelogChannel(InteractionContext ctx, [Option("channel", "Set the channel.")] DiscordChannel channel) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_SET_CHANEGLOG;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"Edited **`changelog channel`** to {channel.Mention}.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.ChangelogChannel = channel.Id;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, channel.Id, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_SET} {SETTINGS_CHANGELOG}", ex);
                }
            }
            #endregion

            #region Settings inactivity channel
            [SlashCommand(SETTINGS_INACTIVITY, "Set the inactivity channel.")]
            [RequirePermission(CommandEnum.SETTINGS_SET_INACTIVITY)]
            public async Task SetInactivityChannel(InteractionContext ctx, [Option("channel", "Set the channel.")] DiscordChannel channel) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_SET_INACTIVITY;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"Edited **`inactivity channel`** to {channel.Mention}.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.InactivityChannel = channel.Id;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, channel.Id, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_SET} {SETTINGS_INACTIVITY}", ex);
                }
            }
            #endregion

            #region Settings punishment channel
            [SlashCommand(SETTINGS_PUNISHMENT, "Set the punishment channel.")]
            [RequirePermission(CommandEnum.SETTINGS_SET_PUNISHMENT)]
            public async Task SetPunishmentChannel(InteractionContext ctx, [Option("channel", "Set the channel.")] DiscordChannel channel) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_SET_PUNISHMENT;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"Edited **`punishment channel`** to {channel.Mention}.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.PunishmentChannel = channel.Id;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, channel.Id, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_SET} {SETTINGS_PUNISHMENT}", ex);
                }
            }
            #endregion

            #region Settings strike channel
            [SlashCommand(SETTINGS_STRIKE, "Set the strike channel.")]
            [RequirePermission(CommandEnum.SETTINGS_SET_STRIKE)]
            public async Task SetStrikeChannel(InteractionContext ctx, [Option("channel", "Set the channel.")] DiscordChannel channel) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_SET_STRIKE;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"Edited **`strike channel`** to {channel.Mention}.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.StrikeChannel = channel.Id;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, channel.Id, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_SET} {SETTINGS_STRIKE}", ex);
                }
            }
            #endregion

            #region Settings verify channel
            [SlashCommand(SETTINGS_VERIFY, "Set the verify channel.")]
            [RequirePermission(CommandEnum.SETTINGS_SET_VERIFY)]
            public async Task SetVerifyChannel(InteractionContext ctx, [Option("channel", "Set the channel.")] DiscordChannel channel) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_SET_VERIFY;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"Edited **`verify channel`** to {channel.Mention}.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.VerifyChannel = channel.Id;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, channel.Id, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_SET} {SETTINGS_VERIFY}", ex);
                }
            }
            #endregion

        }

        [SlashCommandGroup(SETTINGS_REMOVE, "Set your settings.")]
        public class SettingsRemoveCmd : ApplicationCommandModule {

            #region Properties
            public required IDataRepository DataService { private get; set; }
            public required DiscordUtil DiscordUtil { private get; set; }
            #endregion

            #region Command: /settings remove welcome channel
            [SlashCommand(SETTINGS_WELCOME, "Remove the welcome channel.")]
            [RequirePermission(CommandEnum.SETTINGS_REMOVE_WELCOME)]
            public async Task RemoveWelcomeChannel(InteractionContext ctx) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = ctx.Channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_REMOVE_WELCOME;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"removed ####### from **`welcome channel`**.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.WelcomeChannel = null;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, message.ChannelId, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_REMOVE} {SETTINGS_WELCOME}", ex);
                }
            }
            #endregion

            #region Settings introduction channel
            [SlashCommand(SETTINGS_INTRODUCTION, "Remove the introduction channel.")]
            [RequirePermission(CommandEnum.SETTINGS_REMOVE_INTRODUCTION)]
            public async Task RemoveIntroductionChannel(InteractionContext ctx) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = ctx.Channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_REMOVE_INTRODUCTION;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"removed ####### from **`introduction channel`**.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.IntroductionChannel = null;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, message.ChannelId, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_REMOVE} {SETTINGS_INTRODUCTION}", ex);
                }
            }
            #endregion

            #region Settings log channel
            [SlashCommand(SETTINGS_LOG, "Remove the log channel.")]
            [RequirePermission(CommandEnum.SETTINGS_REMOVE_LOG)]
            public async Task RemoveLogChannel(InteractionContext ctx) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = ctx.Channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_REMOVE_LOG;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"removed ####### from **`log channel`**.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.LogChannel = null;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, message.ChannelId, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_REMOVE} {SETTINGS_LOG}", ex);
                }
            }
            #endregion

            #region Settings changelog channel
            [SlashCommand(SETTINGS_CHANGELOG, "Remove the changelog channel.")]
            [RequirePermission(CommandEnum.SETTINGS_REMOVE_CHANEGLOG)]
            public async Task RemoveChangelogChannel(InteractionContext ctx) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = ctx.Channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_REMOVE_CHANEGLOG;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"removed ####### from **`changelog channel`**.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.ChangelogChannel = null;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, message.ChannelId, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_REMOVE} {SETTINGS_CHANGELOG}", ex);
                }
            }
            #endregion

            #region Settings inactivity channel
            [SlashCommand(SETTINGS_INACTIVITY, "Remove the inactivity channel.")]
            [RequirePermission(CommandEnum.SETTINGS_REMOVE_INACTIVITY)]
            public async Task RemoveInactivityChannel(InteractionContext ctx) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = ctx.Channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_REMOVE_INACTIVITY;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"removed ####### from **`inactivity channel`**.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.InactivityChannel = null;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, message.ChannelId, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_REMOVE} {SETTINGS_INACTIVITY}", ex);
                }
            }
            #endregion

            #region Settings punishment channel
            [SlashCommand(SETTINGS_PUNISHMENT, "Remove the punishment channel.")]
            [RequirePermission(CommandEnum.SETTINGS_REMOVE_PUNISHMENT)]
            public async Task RemovePunishmentChannel(InteractionContext ctx) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = ctx.Channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_REMOVE_PUNISHMENT;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"removed ####### from **`punishment channel`**.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.PunishmentChannel = null;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, message.ChannelId, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_REMOVE} {SETTINGS_PUNISHMENT}", ex);
                }
            }
            #endregion

            #region Settings strike channel
            [SlashCommand(SETTINGS_STRIKE, "Remove the strike channel.")]
            [RequirePermission(CommandEnum.SETTINGS_REMOVE_STRIKE)]
            public async Task RemoveStrikeChannel(InteractionContext ctx) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = ctx.Channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_REMOVE_STRIKE;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"removed ####### from **`strike channel`**.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.StrikeChannel = null;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, message.ChannelId, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_REMOVE} {SETTINGS_STRIKE}", ex);
                }
            }
            #endregion

            #region Settings verify channel
            [SlashCommand(SETTINGS_VERIFY, "Remove the verify channel.")]
            [RequirePermission(CommandEnum.SETTINGS_REMOVE_VERIFY)]
            public async Task RemoveVerifyChannel(InteractionContext ctx) {
                try {

                    // Build the embed message with default values
                    var template = await DataService.GetTemplateAsync(ctx.Guild.Id, Identity.TEMPLATE_ACTION_SUCCESS);
                    var message = template!.Message;
                    message.ChannelId = ctx.Channel.Id;
                    message.Sender = ctx.User.Id;
                    message.Type = CommandEnum.SETTINGS_REMOVE_VERIFY;
                    message.AddData(Placeholder.TEXT1, $"executed command");
                    message.AddData(Placeholder.TEXT2, $"removed ####### from **`verify channel`**.");

                    // Save settings
                    var settings = await DataService.GetSettingsAsync(ctx.Guild.Id) ?? throw new CommandException($"Command was not found.");
                    settings.VerifyChannel = null;
                    await DataService.UpdateSettingsAsync(settings);

                    // Create the embed message
                    await DiscordUtil.CreateMessageAsync(message.Type, ctx.Interaction, message, message.ChannelId, true);

                } catch (Exception ex) {
                    throw new CommandException($"An error occured using the command: /{SETTINGS} {SETTINGS_REMOVE} {SETTINGS_VERIFY}", ex);
                }
            }
            #endregion
        }
    }
}
