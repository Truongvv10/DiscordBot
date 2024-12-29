using BLL.Model;
using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Utils {
    public static class ButtonComponent {

        public static DiscordActionRowComponent GetDefault(DiscordChannel channel, bool hidden = false) {
            var buttons = new List<DiscordComponent> {
                new DiscordButtonComponent(ButtonStyle.Success, Identity.BUTTON_CHANNEL, $"Send to {channel.Name}"),
                new DiscordButtonComponent(ButtonStyle.Secondary, Identity.BUTTON_TEMPLATE, $"Use Template"),
                new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_UPDATE, $"Update"),
                new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_CANCEL, "Cancel", hidden)};
            return new DiscordActionRowComponent(buttons);
        }

        public static DiscordActionRowComponent GetEdit(bool hidden = false) {
            var buttons = new List<DiscordComponent> {
                new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_UPDATE, "Update"),
                new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_CANCEL, "Cancel", hidden)};
            return new DiscordActionRowComponent(buttons);
        }

        public static DiscordActionRowComponent GetEvent(bool hidden = false) {
            var buttons = new List<DiscordComponent> {
                new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_EVENT_SETUP, "Setup Event"),
                new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_CANCEL, "Cancel", hidden)};
            return new DiscordActionRowComponent(buttons);
        }

        public static DiscordActionRowComponent GetInactivity() {
            var buttons = new List<DiscordComponent> {
                new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_INACTIVITY_SEEN, "Mark as seen"),
                new DiscordButtonComponent(ButtonStyle.Danger, Identity.BUTTON_INACTIVITY_EDIT, "Edit")};
            return new DiscordActionRowComponent(buttons);
        }

        public static DiscordActionRowComponent GetNitro() {
            var buttons = new List<DiscordComponent> {
                new DiscordButtonComponent(ButtonStyle.Primary, Identity.BUTTON_NITRO, "Claim Nitro")};
            return new DiscordActionRowComponent(buttons);
        }
    }
}
