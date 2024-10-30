using DiscordBot.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Events {
    public class SlashCommandUseEvent {
        public async Task OnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e) {
            if (e.Exception is SlashExecutionChecksFailedException slex) {

                // Loop through all failed checks
                foreach (var check in slex.FailedChecks) {

                    // Check if the failed check is a RequirePermissionAttribute
                    if (check is RequirePermissionAttribute att) {
                        var embed = new DiscordEmbedBuilder()
                            .WithAuthor($"You don't have permission for /{att.Command}", null, "https://cdn-icons-png.flaticon.com/512/2581/2581801.png")
                            .WithColor(new DiscordColor("#d82b40"));
                        await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral(true));
                    }

                }

            } else {

                // Log the error
                Console.WriteLine($"Error executing command {e.Context}:\n{e.Exception}");

                //// Optionally notify the user in the channel
                //if (e.Context.Channel is DiscordChannel channel) {
                //    await channel.SendMessageAsync(e.Exception.Message);
                //}

            }
        }
    }
}
