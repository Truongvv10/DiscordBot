using DiscordBot.Utils;

namespace Test {
    public class PlaceholderTest {

        public PlaceholderTest() {
        }

        [Fact]
        public async Task Test_Time_IsEqual() {
            // Arrange
            string text = "Time: {plchldr.date.start}";
            var data = new Dictionary<string, string> {
                { Placeholder.TIMEZONE, "CET" },
                { Placeholder.DATE_START, "22/10/2024 10:30" }};

            // Act
            string result = await Placeholder.ReplacePlaceholders(text, data);

            // Assert
            Assert.Equal("Time: <t:1729585800:F>", result);
        }

        [Fact]
        public async Task Test_TimeSubVariant_IsEqual() {
            // Arrange
            string text = "Time: {plchldr.date.start} {plchldr.date.start.2}";
            var data = new Dictionary<string, string> {
                { Placeholder.TIMEZONE, "CET" },
                { Placeholder.DATE_START, "22/10/2024 10:30" }};

            // Act
            string result = await Placeholder.ReplacePlaceholders(text, data);

            // Assert
            Assert.Equal("Time: <t:1729585800:F> <t:1729585800:f>", result);
        }
    }
}