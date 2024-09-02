using DiscordBot.Configuration;
using Notion.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services {
    public class NotionService {
        private readonly NotionConfig _notionConfig;
        private readonly NotionClient _notionClient;

        public NotionService(NotionConfig config) {
            _notionConfig = config;
            _notionClient = NotionClientFactory.Create(new ClientOptions {
                AuthToken = _notionConfig.NotionApiKey,
            });
        }
    }
}
