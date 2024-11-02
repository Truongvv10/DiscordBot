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
using DiscordBot.Utils;
using DiscordBot.Model.Enums;

namespace DiscordBot.Events {
    public class SlashCommandUseEvent {
        public async Task OnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e) {
            if (e.Exception is SlashExecutionChecksFailedException ex) {

                // Loop through all failed checks
                foreach (var check in ex.FailedChecks) {

                    // Check if the failed check is a RequirePermissionAttribute
                    if (check is RequirePermissionAttribute att) {
                        var interaction = e.Context.Interaction;
                        var embed = await CacheData.GetTemplate(e.Context.Guild.Id, Identity.TDATA_NO_PERMISSION);
                        await DiscordUtil.CreateMessageAsync(CommandEnum.NONE, interaction, embed, interaction.Channel.Id, embed.IsEphemeral);
                    }
                }
            } else  Console.WriteLine($"Error executing command {e.Context}:\n{e.Exception}");
        }
    }
}
