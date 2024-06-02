using HarmonyLib;
using TUNING;
using WirelessProject.ProwerManager;

namespace WirelessProject {
  public class Patches {
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
      public static void Prefix() {
        LocString.CreateLocStringKeys(typeof(Strings.BUILDINGS));
        ModUtil.AddBuildingToPlanScreen("Equipment", PowerProxyConfig.ID);
        BUILDINGS.PLANSUBCATEGORYSORTING.Add(PowerProxyConfig.ID, "Prower Proxy");
        Db.Get().Techs.Get("AdvancedResearch").unlockedItemIDs.Add(PowerProxyConfig.ID);
      }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.Load))]
    public class Game_Load_Patch {
      public static void Prefix() {
        StaticVar.PowerInfoList.Clear();
      }
    }
  }
}