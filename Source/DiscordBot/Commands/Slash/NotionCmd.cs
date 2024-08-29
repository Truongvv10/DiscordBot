using DiscordBot.Exceptions;
using DiscordBot.Model.Enums;
using DiscordBot.Model;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XironiteDiscordBot.Commands;
using Notion.Client;
using XironiteDiscordBot.Model;
using DiscordBot.Utils;
using DSharpPlus;

namespace DiscordBot.Commands.Slash {
    internal class NotionCmd : SlashCommand {

        private NotionClient _notionClient;
        private string databaseId = "ae8ebac78dfe4407ba11e6c7ff77be03";

        [SlashCommand("notion", "Notion editor")]
        public async Task UseNotionCommand(InteractionContext ctx) {
            var client = NotionClientFactory.Create(new ClientOptions {
                AuthToken = @"secret_4CS7Rgs0Jwr3FpxPdWfSZQsvhQEafvTq21SoxLvtzbA"
            });

            try {
                var dateFilter = new DateFilter("Due", onOrAfter: DateTime.Now);
                var queryParams = new DatabasesQueryParameters { Filter = dateFilter };
                var pages = await client.Databases.QueryAsync(databaseId, queryParams);
                Console.WriteLine(pages);
            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }



            //var usersList = await client.Users.ListAsync();
            //if (usersList.Results != null && usersList.Results.Any()) {
            //    var userInfo = usersList.Results
            //                            .Select(user => $"{user.Id}, {user.Name}")
            //                            .Aggregate((a, b) => $"{a}\n{b}");

            //    var response = new DiscordInteractionResponseBuilder()
            //        .AsEphemeral(true)
            //        .WithContent(userInfo);

            //    // Send the response
            //    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
            //} else {
            //    var response = new DiscordInteractionResponseBuilder()
            //        .AsEphemeral(true)
            //        .WithContent("No users found.");

            //    // Send the response
            //    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
            //}
        }

    }
}
