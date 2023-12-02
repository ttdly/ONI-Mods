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
                public static LocString TOOL_TIP = (LocString)"There is no object being surveyed, or alternatively, there is an object in motion";
                public static LocString APPLY_BUTTON_TEXT = (LocString)"Move It";
                public static LocString APPLY_BUTTON_TOOL_TIP = (LocString)"Move this object to a specific location \n\n It's best not to have your Duplicant stand where the object is about to be moved";
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
                    public static LocString NAME = UI.FormatAsLink("Displacement Beacon", "MOVABLEBEACON");
                    public static LocString DESC = (LocString)"The magical machine that can alter this world.";
                    public static LocString EFFECT = (LocString)"Move the object surveyed.";
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
