using BLL.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace BLL.Model
{
    public class Template {

        #region Fields
        private string name;
        private Message message;
        #endregion

        #region Constructors
        public Template() { }
        public Template(string name, Message message) {
            this.name = name;
            this.message = message;
        }
        #endregion

        #region Properties
        [Column("message_id", TypeName = "decimal(20, 0)")]
        [JsonIgnore]
        public ulong MessageId { get; set; }

        [Column("guild_id", TypeName = "decimal(20, 0)")]
        [JsonIgnore]
        public ulong GuildId { get; set; }

        [Column("template")]
        [MaxLength(32)]
        [JsonProperty("template", NullValueHandling = NullValueHandling.Ignore)]
        public string Name {
            get => name;
            set {
                if (string.IsNullOrWhiteSpace(value) && value.Length <= 32) throw new DomainException($"Template with name \"{name}\" can not be empty or null.");
                else name = value.Replace(" ", "_").ToUpper();
            }
        }

        [Required]
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public Message Message {
            get => message;
            set {
                if (value is null) throw new DomainException($"Template message can not be null.");
                else message = value;
            }
        }
        #endregion
    }
}
