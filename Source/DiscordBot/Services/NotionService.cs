using DiscordBot.Configuration;
using Notion.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiscordBot.Services {
    public class NotionService {

        #region Private Properties
        private readonly NotionConfig _notionConfig;
        private readonly NotionClient _notionClient;
        #endregion

        #region Constructors
        public NotionService(NotionConfig config) {
            _notionConfig = config;
            _notionClient = NotionClientFactory.Create(new ClientOptions {
                AuthToken = _notionConfig.NotionApiKey,
            });
        }
        #endregion

        #region Methods
        private async Task<Dictionary<string, string>> FetchAllDataAsync(string databaseId) {

            // Output
            var fetchedData = new Dictionary<string, string>();

            // Database querries
            var parameters = new DatabasesQueryParameters();
            var queryResult = await _notionClient.Databases.QueryAsync(databaseId, parameters);

            // Result of all pages
            foreach (var result in queryResult.Results) {
                foreach (var property in result.Properties) {
                    var retrieveCommentsParameters = new RetrieveCommentsParameters() { BlockId = result.Id };
                    //var comments = await _notionClient.Comments.RetrieveAsync(retrieveCommentsParameters);
                    //foreach (var comment in comments.Results) {
                    //    var commentText = string.Join(" ", comment.RichText.Select(t => t.PlainText));
                    //    Console.WriteLine($"Comment: {commentText}");
                    //}
                    fetchedData.Add(property.Key, await GetValueAsync(property.Value));
                }
            }

            // Return fetched data
            return fetchedData;
        }

        private async Task<string> GetValueAsync(PropertyValue p) {

            // Property has empty attributes output
            const string notFound = "N/A";

            // Searching what the proprty type is
            switch (p) {
                case TitlePropertyValue titlePropertyValue:
                    if (titlePropertyValue != null) {
                        return titlePropertyValue.Title.FirstOrDefault()?.PlainText;
                    } else return notFound;
                case DatePropertyValue dateProperty:
                    if (dateProperty.Date != null) {
                        return dateProperty.Date.Start.GetValueOrDefault().ToString("dd/MM/yyyy");
                    } else return notFound;
                default:
                    return notFound;
            }
        }
        #endregion
    }
}
