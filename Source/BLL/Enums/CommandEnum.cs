using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Enums {
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
        [Description("/embed-templates")]
        EMBED_TEMPLATES = 3,



        [Description("/embed-template")]
        TEMPLATES = 50,
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
        [Description("/events-setup")]
        EVENTS_SETUP = 205,


        [Description("/timestamp")]
        TIMESTAMP = 300,

        [Description("/nitro")]
        NITRO = 305,


        [Description("/broadcast")]
        BROADCAST = 400,


        [Description("/notion")]
        NOTION = 500,


        [Description("/changelog")]
        CHANGELOG = 600,


        [Description("/settings")]
        SETTINGS = 700,
        [Description("/settings set welcome")]
        SETTINGS_SET_WELCOME = 720,
        [Description("/settings set introduction")]
        SETTINGS_SET_INTRODUCTION = 721,
        [Description("/settings set log")]
        SETTINGS_SET_LOG = 722,
        [Description("/settings set changelog")]
        SETTINGS_SET_CHANEGLOG = 723,
        [Description("/settings set inactivity")]
        SETTINGS_SET_INACTIVITY = 724,
        [Description("/settings set punishment")]
        SETTINGS_SET_PUNISHMENT = 725,
        [Description("/settings set strike")]
        SETTINGS_SET_STRIKE = 726,
        [Description("/settings set verify")]
        SETTINGS_SET_VERIFY = 727,
        [Description("/settings remove welcome")]
        SETTINGS_REMOVE_WELCOME = 740,
        [Description("/settings remove introduction")]
        SETTINGS_REMOVE_INTRODUCTION = 741,
        [Description("/settings remove log")]
        SETTINGS_REMOVE_LOG = 742,
        [Description("/settings remove changelog")]
        SETTINGS_REMOVE_CHANEGLOG = 743,
        [Description("/settings remove inactivity")]
        SETTINGS_REMOVE_INACTIVITY = 744,
        [Description("/settings remove punishment")]
        SETTINGS_REMOVE_PUNISHMENT = 745,
        [Description("/settings remove strike")]
        SETTINGS_REMOVE_STRIKE = 746,
        [Description("/settings remove verify")]
        SETTINGS_REMOVE_VERIFY = 747,
    }
}
