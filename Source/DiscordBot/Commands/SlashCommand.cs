using DiscordBot.Exceptions;
using DiscordBot.Model;
using DiscordBot;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Model.Enums;
using DiscordBot.Utils;
using XironiteDiscordBot.Exceptions;
using System.Diagnostics;
using System.Threading.Channels;

namespace XironiteDiscordBot.Commands {
    public abstract class SlashCommand : ApplicationCommandModule {

        private string folder = Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory)!.FullName)!.FullName)!.FullName;

        protected async Task CreateEmbedMessageAsync(InteractionContext ctx, EmbedBuilder embedBuilder, EmbedType type, ulong channelId, bool hidden) {
            try {
                // Get discord channel through channel id
                Stopwatch stopwatch = Stopwatch.StartNew();
                DiscordChannel channel = await DiscordUtil.GetChannelByIdAsync(ctx.Guild, channelId);

                // List of components
                List<DiscordActionRowComponent> components = new();



                // Define button components with corresponding actions
                var buttonComponent = new List<DiscordComponent> {
                    new DiscordButtonComponent(ButtonStyle.Success, "embedButtonChannel", $"Send to {channel.Name}"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "embedButtonCurrent", $"Send here"),
                    new DiscordButtonComponent(ButtonStyle.Primary, "embedButtonUpdate", $"Update"),
                    new DiscordButtonComponent(ButtonStyle.Danger, "embedButtonCancel", "Cancel", hidden)};

                switch (type) {

                    case EmbedType.DEFAULT:
                        components.Add(DefaultComponent(embedBuilder).First());
                        components.Add(TemplateComponent(embedBuilder).First());
                        components.Add(new DiscordActionRowComponent(buttonComponent));
                        break;

                    case EmbedType.EVENT:
                        components.Add(EventComponent(embedBuilder).First());
                        components.Add(DefaultComponent(embedBuilder).First());
                        components.Add(TemplateComponent(embedBuilder).First());
                        components.Add(new DiscordActionRowComponent(buttonComponent));
                        break;

                    case EmbedType.BROADCAST:
                        break;

                    case EmbedType.PERMISSION:
                        foreach (var item in PermissionComponent(embedBuilder)) {
                            components.Add(item);
                        }
                        break;

                    case EmbedType.CHANGELOG:
                        foreach (var item in ChangelogComponent(embedBuilder)) {
                            components.Add(item);
                        }
                        break;

                    default:
                        break;
                }

                // Build the embed and response
                var embed = embedBuilder.Build();
                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .AsEphemeral(hidden)
                    .AddComponents(components.ToList());

                // Check for any roles to mention
                if (embedBuilder.PingRoles.Any()) {
                    var tasks = embedBuilder.PingRoles.Select(async x => await DiscordUtil.GetRolesByIdAsync(ctx.Guild, x));
                    var roles = await Task.WhenAll(tasks);
                    var content = string.Empty;
                    foreach (var role in roles) if (role.Name == "@everyone") content += "@everyone"; else content += role.Mention;
                    response.WithContent(content + embedBuilder.Content);
                }

                // Send the response
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
                var message = await ctx.GetOriginalResponseAsync().ConfigureAwait(false);

                // Store the message ID and components for future reference
                embedBuilder.WithMessageId(message.Id);

                // Store the embed data
                await CacheData.AddEmbed(ctx.Guild.Id, message.Id, embedBuilder);

                // Stop the stopwatch and log the elapsed time
                stopwatch.Stop();
                Console.WriteLine($"Embed creation took {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine(string.Join(", ", CacheData.Embeds[ctx.Guild.Id]));

            } catch (Exception ex) {
                // Improved error handling with contextual information
                Console.Error.WriteLine($"Error in CreateEmbedAsync: {ex}");
                throw new CommandException($"Embed.UseEmbedCommand: {ex.Message}", ex);
            }
        }

        private async Task<object> CheckIfLocalImage(string? imagePath) {
            if (imagePath != null) {
                using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read)) {
                    return (Path.GetFileName(imagePath), fs);
                }
            } else throw new CommandException($"test");
        }

        protected virtual async Task<bool> CheckPermission(InteractionContext ctx, CommandEnum cmd) {

            var userId = ctx.User.Id;
            var user = await ctx.Guild.GetMemberAsync(userId);
            var permission = await CacheData.GetPermission(ctx.Guild.Id, cmd);
            var roles = user.Roles;

            if (permission.AllowEveryone) {
                return true;
            } else if (user.Permissions.HasPermission(Permissions.All)) {
                return true;
            } else if (permission.AllowAdministrator) {
                return true;
            } else if (permission.AllowedUsers.Any(x => x.Value == user)) {
                return true;
            } else if (permission.AllowedRoles.Values.ToList().Intersect(roles).Any()) {
                return true;
            } else {
                return false;
            }
        }

        protected virtual async Task showNoPermissionMessage(InteractionContext ctx) {
            var embed = new DiscordEmbedBuilder();
            embed.WithAuthor("You don't have permission!", null, ctx.User.AvatarUrl);
            embed.WithColor(new DiscordColor("#e83b3b"));
            embed.Build();
            var ephemeral = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AsEphemeral(true);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, ephemeral);
        }

        protected virtual async void LogCommand(InteractionContext ctx, CommandEnum command) {
            try {
                var cmd = command.ToString().ToLower().Replace("_", "-");
                var embedBuilder = new EmbedBuilder() {
                    Author = ctx.User.Username,
                    AuthorLink = ctx.User.AvatarUrl,
                    AuthorUrl = ctx.User.AvatarUrl,
                    Description = $"**Command**\nUsed **`/{cmd}`**",
                    Footer = $"User: {ctx.User.Id}",
                    HasTimeStamp = true,
                };
                var embed = embedBuilder.Build();
                var ephemeral = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed);
                var channel = await DiscordUtil.GetChannelByIdAsync(ctx.Guild, 846046462139564083);
                await channel.SendMessageAsync(embed);
            } catch (UtilException ex) {
                Console.WriteLine(ex);
            } catch (Exception ex) {
                throw;
            }

        }

        private List<DiscordActionRowComponent> TemplateComponent(EmbedBuilder embed) {

            var selectOptions = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Use from template", Identity.SELECTION_TEMPLATE_USE, "Choose an existing template.", emoji: new DiscordComponentEmoji("🗂")),
                    new DiscordSelectComponentOption("Save to template", Identity.SELECTION_TEMPLATE_ADD, "Save this embed to be a template.", emoji: new DiscordComponentEmoji("🗃️"))};

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.COMPONENT_TEMPLATE, "Select from a template or save template...", selectOptions)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents)};

            return results;
        }

        private List<DiscordActionRowComponent> DefaultComponent(EmbedBuilder embed) {

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
                new DiscordSelectComponent(Identity.COMPONENT_SELECT, "Select which module you want to edit...", selectOptions)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents)};

            return results;
        }

        private List<DiscordActionRowComponent> PermissionComponent(EmbedBuilder embed) {

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

        private List<DiscordActionRowComponent> ChangelogComponent(EmbedBuilder embed) {

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

        private List<DiscordActionRowComponent> EventComponent(EmbedBuilder embed) {

            var selectEventComponents = new List<DiscordSelectComponentOption>() {
                new DiscordSelectComponentOption("Change title", Identity.SELECTION_EVENT_TITLE, "Change title/name of the event.", emoji: new DiscordComponentEmoji("🔶")),
                new DiscordSelectComponentOption("Change intro", Identity.SELECTION_EVENT_INTRO, "Change intro message of the event.", emoji: new DiscordComponentEmoji("📄")),
                new DiscordSelectComponentOption("Change game info", Identity.SELECTION_EVENT_INFO, "Change game information of the event.", emoji: new DiscordComponentEmoji("ℹ️")),
                new DiscordSelectComponentOption("Change top rewards", Identity.SELECTION_EVENT_TOPREWARDS, "Change top rewards of the event.", emoji: new DiscordComponentEmoji("🏆")),
                new DiscordSelectComponentOption("Change timestamp", Identity.SELECTION_EVENT_TIMESTAMP, "Change timestamp of the event.", emoji: new DiscordComponentEmoji("⏰"))
            };

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.COMPONENT_EVENT, "Select an event component you want to edit...", selectEventComponents)};

            List<DiscordActionRowComponent> results = new() {
                new DiscordActionRowComponent(selectComponents)};

            return results;
        }
    }
}
