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
using DiscordBot.Services;

namespace DiscordBot {
    internal class Program {
        static async Task Main(string[] args) {
            var discordService = new DiscordService();

            // Initialize the Discord bot
            await discordService.InitializeAsync();

            // Start data-saving interval
            discordService.StartDataSavingInterval();

            // Keep the application running
            await Task.Delay(-1);

        }
    }
}
