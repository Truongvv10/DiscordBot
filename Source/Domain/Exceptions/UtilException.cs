using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions {
    public class UtilException : Exception {
        public UtilException(string? message) : base(message) {
        }
        public UtilException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
