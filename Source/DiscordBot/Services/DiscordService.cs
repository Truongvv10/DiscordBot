using DSharpPlus.CommandsNext;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XironiteDiscordBot.Manager;
using DiscordBot.Commands.Slash;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using XironiteDiscordBot.Commands.Slash;
using DiscordBot.Listeners;
using DiscordBot.Utils;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBot.Services {
    public class DiscordService {

        private DiscordClient _client;
        private CommandsNextExtension _commands;
        private Timer _saveDataInterval;
        private readonly ConfigManager _configManager = new ConfigManager();

        public async Task InitializeAsync() {
            // Load config
            var config = await _configManager.GetDiscordBotConfig();

            // Set up bot configuration
            var discordConfig = new DiscordConfiguration {
                Intents = DiscordIntents.All,
                Token = config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = config.HasAutoReconnect
            };

            // Initialize Discord client
            _client = new DiscordClient(discordConfig);

            // Set up interactivity
            _client.UseInteractivity(new InteractivityConfiguration {
                Timeout = TimeSpan.FromMinutes(2)
            });

            // Register event handlers
            _client.Ready += OnStartup;
            _client.GuildMemberAdded += GuildMemberAddedHandler;
            _client.ModalSubmitted += ModalEventHandler;
            _client.MessageReactionAdded += MessageReactionHandler;
            _client.ComponentInteractionCreated += ComponentInteractionHandler;

            // Set up command configuration
            var commandsConfig = new CommandsNextConfiguration {
                StringPrefixes = new string[] { config.Prefix },
                EnableMentionPrefix = config.HasEnableMentionPrefix,
                EnableDms = config.HasEnableDms,
                EnableDefaultHelp = config.HasEnableDefaultHelp
            };

            // Initialize CommandsNext
            _commands = _client.UseCommandsNext(commandsConfig);

            // Register commands
            var slashCommandConfiguration = _client.UseSlashCommands();
            slashCommandConfiguration.RegisterCommands<EmbedCmd>();
            slashCommandConfiguration.RegisterCommands<EventCmd>();
            slashCommandConfiguration.RegisterCommands<ChangelogCmd>();
            slashCommandConfiguration.RegisterCommands<BroadcastCmd>();
            slashCommandConfiguration.RegisterCommands<PermissionCmd>();
            slashCommandConfiguration.RegisterCommands<TimestampCmd>();
            slashCommandConfiguration.RegisterCommands<NotionCmd>();

            // Start the bot
            await _client.ConnectAsync();
        }

        private static async Task ModalEventHandler(DiscordClient sender, ModalSubmitEventArgs e) {
            // Modal components events
            if (e.Interaction.Data.CustomId.Contains("embedModal") && e.Interaction.Type == InteractionType.ModalSubmit) {
                await new ModalSubmitListener().HandleEmbedCommand(sender, e);
            }
        }

        private static async Task GuildMemberAddedHandler(DiscordClient sender, GuildMemberAddEventArgs args) {
            throw new NotImplementedException();
        }

        private static async Task ComponentInteractionHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e) {

            // Select components
            if (e.Interaction.Data.ComponentType == ComponentType.StringSelect) {
                if (e.Id == Identity.COMPONENT_SELECT) await new ComponentSelectListener().HandleEmbedInteraction(sender, e);
                if (e.Id == Identity.COMPONENT_TEMPLATE) await new ComponentSelectListener().HandleTemplateInteraction(sender, e);
                if (e.Id == Identity.COMPONENT_EVENT) await new ComponentSelectListener().HandleEventInteraction(sender, e);
            }

            // Button components
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains("embedButton")) await new ComponentButtonListener().HandleEmbedCommand(sender, e);
            }
        }

        public void StartDataSavingInterval() {
            _saveDataInterval = new Timer(SaveAllData, null, 5000, 5000);
        }

        private void SaveAllData(object state) {
            // Save data logic
        }

        public static async Task MessageReactionHandler(DiscordClient sender, MessageReactionAddEventArgs args) {
            throw new NotImplementedException();
        }

        private static Task OnStartup(DiscordClient sender, ReadyEventArgs args) {

            // Set activity
            var activity = new DiscordActivity("mc.xironite.com", ActivityType.Playing);
            sender.UpdateStatusAsync(activity, UserStatus.Online).ConfigureAwait(false);

            // Load Data
            Task.Run(() => CacheData.LoadAllData(sender));


            // Using Big Money-ne ASCII art
            Console.WriteLine(@"    ################################################");
            Console.WriteLine(@"    # __  _____ ____   ___  _   _ ___ _____ _____  #");
            Console.WriteLine(@"    # \ \/ |_ _|  _ \ / _ \| \ | |_ _|_   _| ____| #");
            Console.WriteLine(@"    #  \  / | || |_) | | | |  \| || |  | | |  _|   #");
            Console.WriteLine(@"    #  /  \ | ||  _ <| |_| | |\  || |  | | | |___  #");
            Console.WriteLine(@"    # /_/\_|___|_| \_\\___/|_| \_|___| |_| |_____| #");
            Console.WriteLine(@"    #                                              #");
            Console.WriteLine(@"    ################################################");
            Console.WriteLine();

            // Show config settings
            Console.WriteLine($"{Utils.AnsiColor.RESET}[{DateTime.Now}] {Utils.AnsiColor.CYAN}Bot \"{sender.CurrentUser.Username}\" has succesfully started up");
            Console.WriteLine($"{Utils.AnsiColor.RESET}[{DateTime.Now}] {Utils.AnsiColor.CYAN}Bot used on ({sender.Guilds.Count}) different discord servers");
            Console.ForegroundColor = ConsoleColor.Gray;
            return Task.CompletedTask;
        }

    }
}
