namespace GeyserExpandMachine {
    public class ModString {
        // STRINGS.UI.GEYSEREXPANDSIDESCREEN.MAINCONTENT.TOGGLEGROUP.DORMANCY.LABEL

        public class TOGGLEGROUP {
            public class DORMANCY {
                public static LocString Label = "Always Dormancy";
            }
            public class SKIPDORMANCY {
                public static LocString Label = "Skip Dormancy";
            }
            public class SKIPIDLE {
                public static LocString Label = "Skip Idle";
            }
            public class SKIPERUPTION {
                public static LocString Label = "Skip Eruption";
            }
            public class DEFAULT {
                public static LocString Label = "Default";
            }
        }
        
        public class SIDESCREEN {
            public static LocString TITLE = "Geyser Mode Control";
        }

        public class GASGEYSEREXPAND {
            public static LocString NAME = "Gas Geyser Expand";
            public static LocString DESC = "Gas Geyser Expand Description";
            public static LocString EFFECT = "Gas Geyser Expand Effect";
        }

        public class LIQUIDGEYSEREXPAND {
            public static LocString NAME = "Liquid Geyser Expand LIQUID";
            public static LocString DESC = "Liquid Geyser Expand LIQUID Description";
            public static LocString EFFECT = "Liquid Geyser Expand Effect";
        }
        
        public class Logic {
            public static LocString GroupOutputDesc = "Output geyser status";

            public static LocString GroupOutputActive =
                "Bitwise description, 1 is green 0 is red\nSignal output 0000: Dormant/Idle\nSignal output 1000: Rising pressure\nSignal output 0100: Erupting\nSignal output 0010: Eruption Ended\nSignal output 0001: Overpressure ";

            public static LocString GroupInputDesc = "Input to control geyser behavior";

            public static LocString GroupInputActive =
                "Bitwise description, 1 is green 0 is red\nSignal input 1000: Skip Eruption Mode\nSignal input 0100: Skip Dormant/Idle Mode\nSignal input 0010: Permanent Dormancy Mode";

            public static LocString OutputDesc = "Whether skipping the dormant/idle state is possible";
            public static LocString OutputActive = "At least one skip of the dormant/idle state is possible";
            public static LocString OutputInactive = "Skipping the dormant/idle state is not possible";
        }
    }
}