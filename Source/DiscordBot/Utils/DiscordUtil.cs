using DSharpPlus.Entities;
using DSharpPlus;
using System.Data;
using NodaTime;
using System.Diagnostics;
using System.Text.RegularExpressions;
using BLL.Exceptions;
using BLL.Enums;
using BLL.Model;
using APP.Enums;
using BLL.Services;
using BLL.Interfaces;
using Microsoft.VisualBasic;
using System.Xml;
using System.Threading.Channels;
using Microsoft.IdentityModel.Tokens;
using NodaTime.Text;
using System.Collections.Immutable;

namespace APP.Utils {
    public class DiscordUtil {

        #region Fields
        private IDataRepository dataService;
        #endregion

        #region Constructor
        public DiscordUtil(IDataRepository dataService) {
            this.dataService = dataService;
        }
        #endregion

        public async Task<DiscordMessage> CreateMessageAsync(DiscordInteraction interaction, Message message) {
            try {
                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Set the guild and channel id
                message.GuildId = interaction.Guild.Id;
                message.ChannelId = interaction.Channel.Id;
                message.Sender = interaction.User.Id;

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders(interaction, dataService);

                // Create the response
                var response = await BuildMessageResponse(interaction, translated);
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);

                // Stop the stopwatch and log the elapsed time
                stopwatch.Stop();

                // Store the message ID and components for future reference
                var original = await interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                message.WithMessageId(original.Id);
                message.GuildId = interaction.Guild.Id;

                // Store the embed in the cache
                await dataService.AddMessageAsync(message);

                // Logger
                Console.WriteLine(
                    $"{AnsiColor.RESET}[{DateTime.Now}] " +
                    $"{AnsiColor.BRIGHT_GREEN}-> Message creation took {AnsiColor.YELLOW}{stopwatch.ElapsedMilliseconds}ms " +
                    $"{AnsiColor.RESET}({original.Id}) " +
                    $"{AnsiColor.RESET}({interaction.User.Username})");

                // Return the message
                return original;
            } catch (Exception ex) {
                throw new CommandException($"Embed.CreateMessageAsync: {ex.Message}", ex);
            }
        }

        public async Task<DiscordMessage> CreateMessageToChannelAsync(DiscordInteraction interaction, Message message, DiscordChannel channel) {
            try {
                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Set the guild and channel id
                message.GuildId = interaction.Guild.Id;
                message.ChannelId = channel.Id;
                message.Sender = interaction.User.Id;

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders(interaction, dataService);

                // Create the response
                var response = await BuildMessageResponse(interaction, translated);

                // Build the message with components
                var messageBuilder = new DiscordMessageBuilder()
                    .WithContent(response.Content)
                    .AddEmbeds(response.Embeds)
                    .AddComponents(response.Components)
                    .AddMentions(response.Mentions);

                // Send the message
                var original = await channel.SendMessageAsync(messageBuilder);

                // Stop the stopwatch and log the elapsed time
                stopwatch.Stop();

                // Store the message ID and components for future reference
                message.WithMessageId(original.Id);
                message.GuildId = interaction.Guild.Id;

                // Store the embed in the cache
                await dataService.AddMessageAsync(message);

                // Logger
                Console.WriteLine(
                    $"{AnsiColor.RESET}[{DateTime.Now}] " +
                    $"{AnsiColor.BRIGHT_GREEN}-> Message creation took {AnsiColor.YELLOW}{stopwatch.ElapsedMilliseconds}ms " +
                    $"{AnsiColor.RESET}({original.Id}) " +
                    $"{AnsiColor.RESET}({interaction.User.Username})");

                // Return the message
                return original;
            } catch (Exception ex) {
                throw new CommandException($"Embed.CreateMessageAsync: {ex.Message}", ex);
            }
        }

        public async Task<DiscordMessage> UpdateMessageAsync(DiscordInteraction interaction, Message message, bool isDeferMessageUpdate = true) {
            try {
                // Deferring the message update
                if (isDeferMessageUpdate) await interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Set the guild and channel id
                message.GuildId = interaction.Guild.Id;
                message.ChannelId = interaction.Channel.Id;

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders(interaction, dataService);
                var embed = translated.Embed.Build();

                // Create the response
                var response = await BuildMessageResponse(interaction, translated);
                var original = await interaction.GetOriginalResponseAsync();
                await original.ModifyAsync(new DiscordMessageBuilder()
                    .WithContent(response.Content)
                    .AddEmbeds(response.Embeds)
                    .AddComponents(response.Components));

                // Stop the stopwatch and log the elapsed time
                stopwatch.Stop();

                // Store the message
                await dataService.UpdateMessageAsync(message);

                // Logger
                Console.WriteLine(
                    $"{AnsiColor.RESET}[{DateTime.Now}] " +
                    $"{AnsiColor.BRIGHT_GREEN}-> Message update took {AnsiColor.YELLOW}{stopwatch.ElapsedMilliseconds}ms " +
                    $"{AnsiColor.RESET}({original.Id}) " +
                    $"{AnsiColor.RESET}({interaction.User.Username})");

                // Return the message
                return original;
            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw new UtilException($"Could not create response: {ex.Message}", ex);
            }
        }

        public async Task<DiscordMessage> ModifyMessageAsync(DiscordInteraction interaction, Message message) {
            try {
                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders(interaction, dataService);
                var embed = translated.Embed;

                var response = await BuildMessageResponse(interaction, translated);
                var original = await interaction.GetOriginalResponseAsync();
                await original.ModifyAsync(new DiscordMessageBuilder()
                    .WithContent(response.Content)
                    .AddEmbeds(response.Embeds)
                    .AddComponents(response.Components)
                    .AddFiles(response.Files), false, Enumerable.Empty<DiscordAttachment>());

                // Stop the stopwatch and log the elapsed time
                stopwatch.Stop();

                // Store the embed in the cache
                await dataService.UpdateMessageAsync(message);

                // Logger
                Console.WriteLine(
                    $"{AnsiColor.RESET}[{DateTime.Now}] " +
                    $"{AnsiColor.BRIGHT_GREEN}-> Message modify took {AnsiColor.YELLOW}{stopwatch.ElapsedMilliseconds}ms " +
                    $"{AnsiColor.RESET}({original.Id}) " +
                    $"{AnsiColor.RESET}({interaction.User.Username})");

                // Return the message
                return original;
            } catch (Exception ex) {
                throw new CommandException($"Embed.UpdateMessageAsync: {ex.Message}", ex);
            }
        }

        private async Task<DiscordInteractionResponseBuilder> BuildMessageResponse(DiscordInteraction interaction, Message message) {
            try {

                // Get discord channel through channel id
                DiscordChannel channel = await GetChannelByIdAsync(interaction.Guild, message.ChannelId);
                if (message.Data.ContainsKey(Identity.INTERNAL_SEND_CHANNEL)) {
                    channel = await GetChannelByIdAsync(interaction.Guild, ulong.Parse(message.Data[Identity.INTERNAL_SEND_CHANNEL]));
                }

                // List of components
                List<DiscordActionRowComponent> components = new();

                foreach (var component in message.ComponentSelectOptions) {
                    switch (component) {
                        case ComponentSelectOptions.DEFAULT:
                            components.Add(SelectComponent.GetDefault());
                            break;
                        case ComponentSelectOptions.PLACEHOLDER:
                            components.Add(SelectComponent.GetPlaceholder(message));
                            break;
                        default:
                            break;
                    }
                }

                foreach (var component in message.ComponentButtons) {
                    switch (component) {
                        case ComponentButtons.EMBED:
                            components.Add(ButtonComponent.GetDefault(channel, message.IsEphemeral));
                            break;
                        case ComponentButtons.EVENT:
                            components.Add(ButtonComponent.GetEvent(message.IsEphemeral));
                            break;
                        case ComponentButtons.INACTIVITY:
                            components.Add(ButtonComponent.GetInactivity());
                            break;
                        case ComponentButtons.INTRODUCTION:
                            components.Add(ButtonComponent.GetIntroduction());
                            break;
                        case ComponentButtons.NITRO:
                            components.Add(ButtonComponent.GetNitro());
                            break;
                        case ComponentButtons.EDIT:
                            components.Add(ButtonComponent.GetEdit(message.IsEphemeral));
                            break;
                        default:
                            break;
                    }
                }

                // Build the embed and response
                var response = ResolveImageAttachment(message.Embed)
                    .AsEphemeral(message.IsEphemeral)
                    .AddComponents(components.ToList());

                // Check if the message has content
                if (!string.IsNullOrWhiteSpace(message.Content)) response.WithContent(message.Content);
                if (message.Users.Count() > 0) {
                    var mentions = message.Users.Select(x => new UserMention(x)).ToList();
                    response.AddMentions(mentions.Cast<IMention>());
                }
                if (message.Roles.Count() > 0) {
                    var roles = message.Roles.Select(x => new RoleMention(x)).ToList();
                    response.AddMentions(roles.Cast<IMention>());
                }
                if (message.Sender is not null && message.Sender != 0) {
                    var user = await interaction.Guild.GetMemberAsync((ulong)message.Sender);
                    response.AddMention(new UserMention(user));
                }

                // Return the response
                return response;

            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw new UtilException($"Embed.CreateResponseAsync: {ex.Message}", ex);
            }
        }

        private DiscordInteractionResponseBuilder ResolveImageAttachment(Embed embed) {
            var response = new DiscordInteractionResponseBuilder();
            var folder = Path.Combine(Environment.CurrentDirectory, "Saves", "Images");
            var pattern = @"^local:\/\/.*";
            var patternAttachment = @"^attachment:\/\/.*";
            var replace = "local://";
            var replace2 = "attachment://";

            try {
                if (embed is not null) {
                    if (embed.Image != null && (Regex.IsMatch(embed.Image, pattern) || embed.Image != null && Regex.IsMatch(embed.Image, patternAttachment))) {
                        var image = Path.Combine(folder, embed.Image.Replace(replace, "").Replace(replace2, ""));
                        var imageBytes = File.ReadAllBytes(image);
                        var imageStream = new MemoryStream(imageBytes);
                        var fileName = Path.GetFileName(image);
                        embed.WithImage($"attachment://{fileName}");
                        response.AddFile(fileName, imageStream);
                    }

                    if (embed.Thumbnail != null && (Regex.IsMatch(embed.Thumbnail, pattern) || embed.Thumbnail != null && Regex.IsMatch(embed.Thumbnail, patternAttachment))) {
                        var thumbnail = Path.Combine(folder, embed.Thumbnail.Replace(replace, "").Replace(replace2, ""));
                        var thumbnailBytes = File.ReadAllBytes(thumbnail);
                        var thumbnailStream = new MemoryStream(thumbnailBytes);
                        var fileName = Path.GetFileName(thumbnail);
                        embed.WithThumbnail($"attachment://{fileName}");
                        response.AddFile(fileName, thumbnailStream);
                    }

                    return response.AddEmbed(embed.Build());
                } else return response;

            } catch (Exception ex) {
                throw new CommandException($"Embed.CheckIfLocalImage: {ex.Message}", ex);
            }
        }

        public DiscordMessageBuilder ResolveImageAttachment(Message message) {
            var embed = message.Embed;
            var response = new DiscordMessageBuilder();
            var folder = Path.Combine(Environment.CurrentDirectory, "Saves", "Images");
            var pattern = @"^local:\/\/.*";
            var patternAttachment = @"^attachment:\/\/.*";
            var replace = "local://";
            var replace2 = "attachment://";

            try {
                if (embed is not null) {
                    if (embed.Image != null && (Regex.IsMatch(embed.Image, pattern) || embed.Image != null && Regex.IsMatch(embed.Image, patternAttachment))) {
                        var image = Path.Combine(folder, embed.Image.Replace(replace, "").Replace(replace2, ""));
                        var imageBytes = File.ReadAllBytes(image);
                        var imageStream = new MemoryStream(imageBytes);
                        var fileName = Path.GetFileName(image);
                        embed.WithImage($"attachment://{fileName}");
                        response.AddFile(fileName, imageStream);
                    }

                    if (embed.Thumbnail != null && (Regex.IsMatch(embed.Thumbnail, pattern) || embed.Thumbnail != null && Regex.IsMatch(embed.Thumbnail, patternAttachment))) {
                        var thumbnail = Path.Combine(folder, embed.Thumbnail.Replace(replace, "").Replace(replace2, ""));
                        var thumbnailBytes = File.ReadAllBytes(thumbnail);
                        var thumbnailStream = new MemoryStream(thumbnailBytes);
                        var fileName = Path.GetFileName(thumbnail);
                        embed.WithThumbnail($"attachment://{fileName}");
                        response.AddFile(fileName, thumbnailStream);
                    }

                    return response
                        .AddEmbed(embed.Build());
                } else return response;

            } catch (Exception ex) {
                throw new CommandException($"Embed.CheckIfLocalImage: {ex.Message}", ex);
            }
        }

        public async Task<DiscordMessage?> GetMessageByIdAsync(DiscordChannel channel, ulong? messageId) {
            try {
                if (messageId != null) return await channel.GetMessageAsync((ulong)messageId);
                else return null;
            } catch (Exception) {
                return null;
            }
        }

        public async Task<DiscordRole> GetRolesByIdAsync(DiscordGuild guild, ulong roleId) {
            if (guild.Roles.TryGetValue(roleId, out var role)) {
                var test = role;
                return await Task.FromResult(role);
            } else throw new UtilException($"No roles with id \"{roleId}\" found.");
        }

        public async Task<DiscordChannel> GetChannelByIdAsync(DiscordGuild guild, ulong channelId) {
            if (guild.Channels.TryGetValue(channelId, out var channel)) {
                return await Task.FromResult(channel);
            } else throw new UtilException($"No channel with id \"{channelId}\" found.");
        }

        public async Task<string> AddPingRolesToContentAsync(Message message, DiscordGuild guild) {
            string result = message.Content is null ? "" : $"{message.Content}\n";
            if (message.Roles.Count > 0) {
                var tasks = message.Roles.Select(async x => await GetRolesByIdAsync(guild, x));
                var roles = await Task.WhenAll(tasks);
                var pings = string.Empty;
                foreach (var role in roles) {
                    if (role.Name == "@everyone") {
                        pings += "@everyone "; 
                    } else pings += role.Mention + " ";
                }
                result += pings;
            }
            return result;
        }

        public async Task SendActionMessage(DiscordInteraction interaction, TemplateMessage template, string title, string text = "") {
            var message = (await dataService.GetTemplateAsync(interaction.Guild.Id, template.ToString().ToUpper()))!.Message;
            message.SetData(Placeholder.TEXT1, title);
            message.SetData(Placeholder.TEXT2, text);
            await CreateMessageAsync(interaction, message);
        }
    }
}