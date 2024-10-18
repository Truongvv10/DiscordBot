using DiscordBot.Model.Enums;
using DiscordBot.Utils;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Listeners {
    public class SelectComponentListener {

        public async Task HandleEmbedInteraction(DiscordClient discordClient, ComponentInteractionCreateEventArgs e) {

            var messageId = e.Message.Id;
            var guildId = e.Guild.Id;
            var embed = CacheData.GetEmbed(guildId, messageId);

            try {
                const string text = "Write something...";
                var exampleUrl = @"https://example.com/";
                var options = e.Values;
                var components = e.Message.Components;
                var modal = new DiscordInteractionResponseBuilder();
                var message = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build())
                    .AddComponents(components);

                foreach (var option in options) {
                    switch (option) {
                        case Identity.SELECTION_TITLE:
                            modal.WithTitle($"EDITING TITLE").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TITLE TEXT", Identity.MODAL_TITLE, text, embed.Title, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("TITLE LINK", Identity.MODAL_TITLE_LINK, exampleUrl, embed.TitleUrl, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_DESCRIPTION:
                            modal.WithTitle($"EDITING DESCRIPTION").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent(option, Identity.MODAL_DESCRIPTION, text, embed.Description, false, TextInputStyle.Paragraph));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_FOOTER:
                            modal.WithTitle($"EDITING FOOTER").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("FOOTER TEXT", Identity.MODAL_FOOTER, text, embed.Footer, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("FOOTER IMAGE", Identity.MODAL_FOOTER_URL, exampleUrl, embed.FooterUrl, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_AUTHOR:
                            modal.WithTitle($"EDITING AUTHOR").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("AUTHOR TEXT", Identity.MODAL_AUTHOR, text + " or self", embed.Author, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("AUTHOR LINK", Identity.MODAL_AUTHOR_LINK, exampleUrl, embed.AuthorLink, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("AUTHOR IMAGE", Identity.MODAL_AUTHOR_URL, exampleUrl, embed.AuthorUrl, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_COLOR:
                            modal.WithTitle($"EDITING COLOR").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent(option, Identity.MODAL_COLOR, text, embed.Color, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_IMAGE:
                            modal.WithTitle($"EDITING IMAGE").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent(option, Identity.MODAL_IMAGE, exampleUrl, embed.Image, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_THUMBNAIL:
                            modal.WithTitle($"EDITING THUMBNAIL").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent(option, Identity.MODAL_THUMBNAIL, exampleUrl, embed.Thumbnail, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_PINGROLE:
                            string roleId = "836964332595707955, 1265749062183813242, everyone";
                            modal.WithTitle($"EDITING PINGED ROLES").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent($"ROLES", Identity.MODAL_PINGROLE, roleId, string.Join(", ", embed.PingRoles), true, TextInputStyle.Paragraph));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_TIMESTAMP:
                            embed.HasTimeStamp = !embed.HasTimeStamp;
                            message = new DiscordInteractionResponseBuilder()
                                .AddEmbed(embed.Build())
                                .AddComponents(components);
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, message);
                            await JsonData.SaveEmbedsAsync(guildId);
                            break;
                        case Identity.SELECTION_FIELD_ADD:
                            modal.WithTitle($"ADDING FIELD TEXT").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TITLE", Identity.MODAL_FIELD_TITLE, text, null, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("TEXT", Identity.MODAL_FIELD_TEXT, text, null, false, TextInputStyle.Paragraph));
                            modal.AddComponents(new TextInputComponent("INLINE", Identity.MODAL_FIELD_INLINE, "True or False", "True", false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_FIELD_REMOVE:
                            if (embed.Fields.Count() != 0) {
                                modal.WithTitle($"REMOVING FIELD").WithCustomId($"embedModal;{option};{messageId}");
                                modal.AddComponents(new TextInputComponent("INDEX", Identity.MODAL_FIELD_INDEX, $"Number from 0 to {embed.Fields.Count() - 1}", null, true, TextInputStyle.Short, 1, 2));
                                await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            }
                            break;
                        case Identity.SELECTION_TEMPLATE_ADD:
                            modal.WithTitle($"SAVE TEMPLATE").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("SAVE TEMPLATE", Identity.MODAL_TEMPLATE_ADD, text, null, true, TextInputStyle.Short, 3, 24));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_EVENT_TIMESTAMP:
                            modal.WithTitle($"CHANGE EVENT TIMESTAMP").WithCustomId($"embedModal;{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TIME ZONE", Identity.EVENT_TIMEZONE, "Europe/Brussels", embed.CustomSaves[Identity.EVENT_TIMEZONE] as string, true, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("START DATE", Identity.EVENT_START, "DD/MM/YYYY hh:mm", embed.CustomSaves[Identity.EVENT_START] as string, true, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("END DATE", Identity.EVENT_END, "DD/MM/YYYY hh:mm", embed.CustomSaves[Identity.EVENT_END] as string, true, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        default:
                            // Build the embed for feature unavailability
                            var embedMessage = new DiscordEmbedBuilder()
                                .WithAuthor("Feature doesn't work yet!", null, "https://cdn-icons-png.flaticon.com/512/2581/2581801.png")
                                .WithColor(new DiscordColor("#d82b40"));

                            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .AddEmbed(embedMessage)
                                .AsEphemeral(true));

                            break;

                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("Select");
                throw;
			}
        }

        private async Task<bool> CheckPermission(ComponentInteractionCreateEventArgs e, CommandEnum cmd, ulong ownerid) {

            var permission = await CacheData.GetPermission(e.Guild.Id, cmd);
            var userid = e.User.Id;
            var user = await e.Guild.GetMemberAsync(userid);
            var roles = user.Roles;

            if (user.Permissions.HasPermission(Permissions.All)) {
                return true;
            } else if (permission.AllowAdministrator) {
                return true;
            } else if (e.User.Id == ownerid) {
                return true;
            } else {
                return false;
            }
        }

        private async Task ShowNoPermissionMessage(ComponentInteractionCreateEventArgs e) {
            var embed = new DiscordEmbedBuilder();
            embed.WithAuthor("You don't have permission!", null, e.User.AvatarUrl);
            embed.WithColor(new DiscordColor("#e83b3b"));
            embed.Build();
            var ephemeral = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AsEphemeral(true);
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, ephemeral);
        }

    }
}
