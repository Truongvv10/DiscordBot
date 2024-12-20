using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Enums {
    public enum PronounsEnum {
        [ChoiceName("Prefer not to say")]
        None,
        [ChoiceName("He/Him")]
        HeHim,
        [ChoiceName("She/Her")]
        SheHer,
        [ChoiceName("They/Them")]
        TheyThem,
        [ChoiceName("Other")]
        Other
    }
}
