using DSharpPlus.CommandsNext;
using DSharpPlus;
using DiscordBot.Manager;
using DiscordBot.Commands.Slash;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using DiscordBot.Listeners;
using DiscordBot.Utils;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands.EventArgs;
using DiscordBot.Exceptions;
using AnsiColor = DiscordBot.Utils.AnsiColor;
using DiscordBot.Events;
using System.Formats.Asn1;

namespace DiscordBot.Services {
    public class DiscordService {

        private readonly ConfigManager configManager = new ConfigManager();
        private DiscordClient client;
        private CommandsNextExtension commandsNext;
        private Timer saveDataInterval;

        public async Task InitializeAsync() {
            try {
                // Load config
                var config = await configManager.GetDiscordBotConfig();

                // Set up bot configuration
                var discordConfig = new DiscordConfiguration {
                    Intents = DiscordIntents.All,
                    Token = config.Token,
                    TokenType = TokenType.Bot,
                    AutoReconnect = config.HasAutoReconnect,
                    LogUnknownEvents = config.LogUnknownEvents,
                };

                // Initialize Discord client
                client = new DiscordClient(discordConfig);

                // Set up interactivity
                client.UseInteractivity(new InteractivityConfiguration {
                    Timeout = TimeSpan.FromMinutes(2)
                });

                // Set up command configuration
                var commandsConfig = new CommandsNextConfiguration {
                    StringPrefixes = new string[] { config.Prefix },
                    EnableMentionPrefix = config.HasEnableMentionPrefix,
                    EnableDms = config.HasEnableDms,
                    EnableDefaultHelp = config.HasEnableDefaultHelp
                };

                // Initialize CommandsNext
                commandsNext = client.UseCommandsNext(commandsConfig);

                // Register commands
                var slashCommandConfiguration = client.UseSlashCommands();
                slashCommandConfiguration.RegisterCommands<GeneralCommands>();
                slashCommandConfiguration.RegisterCommands<EmbedCmd>();
                slashCommandConfiguration.RegisterCommands<EventCmd>();

                // Register event handlers
                var discordClientReadyEvent = new DiscordClientReadyEvent();
                client.Ready += discordClientReadyEvent.ClientReady;

                // Register guild member add event handler
                var guildMemberAddHandler = new GuildMemberAddEvent();
                client.GuildMemberAdded += guildMemberAddHandler.GuildMemberAdd;

                // Register reaction event handler
                var reactionHandel = new ReactionAddEvent();
                client.MessageReactionAdded += reactionHandel.ReactionAdd;

                // Register component interaction event
                var buttonHandler = new ButtonClickEvent();
                var selectHandler = new SelectComponentEvent();
                client.ComponentInteractionCreated += buttonHandler.ButtonClick;
                client.ComponentInteractionCreated += selectHandler.ComponentSelect;

                // Register modal event handler
                var modalHandler = new ModalSubmitEvent();
                client.ModalSubmitted += modalHandler.ModalSubmit;

                // Subscribe to the SlashCommandErrored event
                var slashCommandUseEvent = new SlashCommandUseEvent();
                slashCommandConfiguration.SlashCommandErrored += slashCommandUseEvent.OnSlashCommandErrored;

                // Start the bot
                await client.ConnectAsync();

            } catch (Exception ex) {
                throw new ServiceException($"There has been an error starting up the bot.", ex);
            }

        }

        public void StartDataSavingInterval() {
            saveDataInterval = new Timer(SaveAllData, null, 5000, 5000);
        }

        private void SaveAllData(object state) {
            // Save data logic
        }
    }
}
