using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using System.Reflection;
using System.Threading.Channels;
using DiscordBot.Model.Enums;
using DiscordBot.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace DiscordBot.Listeners {
    public class ButtonClickEvent {

        public async Task ButtonClick(DiscordClient sender, ComponentInteractionCreateEventArgs e) {
            if (e.Interaction.Data.ComponentType == ComponentType.Button) {
                if (e.Id.Contains("embedButton")) await UseEmbed(sender, e);
            }
        }

        public async Task UseEmbed(DiscordClient discordClient, ComponentInteractionCreateEventArgs e) {

            var messageId = e.Message.Id;
            var guildId = e.Guild.Id;
            var embed = CacheData.GetEmbed(e.Guild.Id, messageId);

            try {

                var modal = new DiscordInteractionResponseBuilder();
                var message = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build())
                    .AsEphemeral(false);

                string pingRoles = string.Empty;
                if (embed.PingRoles.Count > 0) {

                    // List of roles to be kept
                    List<DiscordRole> roles = new();

                    // Translate each id in to roles
                    foreach (var roleId in embed.PingRoles) {
                        roles.Add(await DiscordUtil.GetRolesByIdAsync(e.Interaction.Guild, roleId));
                    }

                    // Extract the mention property from each role.
                    foreach (var role in roles) {
                        if (role.Name == "@everyone") {
                            pingRoles += "@everyone";
                        } 
                        else pingRoles += role.Mention;
                    }

                }

                if (e.Id == ("embedButtonChannel")) {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    var channel = await DiscordUtil.GetChannelByIdAsync(e.Guild, (ulong)embed.ChannelId!);
                    var sentMessage = await channel.SendMessageAsync(content: pingRoles + " " + embed.Content, embed: embed.Build());
                    embed.AddCopiedMessage(sentMessage.Id, sentMessage.Channel.Id);
                    var copy = embed.DeepClone();
                    copy.MessageId = sentMessage.Id;
                    copy.ChannelId = sentMessage.ChannelId;
                    copy.Time = DateTime.Now.Ticks;
                    await CacheData.AddEmbed(guildId, sentMessage.Id, copy);
                }
                if (e.Id == ("embedButtonCurrent")) {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message.AsEphemeral(false).WithContent(pingRoles + " " + embed.Content));
                    var sentMessage = await e.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                    embed.AddCopiedMessage(sentMessage.Id, sentMessage.Channel.Id);
                }
                if (e.Id == ("embedButtonUpdate")) {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    foreach (var m in embed.ChildEmbeds) {
                        var channel = await discordClient.GetChannelAsync(m.Value);
                        if (channel != null) {
                            var oldMessage = await channel.GetMessageAsync(m.Key);
                            if (oldMessage != null) {
                                await oldMessage.ModifyAsync(msg =>
                                {
                                    msg.Content = pingRoles;
                                    msg.Embed = embed.Build();
                                });
                            }
                        }
                    }
                }
                if (e.Id == ("embedButtonCancel")) {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    var sentMessage = await e.Interaction.GetOriginalResponseAsync().ConfigureAwait(false);
                    await sentMessage.DeleteAsync();
                    await CacheData.RemoveEmbed(guildId, sentMessage.Id);
                }

                if (e.Id == ("embedButtonNitroClaim")) {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    var nitroMessage = new DiscordFollowupMessageBuilder()
                        .WithContent("https://www.icegif.com/wp-content/uploads/2023/01/icegif-162.gif")
                        .AsEphemeral(true);
                    await e.Interaction.CreateFollowupMessageAsync(nitroMessage.AsEphemeral(true));
                }

                if (e.Id == ("embedButtonEventPostCreate")) {
                    modal.WithTitle("EVENT CREATION").WithCustomId($"{Identity.MODAL_EVENT};{Identity.SELECTION_EVENT_CREATION};{messageId}");
                    modal.AddComponents(new TextInputComponent("EVENT NAME", Identity.EVENT_NAME, "Write something...", embed.CustomData[Identity.EVENT_NAME] as string, true, TextInputStyle.Short, 4, 32));
                    modal.AddComponents(new TextInputComponent("TIME ZONE", Identity.EVENT_TIMEZONE, "Europe/Brussels", embed.CustomData[Identity.EVENT_TIMEZONE] as string, true, TextInputStyle.Short));
                    modal.AddComponents(new TextInputComponent("START DATE", Identity.EVENT_START, "DD/MM/YYYY hh:mm", embed.CustomData[Identity.EVENT_START] as string, true, TextInputStyle.Short));
                    modal.AddComponents(new TextInputComponent("END DATE", Identity.EVENT_END, "DD/MM/YYYY hh:mm", embed.CustomData[Identity.EVENT_END] as string, true, TextInputStyle.Short));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                }

                if (e.Id == ("embedButtonTemplateUse")) {
                    modal.WithTitle("USE TEMPLATE").WithCustomId($"{Identity.MODAL_TEMPLATE};{Identity.SELECTION_TEMPLATE_INPUT};{embed.CustomData[Identity.TEMPLATE_REPLACE_MESSAGE_ID]}");
                    modal.AddComponents(new TextInputComponent("TEMPLATE", Identity.TEMPLATE_NAME, "The template you want to use", null, true, TextInputStyle.Short, 1, 64));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
                }

                await JsonData.SaveEmbedsAsync(guildId);

            } catch (Exception ex) {
                Console.WriteLine(ex);
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
