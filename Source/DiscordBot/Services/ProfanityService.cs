using ProfanityFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Services {
    public class ProfanityService {

        private readonly ProfanityFilter.ProfanityFilter profanityFilter;

        public ProfanityService() {
            profanityFilter = new ProfanityFilter.ProfanityFilter();
        }

        public bool ContainsProfanity(string text) {
            return profanityFilter.ContainsProfanity(text);
        }

        public string CensorText(string text, char replacementChar = '#') {
            return profanityFilter.CensorString(text, replacementChar, false);
        }

    }
}
