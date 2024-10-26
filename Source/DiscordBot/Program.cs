using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus;
using DiscordBot.Manager;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DiscordBot.Commands.Slash;
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
using DiscordBot.Services;
using AnsiColor = DiscordBot.Utils.AnsiColor;
using DiscordBot.Exceptions;

namespace DiscordBot {
    internal class Program {
        static async Task Main(string[] args) {
			try {
                var discordService = new DiscordService();

                // Initialize the Discord bot
                await discordService.InitializeAsync();

                // Start data-saving interval
                discordService.StartDataSavingInterval();

                // Keep the application running
                await Task.Delay(-1);

            } catch (ServiceException ex) {
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.BRIGHT_RED}[Error] Service error occurred: {ex.Message}");
            } catch (Exception ex) {
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.BRIGHT_RED}[Error] An unexpected error occurred: {ex.Message}");
			}

        }
    }
}
