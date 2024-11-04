using Domain.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Model {
    public class Embed {
        #region Private Properties
        private CommandEnum type;
        private string? title;
        private string? titleLink;
        private string? description;
        private string? footer;
        private string? footerUrl;
        private string? author;
        private string? authorLink;
        private string? authorUrl;
        private string? image;
        private string? thumbnail;
        private string? color;
        private long? time;
        private bool hasTimeStamp;
        private bool isEphemeral;
        private Dictionary<string, object> data = new();
        private List<(string, string, bool)> fields = new();
        #endregion

        #region Constructors
        public Embed() {
            type = CommandEnum.EMBED_CREATE;
            color = "#0681cd";
            hasTimeStamp = false;
            isEphemeral = false;
            time = DateTime.Now.Ticks;
        }
        public Embed(string description) : this() {
            this.description = description;
        }
        public Embed(string description, string title) : this(description) {
            this.title = title;
        }
        #endregion

        #region Properties
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public CommandEnum Type {
            get => type;
            set => type = value;
        }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string? Title {
            get => title;
            set => title = value;
        }

        [JsonProperty("title_link", NullValueHandling = NullValueHandling.Ignore)]
        public string? TitleUrl {
            get => titleLink;
            set => titleLink = value;
        }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description {
            get => description;
            set => description = value;
        }

        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        public string? Footer {
            get => footer;
            set => footer = value;
        }

        [JsonProperty("footer_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? FooterUrl {
            get => footerUrl;
            set => footerUrl = value;
        }

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public string? Author {
            get => author;
            set => author = value;
        }

        [JsonProperty("author_link", NullValueHandling = NullValueHandling.Ignore)]
        public string? AuthorLink {
            get => authorLink;
            set => authorLink = value;
        }

        [JsonProperty("author_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? AuthorUrl {
            get => authorUrl;
            set => authorUrl = value;
        }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string? Image {
            get => image;
            set => image = value;
        }

        [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public string? Thumbnail {
            get => thumbnail;
            set => thumbnail = value;
        }

        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string? Color {
            get => color;
            set { if (value != null && Regex.IsMatch(value, @"#[a-fA-F0-9]{6}")) color = value; }
        }

        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasTimeStamp {
            get => hasTimeStamp;
            set => hasTimeStamp = value;
        }

        [JsonProperty("ephemeral", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsEphemeral {
            get => isEphemeral;
            set => isEphemeral = value;
        }

        [JsonProperty("creation_date", NullValueHandling = NullValueHandling.Ignore)]
        public long? Time {
            get => time;
            set => time = value;
        }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyDictionary<string, object> CustomData {
            get => data;
            set { foreach (var item in value) { AddData(item.Key, item.Value); } }
        }

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<(string, string, bool)> Fields {
            get => fields;
            set { foreach (var item in value) { AddField(item.Item1, item.Item2, item.Item3); } }
        }
        #endregion

        #region Methods
        public Embed WithType(CommandEnum type) {
            this.type = type;
            return this;
        }
        public Embed WithTitle(string title) {
            this.title = title;
            return this;
        }
        public Embed WithTitle(string? title = null, string? iconLink = null) {
            this.title = title;
            titleLink = iconLink;
            return this;
        }
        public Embed WithDescription(string description) {
            this.description = description;
            return this;
        }
        public Embed WithFooter(string footer) {
            this.footer = footer;
            return this;
        }
        public Embed WithFooter(string? footer = null, string? iconLink = null) {
            this.footer = footer;
            footerUrl = iconLink;
            return this;
        }
        public Embed WithAuthor(string author) {
            this.author = author;
            return this;
        }
        public Embed WithAuthor(string? author = null, string? iconLink = null) {
            this.author = author;
            authorLink = iconLink;
            return this;
        }
        public Embed WithAuthor(string author, string? authorLink = null, string? iconLink = null) {
            this.author = author;
            this.authorLink = authorLink;
            authorUrl = iconLink;
            return this;
        }
        public Embed WithImage(string image) {
            this.image = image;
            return this;
        }
        public Embed WithAttachment(string image) {
            this.image = image;
            return this;
        }
        public Embed WithThumbnail(string thumbnail) {
            this.thumbnail = thumbnail;
            return this;
        }
        public Embed WithColor(string color) {
            this.color = Regex.IsMatch(color, @"#[a-fA-F0-9]{6}") ? color : null;
            return this;
        }
        public Embed WithHastimestamp(bool hasTimeStamp) {
            this.hasTimeStamp = hasTimeStamp;
            return this;
        }
        public Embed WithIsEphemeral(bool isEphemeral) {
            this.isEphemeral = isEphemeral;
            return this;
        }
        public Embed WithTime(long time) {
            this.time = time;
            return this;
        }
        public Embed AddData(string customKey, object customValue) {
            if (!data.TryAdd(customKey, customValue)) {
                SetData(customKey, customValue);
            };
            return this;
        }
        public Embed RemoveData(string customKey) {
            data.Remove(customKey);
            return this;
        }
        public Embed SetData(string id, object value) {
            if (data.ContainsKey(id)) {
                data[id] = value;
            } else {
                AddData(id, value);
            }
            return this;
        }
        public Embed ClearData() {
            data.Clear();
            return this;
        }
        public Embed AddField(string title, string description) {
            fields.Add((title, description, true));
            return this;
        }
        public Embed AddField(string title, string description, bool isInline) {
            fields.Add((title, description, isInline));
            return this;
        }
        public Embed RemoveFieldAt(int index) {
            fields.RemoveAt(index);
            return this;
        }
        public Embed removeFieldRange(int index, int count) {
            fields.RemoveRange(index, count);
            return this;
        }
        public Embed ClearFields() {
            fields.Clear();
            return this;
        }
        #endregion
    }
}
