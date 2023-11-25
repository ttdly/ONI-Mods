using STRINGS;
namespace PackAnything {
    public class PackAnythingString {
        public class UI {
            public class SURVEY {
                public static LocString NAME = (LocString)"Survey";
                public static LocString TOOLTIP = (LocString)"Survey this item for move.";
                public static LocString NAME_OFF = (LocString)"Cancel Survey";
                public static LocString TOOLTIP_OFF = (LocString)"Cancel order to survey this item.";
            }
            public class ACTIVATE {
                public static LocString NAME = (LocString)"Active";
                public static LocString TOOLTIP = (LocString)"Active the Displacement Beacon.";
                public static LocString NAME_OFF = (LocString)"Cancel Active";
                public static LocString TOOLTIP_OFF = (LocString)"Cancel order to active this item.";
            }

            public class SIDE_SCREEN {
                public static LocString NAME = (LocString)"Survyed List";
                public static LocString APPLY_BUTTON_TEXT = (LocString)"Move It";
                public static LocString APPLY_BUTTON_TOOL_TIP = (LocString)"Move this object to a specific location.";
            }
        }

        public class MISC {
            public class DISPLACEMENT_BEACON {
                public static LocString NAME = (LocString)"Displacement Beacon";
                public static LocString DESC = (LocString)"Record the information of an object, activate this beacon, and the recorded object will be moved to this location.";
            }

            public class SKILL {
                public static LocString PERK_DESC = (LocString)"Displacement Beacon Usage";
            }

            public class SURVEY {
                public static LocString NAME = (LocString)"Survey";
                public static LocString DESCRIPTION = (LocString)"Allow the Duplicant to survey the items";
            }
        }
    }

    public class STRINGS {
        public static LocString GENERATE_UNOBTANIUM = (LocString)"Generate Unobtanium";
        public static LocString GENERATE_UNOBTANIUM_DESC = (LocString)"Whether or not does moving a geyser create neutronium";

        public class BUILDINGS {
            public class PREFABS {
                public class MOVABLEBEACON {
                    public static LocString NAME = UI.FormatAsLink("实验机P1", "MOVABLEBEACON");
                    public static LocString DESC = (LocString)("利用“时间之弓”做出来的机器，但对于" + UI.FormatAsLink("中子物质", "UNOBTANIUM") + "的产出，我想那位博士还没有找到解决方案。");
                    public static LocString EFFECT = (LocString)("将殖民地的" + UI.FormatAsLink("精炼金属", "REFINEDMETAL") + "还原为" + UI.FormatAsLink("金属矿石", "RAWMETAL") + "。\n\n由于技术原因，它也会产出少量" + UI.FormatAsLink("中子物质", "UNOBTANIUM") + ".");
                    public static LocString RECIPE_DESCRIPTION = ("将{1}还原为{0}。");
                }
            }
        }

        public class MISC {
            public class STATUSITEMS {
                public class WAITINGSURVEY {
                    public static LocString NAME = (LocString)"Survey Errand";
                    public static LocString TOOLTIP = (LocString)"Item will be surveyed once a Duplicant is available";
                }
                public class WAITINGACTIVE {
                    public static LocString NAME = (LocString)"Active Errand";
                    public static LocString TOOLTIP = (LocString)"Displacement beacon will be actived once a Duplicant is available";
                }
            }
        }

        public class DUPLICANTS {
            public class STATUSITEMS {
                public class SURVEYITEM {
                    public static LocString NAME = (LocString)"Surveying";
                    public static LocString TOOLTIP = (LocString)"This Duplicant is surveying an object";
                }
                public class ACTIVEBEACON {
                    public static LocString NAME = (LocString)"Activating beacon";
                    public static LocString TOOLTIP = (LocString)"This Duplicant is activing a displacement beacon.";
                }
            }

            public class CHORES {
                public class SURVEY {
                    public static LocString NAME = (LocString)"Survey";
                    public static LocString STATUS = (LocString)"Going to Survey";
                    public static LocString TOOLTIP = (LocString)"This Duplicant is surving an object";
                    public static LocString REPORT_NAME = (LocString)"Survey {0}";
                }
                public class ACTIVE {
                    public static LocString NAME = (LocString)"Active";
                    public static LocString STATUS = (LocString)"Going to active";
                    public static LocString TOOLTIP = (LocString)"This Duplicant is active a displacement beacon";
                    public static LocString REPORT_NAME = (LocString)"Active {0}";
                }
            }
        }


    }
}
