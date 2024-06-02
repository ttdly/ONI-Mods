using HarmonyLib;
using TUNING;

namespace FragmentThermostat {
  public class Patches {
    [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public class GeneratedBuildings_Patch {
      public static void Prefix() {
        LocString.CreateLocStringKeys(typeof(ModString.BUILDINGS));
        LocString.CreateLocStringKeys(typeof(ModString.UI_FTMOD));
        ModUtil.AddBuildingToPlanScreen("Conveyance", FragmentThermostatConfig.ID);
        Db.Get().Techs.Get("SolidManagement").unlockedItemIDs.Add(FragmentThermostatConfig.ID);
        BUILDINGS.PLANSUBCATEGORYSORTING.Add(FragmentThermostatConfig.ID, "cooler");
      }
    }
  }
}