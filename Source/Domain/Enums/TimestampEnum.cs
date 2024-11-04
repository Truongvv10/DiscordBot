using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums {
    public enum TimestampEnum {
        SHORT_TIME,
        LONG_TIME,
        SHORT_DATE,
        LONG_DATE,
        LONG_DATE_AND_SHORT_TIME,
        LONG_DATE_WITH_DAY_OF_WEEK_AND_SHORT_TIME,
        RELATIVE,
    }
}
