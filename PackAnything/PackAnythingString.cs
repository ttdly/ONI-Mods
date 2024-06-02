using STRINGS;

namespace PackAnything {
  public class PackAnythingString {
    public class UI {
      public class SURVEY {
        public static LocString NAME = "Survey";
        public static LocString TOOLTIP = "Survey this item for move.";
        public static LocString NAME_OFF = "Cancel Survey";
        public static LocString TOOLTIP_OFF = "Cancel order to survey this item.";
      }

      public class SIDE_SCREEN {
        public static LocString NAME = "Survyed List";

        public static LocString TOOL_TIP =
          "There is no object being surveyed, or alternatively, there is an object in motion";

        public static LocString TOOL_TIP_ELEMENT = "Need some steel to start.";
        public static LocString TOOL_TIP_OBJ = "\n\nDouble-click to view this object.";
        public static LocString APPLY_BUTTON_TEXT = "Move It";
        public static LocString APPLY_BUTTON_TOOL_TIP = "Move this object to a specific location";
        public static LocString CANCEL_BUTTON_TOOL_TIP = "Cancel survey for this item.";
      }
    }

    public class MISC {
      public class DISPLACEMENT_BEACON {
        public static LocString NAME = "Displacement Beacon";

        public static LocString DESC =
          "Record the information of an object, activate this beacon, and the recorded object will be moved to this location.";
      }
    }
  }

  public class STRINGS {
    public static LocString GENERATE_UNOBTANIUM = "Generate Unobtanium";
    public static LocString GENERATE_UNOBTANIUM_DESC = "Whether or not does moving a geyser create neutronium";
    public static LocString NO_NEED_DUPLICANTS_OPERATE = "No need for a duplicate operation.";
    public static LocString DONT_CONSUME_ANYTHING = "No consumption is required";

    public class BUILDINGS {
      public class PREFABS {
        public class MOVABLEBEACON {
          public static LocString NAME = UI.FormatAsLink("Displacement Beacon", "MOVABLEBEACON");
          public static LocString DESC = "The magical machine that can alter this world.";
          public static LocString EFFECT = "Move the object surveyed.";
        }
      }
    }

    public class MISC {
      public class STATUSITEMS {
        public class WAITINGSURVEY {
          public static LocString NAME = "Survey Errand";
          public static LocString TOOLTIP = "Item will be surveyed once a Duplicant is available";
        }
      }
    }

    public class DUPLICANTS {
      public class STATUSITEMS {
        public class SURVEYITEM {
          public static LocString NAME = "Surveying";
          public static LocString TOOLTIP = "This Duplicant is surveying an object";
        }
      }

      public class CHORES {
        public class SURVEY {
          public static LocString NAME = "Survey";
          public static LocString STATUS = "Going to Survey";
          public static LocString TOOLTIP = "This Duplicant is surving an object";
          public static LocString REPORT_NAME = "Survey {0}";
        }
      }
    }
  }
}