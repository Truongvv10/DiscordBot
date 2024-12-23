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

        public async Task CreateMessageAsync(CommandEnum type, DiscordInteraction interaction, Message message, bool hidden = false) {
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
                var response = await CreateResponseAsync(type, interaction, translated, hidden);
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

            } catch (Exception ex) {
                throw new CommandException($"Embed.CreateMessageAsync: {ex.Message}", ex);
            }
        }

        public async Task CreateMessageToChannelAsync(CommandEnum type, DiscordInteraction interaction, Message message, DiscordChannel channel) {
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
                var response = await CreateResponseAsync(type, interaction, translated);
                DiscordMessage original;
                if (response.Embeds.Count() == 0) {
                    original = await channel.SendMessageAsync(response.Content);
                } else {
                    original = await channel.SendMessageAsync(response.Content, response.Embeds.First());
                }

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

            } catch (Exception ex) {
                throw new CommandException($"Embed.CreateMessageAsync: {ex.Message}", ex);
            }
        }

        public async Task UpdateMessageAsync(DiscordInteraction interaction, Message message, bool isDeferMessageUpdate = true) {
            try {
                if (message.MessageId != null) {
                    // Start the stopwatch
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    // Set the guild and channel id
                    message.GuildId = interaction.Guild.Id;
                    message.ChannelId = interaction.Channel.Id;
                    message.Sender = interaction.User.Id;

                    // Translate the placeholders
                    var translated = message.DeepClone();
                    await translated.TranslatePlaceholders(interaction, dataService);
                    var embed = translated.Embed.Build();

                    // Create the response
                    if (isDeferMessageUpdate) await interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    var response = await CreateResponseAsync(message.Type, interaction, translated, message.IsEphemeral);
                    var original = await interaction.GetOriginalResponseAsync();
                    await original.ModifyAsync(new DiscordMessageBuilder()
                        .WithContent(response.Content)
                        .AddEmbeds(response.Embeds)
                        .AddComponents(response.Components));

                    // Stop the stopwatch and log the elapsed time
                    stopwatch.Stop();

                    // Logger
                    Console.WriteLine(
                        $"{AnsiColor.RESET}[{DateTime.Now}] " +
                        $"{AnsiColor.BRIGHT_GREEN}-> Message update took {AnsiColor.YELLOW}{stopwatch.ElapsedMilliseconds}ms " +
                        $"{AnsiColor.RESET}({original.Id}) " +
                        $"{AnsiColor.RESET}({interaction.User.Username})");

                } else throw new UtilException($"Could not create response because message was null");
            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw new UtilException($"Could not create response: {ex.Message}", ex);
            }
        }

        public async Task ModifyMessageAsync(CommandEnum type, DiscordInteraction interaction, Message message, bool hidden = false) {
            try {
                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders(interaction, dataService);
                var embed = translated.Embed;

                var response = await CreateResponseAsync(type, interaction, translated, message.IsEphemeral);
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

            } catch (Exception ex) {
                throw new CommandException($"Embed.UpdateMessageAsync: {ex.Message}", ex);
            }
        }

        private async Task<DiscordInteractionResponseBuilder> CreateResponseAsync(CommandEnum type, DiscordInteraction interaction, Message message, bool hidden = false) {
            try {

                // Get discord channel through channel id
                DiscordChannel channel = await GetChannelByIdAsync(interaction.Guild, message.ChannelId);
                if (message.Data.ContainsKey(Identity.INTERNAL_SEND_CHANNEL)) {
                    channel = await GetChannelByIdAsync(interaction.Guild, ulong.Parse(message.Data[Identity.INTERNAL_SEND_CHANNEL]));
                }

                // List of components
                List<DiscordActionRowComponent> components = new();

                // Define button components with corresponding actions
                var buttonComponent = new List<DiscordComponent> {
                    new DiscordButtonComponent(ButtonStyle.Success, Identity.BUTTON_CHANNEL, $"Send to {channel.Name}"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Identity.BUTTON_TEMPLATE, $"Use Template"),
                    new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_UPDATE, $"Update"),
                    new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_CANCEL, "Cancel", hidden)};

                switch (type) {

                    case CommandEnum.EMBED_CREATE:
                        components.Add(DefaultComponent().First());
                        components.Add(PlaceholderComponent(message).First());
                        components.Add(new DiscordActionRowComponent(buttonComponent));
                        break;

                    case CommandEnum.EMBED_EDIT:
                        components.Add(DefaultComponent().First());
                        components.Add(PlaceholderComponent(message).First());
                        var updateComponent = new List<DiscordComponent> {
                            new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_UPDATE, "Update"),
                            new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_CANCEL, "Cancel", hidden)};
                        components.Add(new DiscordActionRowComponent(updateComponent));
                        break;

                    case CommandEnum.EVENTS_CREATE:
                        components.Add(DefaultComponent().First());
                        components.Add(PlaceholderComponent(message).First());
                        components.Add(new DiscordActionRowComponent(buttonComponent));
                        break;

                    case CommandEnum.NITRO:
                        var buttonNitroClaim = new List<DiscordComponent> {
                            new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_NITRO, "Claim Nitro")};
                        components.Add(new DiscordActionRowComponent(buttonNitroClaim));
                        break;

                    case CommandEnum.EVENTS_SETUP:
                        var buttonEventSetup = new List<DiscordComponent> {
                            new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_EVENT_SETUP, "Setup Event"),
                            new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_CANCEL, "Cancel", hidden)};
                        components.Add(new DiscordActionRowComponent(buttonEventSetup));
                        break;

                    case CommandEnum.BROADCAST:
                        break;

                    case CommandEnum.CHANGELOG:
                        break;

                    default:
                        break;
                }

                // Build the embed and response
                var response = ResolveImageAttachment(message.Embed)
                    .AsEphemeral(hidden)
                    .AddComponents(components.ToList());

                // Check for any roles to mention
                if (message.Roles is not null && message.Roles.Count > 0) {
                    var tasks = message.Roles.Select(async x => await GetRolesByIdAsync(interaction.Guild, x));
                    var roles = await Task.WhenAll(tasks);
                    var content = string.Empty;
                    foreach (var role in roles) if (role.Name == "@everyone") content += "@everyone "; else content += role.Mention + " ";
                    response.WithContent(message.Content + "\n" + content);
                } else response.WithContent(message.Content);

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

        public List<DiscordActionRowComponent> DefaultComponent() {

            var selectOptions = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Edit Title", Identity.SELECTION_TITLE, "Edit your embed title & url.", emoji: new DiscordComponentEmoji("✏️")),
                    new DiscordSelectComponentOption("Edit Description", Identity.SELECTION_DESCRIPTION, "Edit your embed description.", emoji: new DiscordComponentEmoji("📄")),
                    new DiscordSelectComponentOption("Edit Footer", Identity.SELECTION_FOOTER, "Edit your embed footer & image.", emoji: new DiscordComponentEmoji("🧩")),
                    new DiscordSelectComponentOption("Edit Author", Identity.SELECTION_AUTHOR, "Edit your embed author text, link & url.", emoji: new DiscordComponentEmoji("👤")),
                    new DiscordSelectComponentOption("Edit Main Image", Identity.SELECTION_IMAGE, "Edit your embed image.", emoji: new DiscordComponentEmoji("🪪")),
                    new DiscordSelectComponentOption("Edit Thumbnail Image", Identity.SELECTION_THUMBNAIL, "Edit your embed tumbnail.", emoji: new DiscordComponentEmoji("🖼")),
                    new DiscordSelectComponentOption("Edit Color", Identity.SELECTION_COLOR, "Edit your embed color.", emoji: new DiscordComponentEmoji("🎨")),
                    new DiscordSelectComponentOption("Edit Roles To Ping", Identity.SELECTION_PINGROLE, "Edit roles to ping on message sent.", emoji: new DiscordComponentEmoji("🔔")),
                    new DiscordSelectComponentOption("Edit Plain Message", Identity.SELECTION_CONTENT, "Edit plain text to the message.", emoji: new DiscordComponentEmoji("💭")),
                    new DiscordSelectComponentOption("Toggle Timestamp", Identity.SELECTION_TIMESTAMP, "Toggle embed timestamp.", emoji: new DiscordComponentEmoji("🕙")),
                    new DiscordSelectComponentOption("Add Field Message", Identity.SELECTION_FIELD_ADD, "Add field message.", emoji: new DiscordComponentEmoji("📕")),
                    new DiscordSelectComponentOption("Remove Field Message", Identity.SELECTION_FIELD_REMOVE, "Remove field message.", emoji: new DiscordComponentEmoji("❌")),
                    new DiscordSelectComponentOption("Save To Templates", Identity.SELECTION_TEMPLATE_ADD, "Save this message to your templates.", emoji: new DiscordComponentEmoji("📂")),
                    new DiscordSelectComponentOption("Use From Templates", Identity.SELECTION_TEMPLATE_USE, "Change this message using your templates.", emoji: new DiscordComponentEmoji("📑")),
                    new DiscordSelectComponentOption("List Of Templates", Identity.SELECTION_TEMPLATE_LIST, "Have an overview of available templates.", emoji: new DiscordComponentEmoji("📰")),
                    new DiscordSelectComponentOption("Remove A Template", Identity.SELECTION_TEMPLATE_REMOVE, "Remove a template from your templates.", emoji: new DiscordComponentEmoji("❌"))};

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.SELECTION_EMBED, "Select message builder components to edit", selectOptions)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents)};

            return results;
        }

        public List<DiscordActionRowComponent> DefaultButtonComponent(DiscordChannel channel) {

            // Define button components with corresponding actions
            var buttonComponent = new List<DiscordComponent> {
                    new DiscordButtonComponent(ButtonStyle.Success, "embedButtonChannel", $"Send to {channel.Name}"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "embedButtonTemplates", $"Templates"),
                    new DiscordButtonComponent(ButtonStyle.Primary, "embedButtonUpdate", $"Update"),
                    new DiscordButtonComponent(ButtonStyle.Danger, "embedButtonCancel", "Cancel")};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(buttonComponent)};

            return results;
        }

        public List<DiscordActionRowComponent> PlaceholderComponent(Message message) {

            var selectOptions = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Edit name", Identity.SELECTION_PLACEHOLDER_ID, "Edit the name of this message.", emoji: new DiscordComponentEmoji("🏷️")),
                    new DiscordSelectComponentOption("Edit time", Identity.SELECTION_PLACEHOLDER_TIME, "Edit the time of this message.", emoji: new DiscordComponentEmoji("🕙")),
                    new DiscordSelectComponentOption("Edit texts", Identity.SELECTION_PLACEHOLDER_TEXTS, "Edit the texts of this message.", emoji: new DiscordComponentEmoji("💬")),
                    new DiscordSelectComponentOption("Edit urls", Identity.SELECTION_PLACEHOLDER_URLS, "Edit the urls of this message.", emoji: new DiscordComponentEmoji("🔗"))};

            // Split into nested dictionary
            var nestedData = new Dictionary<string, List<string>>();
            List<string> emojis = new() { "🟥", "🟧", "🟨", "🟩", "🟦", "🟪", "⬜", "⬛", "🟫" };

            foreach (var kvp in message.Data) {
                if (kvp.Key.StartsWith("data.custom")) {
                    // Split the key into segments
                    var segments = kvp.Key.Split('.');
                    if (!nestedData.TryAdd(segments[2], new() { segments[3] })) {
                        nestedData[segments[2]].Add(segments[3]);
                    }
                }
            }

            var availableEmojis = new List<string>(emojis);
            int index = 0;

            foreach (var item in nestedData) {
                string property = item.Key;

                // Pick a random emoji from the available ones
                string selectedEmoji = availableEmojis[index];
                index++;

                selectOptions.Add(new DiscordSelectComponentOption(
                    $"Edit {property}",
                    $"{Identity.SELECTION_PLACEHOLDER_CUSTOM}.{property}",
                    $"Edit the {property} of this message.",
                    emoji: new DiscordComponentEmoji(selectedEmoji)
                ));
            }

            selectOptions.Add(new DiscordSelectComponentOption("Placeholder add", Identity.SELECTION_PLACEHOLDER_ADD, "Add a new custom placeholder.", emoji: new DiscordComponentEmoji("➕")));

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.SELECTION_PLACEHOLDER, "Select placeholder components to edit", selectOptions)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents)};

            return results;
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
            await CreateMessageAsync(CommandEnum.NONE, interaction, message, true);
        }
    }
}