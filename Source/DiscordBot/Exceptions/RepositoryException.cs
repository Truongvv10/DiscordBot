﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XironiteDiscordBot.Exceptions {
    public class RepositoryException : Exception {
        public RepositoryException(string message) : base(message) {
        }
        public RepositoryException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
