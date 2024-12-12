using BLL.Exceptions;
using APP.Services;
using AnsiColor = APP.Utils.AnsiColor;
using BLL.Interfaces;
using BLL.Managers;
using BLL.Services;

namespace APP {
    internal class Program {
        static async Task Main(string[] args) {
            try {
                // Initialize the Discord bot
                DiscordManager manager = new DiscordManager();
                CacheData cache = new CacheData();
                var discordService = new DiscordService(await manager.GetDiscordBotConfig(), cache);

                // Initialize the Discord bot
                await discordService.InitializeAsync();

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
