namespace AutomaticGeyser {
  public class ModStrings {
    public class GEYSER {
      public class STATUSITEMS {
        public class SKIPERUPTSTATUSITEM {
          public static LocString NAME = "Skip Eruption Mode (Total Skips: {0})";
          public static LocString TOOLTIP = "Automation enabled, this geyser will not erupt.";
        }

        public class SKIPDORMANTSTATUSITEM {
          public static LocString NAME = "Skip Dormant/Idle Mode (Available Skips: {0})";
          public static LocString TOOLTIP = "Automation enabled, this geyser will not go dormant/idle.";
        }

        public class ALWAYSDORMANT {
          public static LocString NAME = "Permanent Dormancy";
          public static LocString TOOLTIP = "Automation enabled, this geyser will remain dormant indefinitely.";
        }
      }
    }

    public class UI {
      public class SideButton {
        public static LocString NeedAnalyze = "Need Analysis";
        public static LocString NeedAnalyze_Tip = "This geyser can only be automated after it has been analyzed.";
        public static LocString MarkForAddLogic = "Add Logic Port";

        public static LocString MarkForAddLogic_Tip =
          "After adding an logic port, the geyser can be controlled via logic ribbon.";

        public static LocString CancelAddLogic = "Cancel Logic Port Addition";
        public static LocString CancelAddLogic_Tip = "Cancel the task of adding a port.";
        public static LocString LogicAdded = "Logic Port Added";
        public static LocString LogicAdded_Tip = "No further action required.";
      }

      public class Logic {
        public static LocString GroupOutput_desc = "Output geyser status";

        public static LocString GroupOutput_active =
          "Bitwise description, 1 is green 0 is red\nSignal output 0000: Dormant/Idle\nSignal output 1000: Rising pressure\nSignal output 0100: Erupting\nSignal output 0010: Eruption Ended\nSignal output 0001: Overpressure ";

        public static LocString GroupInput_desc = "Input to control geyser behavior";

        public static LocString GroupInput_active =
          "Bitwise description, 1 is green 0 is red\nSignal input 1000: Skip Eruption Mode\nSignal input 0100: Skip Dormant/Idle Mode\nSignal input 0010: Permanent Dormancy Mode";

        public static LocString Output_desc = "Whether skipping the dormant/idle state is possible";
        public static LocString Output_active = "At least one skip of the dormant/idle state is possible";
        public static LocString Output_inactive = "Skipping the dormant/idle state is not possible";
      }
    }
  }
}