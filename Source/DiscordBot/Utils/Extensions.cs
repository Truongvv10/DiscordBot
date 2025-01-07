using BLL.Model;
using BLL.Exceptions;
using DSharpPlus.Entities;
using BLL.Interfaces;
using System.Reflection;
using DSharpPlus.SlashCommands;

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
                if (embed.Fields != null && embed.Fields.Count > 0)
                    foreach (var field in embed.Fields) builder.AddField(field.Item1, field.Item2, field.Item3);

                return builder;

            } catch (Exception ex) {
                throw new DomainException("An error occurred while building embed", ex);
            }
        }

        public static async Task<Message> TranslatePlaceholders(this Message message, DiscordInteraction interaction, IDataRepository data) {
            try {

                var embed = message.Embed;

                for (int i = 0; i < 2; i++) {
                    if (!string.IsNullOrWhiteSpace(message.Content))
                        message.Content = await Placeholder.Translate(message.Content, message, interaction, data);

                    if (embed != null) {
                        if (!string.IsNullOrWhiteSpace(embed.Title))
                            embed.Title = await Placeholder.Translate(embed.Title, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.TitleUrl))
                            embed.TitleUrl = await Placeholder.Translate(embed.TitleUrl, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.Author))
                            embed.Author = await Placeholder.Translate(embed.Author, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.AuthorUrl))
                            embed.AuthorUrl = await Placeholder.Translate(embed.AuthorUrl, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.AuthorLink))
                            embed.AuthorLink = await Placeholder.Translate(embed.AuthorLink, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.Thumbnail))
                            embed.Thumbnail = await Placeholder.Translate(embed.Thumbnail, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.Image))
                            embed.Image = await Placeholder.Translate(embed.Image, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.Author))
                            embed.Author = await Placeholder.Translate(embed.Author, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.Description))
                            embed.Description = await Placeholder.Translate(embed.Description, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.Footer))
                            embed.Footer = await Placeholder.Translate(embed.Footer, message, interaction, data);

                        if (!string.IsNullOrWhiteSpace(embed.FooterUrl))
                            embed.FooterUrl = await Placeholder.Translate(embed.FooterUrl, message, interaction, data);

                        if (embed.Fields.Count > 0) {
                            var fields = embed.Fields.ToList();
                            embed.ClearFields();
                            foreach (var field in fields) {
                                embed.AddField(
                                    await Placeholder.Translate(field.Item1, message, interaction, data),
                                    await Placeholder.Translate(field.Item2, message, interaction, data),
                                    field.Item3);
                            }
                        }
                    }
                }

                return message;

            } catch (Exception ex) {
                throw new DomainException("An error occurred while building embed", ex);
            }
        }

        public static string GetEnumChoiceName(this Enum value) {
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field != null) {
                var attribute = field.GetCustomAttribute<ChoiceNameAttribute>();
                if (attribute != null) {
                    return attribute.Name;
                }
            }
            return value.ToString(); // Fallback to the enum name
        }
    }
}
