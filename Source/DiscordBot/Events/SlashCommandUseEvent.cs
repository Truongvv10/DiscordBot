using DSharpPlus.SlashCommands.EventArgs;
using DSharpPlus.SlashCommands;
using BLL.Enums;
using APP.Utils;
using BLL.Interfaces;
using BLL.Model;
using APP.Attributes;

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

                // Loop through all failed checks
                foreach (var check in ex.FailedChecks) {

                    // Check if the failed check is a RequirePermissionAttribute
                    if (check is RequirePermissionAttribute att) {
                        var interaction = e.Context.Interaction;
                        var message = (await dataService.GetTemplateAsync(interaction.Guild.Id, TemplateMessage.NO_PERMISSION.ToString())).Message;
                        await discordUtil.CreateMessageAsync(CommandEnum.NONE, interaction, message, interaction.Channel.Id, message.IsEphemeral);
                    }
                }
            } else Console.WriteLine($"Error executing command {e.Context}:\n{e.Exception}");
        }
    }
}
