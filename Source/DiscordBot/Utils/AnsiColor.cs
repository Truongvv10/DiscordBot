using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utils {
    public static class AnsiColor {
        public const string RESET = "\u001b[0m";

        // Basic Foreground Colors
        public const string BLACK = "\u001b[30m";
        public const string RED = "\u001b[31m";
        public const string GREEN = "\u001b[32m";
        public const string YELLOW = "\u001b[33m";
        public const string BLUE = "\u001b[34m";
        public const string MAGENTA = "\u001b[35m";
        public const string CYAN = "\u001b[36m";
        public const string WHITE = "\u001b[37m";

        // Bright Foreground Colors
        public const string BRIGHT_BLACK = "\u001b[90m";
        public const string BRIGHT_RED = "\u001b[91m";
        public const string BRIGHT_GREEN = "\u001b[92m";
        public const string BRIGHT_YELLOW = "\u001b[93m";
        public const string BRIGHT_BLUE = "\u001b[94m";
        public const string BRIGHT_MAGENTA = "\u001b[95m";
        public const string BRIGHT_CYAN = "\u001b[96m";
        public const string BRIGHT_WHITE = "\u001b[97m";

        // Basic Background Colors
        public const string BG_BLACK = "\u001b[40m";
        public const string BG_RED = "\u001b[41m";
        public const string BG_GREEN = "\u001b[42m";
        public const string BG_YELLOW = "\u001b[43m";
        public const string BG_BLUE = "\u001b[44m";
        public const string BG_MAGENTA = "\u001b[45m";
        public const string BG_CYAN = "\u001b[46m";
        public const string BG_WHITE = "\u001b[47m";

        // Bright Background Colors
        public const string BG_BRIGHT_BLACK = "\u001b[100m";
        public const string BG_BRIGHT_RED = "\u001b[101m";
        public const string BG_BRIGHT_GREEN = "\u001b[102m";
        public const string BG_BRIGHT_YELLOW = "\u001b[103m";
        public const string BG_BRIGHT_BLUE = "\u001b[104m";
        public const string BG_BRIGHT_MAGENTA = "\u001b[105m";
        public const string BG_BRIGHT_CYAN = "\u001b[106m";
        public const string BG_BRIGHT_WHITE = "\u001b[107m";
    }
}
