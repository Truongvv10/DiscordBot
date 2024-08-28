using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Model.Enums {
    public enum CommandEnum {
        [ChoiceName("/embed")]
        EMBED,
        [ChoiceName("/embed-edit")]
        EMBED_EDIT,
        [ChoiceName("/permissions")]
        PERMISSIONS,
        [ChoiceName("/permission-add")]
        PERMISSION_ADD,
        [ChoiceName("/permission-remove")]
        PERMISSION_REMOVE,
        [ChoiceName("/image")]
        IMAGE,
        [ChoiceName("/gradient")]
        GRADIENT,
        [ChoiceName("/events")]
        EVENTS,
        [ChoiceName("/event-create")]
        EVENT_CREATE,
        [ChoiceName("/event-list")]
        EVENT_LIST,
        [ChoiceName("/timestamp")]
        TIMESTAMP,
        [ChoiceName("/broadcast")]
        BROADCAST
    }
}
