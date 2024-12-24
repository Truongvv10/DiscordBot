namespace BLL.Model {
    public class Identity {

        // Prefix
        private const string PREFIX = "triumph";


        #region Selections
        public const string SELECTION_EMBED = $"{PREFIX}.selection.embed";
        public const string SELECTION_TEMPLATE = $"{PREFIX}.selection.template";
        public const string SELECTION_EVENT = $"{PREFIX}.selection.event";
        public const string SELECTION_PERMISSION = $"{PREFIX}.selection.permission";
        public const string SELECTION_PLACEHOLDER = $"{PREFIX}.selection.placeholder";

        // Selection Embed
        public const string SELECTION_TITLE = $"{SELECTION_EMBED}.title";
        public const string SELECTION_DESCRIPTION = $"{SELECTION_EMBED}.description";
        public const string SELECTION_CONTENT = $"{SELECTION_EMBED}.content";
        public const string SELECTION_FOOTER = $"{SELECTION_EMBED}.footer";
        public const string SELECTION_AUTHOR = $"{SELECTION_EMBED}.author";
        public const string SELECTION_COLOR = $"{SELECTION_EMBED}.color";
        public const string SELECTION_IMAGE = $"{SELECTION_EMBED}.image";
        public const string SELECTION_THUMBNAIL = $"{SELECTION_EMBED}.thumbnail";
        public const string SELECTION_PINGROLE = $"{SELECTION_EMBED}.pingrole";
        public const string SELECTION_TIMESTAMP = $"{SELECTION_EMBED}.timestamp";
        public const string SELECTION_FIELD_ADD = $"{SELECTION_EMBED}.field.add";
        public const string SELECTION_FIELD_REMOVE = $"{SELECTION_EMBED}.field.remove";
        public const string SELECTION_TEMPLATE_ADD = $"{SELECTION_EMBED}.template.add";
        public const string SELECTION_TEMPLATE_USE = $"{SELECTION_EMBED}.template.use";
        public const string SELECTION_TEMPLATE_LIST = $"{SELECTION_EMBED}.template.list";
        public const string SELECTION_TEMPLATE_REMOVE = $"{SELECTION_EMBED}.template.remove";

        // Selection Properties
        public const string SELECTION_PLACEHOLDER_ID = $"{SELECTION_PLACEHOLDER}.id";
        public const string SELECTION_PLACEHOLDER_TIME = $"{SELECTION_PLACEHOLDER}.time";
        public const string SELECTION_PLACEHOLDER_TEXTS = $"{SELECTION_PLACEHOLDER}.texts";
        public const string SELECTION_PLACEHOLDER_URLS = $"{SELECTION_PLACEHOLDER}.urls";
        public const string SELECTION_PLACEHOLDER_CUSTOM = $"{SELECTION_PLACEHOLDER}.custom";
        public const string SELECTION_PLACEHOLDER_ADD = $"{SELECTION_PLACEHOLDER}.add";

        // Selection Permission
        public const string SELECTION_PERMS_PERMS = $"{SELECTION_PERMISSION}.permission";
        public const string SELECTION_PERMS_EMBED = $"{SELECTION_PERMISSION}.embed";
        public const string SELECTION_PERMS_EVENTS = $"{SELECTION_PERMISSION}.events";
        public const string SELECTION_PERMS_TIMESTAMP = $"{SELECTION_PERMISSION}.timestamp";
        public const string SELECTION_PERMS_BROADCAST = $"{SELECTION_PERMISSION}.broadcast";
        public const string SELECTION_PERMS_NOTION = $"{SELECTION_PERMISSION}.notion";
        #endregion

        #region Buttons
        // Button Embed
        public const string BUTTON_EMBED = $"{PREFIX}.button.embed";
        public const string BUTTON_CHANNEL = $"{BUTTON_EMBED}.channel";
        public const string BUTTON_TEMPLATE = $"{BUTTON_EMBED}.template";
        public const string BUTTON_UPDATE = $"{BUTTON_EMBED}.update";
        public const string BUTTON_CANCEL = $"{BUTTON_EMBED}.cancel";
        public const string BUTTON_EDIT = $"{BUTTON_EMBED}.edit";

        // Button Templates
        public const string BUTTON_TEMPLATES = $"{PREFIX}.button.template";
        public const string BUTTON_TEMPLATES_SELECT = $"{BUTTON_TEMPLATES}.select";
        public const string BUTTON_TEMPLATES_ADD = $"{BUTTON_TEMPLATES}.save";
        public const string BUTTON_TEMPLATES_DELETE =  $"{BUTTON_TEMPLATES}.delete";
        public const string BUTTON_TEMPLATES_CANCEL = $"{BUTTON_TEMPLATES}.cancel";
        public const string BUTTON_TEMPLATES_CONFIRM = $"{BUTTON_TEMPLATES}.confirm";

        // Button Nitro
        public const string BUTTON_NITRO = $"{PREFIX}.button.nitro";

        // Button Event
        public const string BUTTON_EVENT = $"{PREFIX}.button.event";
        public const string BUTTON_EVENT_SETUP = $"{BUTTON_EVENT}.setup";

        // Button Inactivity
        public const string BUTTON_INACTIVITY = $"{PREFIX}.button.inactivity";
        public const string BUTTON_INACTIVITY_SEEN = $"{BUTTON_INACTIVITY}.seen";
        public const string BUTTON_INACTIVITY_EDIT = $"{BUTTON_INACTIVITY}.edit";
        #endregion

        #region Modals
        public const string MODAL_EMBED = $"{PREFIX}.modal.embed";
        public const string MODAL_EVENT = $"{PREFIX}.modal.event";
        public const string MODAL_TEMPLATES = $"{PREFIX}.modal.templates";
        public const string MODAL_TIMESTAMP = $"{PREFIX}.modal.timestamp";
        public const string MODAL_PLACEHOLDER = $"{PREFIX}.modal.placeholder";
        public const string MODAL_INTRODUCTION = $"{PREFIX}.modal.introduction";
        public const string MODAL_INACTIVITY = $"{PREFIX}.modal.inactivity";

        // Modal Default
        private const string MODAL_DATA = $"{MODAL_EMBED}.component";
        public const string MODAL_DATA_TITLE = $"{MODAL_DATA}.title";
        public const string MODAL_DATA_TITLE_LINK = $"{MODAL_DATA}.title.link";
        public const string MODAL_DATA_DESCRIPTION = $"{MODAL_DATA}.description";
        public const string MODAL_DATA_CONTENT = $"{MODAL_DATA}.content";
        public const string MODAL_DATA_FOOTER = $"{MODAL_DATA}.footer";
        public const string MODAL_DATA_FOOTER_URL = $"{MODAL_DATA}.footer.url";
        public const string MODAL_DATA_AUTHOR = $"{MODAL_DATA}.author";
        public const string MODAL_DATA_AUTHOR_LINK = $"{MODAL_DATA}.author.link";
        public const string MODAL_DATA_AUTHOR_URL = $"{MODAL_DATA}.author.url";
        public const string MODAL_DATA_COLOR = $"{MODAL_DATA}.color";
        public const string MODAL_DATA_IMAGE = $"{MODAL_DATA}.image";
        public const string MODAL_DATA_THUMBNAIL = $"{MODAL_DATA}.thumnail";
        public const string MODAL_DATA_PINGROLE = $"{MODAL_DATA}.pingrole";
        public const string MODAL_DATA_TIMESTAMP = $"{MODAL_DATA}.timestamp";
        public const string MODAL_DATA_FIELD_TITLE = $"{MODAL_DATA}.field.title";
        public const string MODAL_DATA_FIELD_TEXT = $"{MODAL_DATA}.field.text";
        public const string MODAL_DATA_FIELD_INLINE = $"{MODAL_DATA}.field.inline";
        public const string MODAL_DATA_FIELD_INDEX = $"{MODAL_DATA}.field.index";
        public const string MODAL_DATA_TEMPLATE_ADD = $"{MODAL_DATA}.template.add";
        public const string MODAL_DATA_TEMPLATE_REMOVE = $"{MODAL_DATA}.template.remove";
        public const string MODAL_DATA_TEMPLATE_USE = $"{MODAL_DATA}.template.use";

        // Modal Placeholder
        private const string MODAL_DATA_PLACEHOLDER = $"{MODAL_PLACEHOLDER}.component";
        public const string MODAL_DATA_PLACEHOLDER_ID = $"{MODAL_DATA_PLACEHOLDER}.id";
        public const string MODAL_DATA_PLACEHOLDER_TIMEZONE = $"{MODAL_DATA_PLACEHOLDER}.timzone";
        public const string MODAL_DATA_PLACEHOLDER_DATE_START = $"{MODAL_DATA_PLACEHOLDER}.date.start";
        public const string MODAL_DATA_PLACEHOLDER_DATE_END = $"{MODAL_DATA_PLACEHOLDER}.date.end";
        public const string MODAL_DATA_PLACEHOLDER_URL1 = $"{MODAL_DATA_PLACEHOLDER}.url.1";
        public const string MODAL_DATA_PLACEHOLDER_URL2 = $"{MODAL_DATA_PLACEHOLDER}.url.2";
        public const string MODAL_DATA_PLACEHOLDER_URL3 = $"{MODAL_DATA_PLACEHOLDER}.url.3";
        public const string MODAL_DATA_PLACEHOLDER_URL4 = $"{MODAL_DATA_PLACEHOLDER}.url.4";
        public const string MODAL_DATA_PLACEHOLDER_TEXT1 = $"{MODAL_DATA_PLACEHOLDER}.text.1";
        public const string MODAL_DATA_PLACEHOLDER_TEXT2 = $"{MODAL_DATA_PLACEHOLDER}.text.2";
        public const string MODAL_DATA_PLACEHOLDER_TEXT3 = $"{MODAL_DATA_PLACEHOLDER}.text.3";
        public const string MODAL_DATA_PLACEHOLDER_TEXT4 = $"{MODAL_DATA_PLACEHOLDER}.text.4";
        public const string MODAL_DATA_PLACEHOLDER_ADD_GROUP = $"{MODAL_DATA_PLACEHOLDER}.add.group";
        public const string MODAL_DATA_PLACEHOLDER_ADD_ID = $"{MODAL_DATA_PLACEHOLDER}.add.id";
        public const string MODAL_DATA_PLACEHOLDER_ADD_VALUE = $"{MODAL_DATA_PLACEHOLDER}.add.value";
        public const string MODAL_DATA_PLACEHOLDER_CUSTOM = $"{MODAL_DATA_PLACEHOLDER}.custom";

        // Modal Event
        private const string MODAL_DATA_EVENT = $"{MODAL_EVENT}.component";
        public const string MODAL_DATA_EVENT_TIMEZONE = $"{MODAL_DATA_EVENT}.timezone";
        public const string MODAL_DATA_EVENT_NAME = $"{MODAL_DATA_EVENT}.name";
        public const string MODAL_DATA_EVENT_START = $"{MODAL_DATA_EVENT}.start";
        public const string MODAL_DATA_EVENT_END = $"{MODAL_DATA_EVENT}.end";

        // Modal Templates
        private const string MODAL_DATA_TEMPLATES = $"{MODAL_TEMPLATES}.component";
        public const string MODAL_DATA_TEMPLATES_ADD_NAME = $"{MODAL_DATA_TEMPLATES}.add.name";
        public const string MODAL_DATA_TEMPLATES_ADD_MESSAGE = $"{MODAL_DATA_TEMPLATES}.add.message";
        public const string MODAL_DATA_TEMPLATES_USE_NAME = $"{MODAL_DATA_TEMPLATES}.use.name";
        public const string MODAL_DATA_TEMPLATES_USE_MESSAGE = $"{MODAL_DATA_TEMPLATES}.use.message";
        public const string MODAL_DATA_TEMPLATES_REMOVE_NAME = $"{MODAL_DATA_TEMPLATES}.remove.name";

        // Modal Timestamps
        private const string MODAL_DATA_TIMESTAMPS = $"{MODAL_TIMESTAMP}.component";
        public const string MODAL_DATA_TIMESTAMPS_TIMEZONE = $"{MODAL_DATA_TIMESTAMPS}.timezone";
        public const string MODAL_DATA_TIMESTAMPS_TIME = $"{MODAL_DATA_TIMESTAMPS}.time";

        // Modal Introduction
        private const string MODAL_DATA_INTRODUCTION = $"{MODAL_INTRODUCTION}.component";
        public const string MODAL_DATA_INTRODUCTION_BIRTHDAY = $"{MODAL_DATA_INTRODUCTION}.birthday";
        public const string MODAL_DATA_INTRODUCTION_PRONOUNCE = $"{MODAL_DATA_INTRODUCTION}.pronounce";
        public const string MODAL_DATA_INTRODUCTION_TEXT = $"{MODAL_DATA_INTRODUCTION}.text";

        // Modal Inactivity
        private const string MODAL_DATA_INACTIVITY = $"{MODAL_INTRODUCTION}.component";
        public const string MODAL_DATA_INACTIVITY_START = $"{MODAL_DATA_INACTIVITY}.start";
        public const string MODAL_DATA_INACTIVITY_END = $"{MODAL_DATA_INACTIVITY}.end";
        public const string MODAL_DATA_INACTIVITY_REASON = $"{MODAL_DATA_INACTIVITY}.reason";
        #endregion

        #region Internal
        public const string INTERNAL_SEND_CHANNEL = $"{PREFIX}.internal.send.channel";
        #endregion
    }
}
