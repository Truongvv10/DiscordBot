using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Enums {
    public enum TemplateMessage {
        EMBED_CREATE = 0,
        TIMESTAMP = 1,
        TEMPLATES = 2,
        NITRO = 3,
        NO_PERMISSION = 4,
        INTRODUCTION = 7,


        ACTION_SUCCESS = 100,
        ACTION_FAILED = 101,
        ACTION_INVALID = 102,
    }
}
