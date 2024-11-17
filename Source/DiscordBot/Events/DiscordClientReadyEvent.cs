using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using AnsiColor = APP.Utils.AnsiColor;
using BLL.Exceptions;
using APP.Utils;
using BLL.Interfaces;
using BLL.Services;
using System.Diagnostics;

namespace APP.Events {
    public class DiscordClientReadyEvent {

        #region Fields
        private readonly DiscordClient client;
        private readonly IDataService dataService;
        #endregion

        #region Constructors
        public DiscordClientReadyEvent(DiscordClient client, IDataService dataService) {
            this.client = client;
            this.dataService = dataService;
        }
        #endregion

        public async Task ClientReady(DiscordClient sender, ReadyEventArgs args) {
            try {

                // Set activity
                var activity = new DiscordActivity("mc.xironite.com", ActivityType.Playing);
                await sender.UpdateStatusAsync(activity, UserStatus.Online).ConfigureAwait(false);

                // Using ASCII art to display in console
                using StreamReader sr = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Saves", "Logo.txt"));
                Console.WriteLine(sr.ReadToEnd());

                // Load Data
                await LoadAllData();

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

        public async Task LoadAllData() {
            try {
                // Load all guilds
                var stopwatch = Stopwatch.StartNew();
                await dataService.LoadGuildsAsync(client.Guilds.Select(g => g.Key));
                stopwatch.Stop();
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Successfully loaded default templates {AnsiColor.YELLOW}({stopwatch.ElapsedMilliseconds}ms){AnsiColor.RESET}");

                // Load all templates
                stopwatch = Stopwatch.StartNew();
                await dataService.LoadTemplatesAsync();
                stopwatch.Stop();
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Successfully loaded default templates {AnsiColor.YELLOW}({stopwatch.ElapsedMilliseconds}ms){AnsiColor.RESET}");

                // Load all time zones
                stopwatch.Restart();
                await dataService.LoadTimeZonesAsync();
                stopwatch.Stop();
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Successfully loaded all time zones {AnsiColor.YELLOW}({stopwatch.ElapsedMilliseconds}ms){AnsiColor.RESET}");
            } catch (Exception ex) {
                throw new EventException($"There has been an error loading all data.", ex);
            }

        }
    }
}
