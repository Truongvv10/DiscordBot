using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Enums {
    public enum TemplateMessage {
        EMBED = 0,
        TIMESTAMP = 1,
        TEMPLATES = 2,
        NITRO = 3,
        NO_PERMISSION = 4,
        INTRODUCTION = 5,
        INACTIVITY = 6,

        EVENT = 200,

        ACTION_SUCCESS = 10000,
        ACTION_FAILED = 10001,
        ACTION_INVALID = 10002,
    }
}
