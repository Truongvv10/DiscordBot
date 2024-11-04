using Domain.Enums;
using Domain.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Model {
    public class Message {
        #region Private Properties
        private CommandEnum type;
        private ulong? messageId;
        private ulong? channelId;
        private string? context;
        private ulong? sender;
        private Embed? embed;
        private Dictionary<ulong, ulong> childs = new();
        private HashSet<ulong> roles = new();
        #endregion

        #region Constructors
        public Message() {
            type = CommandEnum.MESSAGE;
        }
        public Message(string context) : this() {
            this.context = context;
        }
        #endregion

        #region Properties
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public CommandEnum Type {
            get => type;
            set => type = value;
        }

        [JsonProperty("message_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? MessageId {
            get => messageId;
            set => messageId = value;
        }

        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ChannelId {
            get => channelId;
            set => channelId = value;
        }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string? Content {
            get => context;
            set => context = value;
        }

        [JsonProperty("sender", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? Sender {
            get => sender;
            set => sender = value;
        }

        [JsonProperty("embed", NullValueHandling = NullValueHandling.Ignore)]
        public Embed? Embed {
            get => embed;
            set => embed = value;
        }

        [JsonProperty("childs", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyDictionary<ulong, ulong> Childs {
            get => childs;
            set { foreach (var item in value) { AddChild(item.Key, item.Value); } }
        }

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public HashSet<ulong>? Roles {
            get => roles;
            set => roles = value ?? new HashSet<ulong>();
        }
        #endregion

        #region Methods
        public void SetType(CommandEnum type) {
            this.type = type;
        }
        public Message WithContext(string content) {
            this.context = content;
            return this;
        }
        public Message WithOwner(ulong sender) {
            this.sender = sender;
            return this;
        }
        public Message WithMessageId(ulong messageId) {
            this.messageId = messageId;
            return this;
        }
        public Message WithChannelId(ulong channelId) {
            this.channelId = channelId;
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