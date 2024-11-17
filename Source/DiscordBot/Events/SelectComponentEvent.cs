using APP.Utils;
using BLL.Enums;
using BLL.Interfaces;
using BLL.Model;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace APP.Events {
    public class SelectComponentEvent {

        #region Fields
        private readonly IDataService dataService;
        #endregion

        #region Constructors
        public SelectComponentEvent(IDataService dataService) {
            this.dataService = dataService;
        }
        #endregion

        public async Task ComponentSelect(DiscordClient sender, ComponentInteractionCreateEventArgs e) {
            if (e.Interaction.Data.ComponentType == ComponentType.StringSelect) {
                if (e.Id == Identity.COMPONENT_SELECT) await HandleEmbedInteraction(sender, e);
                if (e.Id == Identity.COMPONENT_TEMPLATE) await HandleTemplateInteraction(sender, e);
                if (e.Id == Identity.COMPONENT_EVENT) await HandleEventInteraction(sender, e);
            }
        }

        public async Task HandleEmbedInteraction(DiscordClient client, ComponentInteractionCreateEventArgs e) {

            var messageId = e.Message.Id;
            var message = await dataService.GetMessageAsync(e.Guild.Id, messageId);
            var embed = message.Embed;

            try {
                const string text = "Write something...";
                var exampleUrl = @"https://example.com/";
                var options = e.Values;
                var modal = new DiscordInteractionResponseBuilder();

                foreach (var option in options) {
                    switch (option) {
                        case Identity.SELECTION_TITLE:
                            modal.WithTitle($"EDITING TITLE").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TITLE TEXT", Identity.MODAL_COMP_TITLE, text, embed.Title, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("TITLE LINK", Identity.MODAL_COMP_TITLE_LINK, exampleUrl, embed.TitleUrl, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_DESCRIPTION:
                            modal.WithTitle($"EDITING DESCRIPTION").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent(option, Identity.MODAL_COMP_DESCRIPTION, text, embed.Description, false, TextInputStyle.Paragraph));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_CONTENT:
                            modal.WithTitle($"EDITING PLAIN CONTENT").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent(option, Identity.MODAL_COMP_CONTENT, text, message.Content, false, TextInputStyle.Paragraph));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_FOOTER:
                            modal.WithTitle($"EDITING FOOTER").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("FOOTER TEXT", Identity.MODAL_COMP_FOOTER, text, embed.Footer, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("FOOTER IMAGE", Identity.MODAL_COMP_FOOTER_URL, exampleUrl, embed.FooterUrl, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_AUTHOR:
                            modal.WithTitle($"EDITING AUTHOR").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("AUTHOR TEXT", Identity.MODAL_COMP_AUTHOR, text + " or self", embed.Author, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("AUTHOR LINK", Identity.MODAL_COMP_AUTHOR_LINK, exampleUrl, embed.AuthorLink, false, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("AUTHOR IMAGE", Identity.MODAL_COMP_AUTHOR_URL, exampleUrl, embed.AuthorUrl, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_COLOR:
                            modal.WithTitle($"EDITING COLOR").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent(option, Identity.MODAL_COMP_COLOR, text, embed.Color, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_IMAGE:
                            modal.WithTitle($"EDITING IMAGE").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent(option, Identity.MODAL_COMP_IMAGE, exampleUrl, embed.Image, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_THUMBNAIL:
                            modal.WithTitle($"EDITING THUMBNAIL").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent(option, Identity.MODAL_COMP_THUMBNAIL, exampleUrl, embed.Thumbnail, false, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_PINGROLE:
                            string roleId = "836964332595707955, 1265749062183813242, everyone";
                            modal.WithTitle($"EDITING PINGED ROLES").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent($"ROLES", Identity.MODAL_COMP_PINGROLE, roleId, string.Join(", ", message.Roles), true, TextInputStyle.Paragraph));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_TIMESTAMP:
                            embed.HasTimeStamp = !embed.HasTimeStamp;
                            await DiscordUtil.UpdateMessageAsync(e.Interaction, message);
                            break;
                        case Identity.SELECTION_FIELD_ADD:
                            modal.WithTitle($"ADDING FIELD TEXT").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TITLE", Identity.MODAL_COMP_FIELD_TITLE, text, null, true, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("TEXT", Identity.MODAL_COMP_FIELD_TEXT, text, null, true, TextInputStyle.Paragraph));
                            modal.AddComponents(new TextInputComponent("INLINE", Identity.MODAL_COMP_FIELD_INLINE, "True or False", "True", true, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;
                        case Identity.SELECTION_FIELD_REMOVE:
                            if (embed.Fields.Count() != 0) {
                                modal.WithTitle($"REMOVING FIELD").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                                modal.AddComponents(new TextInputComponent("INDEX", Identity.MODAL_COMP_FIELD_INDEX, $"Number from 1 to {embed.Fields.Count()}", null, true, TextInputStyle.Short, 1, 2));
                                await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            }
                            break;
                        default:
                            await SendNotAFeatureYet(e.Interaction);
                            break;
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task HandleTemplateInteraction(DiscordClient discordClient, ComponentInteractionCreateEventArgs e) {

            var messageId = e.Message.Id;
            var message = await dataService.GetMessageAsync(e.Guild.Id, messageId);
            var embed = message.Embed;

            try {
                const string text = "Write something...";
                var exampleUrl = @"https://example.com/";
                var options = e.Values;
                var components = e.Message.Components;
                var modal = new DiscordInteractionResponseBuilder();
                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build())
                    .AddComponents(components);

                foreach (var option in options) {
                    switch (option) {

                        case Identity.SELECTION_TEMPLATE_ADD:
                            modal.WithTitle("SAVE TEMPLATE").WithCustomId($"{Identity.MODAL_EMBED};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("SAVE TEMPLATE", Identity.MODAL_COMP_TEMPLATE_ADD, text, null, true, TextInputStyle.Short, 3, 24));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        case Identity.SELECTION_TEMPLATE_USE:
                            var templateUseMessage = (await dataService.GetTemplateAsync(e.Interaction.Guild.Id, Identity.TDATA_USE)).Message;
                            using (StreamReader sr = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Saves", "Templates.json"))) {
                                var data = await sr.ReadToEndAsync();
                                var json = JsonConvert.DeserializeObject<Dictionary<string, Embed>>(data)!;
                                templateUseMessage.Embed.Description = templateUseMessage.Embed.Description!.Replace($"{{{Identity.TEMPLATE_LIST}}}", json.Keys.Aggregate("", (current, next) => current + "\n- `" + next + "`"));
                            }
                            //var guildTemplates = await CacheData.GetTemplateAll(e.Interaction.Guild.Id);
                            //templateUseEmbed.Description =
                            //    guildTemplates.Count == 0 ?
                            //    templateUseEmbed.Description!.Replace($"{{{Identity.TEMPLATE_LIST_CUSTOM}}}", "- No templates saved yet...") :
                            //    templateUseEmbed.Description!.Replace($"{{{Identity.TEMPLATE_LIST_CUSTOM}}}", guildTemplates.Keys.Aggregate("", (current, next) => current + "\n- `" + next + "`"));

                            templateUseMessage.AddData(Identity.TEMPLATE_REPLACE_MESSAGE_ID, messageId.ToString());
                            await DiscordUtil.CreateMessageAsync(CommandEnum.TEMPLATE_USE, e.Interaction, templateUseMessage, e.Interaction.Channel.Id, templateUseMessage.IsEphemeral);
                            break;

                        default:
                            await SendNotAFeatureYet(e.Interaction);
                            break;
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task HandleEventInteraction(DiscordClient discordClient, ComponentInteractionCreateEventArgs e) {

            var messageId = e.Message.Id;
            var options = e.Values;
            var message = await dataService.GetMessageAsync(e.Guild.Id, messageId);
            var embed = message.Embed;

            try {
                const string text = "Write something...";
                var exampleUrl = @"https://example.com/";
                var components = e.Message.Components;
                var modal = new DiscordInteractionResponseBuilder();
                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build())
                    .AddComponents(components);

                foreach (var option in options) {
                    switch (option) {

                        case Identity.SELECTION_EVENT_PROPERTIES:
                            modal.WithTitle("EVENT PROPERTIES").WithCustomId($"{Identity.MODAL_EVENT};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("EVENT NAME", Identity.EVENT_NAME, "Amongus", message.Data[Identity.EVENT_NAME] as string, true, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("TIME ZONE", Identity.EVENT_TIMEZONE, "Europe/Brussels", message.Data[Identity.EVENT_TIMEZONE] as string, true, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("START DATE", Identity.EVENT_START, "DD/MM/YYYY hh:mm", message.Data[Identity.EVENT_START] as string, true, TextInputStyle.Short));
                            modal.AddComponents(new TextInputComponent("END DATE", Identity.EVENT_END, "DD/MM/YYYY hh:mm", message.Data[Identity.EVENT_END] as string, true, TextInputStyle.Short));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        case Identity.SELECTION_EVENT_INTRODUCTION:
                            modal.WithTitle("EVENT INTRODUCTION").WithCustomId($"{Identity.MODAL_EVENT};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TITLE", Identity.EVENT_TITLE, text, message.Data[Identity.EVENT_TITLE] as string, true, TextInputStyle.Short, 4, 32));
                            modal.AddComponents(new TextInputComponent("INTRODUCTION", Identity.EVENT_INTRO, text, message.Data[Identity.EVENT_INTRO] as string, true, TextInputStyle.Paragraph, 0, 500));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        case Identity.SELECTION_EVENT_INFORMATION:
                            modal.WithTitle("EVENT INFORMATION").WithCustomId($"{Identity.MODAL_EVENT};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TITLE", Identity.EVENT_INFO_TITLE, text, message.Data[Identity.EVENT_INFO_TITLE] as string, true, TextInputStyle.Short, 4, 32));
                            modal.AddComponents(new TextInputComponent("INFORMATION", Identity.EVENT_INFO, text, message.Data[Identity.EVENT_INFO] as string, true, TextInputStyle.Paragraph, 0, 2048));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        case Identity.SELECTION_EVENT_REWARDS:
                            modal.WithTitle("EVENT REWARDS").WithCustomId($"{Identity.MODAL_EVENT};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TITLE", Identity.EVENT_REWARD_TITLE, text, message.Data[Identity.EVENT_REWARD_TITLE] as string, true, TextInputStyle.Short, 4, 32));
                            modal.AddComponents(new TextInputComponent("TOP REWARDS", Identity.EVENT_REWARD, text, message.Data[Identity.EVENT_REWARD] as string, true, TextInputStyle.Paragraph, 0, 1024));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        case Identity.SELECTION_EVENT_TIMESTAMP:
                            modal.WithTitle("EVENT DATE FORMAT").WithCustomId($"{Identity.MODAL_EVENT};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("TITLE", Identity.EVENT_TIME_TITLE, text, message.Data[Identity.EVENT_TIME_TITLE] as string, true, TextInputStyle.Short, 4, 32));
                            modal.AddComponents(new TextInputComponent("FORMAT START DATE", Identity.EVENT_DESCRIPTION_START, text, message.Data[Identity.EVENT_DESCRIPTION_START] as string, false, TextInputStyle.Paragraph, 0, 128));
                            modal.AddComponents(new TextInputComponent("FORMAT END DATE", Identity.EVENT_DESCRIPTION_END, text, message.Data[Identity.EVENT_DESCRIPTION_END] as string, false, TextInputStyle.Paragraph, 0, 128));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        case Identity.SELECTION_EVENT_REACTION:
                            modal.WithTitle("EVENT REACTION FORMAT").WithCustomId($"{Identity.MODAL_EVENT};{option};{messageId}");
                            modal.AddComponents(new TextInputComponent("FORMAT REACTION TEXT", Identity.EVENT_DESCRIPTION_REACTION, text, message.Data[Identity.EVENT_DESCRIPTION_REACTION] as string, false, TextInputStyle.Paragraph, 0, 1024));
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                            break;

                        default:
                            await SendNotAFeatureYet(e.Interaction);
                            break;
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + string.Join(", ", options));
            }
        }

        //private async Task<bool> CheckPermission(ComponentInteractionCreateEventArgs e, CommandEnum cmd, ulong ownerid) {

        //    var permission = await CacheData.GetPermission(e.Guild.Id, cmd);
        //    var userid = e.User.Id;
        //    var user = await e.Guild.GetMemberAsync(userid);
        //    var roles = user.Roles;

        //    if (user.Permissions.HasPermission(Permissions.All)) {
        //        return true;
        //    } else if (permission.AllowAdministrator) {
        //        return true;
        //    } else if (e.User.Id == ownerid) {
        //        return true;
        //    } else {
        //        return false;
        //    }
        //}

        private async Task SendNotAFeatureYet(DiscordInteraction interaction) {
            var embedMessage = new DiscordEmbedBuilder()
                .WithAuthor("Feature doesn't work yet!", null, "https://cdn-icons-png.flaticon.com/512/2581/2581801.png")
                .WithColor(new DiscordColor("#d82b40"));

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(embedMessage)
                .AsEphemeral(true));
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
