using STRINGS;

namespace WirelessProject {
    public class Strings {
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
            public static LocString AddtoProxy = "Add to Terminal";
            public static LocString RemoveFromProxy = "Remove from Terminal";
        }

    }
}
