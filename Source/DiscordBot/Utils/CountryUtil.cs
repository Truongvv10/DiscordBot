using ISO3166;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Utils {
    public static class CountryUtil {

        public static bool IsValidCountry(string country) {
            return Country.List.Any(c => c.Name.Equals(country, StringComparison.OrdinalIgnoreCase));
        }

    }
}
