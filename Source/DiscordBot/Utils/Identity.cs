using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utils {
    public  static class Identity {
        // Button Component
        public const string BUTTON_CHANNEL = "button.channel";
        public const string BUTTON_CURRENT_CHANNEL = "button.current.channel";
        public const string BUTTON_UPDATE = "button.update";
        public const string BUTTON_CANCEL = "button.cancel";

        // Selection Component
        public const string SELECTION_TITLE = "selection.title";
        public const string SELECTION_DESCRIPTION = "selection.description";
        public const string SELECTION_FOOTER = "selection.footer";
        public const string SELECTION_AUTHOR = "selection.author";
        public const string SELECTION_COLOR = "selection.color";
        public const string SELECTION_IMAGE = "selection.image";
        public const string SELECTION_THUMBNAIL = "selection.thumbnail";
        public const string SELECTION_PINGROLE = "selection.pingrole";
        public const string SELECTION_TIMESTAMP = "selection.timestamp";
        public const string SELECTION_TIMESTAMP_CHANGE = "selection.timestamp.change";
        public const string SELECTION_FIELD_ADD = "selection.field.add";
        public const string SELECTION_FIELD_REMOVE = "selection.field.remove";
        public const string SELECTION_TEMPLATE_ADD = "selection.template.add";
        public const string SELECTION_TEMPLATE_USE = "selection.template.use";
        public const string SELECTION_TEMPLATE_REMOVE = "selection.template.remove";

        // Modal
        public const string MODAL_TITLE = "modal.title";
        public const string MODAL_TITLE_LINK = "modal.title.link";
        public const string MODAL_DESCRIPTION = "modal.description";
        public const string MODAL_FOOTER = "modal.footer";
        public const string MODAL_FOOTER_URL = "modal.footer.url";
        public const string MODAL_AUTHOR = "modal.author";
        public const string MODAL_AUTHOR_LINK = "modal.author.link";
        public const string MODAL_AUTHOR_URL = "modal.author.url";
        public const string MODAL_COLOR = "modal.color";
        public const string MODAL_IMAGE = "modal.image";
        public const string MODAL_THUMBNAIL = "modal.thumnail";
        public const string MODAL_PINGROLE = "modal.pingrole";
        public const string MODAL_TIMESTAMP = "modal.timestamp";
        public const string MODAL_FIELD_TITLE = "modal.field.title";
        public const string MODAL_FIELD_TEXT = "modal.field.text";
        public const string MODAL_FIELD_INLINE = "modal.field.inline";
        public static string MODAL_FIELD_INDEX = "modal.field.index";
        public const string MODAL_TEMPLATE_ADD = "modal.template.add";
        public const string MODAL_TEMPLATE_REMOVE = "modal.template.remove";
        public const string MODAL_TEMPLATE_USE = "modal.template.use";

        // Event
        public const string EVENT_TIMEZONE = "event.timezone";
        public const string EVENT_START = "event.start";
        public const string EVENT_END = "event.end";
    }
}
