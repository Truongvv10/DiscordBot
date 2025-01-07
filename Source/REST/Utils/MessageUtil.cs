using APP.Utils;
using BLL.Model;
using DSharpPlus.Entities;
using DSharpPlus;
using BLL.Exceptions;
using System.Text.RegularExpressions;

namespace REST.Utils {
    public static class MessageUtil {
        public static async Task<Message> SendToChannel(DiscordClient client, DiscordChannel channel, Message message) {
            var response = ResolveImageAttachment(message);
            if (message.Users.Count() > 0) {
                var mentions = message.Users.Select(x => new UserMention(x)).ToList();
                response.AddMentions(mentions.Cast<IMention>());
            }
            if (message.Roles.Count() > 0) {
                var roles = message.Roles.Select(x => new RoleMention(x)).ToList();
                response.AddMentions(roles.Cast<IMention>());
            }
            if (!string.IsNullOrWhiteSpace(message.Content)) response.WithContent(message.Content);
            var sentMessage = await channel.SendMessageAsync(response);
            message.AddChild(sentMessage.Id, sentMessage.Channel.Id);
            var sentMessageClone = message.DeepClone();
            sentMessageClone.MessageId = sentMessage.Id;
            sentMessageClone.ChannelId = sentMessage.Channel.Id;
            sentMessageClone.Sender = 0;
            sentMessageClone.ClearChilds();
            return sentMessageClone;
        }

        public static DiscordMessageBuilder ResolveImageAttachment(Message message) {
            var embed = message.Embed;
            var response = new DiscordMessageBuilder();
            var folder = Path.Combine(Environment.CurrentDirectory, "Saves", "Images");
            var pattern = @"^local:\/\/.*";
            var patternAttachment = @"^attachment:\/\/.*";
            var replace = "local://";
            var replace2 = "attachment://";

            try {
                if (embed is not null) {
                    if (embed.Image != null && (Regex.IsMatch(embed.Image, pattern) || embed.Image != null && Regex.IsMatch(embed.Image, patternAttachment))) {
                        var image = Path.Combine(folder, embed.Image.Replace(replace, "").Replace(replace2, ""));
                        var imageBytes = File.ReadAllBytes(image);
                        var imageStream = new MemoryStream(imageBytes);
                        var fileName = Path.GetFileName(image);
                        embed.WithImage($"attachment://{fileName}");
                        response.AddFile(fileName, imageStream);
                    }

                    if (embed.Thumbnail != null && (Regex.IsMatch(embed.Thumbnail, pattern) || embed.Thumbnail != null && Regex.IsMatch(embed.Thumbnail, patternAttachment))) {
                        var thumbnail = Path.Combine(folder, embed.Thumbnail.Replace(replace, "").Replace(replace2, ""));
                        var thumbnailBytes = File.ReadAllBytes(thumbnail);
                        var thumbnailStream = new MemoryStream(thumbnailBytes);
                        var fileName = Path.GetFileName(thumbnail);
                        embed.WithThumbnail($"attachment://{fileName}");
                        response.AddFile(fileName, thumbnailStream);
                    }

                    return response
                        .AddEmbed(embed.Build());
                } else return response;

            } catch (Exception ex) {
                throw new CommandException($"Embed.CheckIfLocalImage: {ex.Message}", ex);
            }
        }
    }
}
