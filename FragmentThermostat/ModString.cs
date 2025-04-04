namespace FragmentThermostat {
  public class ModString {
    public class BUILDINGS {
      public class PREFABS {
        public class FTMOD {
                    public static LocString NAME = "Debris Thermostat";
                    public static LocString DESC = "Adjust the input debris temperature to the set temperature";

                    public static LocString EFFECT =
                      "Adjust the temperature of the input solid material, and apply the heat difference to the building itself.";
                }
            }
        }

    public class UI_FTMOD {
            public static LocString TITLE = "Target Temperature";

      public class Options {
                public static LocString MAX_TEMP = "Max Temperature";
                public static LocString MIN_TEMP = "Min Temperature";
                public static LocString HEAT_MULTIPLY = "Heat Multiply";
                public static LocString MODE2 = "Work as a Thermo Aquatuner";
                public static LocString MODE_OPEN = "Open";

                public static LocString MODE_TIP =
                  "If this option is checked, Debris Thermostat will lower the temperature of the debris passing through the building by 14℃.";

                public static LocString MODE_TIP2 = "Only active when open this mode.";
            }
        }
    }
}