using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utils {
    public  static class Identity {

        // Prefix
        private const string PREFIX = "";

        // Component
        public const string COMPONENT_SELECT = PREFIX + "component.select";
        public const string COMPONENT_TEMPLATE = PREFIX + "component.template";
        public const string COMPONENT_EVENT = PREFIX + "component.event";

        // Button Component
        public const string BUTTON_CHANNEL = PREFIX + "button.channel";
        public const string BUTTON_CURRENT_CHANNEL = PREFIX + "button.current.channel";
        public const string BUTTON_UPDATE = PREFIX + "button.update";
        public const string BUTTON_CANCEL = PREFIX + "button.cancel";

        // Button Component Permission
        public const string BUTTON_PERMISSION_USERS = PREFIX + "button.permission.users";
        public const string BUTTON_PERMISSION_ROLES = PREFIX + "button.permission.roles";
        public const string BUTTON_PERMISSION_CHANNELS = PREFIX + "button.permission.channels";
        public const string BUTTON_PERMISSION_SETTINGS = PREFIX + "button.permission.settings";
        public const string BUTTON_PERMISSION_RESET = PREFIX + "button.permission.reset";

        // Button Component Changelog
        public const string BUTTON_CHANGELOG_CREATE = PREFIX + "button.changelog.create";
        public const string BUTTON_CHANGELOG_REMOVE = PREFIX + "button.changelog.remove";
        public const string BUTTON_CHANGELOG_CANCEL = PREFIX + "button.changelog.cancel";

        // Selection Component Default
        public const string SELECTION_TITLE = PREFIX + "selection.title";
        public const string SELECTION_DESCRIPTION = PREFIX + "selection.description";
        public const string SELECTION_CONTENT = PREFIX + "selection.content";
        public const string SELECTION_FOOTER = PREFIX + "selection.footer";
        public const string SELECTION_AUTHOR = PREFIX + "selection.author";
        public const string SELECTION_COLOR = PREFIX + "selection.color";
        public const string SELECTION_IMAGE = PREFIX + "selection.image";
        public const string SELECTION_THUMBNAIL = PREFIX + "selection.thumbnail";
        public const string SELECTION_PINGROLE = PREFIX + "selection.pingrole";
        public const string SELECTION_TIMESTAMP = PREFIX + "selection.timestamp";
        public const string SELECTION_TIMESTAMP_CHANGE = PREFIX + "selection.timestamp.change";
        public const string SELECTION_FIELD_ADD = PREFIX + "selection.field.add";
        public const string SELECTION_FIELD_REMOVE = PREFIX + "selection.field.remove";
        public const string SELECTION_TEMPLATE_ADD = PREFIX + "selection.template.add";
        public const string SELECTION_TEMPLATE_USE = PREFIX + "selection.template.use";
        public const string SELECTION_TEMPLATE_INPUT = PREFIX + "selection.template.input";
        public const string SELECTION_TEMPLATE_REMOVE = PREFIX + "selection.template.remove";

        // Selection Component Event
        public const string SELECTION_EVENT_CREATION = PREFIX + "selection.event.create";
        public const string SELECTION_EVENT_PROPERTIES = PREFIX + "selection.event.properties";
        public const string SELECTION_EVENT_INTRODUCTION = PREFIX + "selection.event.introduction";
        public const string SELECTION_EVENT_INFORMATION = PREFIX + "selection.event.information";
        public const string SELECTION_EVENT_REWARDS = PREFIX + "selection.event.rewards";
        public const string SELECTION_EVENT_TIMESTAMP = PREFIX + "selection.event.timestamp";
        public const string SELECTION_EVENT_REACTION = PREFIX + "selection.event.reaction";

        // Selection Component Permission
        public const string SELECTION_PERMS = PREFIX + "selection.permission";
        public const string SELECTION_PERMS_PERMS = PREFIX + "selection.permission.permission";
        public const string SELECTION_PERMS_EMBED = PREFIX + "selection.permission.embed";
        public const string SELECTION_PERMS_EVENTS = PREFIX + "selection.permission.events";
        public const string SELECTION_PERMS_TIMESTAMP = PREFIX + "selection.permission.timestamp";
        public const string SELECTION_PERMS_BROADCAST = PREFIX + "selection.permission.broadcast";
        public const string SELECTION_PERMS_NOTION = PREFIX + "selection.permission.notion";

        // Modal
        public const string MODAL_EMBED = PREFIX + "modal.embed";
        public const string MODAL_EVENT = PREFIX + "modal.event";
        public const string MODAL_TEMPLATE = PREFIX + "modal.template";
        public const string MODAL_TIMESTAMP = PREFIX + "modal.timestamp";

        // Modal Component Default
        public const string MODAL_COMP_TITLE = PREFIX + "modal.component.title";
        public const string MODAL_COMP_TITLE_LINK = PREFIX + "modal.component.title.link";
        public const string MODAL_COMP_DESCRIPTION = PREFIX + "modal.component.description";
        public const string MODAL_COMP_CONTENT = PREFIX + "modal.component.content";
        public const string MODAL_COMP_FOOTER = PREFIX + "modal.component.footer";
        public const string MODAL_COMP_FOOTER_URL = PREFIX + "modal.component.footer.url";
        public const string MODAL_COMP_AUTHOR = PREFIX + "modal.component.author";
        public const string MODAL_COMP_AUTHOR_LINK = PREFIX + "modal.component.author.link";
        public const string MODAL_COMP_AUTHOR_URL = PREFIX + "modal.component.author.url";
        public const string MODAL_COMP_COLOR = PREFIX + "modal.component.color";
        public const string MODAL_COMP_IMAGE = PREFIX + "modal.component.image";
        public const string MODAL_COMP_THUMBNAIL = PREFIX + "modal.component.thumnail";
        public const string MODAL_COMP_PINGROLE = PREFIX + "modal.component.pingrole";
        public const string MODAL_COMP_TIMESTAMP = PREFIX + "modal.component.timestamp";
        public const string MODAL_COMP_FIELD_TITLE = PREFIX + "modal.component.field.title";
        public const string MODAL_COMP_FIELD_TEXT = PREFIX + "modal.component.field.text";
        public const string MODAL_COMP_FIELD_INLINE = PREFIX + "modal.component.field.inline";
        public const string MODAL_COMP_FIELD_INDEX = PREFIX + "modal.component.field.index";
        public const string MODAL_COMP_TEMPLATE_ADD = PREFIX + "modal.component.template.add";
        public const string MODAL_COMP_TEMPLATE_REMOVE = PREFIX + "modal.component.template.remove";
        public const string MODAL_COMP_TEMPLATE_USE = PREFIX + "modal.component.template.use";
        public const string MODAL_COMP_EVENT_TITLE = PREFIX + "modal.component.event.title";
        public const string MODAL_COMP_EVENT_INTRO = PREFIX + "modal.component.event.intro";
        public const string MODAL_COMP_EVENT_INFO = PREFIX + "modal.component.event.info";
        public const string MODAL_COMP_EVENT_REWARD = PREFIX + "modal.component.event.reward";
        public const string MODAL_COMP_EVENT_TIMEZONE = PREFIX + "modal.component.event.timezone";
        public const string MODAL_COMP_EVENT_TIME_TITLE = PREFIX + "modal.component.event.time.title";
        public const string MODAL_COMP_EVENT_START = PREFIX + "modal.component.event.start";
        public const string MODAL_COMP_EVENT_END = PREFIX + "modal.component.event.end";

        // Modal Component Timestamp
        public const string MODAL_COMP_TIMESTAMP_TIMEZONE = PREFIX + "modal.component.timestamp.timezone";
        public const string MODAL_COMP_TIMESTAMP_TIME = PREFIX + "modal.component.timestamp.time";

        // Event
        public const string EVENT_NAME = PREFIX + "event.name";
        public const string EVENT_TITLE = PREFIX + "event.title";
        public const string EVENT_INTRO = PREFIX + "event.intro";
        public const string EVENT_INFO_TITLE = PREFIX + "event.info.title";
        public const string EVENT_INFO = PREFIX + "event.info";
        public const string EVENT_REWARD_TITLE = PREFIX + "event.reward.title";
        public const string EVENT_REWARD = PREFIX + "event.reward";
        public const string EVENT_TIMEZONE = PREFIX + "event.timezone";
        public const string EVENT_TIME_TITLE = PREFIX + "event.time.title";
        public const string EVENT_START = PREFIX + "event.start";
        public const string EVENT_END = PREFIX + "event.end";
        public const string EVENT_START_R = PREFIX + "event.start.relative";
        public const string EVENT_END_R = PREFIX + "event.end.relative";
        public const string EVENT_DESCRIPTION_START = PREFIX + "event.description.start";
        public const string EVENT_DESCRIPTION_END = PREFIX + "event.description.end";
        public const string EVENT_DESCRIPTION_REACTION = PREFIX + "event.description.reaction";

        // Template
        public const string TEMPLATE_NAME = PREFIX + "template.name";
        public const string TEMPLATE_LIST = PREFIX + "template.list";
        public const string TEMPLATE_LIST_CUSTOM = PREFIX + "template.list.custom";
        public const string TEMPLATE_REPLACE_MESSAGE_ID = PREFIX + "template.replace.message";

        // Timestamp
        public const string TIMESTAMP_TIME = PREFIX + "timestamp.time";
        public const string TIMESTAMP_TIMEZONE = PREFIX + "timestamp.timezone";
        public const string TIMESTAMP_SHORT_DATE = PREFIX + "timestamp.short.date";
        public const string TIMESTAMP_SHORT_TIME = PREFIX + "timestamp.short.time";
        public const string TIMESTAMP_LONG_DATE = PREFIX + "timestamp.long.date";
        public const string TIMESTAMP_LONG_TIME = PREFIX + "timestamp.long.time";
        public const string TIMESTAMP_LONG_DATE_SHORT_TIME = PREFIX + "timestamp.long.date.short.time";
        public const string TIMESTAMP_LONG_DATE_DAY_OF_WEEK_SHORT_TIME = PREFIX + "timestamp.long.date.day.of.week.short.time";
        public const string TIMESTAMP_RELATIVE_TIME = PREFIX + "timestamp.relative.time";

        // Template
        public const string TDATA_EMBED_CREATE = "EMBED_CREATE";
        public const string TDATA_EVENT_CREATE = "EVENT_CREATE";
        public const string TDATA_EVENT_POST_CREATE = "EVENT_POST_CREATE";
        public const string TDATA_CREATE = "TEMPLATE_CREATE";
        public const string TDATA_USE = "TEMPLATE_USE";
        public const string TDATA_TEMPLATE = "TIMESTAMP";
        public const string TDATA_NITRO = "NITRO";
        public const string TDATA_NO_PERMISSION = "NO_PERMISSION";

        // Other
        public const string REACTION_1 = PREFIX + "reaction.1";
        public const string REACTION_2 = PREFIX + "reaction.2";
        public const string REACTION_3 = PREFIX + "reaction.3";
        public const string REACTION_4 = PREFIX + "reaction.4";
        public const string REACTION_5 = PREFIX + "reaction.5";
        public const string REACTION_6 = PREFIX + "reaction.6";
        public const string REACTION_7 = PREFIX + "reaction.7";
        public const string REACTION_8 = PREFIX + "reaction.8";
        public const string REACTION_9 = PREFIX + "reaction.9";

        // ========================================================================================================
        //                                                                                                       //
        //                                              Placeholders                                             //
        //                                                                                                       //
        // ========================================================================================================

        public const string PLACEHOLDER_TIME = PREFIX + "plchldr.time";
        public const string PLACEHOLDER_TIME_EXPIRE = PREFIX + "plchldr.time.expire";

    }
}
