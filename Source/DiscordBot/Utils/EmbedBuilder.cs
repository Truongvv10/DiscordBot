using DiscordBot.Exceptions;
using DiscordBot.Model.Enums;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Utils {
    public class EmbedBuilder {
        #region Private Properties
        private CommandEnum _type;
        private string? _title;
        private string? _titleLink;
        private string? _description;
        private string? _content;
        private string? _footer;
        private string? _footerUrl;
        private string? _author;
        private string? _authorLink;
        private string? _authorUrl;
        private string? _image;
        private string? _thumbnail;
        private string? _color;
        private bool _hasTimeStamp;
        private long? _time;
        private bool _isEphemeral;
        private ulong? _owner;
        private ulong? _messageId;
        private ulong? _channelId;
        private Dictionary<string, object> _customSaves = new();
        private Dictionary<ulong, ulong> _childEmbeds = new();
        private HashSet<ulong> _pingRoles = new();
        private List<(string, string, bool)> _fields = new();
        #endregion

        #region Constructors
        public EmbedBuilder() {
            _type = CommandEnum.EMBED_CREATE;
            _color = "#0681cd";
            _hasTimeStamp = false;
            _isEphemeral = false;
            _time = DateTime.Now.Ticks;
        }
        public EmbedBuilder(string description) : this() {
            _description = description;
        }
        public EmbedBuilder(string description, string title) : this(description) {
            _title = title;
        }
        #endregion

        #region Properties
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public CommandEnum Type {
            get => _type;
            set => _type = value;
        }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string? Title {
            get => _title;
            set => _title = value;
        }

        [JsonProperty("title_link", NullValueHandling = NullValueHandling.Ignore)]
        public string? TitleUrl {
            get => _titleLink;
            set => _titleLink = value;
        }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description {
            get => _description;
            set => _description = value;
        }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string? Content {
            get => _content;
            set => _content = value;
        }

        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        public string? Footer {
            get => _footer;
            set => _footer = value;
        }

        [JsonProperty("footer_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? FooterUrl {
            get => _footerUrl;
            set => _footerUrl = value;
        }

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public string? Author {
            get => _author;
            set => _author = value;
        }

        [JsonProperty("author_link", NullValueHandling = NullValueHandling.Ignore)]
        public string? AuthorLink {
            get => _authorLink;
            set => _authorLink = value;
        }

        [JsonProperty("author_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? AuthorUrl {
            get => _authorUrl;
            set => _authorUrl = value;
        }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string? Image {
            get => _image;
            set => _image = value;
        }

        [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public string? Thumbnail {
            get => _thumbnail;
            set => _thumbnail = value;
        }

        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string? Color {
            get => _color;
            set { if (value != null && Regex.IsMatch(value, @"#[a-fA-F0-9]{6}")) _color = value; }
        }

        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasTimeStamp {
            get => _hasTimeStamp;
            set => _hasTimeStamp = value;
        }

        [JsonProperty("ephemeral", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsEphemeral {
            get => _isEphemeral;
            set => _isEphemeral = value;
        }

        [JsonProperty("creation_date", NullValueHandling = NullValueHandling.Ignore)]
        public long? Time {
            get => _time;
            set => _time = value;
        }

        [JsonProperty("sender", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? Owner {
            get => _owner;
            set => _owner = value;
        }

        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ChannelId {
            get => _channelId;
            set => _channelId = value;
        }

        [JsonProperty("message_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? MessageId {
            get => _messageId;
            set => _messageId = value;
        }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyDictionary<string, object> CustomSaves {
            get => _customSaves;
            set { foreach (var item in value) { AddCustomSaveMessage(item.Key, item.Value); } }
        }

        [JsonProperty("childs", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyDictionary<ulong, ulong> ChildEmbeds {
            get => _childEmbeds;
            set { foreach (var item in value) { AddCopiedMessage(item.Key, item.Value); } }
        }

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public HashSet<ulong> PingRoles {
            get => _pingRoles;
            set => _pingRoles = value ?? new HashSet<ulong>();
        }

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<(string, string, bool)> Fields {
            get => _fields;
            set { foreach (var item in value) { AddField(item.Item1, item.Item2, item.Item3); } }
        }
        #endregion

        #region Methods
        public EmbedBuilder WithType(CommandEnum type) {
            _type = type;
            return this;
        }
        public EmbedBuilder WithTitle(string title) {
            _title = title;
            return this;
        }
        public EmbedBuilder WithTitle(string? title = null, string? iconLink = null) {
            _title = title;
            _titleLink = iconLink;
            return this;
        }
        public EmbedBuilder WithDescription(string description) {
            _description = description;
            return this;
        }
        public EmbedBuilder WithContent(string content) {
            _content = content;
            return this;
        }
        public EmbedBuilder WithFooter(string footer) {
            _footer = footer;
            return this;
        }
        public EmbedBuilder WithFooter(string? footer = null, string? iconLink = null) {
            _footer = footer;
            _footerUrl = iconLink;
            return this;
        }
        public EmbedBuilder WithAuthor(string author) {
            _author = author;
            return this;
        }
        public EmbedBuilder WithAuthor(string? author = null, string? iconLink = null) {
            _author = author;
            _authorLink = iconLink;
            return this;
        }
        public EmbedBuilder WithAuthor(string author, string? authorLink = null, string? iconLink = null) {
            _author = author;
            _authorLink = authorLink;
            _authorUrl = iconLink;
            return this;
        }
        public EmbedBuilder WithImage(string image) {
            _image = image;
            return this;
        }
        public EmbedBuilder WithAttachment(string image) {
            _image = image;
            return this;
        }
        public EmbedBuilder WithThumbnail(string thumbnail) {
            _thumbnail = thumbnail;
            return this;
        }
        public EmbedBuilder WithColor(string color) {
            _color = Regex.IsMatch(color, @"#[a-fA-F0-9]{6}") ? color : null;
            return this;
        }
        public EmbedBuilder WithHastimestamp(bool hasTimeStamp) {
            _hasTimeStamp = hasTimeStamp;
            return this;
        }
        public EmbedBuilder WithIsEphemeral(bool isEphemeral) {
            _isEphemeral = isEphemeral;
            return this;
        }
        public EmbedBuilder WithTime(long time) {
            _time = time;
            return this;
        }
        public EmbedBuilder WithOwner(ulong owner) {
            _owner = owner;
            return this;
        }
        public EmbedBuilder WithMessageId(ulong messageId) {
            _messageId = messageId;
            return this;
        }
        public EmbedBuilder WithChannelId(ulong channelId) {
            _channelId = channelId;
            return this;
        }
        public EmbedBuilder AddCustomSaveMessage(string customKey, object customValue) {
            _customSaves.Add(customKey, customValue);
            return this;
        }
        public EmbedBuilder RemoveCustomSaveMessage(string customKey) {
            _customSaves.Remove(customKey);
            return this;
        }
        public EmbedBuilder SetCustomSaveMessage(string id, object value) {
            if (_customSaves.TryGetValue(id, out _)) {
                _customSaves[id] = value;
            }
            return this;
        }
        public EmbedBuilder ClearCustomSaveMessages() {
            _customSaves.Clear();
            return this;
        }
        public EmbedBuilder AddCopiedMessage(ulong messageId, ulong channelId) {
            _childEmbeds.Add(messageId, channelId);
            return this;
        }
        public EmbedBuilder RemoveCopiedMessage(ulong messageId) {
            _childEmbeds.Remove(messageId);
            return this;
        }
        public EmbedBuilder ClearCopiedMessages() {
            _childEmbeds.Clear();
            return this;
        }
        public EmbedBuilder AddField(string title, string description) {
            _fields.Add((title, description, true));
            return this;
        }
        public EmbedBuilder AddField(string title, string description, bool isInline) {
            _fields.Add((title, description, isInline));
            return this;
        }
        public EmbedBuilder RemoveFieldAt(int index) {
            _fields.RemoveAt(index);
            return this;
        }
        public EmbedBuilder removeFieldRange(int index, int count) {
            _fields.RemoveRange(index, count);
            return this;
        }
        public EmbedBuilder ClearFields() {
            _fields.Clear();
            return this;
        }
        public EmbedBuilder AddPingRole(ulong roleId) {
            _pingRoles.Add(roleId);
            return this;
        }
        public EmbedBuilder SetPingRoles(ulong[] roleIds) {
            _pingRoles.Clear();
            if (roleIds.Count() > 0)
                foreach (var roleid in roleIds)
                    _pingRoles.Add(roleid);
            return this;
        }
        public EmbedBuilder RemovePingRole(ulong roleId) {
            _pingRoles.Remove(roleId);
            return this;
        }
        public EmbedBuilder ClearPingRoles() {
            _pingRoles.Clear();
            return this;
        }
        public DiscordEmbed Build() {
            try {

                var embed = new DiscordEmbedBuilder();

                if (!string.IsNullOrWhiteSpace(_title))
                    embed.WithTitle(_title);
                if (!string.IsNullOrWhiteSpace(_titleLink))
                    embed.WithUrl(_titleLink);
                if (!string.IsNullOrWhiteSpace(_description))
                    embed.WithDescription(_description);
                if (!string.IsNullOrWhiteSpace(_footer))
                    embed.WithFooter(_footer, _footerUrl is null ? null : _footerUrl);
                if (!string.IsNullOrWhiteSpace(_author))
                    embed.WithAuthor(_author, _authorLink is null ? null : _authorLink, _authorUrl is null ? null : _authorUrl);
                if (!string.IsNullOrWhiteSpace(_image))
                    embed.WithImageUrl(_image);
                if (!string.IsNullOrWhiteSpace(_thumbnail))
                    embed.WithThumbnail(_thumbnail);
                if (!string.IsNullOrWhiteSpace(_color))
                    embed.WithColor(new DiscordColor(_color));
                if (_hasTimeStamp)
                    embed.WithTimestamp(DateTime.Now);
                if (_fields.Count > 0)
                    foreach (var field in _fields) embed.AddField(field.Item1, field.Item2, field.Item3);

                return embed.Build();

            } catch (Exception ex) {
                throw new DomainException("An error occurred while building embed", ex);
            }
        }

        public EmbedBuilder DeepClone() {
            var clone = new EmbedBuilder {
                _type = _type,
                _content = _content,
                _title = _title,
                _titleLink = _titleLink,
                _description = _description,
                _footer = _footer,
                _footerUrl = _footerUrl,
                _author = _author,
                _authorLink = _authorLink,
                _authorUrl = _authorUrl,
                _image = _image,
                _thumbnail = _thumbnail,
                _color = _color,
                _hasTimeStamp = _hasTimeStamp,
                _isEphemeral = _isEphemeral,
                _time = _time,
                _owner = _owner,
                _messageId = _messageId,
                _channelId = _channelId,
                _childEmbeds = _childEmbeds,
                _pingRoles = _pingRoles,
                _fields = _fields,
            };
            return clone;
        }
        public EmbedBuilder DeepClone2() {
            string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return JsonConvert.DeserializeObject<EmbedBuilder>(json, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto
            })!;
        }
        #endregion
    }
}
