using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Model.Enums {
    public enum EmbedType {
        CUSTOM = 0,
        BROADCAST = 10,
        EVENT = 80,
        EVENT_WINNERS = 81,
        EVENT_REMINDER = 82,
        CHANGELOG = 90,
    }
}
