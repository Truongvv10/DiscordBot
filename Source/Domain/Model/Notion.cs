using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model {
    internal class Notion {
        //#region Private Properties
        //private readonly NotionConfig _notionConfig;
        //private readonly NotionClient _notionClient;
        //#endregion

        //#region Constructors
        //public NotionService(NotionConfig config) {
        //    _notionConfig = config;
        //    _notionClient = NotionClientFactory.Create(new ClientOptions {
        //        AuthToken = _notionConfig.NotionApiKey,
        //    });
        //}
        //#endregion

        //#region Methods
        //private async Task<Dictionary<string, (string, string)>> FetchAllDataAsync(string databaseId) {

        //    // Data output
        //    var fetchedData = new Dictionary<string, (string, string)>();

        //    // Database querries
        //    var parameters = new DatabasesQueryParameters();
        //    var queryResult = await _notionClient.Databases.QueryAsync(databaseId, parameters);

        //    // Result of all pages
        //    foreach (var result in queryResult.Results) {
        //        foreach (var property in result.Properties) {
        //            fetchedData.Add(result.Id, (property.Key, await GetValueAsync(property.Value)));
        //        }
        //    }

        //    // Return fetched data
        //    return fetchedData;
        //}

        //private async Task<Dictionary<string, string>> FetchPageCommentAsync(string pageId) {
        //    // Data output
        //    var fetchedData = new Dictionary<string, string>();

        //    // Database page queries
        //    var retrieveCommentsParameters = new RetrieveCommentsParameters() { BlockId = pageId };
        //    var comments = await _notionClient.Comments.RetrieveAsync(retrieveCommentsParameters);

        //    foreach (var comment in comments.Results) {
        //        // Extract the comment text
        //        var commentText = string.Join(" ", comment.RichText.Select(t => t.PlainText));

        //        // Get the author's information
        //        var author = comment.CreatedBy;

        //        // Output the comment and author
        //        Console.WriteLine($"Comment: {commentText} - by {author}");

        //        // Add to the dictionary (you can customize the key-value pair as needed)
        //        fetchedData.Add(author.Id, commentText);
        //    }

        //    // Return fetched comment
        //    return fetchedData;
        //}

        //private async Task<string> GetValueAsync(PropertyValue p) {

        //    // Property has empty attributes output
        //    const string notFound = "N/A";

        //    // Searching what the proprty type is
        //    switch (p) {
        //        case TitlePropertyValue titlePropertyValue:
        //            if (titlePropertyValue != null) {
        //                return titlePropertyValue.Title.FirstOrDefault()?.PlainText;
        //            } else return notFound;
        //        case DatePropertyValue dateProperty:
        //            if (dateProperty.Date != null) {
        //                return dateProperty.Date.Start.GetValueOrDefault().ToString("dd/MM/yyyy");
        //            } else return notFound;
        //        default:
        //            return notFound;
        //    }
        //}
        //#endregion














        //private NotionClient _notionClient;
        //private const string token = @"secret_4CS7Rgs0Jwr3FpxPdWfSZQsvhQEafvTq21SoxLvtzbA";
        //private const string databaseId = "ae8ebac78dfe4407ba11e6c7ff77be03";

        //[SlashCommand("notion", "Notion editor")]
        //[RequirePermission(CommandEnum.NOTION)]
        //public async Task UseNotionCommand(InteractionContext ctx) {
        //    var client = NotionClientFactory.Create(new ClientOptions {
        //        AuthToken = token
        //    });

        //    try {
        //        List<Task<string>> tasks = new();
        //        var databasesQueryParameters = new DatabasesQueryParameters();
        //        var queryResult = await client.Databases.QueryAsync(databaseId, databasesQueryParameters);

        //        foreach (var result in queryResult.Results) {
        //            Console.WriteLine("Page Id: " + result.Id);
        //            foreach (var property in result.Properties) {
        //                var retrieveCommentsParameters = new RetrieveCommentsParameters() {
        //                    BlockId = result.Id
        //                };
        //                tasks.Add(RetrieveValues(property));
        //                //var comments = await client.Comments.RetrieveAsync(retrieveCommentsParameters);
        //                //foreach (var comment in comments.Results) {
        //                //    var commentText = string.Join(" ", comment.RichText.Select(t => t.PlainText));
        //                //    Console.WriteLine($"Comment: {commentText}");
        //                //}
        //            }
        //        }

        //        var results = await Task.WhenAll(tasks);

        //        // Now you can process the results if needed
        //        foreach (var result in results) {
        //            Console.WriteLine(result);
        //        }

        //    } catch (Exception ex) {
        //        Console.WriteLine(ex);
        //        throw;
        //    }
        //}

        //private async Task<string> RetrieveValues(KeyValuePair<string, PropertyValue> value) {
        //    return value.Key + ": " + await GetValue(value.Value);
        //}

        //private async Task<object> GetValue(PropertyValue p) {
        //    switch (p) {
        //        case TitlePropertyValue titlePropertyValue:
        //            return titlePropertyValue.Title.FirstOrDefault()?.PlainText;
        //        case DatePropertyValue dateProperty:
        //            if (dateProperty.Date != null) {
        //                return dateProperty.Date.Start.GetValueOrDefault().ToString("dd/MM/yyyy");
        //            } else {
        //                return "N/A";
        //            }
        //        default:
        //            return "???";
        //    }
        //}
    }
}
