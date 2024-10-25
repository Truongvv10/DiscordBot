using DiscordBot.Configuration;
using DiscordBot.Model;
using DiscordBot.Model.Enums;
using DSharpPlus;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XironiteDiscordBot.Exceptions;

namespace DiscordBot.Utils
{
    public static class CacheData {

        #region Properties
        private static string folder = Path.Combine(Environment.CurrentDirectory, "Saves");
        private static Dictionary<ulong, Dictionary<ulong, EmbedBuilder>> embeds = new();
        private static Dictionary<ulong, Dictionary<string, EmbedBuilder>> templates = new();
        private static Dictionary<ulong, Dictionary<CommandEnum, Permission>> permissions = new();
        private static Dictionary<ulong, Dictionary<string, Changelog>> changelogs = new();
        private static Dictionary<ulong, List<string>> activities = new();
        private static Dictionary<ulong, BotConfig> configs = new();
        private static List<string> timeZones = new();
        #endregion

        #region Getter & Setter
        public static IReadOnlyDictionary<ulong, Dictionary<ulong, EmbedBuilder>> Embeds {
            get => embeds;
        }
        public static IReadOnlyDictionary<ulong, Dictionary<string, EmbedBuilder>> Templates {
            get => templates;
        }
        public static IReadOnlyDictionary<ulong, Dictionary<CommandEnum, Permission>> Permissions {
            get => permissions;
        }
        public static IReadOnlyDictionary<ulong, Dictionary<string, Changelog>> Changelogs {
            get => changelogs;
        }
        public static IReadOnlyList<string> Timezones {
            get => timeZones;
        }
        #endregion

        #region Methods
        public static async Task LoadAllData(DiscordClient client) {
            try {
                var stopwatch = Stopwatch.StartNew();

                // Ensure files exist for all guilds
                foreach (var guild in client.Guilds) {
                    await CheckFiles(guild.Key);
                }
                stopwatch.Stop();
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Successfully checked and updated files {AnsiColor.YELLOW}({stopwatch.ElapsedMilliseconds}ms)");

                // Restart stopwatch for loading files
                stopwatch.Restart();

                // Load files for all guilds
                foreach (var guild in client.Guilds) {
                    await LoadDataToCache(guild.Key);
                }
                stopwatch.Stop();
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Successfully loaded all files to storage {AnsiColor.YELLOW}({stopwatch.ElapsedMilliseconds}ms){AnsiColor.RESET}");


                // Restart stopwatch for loading files
                stopwatch.Restart();

                // Print all time zone identifiers and display names
                foreach (var zone in DateTimeZoneProviders.Tzdb.GetAllZones()) {
                    timeZones.Add(zone.Id);
                }
                stopwatch.Stop();
                Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Successfully loaded available timezones {AnsiColor.YELLOW}({stopwatch.ElapsedMilliseconds}ms){AnsiColor.RESET}");
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

        }

        private static async Task CheckFiles(ulong guildId) {

            // Create a dynamic dictionary mapping FileEnum values to their respective file paths
            string dataFolder = Path.Combine(folder, "Servers", guildId.ToString());
            var dataFiles = Enum.GetValues(typeof(FileEnum))
                                .Cast<FileEnum>()
                                .Select(fileEnum => Path.Combine(dataFolder, $"{fileEnum.ToString().ToLower()}.json"))
                                .ToList();

            // Ensure the directory data exists
            if (!Directory.Exists(dataFolder)) {
                Directory.CreateDirectory(dataFolder);
            }

            // List to store all the file checking and creation tasks
            var tasks = new List<Task>();

            // Check each file and create it if it doesn't exist
            foreach (var filePath in dataFiles) {
                tasks.Add(EnsureFileExists(filePath));
            }

            // Await all tasks concurrently
            await Task.WhenAll(tasks);
        }

        private static async Task EnsureFileExists(string filePath) {
            if (!File.Exists(filePath)) {
                // Create an empty file asynchronously
                await File.WriteAllTextAsync(filePath, string.Empty);
            }
        }

        private static async Task LoadDataToCache(ulong guildId) {
            try {
                string pathSaves = Path.Combine(folder, "Servers", guildId.ToString());
                string pathPermission = Path.Combine(pathSaves, $"{FileEnum.PERMISSION.ToString().ToLower()}.json");
                string pathConfig = Path.Combine(pathSaves, $"{FileEnum.CONFIG.ToString().ToLower()}.json");
                string pathEmbed = Path.Combine(pathSaves, $"{FileEnum.EMBED.ToString().ToLower()}.json");
                string pathTemplate = Path.Combine(pathSaves, $"{FileEnum.TEMPLATES.ToString().ToLower()}.json");
                string pathActivity = Path.Combine(pathSaves, $"{FileEnum.ACTIVITIES.ToString().ToLower()}.json");
                string pathLogs = Path.Combine(pathSaves, $"{FileEnum.LOGS.ToString().ToLower()}.json");

                // Read embed data
                List<EmbedBuilder> serverEmbeds = (await JsonData.ReadFileAsync(guildId, FileEnum.EMBED) as List<EmbedBuilder>)!;

                if (!embeds.TryGetValue(guildId, out var guildEmbeds)) {
                    guildEmbeds = new Dictionary<ulong, EmbedBuilder>();
                    embeds[guildId] = guildEmbeds;
                }

                foreach (var embed in serverEmbeds) {
                    guildEmbeds.Add(embed.Id, embed);
                }

                // Read template data
                Dictionary<string, EmbedBuilder> serverTemplates = (await JsonData.ReadFileAsync(guildId, FileEnum.TEMPLATES) as Dictionary<string, EmbedBuilder>)!;

                if (!templates.TryGetValue(guildId, out var guildTemplates)) {
                    guildTemplates = new Dictionary<string, EmbedBuilder>();
                    templates[guildId] = guildTemplates;
                }

                foreach (var embed in serverTemplates) {
                    guildTemplates.Add(embed.Key, embed.Value);
                }

                // Read changelog data
                Dictionary<string, Changelog> serverChangelogs = (await JsonData.ReadFileAsync(guildId, FileEnum.CHANGELOG) as Dictionary<string, Changelog>)!;

                if (!changelogs.TryGetValue(guildId, out var guildChangelogs)) {
                    guildChangelogs = new Dictionary<string, Changelog>();
                    changelogs[guildId] = guildChangelogs;
                }

                foreach (var embed in serverChangelogs) {
                    guildChangelogs.Add(embed.Key, embed.Value);
                }

                // Read permisison data
                List<Permission> serverPermissions = (await JsonData.ReadFileAsync(guildId, FileEnum.PERMISSION) as List<Permission>)!;

                if (!permissions.TryGetValue(guildId, out var guildPermissions)) {
                    guildPermissions = new Dictionary<CommandEnum, Permission>();
                    permissions[guildId] = guildPermissions;
                }

                foreach (var permission in serverPermissions) {
                    CommandEnum cmd = (CommandEnum)Enum.Parse(typeof(CommandEnum), permission.Cmd);
                    guildPermissions[cmd] = permission;
                }
            } catch (Exception ex) {
                Console.WriteLine($"LoadFileAsync werkt niet {ex}");
                throw;
            }
        }
        #endregion

        #region Retrieving & Receiving Data
        public static Task<Permission> GetPermission(ulong guildId, CommandEnum cmd) {
            try {
                if (permissions.TryGetValue(guildId, out var commandPermissions)) {
                    if (commandPermissions.TryGetValue(cmd, out var permission)) {
                        return Task.FromResult(permission);
                    }
                }
                throw new KeyNotFoundException("The specified guild or command permission was not found.");
            } catch (Exception ex) {
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Permission for guild {guildId} using command {cmd} can not be found", ex);
            }
        }

        public static async Task SaveTemplate(ulong guildId, string name, EmbedBuilder embed) {
            try {
                if (templates.TryGetValue(guildId, out var serverTemplates)) {
                    if (templates[guildId].TryAdd(name, embed)) {
                        await JsonData.SaveFileAsync(guildId, FileEnum.TEMPLATES);
                    } else throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with template {name} could not be added.");
                }
            } catch (Exception ex) {
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with template {name} could not be added.", ex);
            }
        }

        public static EmbedBuilder GetEmbed(ulong guildId, ulong messageId) {
            try {
                if (embeds.TryGetValue(guildId, out var embededMessage)) {
                    if (embededMessage.TryGetValue(messageId, out var embedBuilder)) {
                        return embeds[guildId][messageId];
                    }
                }
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with message id {messageId} could not be found.");
            } catch (Exception ex) {
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with message id {messageId} could not be found.", ex);
            }
        }

        public static async Task AddEmbed(ulong guildId, ulong messageId, EmbedBuilder embed) {
            try {
                if (embeds.TryGetValue(guildId, out var embededMessage)) {
                    embeds[guildId].Add(messageId, embed);
                    await JsonData.SaveEmbedsAsync(guildId);
                }
            } catch (Exception ex) {
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with message id {messageId} could not be added.", ex);
            }
        }

        public static async Task RemoveEmbed(ulong guildId, ulong messageId) {
            try {
                if (embeds.TryGetValue(guildId, out var embededMessage)) {
                    embeds[guildId].Remove(messageId);
                    await JsonData.SaveEmbedsAsync(guildId);
                }
            } catch (Exception ex) {
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with message id {messageId} could not be added.", ex);
            }
        }
        #endregion
    }
}
