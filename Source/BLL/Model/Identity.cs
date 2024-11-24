namespace BLL.Model {
    public class Identity {

        // Prefix
        private const string PREFIX = "";


        #region Selections
        public const string SELECTION_EMBED = $"{PREFIX}.selection.embed";
        public const string SELECTION_TEMPLATE = $"{PREFIX}.selection.template";
        public const string SELECTION_EVENT = $"{PREFIX}.selection.event";
        public const string SELECTION_PERMISSION = $"{PREFIX}.selection.permission";

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
        public const string SELECTION_TEMPLATE_INPUT = $"{SELECTION_EMBED}.template.input";
        public const string SELECTION_TEMPLATE_REMOVE = $"{SELECTION_EMBED}.template.remove";

        // Selection Event
        public const string SELECTION_EVENT_CREATION = $"{SELECTION_EVENT}.create";
        public const string SELECTION_EVENT_PROPERTIES = $"{SELECTION_EVENT}.properties";
        public const string SELECTION_EVENT_INTRODUCTION = $"{SELECTION_EVENT}.introduction";
        public const string SELECTION_EVENT_INFORMATION = $"{SELECTION_EVENT}.information";
        public const string SELECTION_EVENT_REWARDS = $"{SELECTION_EVENT}.rewards";
        public const string SELECTION_EVENT_TIMESTAMP = $"{SELECTION_EVENT}.timestamp";
        public const string SELECTION_EVENT_REACTION = $"{SELECTION_EVENT}.reaction";

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

        // Button Templates
        public const string BUTTON_TEMPLATES = $"{PREFIX}.button.template";
        public const string BUTTON_TEMPLATES_SELECT = $"{BUTTON_TEMPLATES}.select";
        public const string BUTTON_TEMPLATES_ADD = $"{BUTTON_TEMPLATES}.save";
        public const string BUTTON_TEMPLATES_DELETE =  $"{BUTTON_TEMPLATES}.delete";
        public const string BUTTON_TEMPLATES_CANCEL = $"{BUTTON_TEMPLATES}.cancel";
        public const string BUTTON_TEMPLATES_CONFIRM = $"{BUTTON_TEMPLATES}.confirm";

        // Button Nitro
        public const string BUTTON_NITRO = $"{PREFIX}.button.nitro";
        #endregion

        #region Modals
        public const string MODAL_EMBED = $"{PREFIX}.modal.embed";
        public const string MODAL_EVENT = $"{PREFIX}.modal.event";
        public const string MODAL_TEMPLATES = $"{PREFIX}.modal.templates";
        public const string MODAL_TIMESTAMP = $"{PREFIX}.modal.timestamp";

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

        // Modal Event
        private const string MODAL_DATA_EVENT = $"{MODAL_EVENT}.component";
        public const string MODAL_DATA_EVENT_TITLE = $"{MODAL_DATA_EVENT}.title";
        public const string MODAL_DATA_EVENT_INTRO = $"{MODAL_DATA_EVENT}.intro";
        public const string MODAL_DATA_EVENT_INFO = $"{MODAL_DATA_EVENT}.info";
        public const string MODAL_DATA_EVENT_REWARD = $"{MODAL_DATA_EVENT}.reward";
        public const string MODAL_DATA_EVENT_TIMEZONE = $"{MODAL_DATA_EVENT}.timezone";
        public const string MODAL_DATA_EVENT_TIME_TITLE = $"{MODAL_DATA_EVENT}.time.title";
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
        #endregion

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

        // Template
        public const string TDATA_EMBED_CREATE = "EMBED_CREATE";
        public const string TDATA_EVENT_CREATE = "EVENT_CREATE";
        public const string TDATA_EVENT_POST_CREATE = "EVENT_POST_CREATE";
        public const string TDATA_CREATE = "TEMPLATE_CREATE";
        public const string TDATA_USE = "TEMPLATE_USE";
        public const string TDATA_TIMESTAMP = "TIMESTAMP";
        public const string TDATA_TEMPLATES = "TEMPLATES";
        public const string TDATA_TEMPLATES_SAVE = "TEMPLATES_SAVE";
        public const string TDATA_NITRO = "NITRO";
        public const string TDATA_NO_PERMISSION = "NO_PERMISSION";

        // ========================================================================================================
        //                                                                                                       //
        //                                                  Data                                                 //
        //                                                                                                       //
        // ========================================================================================================

        public const string DATA_CONTENT = PREFIX + "data.content";
        public const string DATA_EMBED_TITLE = PREFIX + "data.embed.title";
        public const string DATA_EMBED_TITLE_LINK = PREFIX + "data.embed.title.link";
        public const string DATA_EMBED_DESCRIPTION = PREFIX + "data.embed.description";
        public const string DATA_EMBED_AUTHOR = PREFIX + "data.embed.author";
        public const string DATA_EMBED_AUTHOR_LINK = PREFIX + "data.embed.author.link";
        public const string DATA_EMBED_AUTHOR_URL = PREFIX + "data.embed.author.url";
        public const string DATA_EMBED_FOOTER = PREFIX + "data.embed.footer";
        public const string DATA_EMBED_FOOTER_URL = PREFIX + "data.embed.footer.url";
        public const string DATA_EMBED_FIELDS = PREFIX + "data.embed.fields";
    }
}
