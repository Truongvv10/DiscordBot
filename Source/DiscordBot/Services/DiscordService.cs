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

namespace DiscordBot.Services {
    public class DiscordService {

        private readonly ConfigManager _configManager = new ConfigManager();
        private DiscordClient _client;
        private CommandsNextExtension _commands;
        private Timer _saveDataInterval;

        public async Task InitializeAsync() {
            try {
                // Load config
                var config = await _configManager.GetDiscordBotConfig();

                // Set up bot configuration
                var discordConfig = new DiscordConfiguration {
                    Intents = DiscordIntents.All,
                    Token = config.Token,
                    TokenType = TokenType.Bot,
                    AutoReconnect = config.HasAutoReconnect,
                    LogUnknownEvents = config.LogUnknownEvents,
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

                // Subscribe to the SlashCommandErrored event
                slashCommandConfiguration.SlashCommandErrored += OnSlashCommandErrored;

                // Start the bot
                await _client.ConnectAsync();
            } catch (Exception ex) {
                throw new ServiceException($"There has been an error starting up the bot.", ex);
            }

        }

        private async Task OnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e) {
            if (e.Exception is SlashExecutionChecksFailedException slex) {

                // Loop through all failed checks
                foreach (var check in slex.FailedChecks) {

                    // Check if the failed check is a RequirePermissionAttribute
                    if (check is RequirePermissionAttribute att) {
                        var embed = new DiscordEmbedBuilder()
                            .WithAuthor($"You don't have permission for /{att.Command}", null, "https://cdn-icons-png.flaticon.com/512/2581/2581801.png")
                            .WithColor(new DiscordColor("#d82b40"));
                        await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral(true));
                    }

                }

            } else {

                // Log the error
                Console.WriteLine($"Error executing command {e.Context}: {e.Exception}");

                // Optionally notify the user in the channel
                if (e.Context.Channel is DiscordChannel channel) {
                    await channel.SendMessageAsync(e.Exception.Message);
                }

            }
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

        private static async Task OnStartup(DiscordClient sender, ReadyEventArgs args) {
            try {

                // Set activity
                var activity = new DiscordActivity("mc.xironite.com", ActivityType.Playing);
                await sender.UpdateStatusAsync(activity, UserStatus.Online).ConfigureAwait(false);

                // Using ASCII art to display in console
                using StreamReader sr = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Saves", "Logo.txt"));
                Console.WriteLine(sr.ReadToEnd());

                // Load Data
                await CacheData.LoadAllData(sender);

                // Show config settings
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Bot used on ({sender.Guilds.Count}) different discord servers");
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Bot \"{sender.CurrentUser.Username}\" has succesfully started up");
                Console.ForegroundColor = ConsoleColor.Gray;

            } catch (UtilException ex) {
                throw new ServiceException("There has been an error starting up the bot due to a utility error.", ex);
            } catch (Exception ex) {
                throw new ServiceException("There has been an error starting up the bot.", ex);
            }
        }

    }
}
