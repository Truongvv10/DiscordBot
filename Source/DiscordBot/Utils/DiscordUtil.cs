using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Exceptions;
using System.Data;
using DiscordBot.Model.Enums;
using NodaTime;
using System.Threading.Channels;
using DSharpPlus.SlashCommands;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Microsoft.VisualBasic;

namespace DiscordBot.Utils {
    public static class DiscordUtil {

        public static async Task CreateMessageAsync(CommandEnum type, DiscordInteraction interaction, EmbedBuilder embedBuilder, ulong channelId, bool hidden) {
            try {
                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Create the response
                var response = await CreateResponseAsync(type, interaction, embedBuilder, channelId, hidden);
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
                var message = await interaction.GetOriginalResponseAsync().ConfigureAwait(false);

                // Stop the stopwatch and log the elapsed time
                stopwatch.Stop();

                // Store the message ID and components for future reference
                embedBuilder.WithMessageId(message.Id);

                // Store the embed data
                await CacheData.AddEmbed(interaction.Guild.Id, message.Id, embedBuilder);

                // Logger
                Console.WriteLine(
                    $"{AnsiColor.RESET}[{DateTime.Now}] " +
                    $"{AnsiColor.BRIGHT_GREEN}-> Message creation took {AnsiColor.YELLOW}{stopwatch.ElapsedMilliseconds}ms " +
                    $"{AnsiColor.RESET}(message: {message.Id})");

            } catch (Exception ex) {
                throw new CommandException($"Embed.CreateMessageAsync: {ex.Message}", ex);
            }
        }

        public static async Task UpdateMessageAsync(CommandEnum type, DiscordInteraction interaction, EmbedBuilder embedBuilder, ulong channelId, bool hidden) {
            try {
                // Start the stopwatch
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Create the response
                var response = await CreateResponseAsync(type, interaction, embedBuilder, channelId, hidden);
                await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, response);
                var message = await interaction.GetOriginalResponseAsync().ConfigureAwait(false);

                // Stop the stopwatch and log the elapsed time
                stopwatch.Stop();

                // Logger
                Console.WriteLine(
                    $"{AnsiColor.RESET}[{DateTime.Now}] " +
                    $"{AnsiColor.BRIGHT_GREEN}-> Message edited took {AnsiColor.YELLOW}{stopwatch.ElapsedMilliseconds}ms " +
                    $"{AnsiColor.RESET}(message: {message.Id})");

            } catch (Exception ex) {
                throw new CommandException($"Embed.UpdateMessageAsync: {ex.Message}", ex);
            }
        }

        private static async Task<DiscordInteractionResponseBuilder> CreateResponseAsync(CommandEnum type, DiscordInteraction interaction, EmbedBuilder embedBuilder, ulong channelId, bool hidden) {
            try {
                // Get discord channel through channel id
                Stopwatch stopwatch = Stopwatch.StartNew();
                DiscordChannel channel = await GetChannelByIdAsync(interaction.Guild, channelId);

                // List of components
                List<DiscordActionRowComponent> components = new();

                // Define button components with corresponding actions
                var buttonComponent = new List<DiscordComponent> {
                    new DiscordButtonComponent(ButtonStyle.Success, "embedButtonChannel", $"Send to {channel.Name}"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "embedButtonCurrent", $"Send here"),
                    new DiscordButtonComponent(ButtonStyle.Primary, "embedButtonUpdate", $"Update"),
                    new DiscordButtonComponent(ButtonStyle.Danger, "embedButtonCancel", "Cancel", hidden)};

                switch (type) {

                    case CommandEnum.EMBED_CREATE:
                        components.Add(DefaultComponent().First());
                        components.Add(TemplateComponent().First());
                        components.Add(new DiscordActionRowComponent(buttonComponent));
                        break;

                    case CommandEnum.EVENTS_CREATE:
                        var buttonComponentEventPost = new List<DiscordComponent> {
                            new DiscordButtonComponent(ButtonStyle.Success, "embedButtonEventPostCreate", "Create"),
                            new DiscordButtonComponent(ButtonStyle.Danger, "embedButtonCancel", "Cancel")};
                        components.Add(new DiscordActionRowComponent(buttonComponentEventPost));
                        break;

                    case CommandEnum.EVENTS_EDIT:
                        components.Add(EventComponent().First());
                        components.Add(DefaultComponent().First());
                        components.Add(TemplateComponent().First());
                        components.Add(new DiscordActionRowComponent(buttonComponent));
                        break;

                    case CommandEnum.BROADCAST:
                        foreach (var item in PermissionComponent(embedBuilder)) {
                            components.Add(item);
                        }
                        break;

                    case CommandEnum.CHANGELOG:
                        foreach (var item in ChangelogComponent(embedBuilder)) {
                            components.Add(item);
                        }
                        break;

                    default:
                        break;
                }

                // Build the embed and response
                var response = ResolveImageAttachment(embedBuilder)
                    .AsEphemeral(hidden)
                    .AddComponents(components.ToList());

                // Check for any roles to mention
                if (embedBuilder.PingRoles.Any()) {
                    var tasks = embedBuilder.PingRoles.Select(async x => await GetRolesByIdAsync(interaction.Guild, x));
                    var roles = await Task.WhenAll(tasks);
                    var content = string.Empty;
                    foreach (var role in roles) if (role.Name == "@everyone") content += "@everyone"; else content += role.Mention;
                    response.WithContent(content + embedBuilder.Content);
                }

                // Return the response
                return response;

            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw new CommandException($"Embed.CreateResponseAsync: {ex.Message}", ex);
            }
        }

        private static DiscordInteractionResponseBuilder ResolveImageAttachment(EmbedBuilder embed) {
            var response = new DiscordInteractionResponseBuilder();
            var folder = Path.Combine(Environment.CurrentDirectory, "Saves", "Images");
            var pattern = @"^local:\/\/.*";
            var replace = "local://";

            try {
                if (embed.Image != null && Regex.IsMatch(embed.Image, pattern)) {
                    var image = Path.Combine(folder, embed.Image.Replace(replace, ""));
                    var imageBytes = File.ReadAllBytes(image);
                    var imageStream = new MemoryStream(imageBytes);
                    var fileName = Path.GetFileName(image);
                    embed.WithImage($"attachment://{fileName}");
                    response.AddFile(fileName, imageStream);
                }

                if (embed.Thumbnail != null && Regex.IsMatch(embed.Thumbnail, pattern)) {
                    var thumbnail = Path.Combine(folder, embed.Thumbnail.Replace(replace, ""));
                    var thumbnailBytes = File.ReadAllBytes(thumbnail);
                    var thumbnailStream = new MemoryStream(thumbnailBytes);
                    var fileName = Path.GetFileName(thumbnail);
                    embed.WithThumbnail($"attachment://{fileName}");
                    response.AddFile(fileName, thumbnailStream);
                }

                return response.AddEmbed(embed.Build());

            } catch (Exception ex) {
                throw new CommandException($"Embed.CheckIfLocalImage: {ex.Message}", ex);
            }
        }

        private static string BuildEventDesciption(EmbedBuilder embed) {

            string name = (string)embed.CustomSaves[Identity.EVENT_NAME];
            string title = (string)embed.CustomSaves[Identity.EVENT_TITLE];
            string intro = (string)embed.CustomSaves[Identity.EVENT_INTRO];
            string infoTitle = (string)embed.CustomSaves[Identity.EVENT_INFO_TITLE];
            string info = (string)embed.CustomSaves[Identity.EVENT_INFO];
            string rewardTitle = (string)embed.CustomSaves[Identity.EVENT_REWARD_TITLE];
            string reward = (string)embed.CustomSaves[Identity.EVENT_REWARD];
            string timeTitle = (string)embed.CustomSaves[Identity.EVENT_TIME_TITLE];

            return
                title + "\n" +
                intro + "\n" +
                infoTitle + "\n" +
                info + "\n" +
                rewardTitle + "\n" +
                reward + "\n" +
                timeTitle + "\n";

        }

        public static List<DiscordActionRowComponent> DefaultComponent() {

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
                    new DiscordSelectComponentOption("Remove field message", Identity.SELECTION_FIELD_REMOVE, "Remove field message.", emoji: new DiscordComponentEmoji("❌"))};

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.COMPONENT_SELECT, "Select default embed component", selectOptions)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents)};

            return results;
        }

        public static List<DiscordActionRowComponent> DefaultButtonComponent(DiscordChannel channel) {

            // Define button components with corresponding actions
            var buttonComponent = new List<DiscordComponent> {
                    new DiscordButtonComponent(ButtonStyle.Success, "embedButtonChannel", $"Send to {channel.Name}"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "embedButtonCurrent", $"Send here"),
                    new DiscordButtonComponent(ButtonStyle.Primary, "embedButtonUpdate", $"Update"),
                    new DiscordButtonComponent(ButtonStyle.Danger, "embedButtonCancel", "Cancel")};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(buttonComponent)};

            return results;
        }

        public static List<DiscordActionRowComponent> TemplateComponent() {

            var selectOptions = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Use from template", Identity.SELECTION_TEMPLATE_USE, "Choose an existing template.", emoji: new DiscordComponentEmoji("🗂")),
                    new DiscordSelectComponentOption("Save to template", Identity.SELECTION_TEMPLATE_ADD, "Save this embed to be a template.", emoji: new DiscordComponentEmoji("🗃️"))};

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.COMPONENT_TEMPLATE, "Choose from template options", selectOptions)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents)};

            return results;
        }

        public static List<DiscordActionRowComponent> PermissionComponent(EmbedBuilder embed) {

            var selectPermissionOption = new List<DiscordSelectComponentOption>();
            var buttonEditOption = new List<DiscordActionRowComponent>();
            var hideButtons = embed.CustomSaves.ContainsKey(Identity.SELECTION_PERMS);
            var commandPermissions = Enum.GetValues(typeof(CommandEnum)).Cast<CommandEnum>().ToList();

            foreach (var cmd in commandPermissions) {
                string title = cmd.ToString().ToUpper();
                string id = $"{Identity.SELECTION_PERMS}.{cmd.ToString().ToLower()}";
                string desc = $"Edit permissions for command /{cmd.ToString().ToLower()}";
                selectPermissionOption.Add(new DiscordSelectComponentOption(title, id, desc));
            }

            List<DiscordComponent> buttonComponents = new() {
                new DiscordButtonComponent(ButtonStyle.Success, Identity.BUTTON_PERMISSION_USERS, "Users", !hideButtons),
                new DiscordButtonComponent(ButtonStyle.Success, Identity.BUTTON_PERMISSION_ROLES, "Roles", !hideButtons),
                new DiscordButtonComponent(ButtonStyle.Success, Identity.BUTTON_PERMISSION_CHANNELS, "Channels", !hideButtons),
                new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_PERMISSION_SETTINGS, "Settings", !hideButtons),
                new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_PERMISSION_RESET, "Reset", !hideButtons)};

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.SELECTION_PERMS, "Choose which permission you want to edit...", selectPermissionOption)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents),
                new DiscordActionRowComponent(buttonComponents)};

            return results;
        }

        public static List<DiscordActionRowComponent> ChangelogComponent(EmbedBuilder embed) {

            var selectPermissionOption = new List<DiscordSelectComponentOption>();
            var buttonEditOption = new List<DiscordActionRowComponent>();
            var hideButtons = embed.CustomSaves.ContainsKey(Identity.SELECTION_PERMS);
            var commandPermissions = Enum.GetValues(typeof(CommandEnum)).Cast<CommandEnum>().ToList();

            foreach (var cmd in commandPermissions) {
                string title = cmd.ToString().ToUpper();
                string id = $"{Identity.SELECTION_PERMS}.{cmd.ToString().ToLower()}";
                string desc = $"Edit permissions for command /{cmd.ToString().ToLower()}";
                selectPermissionOption.Add(new DiscordSelectComponentOption(title, id, desc));
            }

            List<DiscordComponent> buttonComponents = new() {
                new DiscordButtonComponent(ButtonStyle.Success, Identity.BUTTON_CHANGELOG_CREATE, "Create"),
                new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_CHANGELOG_REMOVE, "Delete"),
                new DiscordButtonComponent(ButtonStyle.Secondary, Identity.BUTTON_CANCEL, "Cancel")};

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.SELECTION_PERMS, "Select from existing changelogs...", selectPermissionOption)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents),
                new DiscordActionRowComponent(buttonComponents)};

            return results;
        }

        public static List<DiscordActionRowComponent> EventComponent() {

            var selectEventComponents = new List<DiscordSelectComponentOption>() {
                new DiscordSelectComponentOption("Introduction", Identity.SELECTION_EVENT_INTRODUCTION, "Edit introduction of the event.", emoji: new DiscordComponentEmoji("🔶")),
                new DiscordSelectComponentOption("Information", Identity.SELECTION_EVENT_INFORMATION, "Edit information of the event.", emoji: new DiscordComponentEmoji("ℹ️")),
                new DiscordSelectComponentOption("Top rewards", Identity.SELECTION_EVENT_REWARDS, "Edit top rewards of the event.", emoji: new DiscordComponentEmoji("🏆")),
                new DiscordSelectComponentOption("Timestamps", Identity.SELECTION_EVENT_TIMESTAMP, "Edit timestamp of the event.", emoji: new DiscordComponentEmoji("⏰"))
            };

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.COMPONENT_EVENT, "Select event specific component", selectEventComponents)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents)};

            return results;
        }

        public static async Task SendImageEmbedAsync(DiscordInteraction interaction, string imagePath, string title = "Here is your image!", string description = "This image was uploaded as an attachment.") {
            try {
                // Check if the file exists
                if (!File.Exists(imagePath)) {
                    return;
                }

                Console.WriteLine(imagePath);
                var message = new DiscordInteractionResponseBuilder();

                // Read the image as a file stream
                using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read)) {
                    // Create the embed message
                    message
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle(title)
                            .WithDescription(description)
                            .WithImageUrl($"attachment://{Path.GetFileName(imagePath)}")) // Link to the attached image
                        .AddFile(Path.GetFileName(imagePath), fs); // Attach the image with its filename

                    // Send the embed with the image attached
                    await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);
                    Console.WriteLine($"attachment://{Path.GetFileName(imagePath)}");

                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        public static async Task<DiscordMessage?> GetMessageByIdAsync(DiscordChannel channel, ulong messageId) {
            try {
                return await channel.GetMessageAsync(messageId); ;
            } catch (Exception) {
                return null;
            }
        }

        public static async Task<DiscordRole> GetRolesByIdAsync(DiscordGuild guild, ulong roleId) {
            if (guild.Roles.TryGetValue(roleId, out var role)) {
                var test = role;
                return await Task.FromResult(role);
            } else throw new UtilException($"No roles with id \"{roleId}\" found.");
        }

        public static async Task<DiscordChannel> GetChannelByIdAsync(DiscordGuild guild, ulong channelId) {
            if (guild.Channels.TryGetValue(channelId, out var channel)) {
                return await Task.FromResult(channel);
            } else throw new UtilException($"No channel with id \"{channelId}\" found.");
        }

        public static async Task<string> TranslateToDynamicTimestamp(DateTime localDateTime, string timeZoneId, TimestampEnum timestampType) {
            try {
                // Map TimestampEnum to Discord format characters
                char timestampTypeChar = timestampType switch {
                    TimestampEnum.SHORT_TIME => 't',
                    TimestampEnum.LONG_TIME => 'T',
                    TimestampEnum.SHORT_DATE => 'd',
                    TimestampEnum.LONG_DATE => 'D',
                    TimestampEnum.LONG_DATE_AND_SHORT_TIME => 'f',
                    TimestampEnum.LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME => 'F',
                    TimestampEnum.RELATIVE => 'R',
                    _ => 'R',
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
                throw new Exception($"Could not translate {localDateTime} to Discord timestamp.", ex);
            }
        }
    }
}