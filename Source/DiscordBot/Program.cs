using BLL.Exceptions;
using APP.Services;
using AnsiColor = APP.Utils.AnsiColor;
using BLL.Interfaces;
using BLL.Managers;
using BLL.Services;
using DLLSQLite.Repositories;
using DLLSQLite.Contexts;
using DSharpPlus;

namespace APP {
    public static class Program {
        private static DiscordService _discordService;
        private static readonly object _lock = new object();

        public static DiscordService DiscordService {
            get {
                if (_discordService == null) {
                    throw new InvalidOperationException("DiscordService is not initialized yet.");
                }
                return _discordService;
            }
        }

        public static async Task Main(string[] args) {
            try {
                // Initialize the Discord bot
                DiscordManager manager = new DiscordManager();
                ICacheData cache = new CacheData();
                var service = new DiscordService(await manager.GetDiscordBotConfig(), cache);

                lock (_lock) {
                    _discordService = service;
                }

                // Initialize the Discord bot
                await _discordService.InitializeAsync();

                // Keep the application running
                await Task.Delay(-1);

            } catch (ServiceException ex) {
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.BRIGHT_RED}[Error] Service error occurred: {ex.Message}");
            } catch (EventException ex) {
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.BRIGHT_RED}[Error] Event error occurred: {ex.Message}");
            } catch (Exception ex) {
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.BRIGHT_RED}[Error] An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
