using DiscordBot.Model.Enums;
using DSharpPlus.SlashCommands;
using XironiteDiscordBot.Commands;

namespace DiscordBot.Commands.Slash {
    public class PermissionCmd : SlashCommand {

        [SlashCommand("permissions", "Show an overview of permissions")]
        public async Task UsePermissionEditCommand(InteractionContext ctx,
            [Option("cmd", "Choose which command permission to edit")] CommandEnum cmd) {

            if (!await CheckPermission(ctx, CommandEnum.PERMISSIONS)) {
                await showNoPermissionMessage(ctx);
                return;
            }
            LogCommand(ctx, CommandEnum.PERMISSIONS);

            //try {
            //    var permission = await Json.ReadPermissionFile(ctx.Guild.Id, cmd);

            //    List<string> roles = new() { "Administrator", "Everyone" };
            //    List<bool> canUse = new() { permission.AllowAdministrator, permission.AllowEveryone };

            //    foreach (var role in permission.AllowedRoles) {
            //        roles.Add(role.Value.Name);
            //        canUse.Add(true);
            //    }
            //    foreach (var role in permission.AllowedUsers) {
            //        roles.Add(role.Value.Username);
            //        canUse.Add(true);
            //    }

            //    EmbedBuilder embedBuilder = new EmbedBuilder() {
            //        Author = $"Command /{cmd.ToString().ToLower()} management",
            //        AuthorUrl = ctx.Client.CurrentUser.AvatarUrl,
            //    };

            //    embedBuilder.AddField("Roles & Users", $"```\n------------------------------\n{string.Join("\n", roles)}```", true);
            //    embedBuilder.AddField("Access", $"```\n \n{canUse.Select(b => b ? " ✅ " : " ❌ ").Aggregate((a, b) => $"{a}\n{b}")}```", true);


            //    var ephemeral = new DiscordInteractionResponseBuilder()
            //        .AddEmbed(embedBuilder.Build())
            //        .AsEphemeral(true);

            //    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, ephemeral);

            //} catch (Exception ex) {
            //    throw new CommandException($"Permission.UsePermissionEditCommand: {ex}");
            //}
        }

    }
}
