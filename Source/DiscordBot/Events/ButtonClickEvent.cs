using DSharpPlus.EventArgs;
using DSharpPlus;
using DSharpPlus.Entities;
using APP.Utils;
using BLL.Interfaces;
using BLL.Services;
using BLL.Model;
using BLL.Enums;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

namespace APP.Events {
    public class ButtonClickEvent {

        #region Fields
        private readonly IDataService dataService;
        #endregion

        #region Constructors
        public ButtonClickEvent(IDataService dataService) {
            this.dataService = dataService;
        }
        #endregion

        #region Methods
        public async Task ButtonClick(DiscordClient sender, ComponentInteractionCreateEventArgs e) {
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains("embedButton")) await UseEmbed(sender, e);
            }
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains(Identity.BUTTON_TEMPLATE)) await UseTemplate(e);
            }
        }

        public async Task UseEmbed(DiscordClient discordClient, ComponentInteractionCreateEventArgs e) {

            var messageId = e.Message.Id;
            var guildId = e.Guild.Id;
            var message = await dataService.GetMessageAsync(e.Guild.Id, messageId);

            // Translate the placeholders
            var translated = message.DeepClone();
            await translated.TranslatePlaceholders();
            var embed = translated.Embed;

            try {

                var modal = new DiscordInteractionResponseBuilder();
                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build())
                    .AsEphemeral(false);

                string pingRoles = string.Empty;
                if (message.Roles.Count > 0) {

                    // List of roles to be kept
                    List<DiscordRole> roles = new();

                    // Translate each id in to roles
                    foreach (var roleId in message.Roles) {
                        roles.Add(await DiscordUtil.GetRolesByIdAsync(e.Interaction.Guild, roleId));
                    }

                    // Extract the mention property from each role.
                    foreach (var role in roles) {
                        if (role.Name == "@everyone") {
                            pingRoles += "@everyone";
                        } else pingRoles += role.Mention;
                    }
                }

                if (e.Id == "embedButtonChannel") {

                    // Create the response
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    var channel = await DiscordUtil.GetChannelByIdAsync(e.Guild, (ulong)message.ChannelId!);
                    var sentMessage = await channel.SendMessageAsync(content: pingRoles + " " + translated.Content, embed: embed.Build());
                    message.AddData($"{sentMessage.Id}", $"{sentMessage.Channel.Id}");
                    var copy = message.DeepClone();
                    copy.MessageId = sentMessage.Id;
                    copy.ChannelId = sentMessage.ChannelId;
                    copy.Embed.Time = DateTime.Now.Ticks;
                    //await CacheData.AddEmbed(guildId, sentMessage.Id, copy);
                }
                //if (e.Id == "embedButtonCurrent") {
                //    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response.AsEphemeral(false).WithContent(pingRoles + " " + translated.Content));
                //    var sentMessage = await e.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                //    message.AddData($"{sentMessage.Id}", $"{sentMessage.Channel.Id}");
                //}
                if (e.Id == "embedButtonTemplates") {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    await TemplateButtonClick(e.Interaction, message);
                }
                //if (e.Id == "embedButtonUpdate") {
                //    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                //    foreach (var m in message.Childs) {
                //        var channel = await discordClient.GetChannelAsync(m.Value);
                //        if (channel != null) {
                //            var oldMessage = await channel.GetMessageAsync(m.Key);
                //            if (oldMessage != null) {
                //                await oldMessage.ModifyAsync(msg => {
                //                    msg.Content = pingRoles;
                //                    msg.Embed = embed.Build();
                //                });
                //            }
                //        }
                //    }
                //}
                if (e.Id == "embedButtonCancel") {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    var sentMessage = await e.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                    await sentMessage.DeleteAsync();
                    await dataService.GetMessageAsync(guildId, (ulong)message.MessageId);
                    //await CacheData.RemoveEmbed(guildId, sentMessage.Id);
                }

                if (e.Id == "embedButtonNitroClaim") {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    var nitroMessage = new DiscordFollowupMessageBuilder()
                        .WithContent("https://www.icegif.com/wp-content/uploads/2023/01/icegif-162.gif")
                        .AsEphemeral(true);
                    await e.Interaction.CreateFollowupMessageAsync(nitroMessage.AsEphemeral(true));
                }

                if (e.Id == "embedButtonEventPostCreate") {
                    modal.WithTitle("EVENT CREATION").WithCustomId($"{Identity.MODAL_EVENT};{Identity.SELECTION_EVENT_CREATION};{messageId}");
                    modal.AddComponents(new TextInputComponent("EVENT NAME", Identity.EVENT_NAME, "Write something...", message.Data[Identity.EVENT_NAME] as string, true, TextInputStyle.Short, 4, 32));
                    modal.AddComponents(new TextInputComponent("TIME ZONE", Identity.EVENT_TIMEZONE, "Europe/Brussels", message.Data[Identity.EVENT_TIMEZONE] as string, true, TextInputStyle.Short));
                    modal.AddComponents(new TextInputComponent("START DATE", Identity.EVENT_START, "DD/MM/YYYY hh:mm", message.Data[Identity.EVENT_START] as string, true, TextInputStyle.Short));
                    modal.AddComponents(new TextInputComponent("END DATE", Identity.EVENT_END, "DD/MM/YYYY hh:mm", message.Data[Identity.EVENT_END] as string, true, TextInputStyle.Short));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                }

                if (e.Id == "embedButtonTemplateUse") {
                    modal.WithTitle("USE TEMPLATE").WithCustomId($"{Identity.MODAL_TEMPLATE};{Identity.SELECTION_TEMPLATE_INPUT};{message.Data[Identity.TEMPLATE_REPLACE_MESSAGE_ID]}");
                    modal.AddComponents(new TextInputComponent("TEMPLATE", Identity.TEMPLATE_NAME, "The template you want to use", null, true, TextInputStyle.Short, 1, 64));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                }

            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task UseTemplate(ComponentInteractionCreateEventArgs e) {
            try {

                // Get the message
                var messageId = e.Message.Id;
                var guildId = e.Guild.Id;
                var message = await dataService.GetMessageAsync(e.Guild.Id, messageId);
                var interaction = e.Interaction;

                // Translate the placeholders
                var translated = message.DeepClone();
                await translated.TranslatePlaceholders();
                var embed = translated.Embed;

                // Initialize the modal
                var modal = new DiscordInteractionResponseBuilder();
                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build())
                    .AsEphemeral(false);

                if (e.Id == Identity.BUTTON_TEMPLATE_CANCEL) {
                    var awrwawar = message.Data[Placeholder.TEMPLATE];
                    message = JsonConvert.DeserializeObject<Message>(awrwawar);
                    if (message != null)
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        await DiscordUtil.ModifyMessageAsync(message.Type, interaction, message, interaction.Channel.Id);
                }



            } catch (Exception) {

                throw;
            }
        }

        private async Task TemplateButtonClick(DiscordInteraction interaction, Message message) {
            var guildId = interaction.Guild.Id;
            var templates = await dataService.GetTemplateAsync(guildId, Identity.TDATA_USE);
            if (templates != null) {
                var template = templates.Message;
                template.MessageId = message.MessageId;
                template.GuildId = message.GuildId;
                template.ChannelId = message.ChannelId;
                template.Sender = message.Sender;
                template.IsEphemeral = message.IsEphemeral;
                template.AddData(Placeholder.TEMPLATE, JsonConvert.SerializeObject(message));
                message = template;
            }
            await DiscordUtil.ModifyMessageAsync(CommandEnum.TEMPLATE_USE, interaction, message, interaction.Channel.Id);
        }

        #endregion

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

    }
}
