using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ISO3166;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Choices {
    public class CountryChoiceProvider : IAutocompleteProvider {

        private readonly string[] countries = Country.List.Select(c => c.Name).ToArray();

        public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
            var value = ctx.OptionValue?.ToString()?.Trim() ?? string.Empty;
            var results = countries
                .Where(c => c.StartsWith(value, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .Select(c => new DiscordAutoCompleteChoice(c, c));
            return Task.FromResult(results.AsEnumerable());
        }
    }
}
