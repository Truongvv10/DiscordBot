﻿using BLL.Model;
using BLL.Exceptions;
using DSharpPlus.Entities;

namespace APP.Utils {
    public static class Extensions {
        public static DiscordEmbed Build(this Embed embed) {
            try {
                var builder = new DiscordEmbedBuilder();

                if (!string.IsNullOrWhiteSpace(embed.Title))
                    builder.WithTitle(embed.Title);
                if (!string.IsNullOrWhiteSpace(embed.TitleUrl))
                    builder.WithUrl(embed.TitleUrl);
                if (!string.IsNullOrWhiteSpace(embed.Description))
                    builder.WithDescription(embed.Description);
                if (!string.IsNullOrWhiteSpace(embed.Footer))
                    builder.WithFooter(embed.Footer, embed.FooterUrl);
                if (!string.IsNullOrWhiteSpace(embed.Author))
                    builder.WithAuthor(embed.Author, embed.AuthorLink, embed.AuthorUrl);
                if (!string.IsNullOrWhiteSpace(embed.Image))
                    builder.WithImageUrl(embed.Image);
                if (!string.IsNullOrWhiteSpace(embed.Thumbnail))
                    builder.WithThumbnail(embed.Thumbnail);
                if (!string.IsNullOrWhiteSpace(embed.Color))
                    builder.WithColor(new DiscordColor(embed.Color));
                if (embed.HasTimeStamp)
                    builder.WithTimestamp(DateTime.Now);
                if (embed.Fields.Count > 0)
                    foreach (var field in embed.Fields) builder.AddField(field.Item1, field.Item2, field.Item3);

                return builder;

            } catch (Exception ex) {
                throw new DomainException("An error occurred while building embed", ex);
            }
        }
        public static async Task<Message> TranslatePlaceholders(this Message message) {
            try {

                var embed = message.Embed;

                if (!string.IsNullOrWhiteSpace(message.Content))
                    message.Content = await Placeholder.Translate(message.Content, message.Data);

                if (!string.IsNullOrWhiteSpace(embed.Title))
                    embed.Title = await Placeholder.Translate(embed.Title, message.Data);

                if (!string.IsNullOrWhiteSpace(embed.TitleUrl))
                    embed.TitleUrl = await Placeholder.Translate(embed.TitleUrl, message.Data);

                if (!string.IsNullOrWhiteSpace(embed.Description))
                    embed.Description = await Placeholder.Translate(embed.Description, message.Data);

                if (!string.IsNullOrWhiteSpace(embed.Footer))
                    embed.Footer = await Placeholder.Translate(embed.Footer, message.Data);

                if (!string.IsNullOrWhiteSpace(embed.FooterUrl))
                    embed.FooterUrl = await Placeholder.Translate(embed.FooterUrl, message.Data);

                if (embed.Fields.Count > 0) {
                    var fields = embed.Fields.ToList();
                    embed.ClearFields();
                    foreach (var field in fields) { 
                        embed.AddField(
                            await Placeholder.Translate(field.Item1, message.Data), 
                            await Placeholder.Translate(field.Item2, message.Data), 
                            field.Item3);
                    }
                }

                return message;

            } catch (Exception ex) {
                throw new DomainException("An error occurred while building embed", ex);
            }
        }

    }
}
