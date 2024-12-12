using DSharpPlus.CommandsNext;
using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using BLL.Exceptions;
using APP.Commands.Slash;
using APP.Events;
using BLL.Model;
using Microsoft.Extensions.DependencyInjection;
using APP.Utils;
using BLL.Interfaces;
using BLL.Enums;
using DLLSQLServer.Repositories;
using BLL.Services;

namespace APP.Services {
    public class DiscordService {

        private IDataService dataService;
        private Config config;
        private DiscordClient client;
        private SlashCommandsExtension commands;
        private CacheData cache;
        private DiscordUtil discordUtil;

        public DiscordService(Config config, CacheData cache) {

            // Initialize fields
            if (config.DatabaseType == DatabaseSaveType.SqlServer && config.ConnectionString != null) {
                dataService = new SqlServerRepository(cache, config.ConnectionString);
            } else {
                throw new ServiceException($"Connection can not be established");
            }

            this.cache = cache;
            this.config = config;
            discordUtil = new DiscordUtil(dataService);

            // Set up services
            var services = new ServiceCollection()
                .AddSingleton(dataService)
                .AddSingleton(discordUtil)
                .BuildServiceProvider();

            // Set up bot configuration
            var discordConfig = new DiscordConfiguration {
                Intents = DiscordIntents.All,
                Token = config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = config.HasAutoReconnect,
                LogUnknownEvents = config.LogUnknownEvents};

            // Initialize Discord client
            client = new DiscordClient(discordConfig);

            // Initialize slash commands
            commands = client.UseSlashCommands(new SlashCommandsConfiguration() { Services = services });
        }

        public async Task InitializeAsync() {
            try {

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
                CommandsNextExtension commandsNext = client.UseCommandsNext(commandsConfig);

                // Register commands
                commands.RegisterCommands<GeneralCommands>();
                commands.RegisterCommands<EmbedCmd>();
                commands.RegisterCommands<EventCmd>();

                // Register events
                RegisterEvents();

                // Start the bot
                await client.ConnectAsync();

            } catch (Exception ex) {
                throw new ServiceException($"There has been an error starting up the bot.", ex);
            }
        }

        private void RegisterEvents() {
            // Register event handlers
            var discordClientReadyEvent = new DiscordClientReadyEvent(client, dataService);
            client.Ready += discordClientReadyEvent.ClientReady;

            // Register guild member add event handler
            var guildMemberAddHandler = new GuildMemberAddEvent(dataService);
            client.GuildMemberAdded += guildMemberAddHandler.GuildMemberAdd;

            // Register reaction event handler
            var reactionHandel = new ReactionAddEvent(dataService);
            client.MessageReactionAdded += reactionHandel.ReactionAdd;

            // Register component interaction event button
            var buttonHandler = new ButtonClickEvent(dataService, discordUtil);
            client.ComponentInteractionCreated += buttonHandler.ButtonClick;

            // Register component interaction event selection
            var selectHandler = new SelectComponentEvent(dataService, discordUtil);
            client.ComponentInteractionCreated += selectHandler.ComponentSelect;

            // Register modal event handler
            var modalHandler = new ModalSubmitEvent(dataService, discordUtil);
            client.ModalSubmitted += modalHandler.ModalSubmit;

            // Subscribe to the SlashCommandErrored event
            var slashCommandUseEvent = new SlashCommandUseEvent(dataService, discordUtil);
            commands.SlashCommandErrored += slashCommandUseEvent.OnSlashCommandErrored;
        }
    }
}
