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

namespace APP.Utils {
    public class DiscordUtil {

        #region Fields
        private IDataService dataService;
        #endregion

        #region Constructor
        public DiscordUtil(IDataService dataService) {
            this.dataService = dataService;
        }
        #endregion

        public async Task CreateMessageAsync(CommandEnum type, DiscordInteraction interaction, Message message, ulong channelId, bool hidden = false) {
            try {
                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders();

                // Create the response
                var response = await CreateResponseAsync(type, interaction, translated, channelId, hidden);
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
                var original = await interaction.GetOriginalResponseAsync().ConfigureAwait(true);

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
                    $"{AnsiColor.RESET}(message: {original.Id})");

            } catch (Exception ex) {
                throw new CommandException($"Embed.CreateMessageAsync: {ex.Message}", ex);
            }
        }

        public async Task UpdateMessageAsync(DiscordInteraction interaction, Message message, bool isDeferMessageUpdate = true) {
            try {
                if (message.MessageId != null) {
                    // Start the stopwatch
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    // Translate the placeholders
                    var translated = message.DeepClone();
                    await translated.TranslatePlaceholders();
                    var embed = translated.Embed.Build();

                    // Create the response
                    if (isDeferMessageUpdate) await interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    var response = await CreateResponseAsync(message.Type, interaction, translated, (ulong)message.ChannelId!, message.IsEphemeral);
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
                        $"{AnsiColor.RESET}(message: {message.MessageId})");

                } else throw new UtilException($"Could not create response because message was null");
            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw new UtilException($"Could not create response: {ex.Message}", ex);
            }
        }

        public async Task ModifyMessageAsync(CommandEnum type, DiscordInteraction interaction, Message message, ulong channelId, bool hidden = false) {
            try {
                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders();
                var embed = translated.Embed;

                var response = await CreateResponseAsync(type, interaction, translated, (ulong)message.ChannelId!, message.IsEphemeral);
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
                    $"{AnsiColor.RESET}(message: {message.MessageId})");

            } catch (Exception ex) {
                throw new CommandException($"Embed.UpdateMessageAsync: {ex.Message}", ex);
            }
        }

        private async Task<DiscordInteractionResponseBuilder> CreateResponseAsync(CommandEnum type, DiscordInteraction interaction, Message message, ulong toChannelId, bool hidden = false) {
            try {

                // Get discord channel through channel id
                DiscordChannel channel = await GetChannelByIdAsync(interaction.Guild, toChannelId);

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
                        components.Add(new DiscordActionRowComponent(buttonComponent));
                        break;

                    case CommandEnum.EVENTS_CREATE:
                        var buttonEventPost = new List<DiscordComponent> {
                            new DiscordButtonComponent(ButtonStyle.Success, "embedButtonEventPostCreate", "Create Event"),
                            new DiscordButtonComponent(ButtonStyle.Danger, "embedButtonCancel", "Cancel", hidden)};
                        components.Add(new DiscordActionRowComponent(buttonEventPost));
                        break;

                    case CommandEnum.TEMPLATES:
                        var buttonTemplateUse = new List<DiscordComponent> {
                            new DiscordButtonComponent(ButtonStyle.Success, Identity.BUTTON_TEMPLATES_ADD, "Add Template"),
                            new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_TEMPLATES_SELECT, "Use Template"),
                            new DiscordButtonComponent(ButtonStyle.Secondary, Identity.BUTTON_TEMPLATES_DELETE, "Delete"),
                            new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_TEMPLATES_CANCEL, "Cancel")};
                        components.Add(new DiscordActionRowComponent(buttonTemplateUse));
                        break;

                    case CommandEnum.NITRO:
                        var buttonNitroClaim = new List<DiscordComponent> {
                            new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_NITRO, "Claim Nitro")};
                        components.Add(new DiscordActionRowComponent(buttonNitroClaim));
                        break;

                    case CommandEnum.EVENTS_EDIT:
                        components.Add(EventComponent().First());
                        components.Add(DefaultComponent().First());
                        components.Add(TemplateComponent().First());
                        components.Add(new DiscordActionRowComponent(buttonComponent));
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

        public string BuildEventDesciption(Message message) {
            var embed = message.Embed;
            if (embed is not null) {

                string name = (string)message.Data[Identity.EVENT_NAME];
                string title = (string)message.Data[Identity.EVENT_TITLE];
                string intro = (string)message.Data[Identity.EVENT_INTRO];
                string infoTitle = (string)message.Data[Identity.EVENT_INFO_TITLE];
                string info = (string)message.Data[Identity.EVENT_INFO];
                string rewardTitle = (string)message.Data[Identity.EVENT_REWARD_TITLE];
                string reward = (string)message.Data[Identity.EVENT_REWARD];
                string timeTitle = (string)message.Data[Identity.EVENT_TIME_TITLE];

                return
                    title + "\n" +
                    intro + "\n" +
                    infoTitle + "\n" +
                    info + "\n" +
                    rewardTitle + "\n" +
                    reward + "\n" +
                    timeTitle + "\n";

            } else throw new UtilException("YEYSY EYWAAWRWA");

        }

        public List<DiscordActionRowComponent> DefaultComponent() {

            var selectOptions = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Edit title", Identity.SELECTION_TITLE, "Edit your embed title & url.", emoji: new DiscordComponentEmoji("✏️")),
                    new DiscordSelectComponentOption("Edit description", Identity.SELECTION_DESCRIPTION, "Edit your embed description.", emoji: new DiscordComponentEmoji("📄")),
                    new DiscordSelectComponentOption("Edit footer", Identity.SELECTION_FOOTER, "Edit your embed footer & image.", emoji: new DiscordComponentEmoji("🧩")),
                    new DiscordSelectComponentOption("Edit author", Identity.SELECTION_AUTHOR, "Edit your embed author text, link & url.", emoji: new DiscordComponentEmoji("👤")),
                    new DiscordSelectComponentOption("Edit main image", Identity.SELECTION_IMAGE, "Edit your embed image.", emoji: new DiscordComponentEmoji("🪪")),
                    new DiscordSelectComponentOption("Edit thumbnail image", Identity.SELECTION_THUMBNAIL, "Edit your embed tumbnail.", emoji: new DiscordComponentEmoji("🖼")),
                    new DiscordSelectComponentOption("Edit color", Identity.SELECTION_COLOR, "Edit your embed color.", emoji: new DiscordComponentEmoji("🎨")),
                    new DiscordSelectComponentOption("Edit roles to ping", Identity.SELECTION_PINGROLE, "Edit roles to ping on message sent.", emoji: new DiscordComponentEmoji("🔔")),
                    new DiscordSelectComponentOption("Edit plain text", Identity.SELECTION_CONTENT, "Edit plain text to the message.", emoji: new DiscordComponentEmoji("💭")),
                    new DiscordSelectComponentOption("Toggle timestamp", Identity.SELECTION_TIMESTAMP, "Toggle embed timestamp.", emoji: new DiscordComponentEmoji("🕙")),
                    new DiscordSelectComponentOption("Add field message", Identity.SELECTION_FIELD_ADD, "Add field message.", emoji: new DiscordComponentEmoji("📕")),
                    new DiscordSelectComponentOption("Remove field message", Identity.SELECTION_FIELD_REMOVE, "Remove field message.", emoji: new DiscordComponentEmoji("❌")),
                    new DiscordSelectComponentOption("Save to templates", Identity.SELECTION_TEMPLATE_ADD, "Save this message to your templates.", emoji: new DiscordComponentEmoji("📂")),
                    new DiscordSelectComponentOption("Use from templates", Identity.SELECTION_TEMPLATE_USE, "Change this message using your templates.", emoji: new DiscordComponentEmoji("📑")),
                    new DiscordSelectComponentOption("List of templates", Identity.SELECTION_TEMPLATE_LIST, "Have an overview of available templates.", emoji: new DiscordComponentEmoji("📰")),
                    new DiscordSelectComponentOption("Remove a template", Identity.SELECTION_TEMPLATE_REMOVE, "Remove a template from your templates.", emoji: new DiscordComponentEmoji("❌"))};

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.SELECTION_EMBED, "Select default embed component", selectOptions)};

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

        public List<DiscordActionRowComponent> TemplateComponent() {

            var selectOptions = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Use from template", Identity.SELECTION_TEMPLATE_USE, "Choose an existing template.", emoji: new DiscordComponentEmoji("🗂")),
                    new DiscordSelectComponentOption("Save to template", Identity.SELECTION_TEMPLATE_ADD, "Save this embed to be a template.", emoji: new DiscordComponentEmoji("🗃️")),
                    new DiscordSelectComponentOption("Delete template", Identity.MODAL_DATA_TEMPLATE_REMOVE, "Remove saved templates.", emoji: new DiscordComponentEmoji("❌"))};

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.SELECTION_TEMPLATE, "Choose from template options", selectOptions)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents)};

            return results;
        }

        public List<DiscordActionRowComponent> EventComponent() {

            var selectEventComponents = new List<DiscordSelectComponentOption>() {
                new DiscordSelectComponentOption("Properties", Identity.SELECTION_EVENT_PROPERTIES, "Edit properties of the event.", emoji: new DiscordComponentEmoji("🔶")),
                new DiscordSelectComponentOption("Introduction", Identity.SELECTION_EVENT_INTRODUCTION, "Edit introduction of the event.", emoji: new DiscordComponentEmoji("📑")),
                new DiscordSelectComponentOption("Information", Identity.SELECTION_EVENT_INFORMATION, "Edit information of the event.", emoji: new DiscordComponentEmoji("ℹ️")),
                new DiscordSelectComponentOption("Top rewards", Identity.SELECTION_EVENT_REWARDS, "Edit top rewards of the event.", emoji: new DiscordComponentEmoji("🏆")),
                new DiscordSelectComponentOption("Date Format", Identity.SELECTION_EVENT_TIMESTAMP, "Edit timestamp of the event.", emoji: new DiscordComponentEmoji("📅")),
                new DiscordSelectComponentOption("Reaction Format", Identity.SELECTION_EVENT_REACTION, "Edit reactions of the event.", emoji: new DiscordComponentEmoji("☺️"))
            };

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.SELECTION_EVENT, "Select event specific component", selectEventComponents)};

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

        public Task<bool> ExistTimeZone(string timeZoneId) {
            var test = Task.FromResult(DateTimeZoneProviders.Tzdb.GetZoneOrNull(TranslateCustomTimezone(timeZoneId)) is null ? false : true);
            return test;
        }

        public async Task<string> TranslateToDynamicTimestamp(DateTime localDateTime, string timeZoneId, TimestampEnum timestampType) {
            try {
                // Translate custom timezones to Discord timezones
                timeZoneId = TranslateCustomTimezone(timeZoneId);

                // Map TimestampEnum to Discord format characters
                char timestampTypeChar = timestampType switch {
                    TimestampEnum.SHORT_TIME => 't',
                    TimestampEnum.LONG_TIME => 'T',
                    TimestampEnum.SHORT_DATE => 'd',
                    TimestampEnum.LONG_DATE => 'D',
                    TimestampEnum.LONG_DATE_AND_SHORT_TIME => 'f',
                    TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME => 'F',
                    TimestampEnum.RELATIVE => 'R',
                    _ => 'F',
                };

                // Convert the DateTime to NodaTime LocalDateTime
                LocalDateTime localDateTimeValue = LocalDateTime.FromDateTime(localDateTime);

                // Get the time zone
                DateTimeZone timeZone = DateTimeZoneProviders.Tzdb[timeZoneId];

                // Convert LocalDateTime to ZonedDateTime in the specified time zone
                ZonedDateTime zonedDateTime = timeZone.AtLeniently(localDateTimeValue);

                // Convert ZonedDateTime to Instant
                Instant instantFromZonedDateTime = zonedDateTime.ToInstant();

                // Convert to Unix Timestamp
                long unixTimestamp = instantFromZonedDateTime.ToUnixTimeSeconds();

                // Format the timestamp for Discord
                string discordTimestamp = $"<t:{unixTimestamp}:{timestampTypeChar}>";

                return await Task.FromResult(discordTimestamp);

            } catch (Exception ex) {
                throw new UtilException($"Could not translate {localDateTime} to Discord timestamp.", ex);
            }
        }

        private string TranslateCustomTimezone(string timeZone) {

            var copy = timeZone.ToUpper();

            if (copy == TimeZoneEnum.BST.ToString())
                return "Europe/London";

            if (copy == TimeZoneEnum.MSK.ToString())
                return "Europe/Moscow";

            if (copy == TimeZoneEnum.GST.ToString())
                return "Etc/GMT+4";

            if (copy == TimeZoneEnum.PKT.ToString())
                return "Etc/GMT+5";

            if (copy == TimeZoneEnum.IST.ToString())
                return "Etc/GMT+5";

            if (copy == TimeZoneEnum.BST.ToString())
                return "Etc/GMT+6";

            if (copy == TimeZoneEnum.WIB.ToString())
                return "Etc/GMT+7";

            if (copy == TimeZoneEnum.JST.ToString())
                return "Japan";

            if (copy == TimeZoneEnum.BST.ToString())
                return "Japan";

            if (copy == TimeZoneEnum.AEST.ToString())
                return "Etc/GMT+10";

            if (copy == TimeZoneEnum.NZST.ToString())
                return "Etc/GMT+12";

            if (copy == TimeZoneEnum.HST.ToString())
                return "US/Hawaii";

            if (copy == TimeZoneEnum.AKST.ToString())
                return "US/Alaska";

            if (copy == TimeZoneEnum.PST.ToString())
                return "US/Pacific";

            if (copy == TimeZoneEnum.MST.ToString())
                return "US/Mountain";

            if (copy == TimeZoneEnum.CST.ToString())
                return "US/Central";

            if (copy == TimeZoneEnum.AST.ToString())
                return "Canada/Atlantic";

            return timeZone;
        }
    }
}