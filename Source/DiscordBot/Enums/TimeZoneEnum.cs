using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Enums {
    public enum TimeZoneEnum {

        [Description("GMT (UTC ±00:00)")]
        [ChoiceName("GMT (Greenwhich Mean Time)")]
        GMT,

        [Description("GMT (UTC ±00:00)")]
        [ChoiceName("UTC (Coordinated Universal Time)")]
        UTC,

        [Description("GMT (UTC ±01:00)")]
        [ChoiceName("BST (British Summer Time)")]
        BST,

        [Description("GMT (UTC ±01:00) & GMT (UTC ±02:00)")]
        [ChoiceName("CET (Central European Time)")]
        CET,

        [Description("GMT (UTC ±02:00)")]
        [ChoiceName("EET (Eastern European Time)")]
        EET,

        [Description("GMT (UTC ±03:00)")]
        [ChoiceName("MSK (Moscow Standard Time)")]
        MSK,

        [Description("GMT (UTC ±04:00)")]
        [ChoiceName("GST (Gulf Standard Time)")]
        GST,

        [Description("GMT (UTC ±05:00)")]
        [ChoiceName("PKT (Pakistan Standard Time)")]
        PKT,

        [Description("GMT (UTC ±05:30)")]
        [ChoiceName("IST (Indian Standard Time)")]
        IST,

        [Description("GMT (UTC ±06:00)")]
        [ChoiceName("BST (Bangladesh Standard Time)")]
        BST_BANGLADESH,

        [Description("GMT (UTC ±07:00)")]
        [ChoiceName("WIB (Western Indonesian Time)")]
        WIB,

        [Description("GMT (UTC ±08:00)")]
        [ChoiceName("CST (China Standard Time)")]
        CST_CHINA,

        [Description("GMT (UTC ±09:00)")]
        [ChoiceName("JST (Japan Standard Time)")]
        JST,

        [Description("GMT (UTC ±09:00)")]
        [ChoiceName("KST (Korean Standard Time)")]
        KST,

        [Description("GMT (UTC ±10:00)")]
        [ChoiceName("AEST (Australian Eastern Standard Time)")]
        AEST,

        [Description("GMT (UTC ±12:00)")]
        [ChoiceName("NZST (New Zealand Standard Time)")]
        NZST,

        [Description("GMT (UTC ±13:00)")]
        [ChoiceName("NZDT (New Zealand Daylight Time)")]
        NZDT,

        [Description("GMT (UTC -10:00)")]
        [ChoiceName("HST (Hawaii Standard Time)")]
        HST,

        [Description("GMT (UTC -09:00)")]
        [ChoiceName("AKST (Alaska Standard Time)")]
        AKST,

        [Description("GMT (UTC -08:00)")]
        [ChoiceName("PST (Pacific Standard Time)")]
        PST,

        [Description("GMT (UTC -07:00)")]
        [ChoiceName("MST (Mountain Standard Time)")]
        MST,

        [Description("GMT (UTC -06:00)")]
        [ChoiceName("CST (Central Standard Time)")]
        CST,

        [Description("GMT (UTC -05:00)")]
        [ChoiceName("EST (Eastern Standard Time)")]
        EST,

        [Description("GMT (UTC -04:00)")]
        [ChoiceName("AST (Atlantic Standard Time)")]
        AST,

    }
}
