using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Exceptions {
    public class ListenerException : Exception {
        public ListenerException(string message) : base(message) {
        }
        public ListenerException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
