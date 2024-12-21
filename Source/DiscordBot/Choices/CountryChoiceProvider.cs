using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Nager.Country;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Choices {
    public class CountryChoiceProvider : IAutocompleteProvider {

        private readonly IEnumerable<ICountryInfo> countries = new CountryProvider().GetCountries();

        public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
            var value = ctx.OptionValue?.ToString()?.Trim() ?? string.Empty;
            var results = countries
                .Where(c => c.CommonName.StartsWith(value, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .Select(c => new DiscordAutoCompleteChoice(c.CommonName, c.CommonName));
            return Task.FromResult(results.AsEnumerable());
        }
    }
}
