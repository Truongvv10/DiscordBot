using APP.Services;
using APP.Utils;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Model;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Notion.Client;
using REST.Utils;
using System.Reflection.Metadata.Ecma335;

namespace REST.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class DiscordController : ControllerBase {

        private readonly IDataRepository repository;
        private readonly DiscordService discordService;
        private readonly ILogger logger;

        public DiscordController(IDataRepository repository, ILogger<DiscordController> logger, DiscordService discordService) {
            this.repository = repository;
            this.logger = logger;
            this.discordService = discordService;
        }

        [HttpGet("Message/{guildId}/{messageId}")]
        public async Task<ActionResult<string>> GetMessage(ulong guildId, ulong messageId) {
            try {
                var message = await repository.GetMessageAsync(guildId, messageId);
                var result = JsonConvert.SerializeObject(message, Formatting.Indented);
                return Ok(result);
            } catch (ServiceException ex) {
                return BadRequest(ex.Message);
            } catch (Exception ex) {
                return BadRequest(ex);
            }
        }

        [HttpPost("Message/{guildId}/{channelId}")]
        public async Task<ActionResult<object>> CreateMessage(ulong guildId, ulong channelId, [FromBody] object input) {
            try {
                logger.LogInformation($"Creating message in guild {guildId} and channel {channelId}");
                string stringify = input.ToString() ?? throw new Exception("Invalid input");
                Message message = JsonConvert.DeserializeObject<Message>(stringify)!;
                message.GuildId = guildId;
                message.ChannelId = channelId;
                var client = discordService.GetDiscordClient();
                var guild = await client.GetGuildAsync(guildId);
                var channel = guild.GetChannel(channelId);
                var sent = await MessageUtil.SendToChannel(client, channel, message);
                sent = await repository.AddMessageAsync(sent);
                return Ok(JsonConvert.SerializeObject(message));
            } catch (ServiceException ex) {
                return BadRequest(ex.Message);
            } catch (Exception ex) {
                return BadRequest(ex);
            }
        }
    }
}
