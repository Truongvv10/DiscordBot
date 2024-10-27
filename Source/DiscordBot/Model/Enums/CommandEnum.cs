﻿using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Model.Enums {
    public enum CommandEnum {


        [ChoiceName("/embed")]
        EMBED = 0,
        [ChoiceName("/embed-create")]
        EMBED_CREATE = 1,
        [ChoiceName("/embed-edit")]
        EMBED_EDIT = 2,


        [ChoiceName("/permissions")]
        PERMISSIONS = 100,


        [ChoiceName("/events")]
        EVENTS = 200,
        [ChoiceName("/events-create")]
        EVENTS_CREATE = 201,
        [ChoiceName("/events-edit")]
        EVENTS_EDIT = 202,
        [ChoiceName("/events-reminder")]
        EVENTS_REMINDER = 203,
        [ChoiceName("/events-winners")]
        EVENTS_WINNERS = 204,


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
