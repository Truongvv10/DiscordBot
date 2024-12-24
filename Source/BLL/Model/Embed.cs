using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Model {
    public class Embed {
        #region Private Properties
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
        private List<(string, string, bool)> fields = new();
        #endregion

        #region Constructors
        public Embed() {
            color = "#0681cd";
            hasTimeStamp = false;
            time = DateTime.Now.Ticks;
        }
        public Embed(string description) : this() {
            this.description = description;
        }
        public Embed(string description, string title) : this(description) {
            this.title = title;
        }
        public Embed(string description, string title, string footer) : this(description, title) {
            this.footer = footer;
        }
        #endregion

        #region Properties
        [Column("message_id", TypeName = "decimal(20, 0)")]
        [JsonIgnore]
        public ulong MessageId {
            get;
            set;
        }

        [Column("guild_id", TypeName = "decimal(20, 0)")]
        [JsonIgnore]
        public ulong GuildId {
            get;
            set;
        }
            
        [JsonIgnore]
        public Message Message {
            get;
            set;
        }

        [Column("title")]
        [MaxLength(250)]
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string? Title {
            get => title;
            set => title = value;
        }

        [Column("title_link")]
        [MaxLength(250)]
        [JsonProperty("title_link", NullValueHandling = NullValueHandling.Ignore)]
        public string? TitleUrl {
            get => titleLink;
            set => titleLink = value;
        }

        [Column("description")]
        [MaxLength(4000)]
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description {
            get => description;
            set => description = value;
        }

        [Column("footer")]
        [MaxLength(250)]
        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        public string? Footer {
            get => footer;
            set => footer = value;
        }

        [Column("footer_url")]
        [MaxLength(250)]
        [JsonProperty("footer_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? FooterUrl {
            get => footerUrl;
            set => footerUrl = value;
        }

        [Column("author")]
        [MaxLength(250)]
        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public string? Author {
            get => author;
            set => author = value;
        }

        [Column("author_link")]
        [MaxLength(250)]
        [JsonProperty("author_link", NullValueHandling = NullValueHandling.Ignore)]
        public string? AuthorLink {
            get => authorLink;
            set => authorLink = value;
        }

        [Column("author_url")]
        [MaxLength(250)]
        [JsonProperty("author_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? AuthorUrl {
            get => authorUrl;
            set => authorUrl = value;
        }

        [Column("image")]
        [MaxLength(250)]
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string? Image {
            get => image;
            set => image = value;
        }

        [Column("thumbnail")]
        [MaxLength(250)]
        [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public string? Thumbnail {
            get => thumbnail;
            set => thumbnail = value;
        }

        [Column("color")]
        [MaxLength(10)]
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string? Color {
            get => color;
            set { if (value != null && Regex.IsMatch(value, @"#[a-fA-F0-9]{6}")) color = value; }
        }

        [Column("timestamp")]
        [Required]
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasTimeStamp {
            get => hasTimeStamp;
            set => hasTimeStamp = value;
        }

        [Column("fields")]
        [MaxLength]
        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<(string, string, bool)> Fields {
            get => fields;
            set {
                foreach (var item in value) { AddField(item.Item1, item.Item2, item.Item3); } 
            }
        }
        #endregion

        #region Methods
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
        public Embed WithTime(long time) {
            this.time = time;
            return this;
        }
        public Embed AddField(string title, string description, bool isInline = true) {
            fields.Add((title, description, isInline));
            return this;
        }
        public Embed AddFields(List<(string, string, bool)> fields) {
            foreach (var field in fields) {
                AddField(field.Item1, field.Item2, field.Item3);
            }
            return this;
        }
        public Embed InsertFieldAt(int index, string title, string description, bool isInline = true) {
            fields.Insert(index, (title, description, isInline));
            return this;
        }
        public Embed ReplaceFieldAt(int index, string title, string description, bool isInline = true) {
            fields[index] = (title, description, isInline);
            return this;
        }
        public Embed RemoveFieldAt(int index) {
            fields.RemoveAt(index);
            return this;
        }
        public Embed RemoveFieldRange(int index, int count) {
            fields.RemoveRange(index, count);
            return this;
        }
        public Embed SetFields(List<(string, string, bool)> fields) {
            fields.Clear();
            foreach (var field in fields) {
                AddField(field.Item1, field.Item2, field.Item3);
            }
            return this;
        }
        public Embed ClearFields() {
            fields.Clear();
            return this;
        }
        #endregion
    }
}
