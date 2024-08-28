using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Exceptions {
    public class CommandException : Exception {
        public CommandException(string message) : base(message) {
        }
        public CommandException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
