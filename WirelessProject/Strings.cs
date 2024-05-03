using STRINGS;

namespace WirelessProject {
    public class Strings {
        public class BUILDING {
            public class STATUSITEMS {
                public class PROXYCIRCUITSTATUS {
                    public static LocString NAME = (LocString)"Current Load: {CurrentLoadAndColor} / {MaxLoad}";
                    public static LocString TOOLTIP = (LocString)("The current " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " load on this wire\n\nOverloading a wire will cause damage to the wire over time and cause it to break");
                }

                public class PROXYMAXWATTAGESTATUS {
                    public static LocString NAME = (LocString)"Potential Load: {TotalPotentialLoadAndColor} / {MaxLoad}";
                    public static LocString TOOLTIP = (LocString)("How much wattage this network will draw if all " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " consumers on the network become active at once");
                }
            }
        }

        public static class BUILDINGS {
            public class PREFABS {
                public class POWERPROXY {
                    public static LocString NAME = UI.FormatAsLink("WPT", "POWERPROXY");
                    public static LocString DESC = (LocString)"WPT (Wireless Power Terminal) \n\n What? You mean it’s providing power for my entire base?";
                    public static LocString EFFECT = (LocString)"You can choose to connect the power consumption, generation, and batteries in the base to this terminal, which will then coordinate these devices.";
                }
            }
        }

        public static class PowerManager {
            public static LocString WindowName = "Select Terminal";
            public static LocString NoTerminal = "None";
        }

    }
}
