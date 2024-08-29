using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus;
using XironiteDiscordBot.Manager;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using XironiteDiscordBot.Commands.Slash;
using DiscordBot.Listeners;
using DiscordBot.Model;
using Newtonsoft.Json;
using System.Diagnostics;
using DiscordBot.Utils;
using DiscordBot.Commands.Slash;
using DiscordBot.Model.Enums;
using System.Security;
using DSharpPlus.Entities;
using System.Runtime.CompilerServices;

namespace DiscordBot {
    internal class Program {

        private static Timer SaveDataInterval { get; set; }
        private static CommandsNextExtension Commands { get; set; }
        private static ConfigManager ConfigManager = new ConfigManager();
        private static DiscordClient Client { get; set; }

        static async Task Main(string[] args) {

            // (01) How long of an interval should we wait to save data
            SaveDataInterval = new Timer(SaveAllData!, null, 5000, 5000);

            // (02) Get the details of your config.json file
            var config = await ConfigManager.GetDiscordBotConfig();

            // (03) Setting up the bot configuration
            var discordConfig = new DiscordConfiguration() {
                Intents = DiscordIntents.All,
                Token = config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = config.HasAutoReconnect
            };

            // (04) Apply this config to our Discord Client
            Client = new DiscordClient(discordConfig);

            // (05) Set the default timeout for commands that use interactivity
            Client.UseInteractivity(new InteractivityConfiguration() {
                Timeout = TimeSpan.FromMinutes(2)
            });

            // (06) Set up the different handelers
            Client.Ready += OnStartup;
            Client.GuildMemberAdded += GuildMemberAddedHandler;
            Client.ModalSubmitted += ModalEventHandler;
            Client.MessageReactionAdded += MessageReactionHander;
            Client.ComponentInteractionCreated += ComponentInteractionHandler;

            // (07) Set up the Commands Configuration
            var commandsConfig = new CommandsNextConfiguration() {
                StringPrefixes = new String[] { config.Prefix },
                EnableMentionPrefix = config.HasEnableMentionPrefix,
                EnableDms = config.HasEnableDms,
                EnableDefaultHelp = config.HasEnableDefaultHelp
            };

            // (08) Initialize the CommandNextExtension Property
            Commands = Client.UseCommandsNext(commandsConfig);
            
            // (09) Register your Commands Classes
            var slashCommandConfiguration = Client.UseSlashCommands();
            slashCommandConfiguration.RegisterCommands<EmbedCmd>();
            slashCommandConfiguration.RegisterCommands<EventCmd>();
            slashCommandConfiguration.RegisterCommands<BroadcastCmd>();
            slashCommandConfiguration.RegisterCommands<PermissionCmd>();
            slashCommandConfiguration.RegisterCommands<TimestampCmd>();
            slashCommandConfiguration.RegisterCommands<NotionCmd>();


            // (10) Connect to get the Bot online
            await Client.ConnectAsync();
            await Task.Delay(-1);

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

            // Select components events
            if (e.Interaction.Data.ComponentType == ComponentType.StringSelect) {
                if (e.Id == "embedSelect") await new SelectComponentListener().HandleEmbedCommand(sender, e);
            }

            // Button components events
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains("embedButton")) await new ButtonComponentListener().HandleEmbedCommand(sender, e);
            }
        }

        private static void SaveAllData(object state) {
        }

        public static async Task MessageReactionHander(DiscordClient sender, MessageReactionAddEventArgs args) {
            throw new NotImplementedException();
        }

        private static Task OnStartup(DiscordClient sender, ReadyEventArgs args) {

            // Set activity
            var activity = new DiscordActivity("mc.xironite.com", ActivityType.Playing);
            Client.UpdateStatusAsync(activity, UserStatus.Online).ConfigureAwait(false);

            // Load Data
            Task.Run(() => CacheData.LoadDataAsync(sender));


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
