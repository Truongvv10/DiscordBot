using BLL.Model;
using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Utils {
    public static class SelectComponent {

        public static DiscordActionRowComponent GetDefault() {

            var selectOptions = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Edit Title", Identity.SELECTION_TITLE, "Edit your embed title & url.", emoji: new DiscordComponentEmoji("✏️")),
                    new DiscordSelectComponentOption("Edit Description", Identity.SELECTION_DESCRIPTION, "Edit your embed description.", emoji: new DiscordComponentEmoji("📄")),
                    new DiscordSelectComponentOption("Edit Footer", Identity.SELECTION_FOOTER, "Edit your embed footer & image.", emoji: new DiscordComponentEmoji("🧩")),
                    new DiscordSelectComponentOption("Edit Author", Identity.SELECTION_AUTHOR, "Edit your embed author text, link & url.", emoji: new DiscordComponentEmoji("👤")),
                    new DiscordSelectComponentOption("Edit Main Image", Identity.SELECTION_IMAGE, "Edit your embed image.", emoji: new DiscordComponentEmoji("🪪")),
                    new DiscordSelectComponentOption("Edit Thumbnail Image", Identity.SELECTION_THUMBNAIL, "Edit your embed tumbnail.", emoji: new DiscordComponentEmoji("🖼")),
                    new DiscordSelectComponentOption("Edit Color", Identity.SELECTION_COLOR, "Edit your embed color.", emoji: new DiscordComponentEmoji("🎨")),
                    new DiscordSelectComponentOption("Edit Roles To Ping", Identity.SELECTION_PINGROLE, "Edit roles to ping on message sent.", emoji: new DiscordComponentEmoji("🔔")),
                    new DiscordSelectComponentOption("Edit Plain Message", Identity.SELECTION_CONTENT, "Edit plain text to the message.", emoji: new DiscordComponentEmoji("💭")),
                    new DiscordSelectComponentOption("Toggle Timestamp", Identity.SELECTION_TIMESTAMP, "Toggle embed timestamp.", emoji: new DiscordComponentEmoji("🕙")),
                    new DiscordSelectComponentOption("Add Field Message", Identity.SELECTION_FIELD_ADD, "Add field message.", emoji: new DiscordComponentEmoji("📕")),
                    new DiscordSelectComponentOption("Remove Field Message", Identity.SELECTION_FIELD_REMOVE, "Remove field message.", emoji: new DiscordComponentEmoji("❌")),
                    new DiscordSelectComponentOption("Save To Templates", Identity.SELECTION_TEMPLATE_ADD, "Save this message to your templates.", emoji: new DiscordComponentEmoji("📂")),
                    new DiscordSelectComponentOption("Use From Templates", Identity.SELECTION_TEMPLATE_USE, "Change this message using your templates.", emoji: new DiscordComponentEmoji("📑")),
                    new DiscordSelectComponentOption("List Of Templates", Identity.SELECTION_TEMPLATE_LIST, "Have an overview of available templates.", emoji: new DiscordComponentEmoji("📰")),
                    new DiscordSelectComponentOption("Remove A Template", Identity.SELECTION_TEMPLATE_REMOVE, "Remove a template from your templates.", emoji: new DiscordComponentEmoji("❌"))};

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.SELECTION_EMBED, "Select message builder components to edit", selectOptions)};

            return new DiscordActionRowComponent(selectComponents);
        }

        public static DiscordActionRowComponent GetPlaceholder(Message message) {

            var selectOptions = new List<DiscordSelectComponentOption>() {
                    new DiscordSelectComponentOption("Edit name", Identity.SELECTION_PLACEHOLDER_ID, "Edit the name of this message.", emoji: new DiscordComponentEmoji("🏷️")),
                    new DiscordSelectComponentOption("Edit time", Identity.SELECTION_PLACEHOLDER_TIME, "Edit the time of this message.", emoji: new DiscordComponentEmoji("🕙")),
                    new DiscordSelectComponentOption("Edit texts", Identity.SELECTION_PLACEHOLDER_TEXTS, "Edit the texts of this message.", emoji: new DiscordComponentEmoji("💬")),
                    new DiscordSelectComponentOption("Edit urls", Identity.SELECTION_PLACEHOLDER_URLS, "Edit the urls of this message.", emoji: new DiscordComponentEmoji("🔗"))};

            // Split into nested dictionary
            var nestedData = new Dictionary<string, List<string>>();
            List<string> emojis = new() { "🟥", "🟧", "🟨", "🟩", "🟦", "🟪", "⬜", "⬛", "🟫" };

            foreach (var kvp in message.Data) {
                if (kvp.Key.StartsWith("data.custom")) {
                    // Split the key into segments
                    var segments = kvp.Key.Split('.');
                    if (!nestedData.TryAdd(segments[2], new() { segments[3] })) {
                        nestedData[segments[2]].Add(segments[3]);
                    }
                }
            }

            var availableEmojis = new List<string>(emojis);
            int index = 0;

            foreach (var item in nestedData) {
                string property = item.Key;

                // Pick a random emoji from the available ones
                string selectedEmoji = availableEmojis[index];
                index++;

                selectOptions.Add(new DiscordSelectComponentOption(
                    $"Edit {property}",
                    $"{Identity.SELECTION_PLACEHOLDER_CUSTOM}.{property}",
                    $"Edit the {property} of this message.",
                    emoji: new DiscordComponentEmoji(selectedEmoji)
                ));
            }

            selectOptions.Add(new DiscordSelectComponentOption("Placeholder add", Identity.SELECTION_PLACEHOLDER_ADD, "Add a new custom placeholder.", emoji: new DiscordComponentEmoji("➕")));

            List<DiscordComponent> selectComponents = new() {
                new DiscordSelectComponent(Identity.SELECTION_PLACEHOLDER, "Select placeholder components to edit", selectOptions)};

            return new DiscordActionRowComponent(selectComponents);
        }

    }
}
