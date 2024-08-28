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


        protected async Task CreateEmbedMessageAsync(InteractionContext ctx, EmbedBuilder embedBuilder, EmbedType type, ulong channelId, bool hidden) {
            try {
                // Get discord channel through channel id
                Stopwatch stopwatch = Stopwatch.StartNew();
                DiscordChannel channel = await DiscordUtil.GetChannelByIdAsync(ctx.Guild, channelId);

                // List of components
                List<DiscordActionRowComponent> components = new();

                // Define select menu options for editing the embed
                var selectOptionDefault = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Edit title", Identity.SELECTION_TITLE, "Edit your embed title & url.", emoji: new DiscordComponentEmoji("✏️")),
                    new DiscordSelectComponentOption("Edit description", Identity.SELECTION_DESCRIPTION, "Edit your embed description.", emoji: new DiscordComponentEmoji("📄")),
                    new DiscordSelectComponentOption("Edit footer", Identity.SELECTION_FOOTER, "Edit your embed footer & image.", emoji: new DiscordComponentEmoji("🧩")),
                    new DiscordSelectComponentOption("Edit author", Identity.SELECTION_AUTHOR, "Edit your embed author text, link & url.", emoji: new DiscordComponentEmoji("👤")),
                    new DiscordSelectComponentOption("Edit main image", Identity.SELECTION_IMAGE, "Edit your embed image.", emoji: new DiscordComponentEmoji("🪪")),
                    new DiscordSelectComponentOption("Edit thumbnail image", Identity.SELECTION_THUMBNAIL, "Edit your embed tumbnail.", emoji: new DiscordComponentEmoji("🖼")),
                    new DiscordSelectComponentOption("Edit color", Identity.SELECTION_COLOR, "Edit your embed color.", emoji: new DiscordComponentEmoji("🎨")),
                    new DiscordSelectComponentOption("Edit roles to ping", Identity.SELECTION_PINGROLE, "Add roles to ping on message sent.", emoji: new DiscordComponentEmoji("🔔")),
                    new DiscordSelectComponentOption("Toggle timestamp", Identity.SELECTION_TIMESTAMP, "Toggle embed timestamp.", emoji: new DiscordComponentEmoji("🕙")),
                    new DiscordSelectComponentOption("Add field message", Identity.SELECTION_FIELD_ADD, "Add field message.", emoji: new DiscordComponentEmoji("📕")),
                    new DiscordSelectComponentOption("Remove field message", Identity.SELECTION_FIELD_REMOVE, "Remove field message.", emoji: new DiscordComponentEmoji("❌"))};

                var selectOptionsTemplate = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Use from template", Identity.SELECTION_TEMPLATE_USE, "Choose an existing template.", emoji: new DiscordComponentEmoji("🗂")),
                    new DiscordSelectComponentOption("Save to template", Identity.SELECTION_TEMPLATE_ADD, "Save this embed to be a template.", emoji: new DiscordComponentEmoji("🗃️"))};

                List<DiscordComponent> selectComponentDefault = new() {
                new DiscordSelectComponent("embedSelect", "Select which module you want to edit...", selectOptionDefault)};
                List<DiscordComponent> selectComponentTemplate = new() {
                new DiscordSelectComponent("embedSelectTemplate", "Select from a template or save template...", selectOptionsTemplate)};

                switch (type) {
                    case EmbedType.DEFAULT:
                        components.Add(new DiscordActionRowComponent(selectComponentDefault));
                        components.Add(new DiscordActionRowComponent(selectComponentTemplate));
                        break;
                    case EmbedType.EVENT:
                        selectOptionDefault.Insert(0, new DiscordSelectComponentOption("Change Timestamp", Identity.SELECTION_TIMESTAMP_CHANGE, "Change timestamp of the event.", emoji: new DiscordComponentEmoji("⏰")));
                        components.Add(new DiscordActionRowComponent(selectComponentDefault));
                        components.Add(new DiscordActionRowComponent(selectComponentTemplate));
                        break;
                    case EmbedType.BROADCAST:
                        break;
                    default:
                        break;
                }

                // Define button components with corresponding actions
                List<DiscordComponent> buttonComponent = new() {
                    new DiscordButtonComponent(ButtonStyle.Success, "embedButtonChannel", $"Send to {channel.Name}"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "embedButtonCurrent", $"Send here"),
                    new DiscordButtonComponent(ButtonStyle.Primary, "embedButtonUpdate", $"Update"),
                    new DiscordButtonComponent(ButtonStyle.Danger, "embedButtonCancel", "Cancel", hidden)};
                components.Add(new DiscordActionRowComponent(buttonComponent));

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
                    response.WithContent(content);
                }

                // Send the response
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
                var message = await ctx.GetOriginalResponseAsync().ConfigureAwait(false);

                // Store the message ID and components for future reference
                embedBuilder.WithMessageId(message.Id);

                // Store the embed data
                await CacheData.AddEmbedAsync(ctx.Guild.Id, message.Id, embedBuilder);

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

        protected virtual async Task<bool> CheckPermission(InteractionContext ctx, CommandEnum cmd) {

            var userId = ctx.User.Id;
            var user = await ctx.Guild.GetMemberAsync(userId);
            var permission = await CacheData.GetPermissionAsync(ctx.Guild.Id, cmd);
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
    }
}
