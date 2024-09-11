using DiscordBot.Model;
using DiscordBot.Model.Enums;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using XironiteDiscordBot.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiscordBot.Utils {
    public static class JsonData {

        #region Private Properties
        private static string folder = Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory)!.FullName)!.FullName)!.FullName + @"\Saves\Servers";
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guildId">The discord server ID</param>
        /// <param name="save">The file to be saved</param>
        /// <returns>Task</returns>
        /// <exception cref="UtilException"></exception>
        public static async Task SaveFileAsync(ulong guildId, FileEnum save) {
            try {

                // general file
                var stopwatch = Stopwatch.StartNew();
                var name = save.ToString().ToLower();
                var file = $@"{folder}\{guildId}\{name}.json";
                var json = string.Empty;

                // check which file
                switch (save) {
                    case FileEnum.EMBED:
                        if (CacheData.Embeds.TryGetValue(guildId, out var serverEmbeds))
                            json = JsonConvert.SerializeObject(serverEmbeds.Values.ToList(), Formatting.Indented);
                        break;
                    case FileEnum.TEMPLATES:
                        if (CacheData.Templates.TryGetValue(guildId, out var serverTemplates))
                            json = JsonConvert.SerializeObject(serverTemplates, Formatting.Indented);
                        break;
                    case FileEnum.ACTIVITIES:
                        break;
                    case FileEnum.PERMISSION:
                        if (CacheData.Permissions.TryGetValue(guildId, out var serverPermissions))
                            json = JsonConvert.SerializeObject(serverPermissions.Values.ToList(), Formatting.Indented);
                        break;
                    case FileEnum.LOGS:
                        break;
                    case FileEnum.CONFIG:
                        break;
                    case FileEnum.CHANGELOG:
                        if (CacheData.Changelogs.TryGetValue(guildId, out var serverChangelogs))
                            json = JsonConvert.SerializeObject(serverChangelogs.Values.ToList(), Formatting.Indented);
                        break;
                    default:
                        throw new UtilException($"JsonData.SaveFileAsync: Couldn't save \"{name}.json\"");
                }

                // Saving file
                await File.WriteAllTextAsync(file, json);

                // Print feedback
                stopwatch.Stop();
                await Console.Out.WriteLineAsync($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Saved file {AnsiColor.RESET}{name}.json {AnsiColor.CYAN}for guild {guildId} {AnsiColor.YELLOW}({stopwatch.ElapsedMilliseconds}ms){AnsiColor.RESET}");

            } catch (Exception ex) {
                throw new UtilException($"JsonData.SaveFileAsync: Couldn't save \"{save.ToString().ToLower()}.json\"", ex);
            }
        }

        public static async Task<object> ReadFileAsync(ulong guildId, FileEnum save) {
            try {

                // general file
                var stopwatch = Stopwatch.StartNew();
                var name = save.ToString().ToLower();
                var file = $@"{folder}\{guildId}\{name}.json";
                var json = new object();

                // reading file
                using (StreamReader sr = new StreamReader(file)) {
                    var data = await sr.ReadToEndAsync();

                    // check which file to read
                    switch (save) {
                        case FileEnum.EMBED:
                            json = JsonConvert.DeserializeObject<List<EmbedBuilder>>(data) ?? new List<EmbedBuilder>();
                            break;
                        case FileEnum.TEMPLATES:
                            json = JsonConvert.DeserializeObject<Dictionary<string, EmbedBuilder>>(data) ?? new Dictionary<string, EmbedBuilder>();
                            break;
                        case FileEnum.ACTIVITIES:
                            break;
                        case FileEnum.PERMISSION:
                            json = JsonConvert.DeserializeObject<List<Permission>>(data) ?? Enum.GetValues<CommandEnum>().Select(cmd => new Permission(cmd)).ToList();
                            break;
                        case FileEnum.LOGS:
                            break;
                        case FileEnum.CONFIG:
                            break;
                        case FileEnum.CHANGELOG:
                            json = JsonConvert.DeserializeObject<Dictionary<string, Changelog>>(data) ?? new Dictionary<string, Changelog>();
                            break;
                        default:
                            throw new UtilException($"JsonData.ReadFileAsync: Couldn't read \"{name}.json\"");
                    }

                    // Print feedback
                    stopwatch.Stop();
                    await Console.Out.WriteLineAsync($"{AnsiColor.RESET}[{DateTime.Now}] {AnsiColor.CYAN}Loading file {AnsiColor.RESET}{name}.json {AnsiColor.CYAN}for guild {guildId} {AnsiColor.YELLOW}({stopwatch.ElapsedMilliseconds}ms){AnsiColor.RESET}");

                    // return object
                    return json;

                }
            } catch (Exception ex) {
                throw new UtilException($"JsonData.ReadFileAsync: Couldn't read \"{save.ToString().ToLower()}.json\"", ex);
            }
        }



        public static async Task SaveEmbedsAsync(ulong serverId) {
            try {
                var file = $@"{folder}\{serverId}\{FileEnum.EMBED.ToString().ToLower()}.json";
                if (CacheData.Embeds.TryGetValue(serverId, out var serverEmbeds)) {
                    var json = JsonConvert.SerializeObject(serverEmbeds.Values.ToList(), Formatting.Indented);
                    await File.WriteAllTextAsync(file, json);
                }
            } catch (Exception ex) {
                throw new UtilException($"Couldn't save \"{serverId}\" embed data.", ex);
            }
        }
        public static async Task<List<EmbedBuilder>> ReadEmbedsAsync(ulong serverId) {
            try {
                // Read file and convert to object
                string file = $@"{folder}\{serverId}\{FileEnum.EMBED.ToString().ToLower()}.json";
                using (StreamReader sr = new StreamReader(file)) {
                    var data = await sr.ReadToEndAsync();
                    var embeds = JsonConvert.DeserializeObject<List<EmbedBuilder>>(data)!;
                    return embeds;
                }
            } catch (Exception ex) {
                throw new UtilException($"Couldn't read \"{serverId}\" embed data.", ex);
            }
        }
        public static async Task SavePermissionsAsync(ulong serverId) {
            try {
                var file = $@"{folder}\{serverId}\{FileEnum.PERMISSION.ToString().ToLower()}.json";
                if (CacheData.Permissions.TryGetValue(serverId, out var serverPermissions)) {
                    var json = JsonConvert.SerializeObject(serverPermissions.Values.ToList(), Formatting.Indented);
                    await File.WriteAllTextAsync(file, json);
                }
            } catch (Exception ex) {
                throw new UtilException($"Couldn't save \"{serverId}\" permission data.", ex);
            }
        }
        public static async Task<List<Permission>> ReadPermissionsAsync(ulong serverId) {
            try {
                // Read file and convert to object
                string file = $@"{folder}\{serverId}\{FileEnum.PERMISSION.ToString().ToLower()}.json";
                using (StreamReader sr = new StreamReader(file)) {
                    var data = await sr.ReadToEndAsync();
                    var perms = JsonConvert.DeserializeObject<List<Permission>>(data)!;
                    return perms;
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw new UtilException($"Couldn't read \"{serverId}\" permission data.", ex);
            }
        }
        #endregion
    }
}
