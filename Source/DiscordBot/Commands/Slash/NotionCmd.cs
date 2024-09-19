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
using DiscordBot.Utils;
using DSharpPlus;
using DSharpPlus.Interactivity;

namespace DiscordBot.Commands.Slash {
    internal class NotionCmd : SlashCommand {

        private NotionClient _notionClient;
        private const string token = @"secret_4CS7Rgs0Jwr3FpxPdWfSZQsvhQEafvTq21SoxLvtzbA";
        private const string databaseId = "ae8ebac78dfe4407ba11e6c7ff77be03";

        [SlashCommand("notion", "Notion editor")]
        public async Task UseNotionCommand(InteractionContext ctx) {
            var client = NotionClientFactory.Create(new ClientOptions {
                AuthToken = token
            });

            try {
                List<Task<string>> tasks = new();
                var databasesQueryParameters = new DatabasesQueryParameters();
                var queryResult = await client.Databases.QueryAsync(databaseId, databasesQueryParameters);

                foreach (var result in queryResult.Results) {
                    Console.WriteLine("Page Id: " + result.Id);
                    foreach (var property in result.Properties) {
                        var retrieveCommentsParameters = new RetrieveCommentsParameters() {
                            BlockId = result.Id
                        };
                        tasks.Add(RetrieveValues(property));
                        //var comments = await client.Comments.RetrieveAsync(retrieveCommentsParameters);
                        //foreach (var comment in comments.Results) {
                        //    var commentText = string.Join(" ", comment.RichText.Select(t => t.PlainText));
                        //    Console.WriteLine($"Comment: {commentText}");
                        //}
                    }
                }

                var results = await Task.WhenAll(tasks);

                // Now you can process the results if needed
                foreach (var result in results) {
                    Console.WriteLine(result);
                }

            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }

        private async Task<string> RetrieveValues(KeyValuePair<string, PropertyValue> value) {
            return value.Key + ": " + await GetValue(value.Value);
        }

        private async Task<object> GetValue(PropertyValue p) {
            switch (p) {
                case TitlePropertyValue titlePropertyValue:
                    return titlePropertyValue.Title.FirstOrDefault()?.PlainText;
                case DatePropertyValue dateProperty:
                    if (dateProperty.Date != null) {
                        return dateProperty.Date.Start.GetValueOrDefault().ToString("dd/MM/yyyy");
                    } else {
                        return "N/A";
                    }
                default:
                    return "???";
            }
        }

    }
}
