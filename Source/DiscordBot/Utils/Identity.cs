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
        public const string SELECTION_TEMPLATE_REMOVE = PREFIX + "selection.template.remove";

        // Selection Component Event
        public const string SELECTION_EVENT_TITLE = PREFIX + "selection.event.title";
        public const string SELECTION_EVENT_INTRO = PREFIX + "selection.event.intro";
        public const string SELECTION_EVENT_INFO = PREFIX + "selection.event.info";
        public const string SELECTION_EVENT_TOPREWARDS = PREFIX + "selection.event.toprewards";
        public const string SELECTION_EVENT_TIMESTAMP = PREFIX + "selection.event.timestamp";

        // Selection Component Permission
        public const string SELECTION_PERMS = PREFIX + "selection.permission";
        public const string SELECTION_PERMS_PERMS = PREFIX + "selection.permission.permission";
        public const string SELECTION_PERMS_EMBED = PREFIX + "selection.permission.embed";
        public const string SELECTION_PERMS_EVENTS = PREFIX + "selection.permission.events";
        public const string SELECTION_PERMS_TIMESTAMP = PREFIX + "selection.permission.timestamp";
        public const string SELECTION_PERMS_BROADCAST = PREFIX + "selection.permission.broadcast";
        public const string SELECTION_PERMS_NOTION = PREFIX + "selection.permission.notion";

        // Modal
        public const string MODAL_TITLE = PREFIX + "modal.title";
        public const string MODAL_TITLE_LINK = PREFIX + "modal.title.link";
        public const string MODAL_DESCRIPTION = PREFIX + "modal.description";
        public const string MODAL_FOOTER = PREFIX + "modal.footer";
        public const string MODAL_FOOTER_URL = PREFIX + "modal.footer.url";
        public const string MODAL_AUTHOR = PREFIX + "modal.author";
        public const string MODAL_AUTHOR_LINK = PREFIX + "modal.author.link";
        public const string MODAL_AUTHOR_URL = PREFIX + "modal.author.url";
        public const string MODAL_COLOR = PREFIX + "modal.color";
        public const string MODAL_IMAGE = PREFIX + "modal.image";
        public const string MODAL_THUMBNAIL = PREFIX + "modal.thumnail";
        public const string MODAL_PINGROLE = PREFIX + "modal.pingrole";
        public const string MODAL_TIMESTAMP = PREFIX + "modal.timestamp";
        public const string MODAL_FIELD_TITLE = PREFIX + "modal.field.title";
        public const string MODAL_FIELD_TEXT = PREFIX + "modal.field.text";
        public const string MODAL_FIELD_INLINE = PREFIX + "modal.field.inline";
        public const string MODAL_FIELD_INDEX = PREFIX + "modal.field.index";
        public const string MODAL_TEMPLATE_ADD = PREFIX + "modal.template.add";
        public const string MODAL_TEMPLATE_REMOVE = PREFIX + "modal.template.remove";
        public const string MODAL_TEMPLATE_USE = PREFIX + "modal.template.use";

        // Event
        public const string EVENT_TITLE = PREFIX + "event.title";
        public const string EVENT_INTRO = PREFIX + "event.intro";
        public const string EVENT_INFO = PREFIX + "event.info";
        public const string EVENT_REWARD = PREFIX + "event.reward";
        public const string EVENT_TIMEZONE = PREFIX + "event.timezone";
        public const string EVENT_START = PREFIX + "event.start";
        public const string EVENT_END = PREFIX + "event.end";
    }
}
