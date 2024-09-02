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
        private static string folder = Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory)!.FullName)!.FullName)!.FullName + @"\Saves";
        private static Dictionary<ulong, Dictionary<ulong, EmbedBuilder>> embeds = new();
        private static Dictionary<ulong, Dictionary<string, EmbedBuilder>> templates = new();
        private static Dictionary<ulong, Dictionary<CommandEnum, Permission>> permissions = new();
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
        public static IReadOnlyList<string> Timezones {
            get => timeZones;
        }
        #endregion

        #region Methods
        public static async Task LoadDataAsync(DiscordClient client) {
            var stopwatch = Stopwatch.StartNew();

            // Ensure files exist for all guilds
            foreach (var guild in client.Guilds) {
                await CheckFilesAsync(guild.Key);
            }
            stopwatch.Stop();
            Console.WriteLine($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Successfully checked and updated files {AnsiColor.YELLOW}({stopwatch.ElapsedMilliseconds}ms)");

            // Restart stopwatch for loading files
            stopwatch.Restart();

            // Load files for all guilds
            foreach (var guild in client.Guilds) {
                await LoadFileAsync(guild.Key);
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
        }

        private static async Task CheckFilesAsync(ulong guildId) {
            List<Permission> permissions = Enum.GetValues<CommandEnum>().Select(cmd => new Permission(cmd)).ToList();
            string pathSaves = $"{folder}/Servers/{guildId}";
            string pathPermission = $"{pathSaves}/{FileEnum.PERMISSION.ToString().ToLower()}.json";
            string pathConfig = $"{pathSaves}/{FileEnum.CONFIG.ToString().ToLower()}.json";
            string pathEmbed = $"{pathSaves}/{FileEnum.EMBED.ToString().ToLower()}.json";
            string pathTemplate = $"{pathSaves}/{FileEnum.TEMPLATES.ToString().ToLower()}.json";
            string pathActivity = $"{pathSaves}/{FileEnum.ACTIVITIES.ToString().ToLower()}.json";
            string pathLogs = $"{pathSaves}/{FileEnum.LOGS.ToString().ToLower()}.json";
            if (!Directory.Exists(pathSaves)) {
                Directory.CreateDirectory(pathSaves);
            }
            if (!File.Exists(pathConfig)) {
                string json = JsonConvert.SerializeObject(new BotConfig(), Formatting.Indented);
                await File.WriteAllTextAsync(pathConfig, json);
            }
            if (!File.Exists(pathEmbed)) {
                string embed = JsonConvert.SerializeObject(new List<EmbedBuilder>(), Formatting.Indented);
                await File.WriteAllTextAsync(pathEmbed, embed);
            }
            if (!File.Exists(pathTemplate)) {
                string embed = JsonConvert.SerializeObject(new Dictionary<string, EmbedBuilder>(), Formatting.Indented);
                await File.WriteAllTextAsync(pathTemplate, embed);
            }
            if (!File.Exists(pathPermission)) {
                string permission = JsonConvert.SerializeObject(permissions, Formatting.Indented);
                await File.WriteAllTextAsync(pathPermission, permission);
            } else {
                string json = File.ReadAllText(pathPermission);
                permissions = JsonConvert.DeserializeObject<List<Permission>>(json) ?? new List<Permission>();
                foreach (var cmd in Enum.GetValues<CommandEnum>()) {
                    if (!permissions.Any(x => x.Cmd == cmd.ToString())) permissions.Add(new Permission(cmd));
                }
                string jsonString = JsonConvert.SerializeObject(permissions, Formatting.Indented);
                await File.WriteAllTextAsync(pathPermission, jsonString);
            }
        }

        private static async Task LoadFileAsync(ulong guildId) {
            try {
                string pathSaves = $"{folder}/Servers/{guildId}";
                string pathPermission = $"{pathSaves}/{FileEnum.PERMISSION.ToString().ToLower()}.json";
                string pathConfig = $"{pathSaves}/{FileEnum.CONFIG.ToString().ToLower()}.json";
                string pathEmbed = $"{pathSaves}/{FileEnum.EMBED.ToString().ToLower()}.json";
                string pathTemplate = $"{pathSaves}/{FileEnum.TEMPLATES.ToString().ToLower()}.json";
                string pathActivity = $"{pathSaves}/{FileEnum.ACTIVITIES.ToString().ToLower()}.json";
                string pathLogs = $"{pathSaves}/{FileEnum.LOGS.ToString().ToLower()}.json";

                // Read embed data
                List<EmbedBuilder> serverEmbeds = (await JsonData.ReadFileAsync(guildId, FileEnum.EMBED) as List<EmbedBuilder>)!;

                if (!embeds.TryGetValue(guildId, out var guildEmbeds)) {
                    guildEmbeds = new Dictionary<ulong, EmbedBuilder>();
                    embeds[guildId] = guildEmbeds;
                }

                foreach (var embed in serverEmbeds) {
                    guildEmbeds.Add(embed.Id, embed);
                }

                // Read embed data
                Dictionary<string, EmbedBuilder> serverTemplates = (await JsonData.ReadFileAsync(guildId, FileEnum.TEMPLATES) as Dictionary<string, EmbedBuilder>)!;

                if (!templates.TryGetValue(guildId, out var guildTemplates)) {
                    guildTemplates = new Dictionary<string, EmbedBuilder>();
                    templates[guildId] = guildTemplates;
                }

                foreach (var embed in serverTemplates) {
                    guildTemplates.Add(embed.Key, embed.Value);
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
        public static Task<Permission> GetPermissionAsync(ulong guildId, CommandEnum cmd) {
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
        public static async Task SaveTemplateAsync(ulong guildId, string name, EmbedBuilder embed) {
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
        public static async Task AddEmbedAsync(ulong guildId, ulong messageId, EmbedBuilder embed) {
            try {
                if (embeds.TryGetValue(guildId, out var embededMessage)) {
                    embeds[guildId].Add(messageId, embed);
                    await JsonData.SaveEmbedsAsync(guildId);
                }
            } catch (Exception ex) {
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with message id {messageId} could not be added.", ex);
            }
        }
        public static async Task RemoveEmbedAsync(ulong guildId, ulong messageId) {
            try {
                if (embeds.TryGetValue(guildId, out var embededMessage)) {
                    embeds[guildId].Remove(messageId);
                    await JsonData.SaveEmbedsAsync(guildId);
                }
            } catch (Exception ex) {
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with message id {messageId} could not be added.", ex);
            }
        }
        public static Task<EmbedBuilder> GetEmbedAsync(ulong guildId, ulong messageId) {
            try {
                if (embeds.TryGetValue(guildId, out var embededMessage)) {
                    if (embededMessage.TryGetValue(messageId, out var embedBuilder)) {
                        return Task.FromResult(embeds[guildId][messageId]);
                    }
                }
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with message id {messageId} could not be found.");
            } catch (Exception ex) {
                throw new UtilException($"{AnsiColor.BRIGHT_RED}[Error] {AnsiColor.RESET}Embeded message for guild {guildId} with message id {messageId} could not be found.", ex);
            }
        }
        #endregion
    }
}
