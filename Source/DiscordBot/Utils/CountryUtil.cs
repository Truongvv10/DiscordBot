
using Nager.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Utils {
    public static class CountryUtil {

        private static readonly CountryProvider countryProvider = new CountryProvider();

        public static bool IsValidCountry(string country) {
            return countryProvider.GetCountries().Any(c => c.CommonName.Equals(country, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsValidCountryByCode(int code) {
            return countryProvider.GetCountries().Any(c => c.NumericCode.Equals(code));
        }

        public static int? GetCountryCode(string country) {
            return countryProvider.GetCountries().FirstOrDefault(c => c.CommonName.Equals(country, StringComparison.OrdinalIgnoreCase))?.NumericCode;
        }

        public static string? GetCountryName(int code) {
            return countryProvider.GetCountries().FirstOrDefault(c => c.NumericCode.Equals(code))?.CommonName;
        }

    }
}
