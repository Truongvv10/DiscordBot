using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Model.Enums {
    public enum CommandEnum {


        [ChoiceName("/embed")]
        EMBED = 0,
        [ChoiceName("/embed-edit")]
        EMBED_EDIT = 1,


        [ChoiceName("/permissions")]
        PERMISSIONS = 100,


        [ChoiceName("/events")]
        EVENTS = 200,


        [ChoiceName("/timestamp")]
        TIMESTAMP = 300,


        [ChoiceName("/broadcast")]
        BROADCAST = 400,


        [ChoiceName("/notion")]
        NOTION = 500,


        [ChoiceName("/changelog")]
        CHANGELOG = 600
    }
}
