using HarmonyLib;
using Klei.AI;

namespace GravitasMemory {
  internal class Patch {
    [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
      public static void Prefix() {
        ModUtil.AddBuildingToPlanScreen("Equipment", OverriderConfig.ID);
        Db.Get().Techs.Get("RoboticTools").unlockedItemIDs.Add(OverriderConfig.ID);
        ModUtil.AddBuildingToPlanScreen("Refining", BaseP1Config.ID);
        Db.Get().Techs.Get("ImprovedCombustion").unlockedItemIDs.Add(BaseP1Config.ID);
        ModUtil.AddBuildingToPlanScreen("Refining", GeyserC4Config.ID);
        Db.Get().Techs.Get("ImprovedCombustion").unlockedItemIDs.Add(GeyserC4Config.ID);
        ModUtil.AddBuildingToPlanScreen("Utilities", ConditionerL8Config.ID);
        Db.Get().Techs.Get("LiquidTemperature").unlockedItemIDs.Add(ConditionerL8Config.ID);
        ModUtil.AddBuildingToPlanScreen("Utilities", SeedFermenterConfig.ID);
        Db.Get().Techs.Get("FoodRepurposing").unlockedItemIDs.Add(SeedFermenterConfig.ID);
        ModUtil.AddBuildingToPlanScreen("Utilities", SongMachineConfig.ID);
        Db.Get().Techs.Get("FoodRepurposing").unlockedItemIDs.Add(SongMachineConfig.ID);
        LocString.CreateLocStringKeys(typeof(BUILDINGS));
      }
    }

    [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
    public class Db_Initialize_Patch {
      private static void Postfix() {
        LocString.CreateLocStringKeys(typeof(CREATURES));
        var resource = new Effect("EggCrazy", CREATURES.MODIFIERS.EGGCRAZY.NAME, CREATURES.MODIFIERS.EGGCRAZY.DESC,
          1200f, true, true, false);
        resource.Add(new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, 600f,
          CREATURES.MODIFIERS.EGGCRAZY.NAME, true));
        Db.Get().effects.Add(resource);
      }
    }
  }
}