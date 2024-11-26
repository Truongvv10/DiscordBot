using BLL.Enums;
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
    public class Message {
        #region Fields
        private CommandEnum type;
        private ulong channelId;
        private string? content;
        private ulong? sender;
        private bool isEphemeral;
        private DateTime? creationDate;
        private Dictionary<string, string> data = new();
        private Dictionary<ulong, ulong> childs = new();
        private List<ulong> roles = new();
        private Embed embed;
        #endregion

        #region Constructors
        public Message() {
            type = CommandEnum.MESSAGE;
            isEphemeral = false;
        }
        public Message(string content) : this() {
            this.content = content;
        }
        public Message(ulong guildId, ulong messageId, string content) : this(content) {
            GuildId = guildId;
            MessageId = messageId;
        }
        #endregion

        #region Properties
        [Column("message_id", TypeName = "decimal(20, 0)")]
        [JsonProperty("message_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong MessageId {
            get;
            set;
        }

        [Column("channel_id", TypeName = "decimal(20, 0)")]
        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong ChannelId {
            get => channelId;
            set => channelId = value;
        }

        [Column("guild_id", TypeName = "decimal(20, 0)")]
        [JsonProperty("guild_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong GuildId { 
            get; 
            set;
        }

        [Column("guild")]
        [JsonIgnore]
        public Guild guild {
            get;
            set;
        }

        [Column("template")]
        [JsonIgnore]
        public Template Template {
            get;
            set;
        }

        [Column("content")]
        [MaxLength(4000)]
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string? Content {
            get => content;
            set => content = value;
        }

        [Column("sender", TypeName = "decimal(20, 0)")]
        [JsonProperty("sender", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? Sender {
            get => sender;
            set => sender = value;
        }

        [Column("ephemeral")]
        [JsonProperty("ephemeral", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsEphemeral {
            get => isEphemeral;
            set => isEphemeral = value;
        }

        [Column("creation")]
        [JsonProperty("creation", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreationDate {
            get => creationDate;
            set => creationDate = value;
        }

        [Column("data", TypeName = "nvarchar(max)")]
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Data {
            get => data;
            set { foreach (var item in value) { AddData(item.Key, item.Value); } }
        }

        [Column("children")]
        [JsonProperty("childs", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<ulong, ulong> Childs {
            get => childs;
            set { foreach (var item in value) { AddChild(item.Key, item.Value); } }
        }

        [Column("roles")]
        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<ulong> Roles {
            get => roles;
            set => roles = value != null ? new List<ulong>(value) : new List<ulong>();
        }

        [Column("type")]
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public CommandEnum Type {
            get => type;
            set => type = value;
        }

        [JsonProperty("embed", NullValueHandling = NullValueHandling.Ignore)]
        public Embed Embed {
            get => embed;
            set => embed = value != null ? value : new Embed();
        }
        #endregion

        #region Methods
        public Message SetType(CommandEnum type) {
            this.type = type;
            return this;
        }
        public Message WithContext(string content) {
            this.content = content;
            return this;
        }
        public Message SetEphemeral(bool isEphemeral) {
            this.isEphemeral = isEphemeral;
            return this;
        }
        public Message WithOwner(ulong sender) {
            this.sender = sender;
            return this;
        }
        public Message WithMessageId(ulong messageId) {
            MessageId = messageId;
            return this;
        }
        public Message WithChannelId(ulong channelId) {
            this.channelId = channelId;
            return this;
        }
        public Message AddData(string customKey, string customValue) {
            if (!data.TryAdd(customKey, customValue)) {
                SetData(customKey, customValue);
            };
            return this;
        }
        public Message RemoveData(string customKey) {
            data.Remove(customKey);
            return this;
        }
        public Message SetData(string id, string value) {
            if (data.ContainsKey(id)) {
                data[id] = value;
            } else {
                AddData(id, value);
            }
            return this;
        }
        public Message ClearData() {
            data.Clear();
            return this;
        }
        public Message AddChild(ulong messageId, ulong channelId) {
            childs.Add(messageId, channelId);
            return this;
        }
        public Message RemoveChild(ulong messageId) {
            childs.Remove(messageId);
            return this;
        }
        public Message ClearChilds() {
            childs.Clear();
            return this;
        }
        public Message AddRole(ulong roleId) {
            roles.Add(roleId);
            return this;
        }
        public Message SetRole(ulong roleId) {
            roles.Clear();
            roles.Add(roleId);
            return this;
        }
        public Message SetRole(ulong[] roleIds) {
            roles.Clear();
            if (roleIds.Count() > 0)
                foreach (var id in roleIds) roles.Add(id);
            return this;
        }
        public Message RemoveRole(ulong roleId) {
            roles.Remove(roleId);
            return this;
        }
        public Message ClearRoles() {
            roles.Clear();
            return this;
        }
        public Message WithEmbed(Embed embed) {
            this.embed = embed;
            return this;
        }
        public Message DeepClone() {
            string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return JsonConvert.DeserializeObject<Message>(json, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto
            })!;
        }
        #endregion
    }
}