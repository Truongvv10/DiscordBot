using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums {
    public enum CommandEnum {
        [Description("/message")]
        MESSAGE = -2,
        [Description("/none")]
        NONE = -1,


        [Description("/embed")]
        EMBED = 0,
        [Description("/embed-create")]
        EMBED_CREATE = 1,
        [Description("/embed-edit")]
        EMBED_EDIT = 2,


        [Description("/embed-template")]
        TEMPLATE = 50,
        [Description("/embed-template-create")]
        TEMPLATE_CREATE = 51,
        [Description("/embed-template-edit")]
        TEMPLATE_EDIT = 52,
        [Description("/embed-template-use")]
        TEMPLATE_USE = 53,


        [Description("/permissions")]
        PERMISSIONS = 100,


        [Description("/events")]
        EVENTS = 200,
        [Description("/events-create")]
        EVENTS_CREATE = 201,
        [Description("/events-edit")]
        EVENTS_EDIT = 202,
        [Description("/events-reminder")]
        EVENTS_REMINDER = 203,
        [Description("/events-winners")]
        EVENTS_WINNERS = 204,


        [Description("/timestamp")]
        TIMESTAMP = 300,

        [Description("/nitro")]
        NITRO = 305,


        [Description("/broadcast")]
        BROADCAST = 400,


        [Description("/notion")]
        NOTION = 500,


        [Description("/changelog")]
        CHANGELOG = 600
    }
}
