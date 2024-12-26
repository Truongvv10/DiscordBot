using DSharpPlus.SlashCommands.EventArgs;
using DSharpPlus.SlashCommands;
using BLL.Enums;
using APP.Utils;
using BLL.Interfaces;
using BLL.Model;
using APP.Attributes;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands.Attributes;

namespace APP.Events {
    public class SlashCommandUseEvent {

        #region Fields
        private readonly IDataRepository dataService;
        private readonly DiscordUtil discordUtil;
        #endregion

        #region Constructors
        public SlashCommandUseEvent(IDataRepository dataService, DiscordUtil discordUtil) {
            this.dataService = dataService;
            this.discordUtil = discordUtil;
        }
        #endregion

        public async Task OnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e) {
            if (e.Exception is SlashExecutionChecksFailedException ex) {
                var interaction = e.Context.Interaction;

                // Loop through all failed checks
                foreach (var check in ex.FailedChecks) {

                    // Check if the failed check is a RequirePermissionAttribute
                    if (check is RequirePermissionAttribute att1)
                        await discordUtil.SendActionMessage(interaction, TemplateMessage.ACTION_FAILED, "You don't have permission.", $"Missing permission for **`{att1.Command}`**.");

                    if (check is SlashCooldownAttribute att2) {
                        var cooldown = Math.Round(att2.GetRemainingCooldown(e.Context).TotalSeconds, 1);
                        await discordUtil.SendActionMessage(interaction, TemplateMessage.ACTION_FAILED, "Command is on cooldown.", $"Please wait for another **`{cooldown}s`**.");
                    }
                }
            } else Console.WriteLine($"Error executing command {e.Context}:\n{e.Exception}");
        }
    }
}
