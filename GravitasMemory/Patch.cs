using HarmonyLib;


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
                LocString.CreateLocStringKeys(typeof(BUILDINGS), "STRINGS.");
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch {
            static void Postfix() {
                Database.Story GravitasMemory = new Database.Story("GravitasMemory", "storytraits/GravitasMemory", -1);
                Db.Get().Stories.AddStoryMod(GravitasMemory);
                GameTags.MaterialCategories.Add(TagManager.Create("Memory"));
                LocString.CreateLocStringKeys(typeof(CODEX), "STRINGS.");
            }
        }
    }
}
