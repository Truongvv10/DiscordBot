using DiscordBot.Exceptions;
using DiscordBot.Utils;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnsiColor = DiscordBot.Utils.AnsiColor;

namespace DiscordBot.Events {
    public class DiscordClientReadyEvent {
        public async Task ClientReady(DiscordClient sender, ReadyEventArgs args) {
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
