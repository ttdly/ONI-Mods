using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using ProcGen;
using System.Collections.Generic;

namespace GravitasMemory {
    internal class Patch {
        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                ModUtil.AddBuildingToPlanScreen("Refining", BaseP1Config.ID);
                Db.Get().Techs.Get("Catalytics").unlockedItemIDs.Add(BaseP1Config.ID);
                ModUtil.AddBuildingToPlanScreen("Equipment", OverriderConfig.ID);
                Db.Get().Techs.Get("Acoustics").unlockedItemIDs.Add(OverriderConfig.ID);
                ModUtil.AddBuildingToPlanScreen("Equipment", GeyserV2Config.ID);
                Db.Get().Techs.Get("Acoustics").unlockedItemIDs.Add(GeyserV2Config.ID);
                ModUtil.AddBuildingToPlanScreen("Equipment", GeyserC4Config.ID);
                Db.Get().Techs.Get("Acoustics").unlockedItemIDs.Add(GeyserC4Config.ID);
                ModUtil.AddBuildingToPlanScreen("Equipment", ConditionerL8.ID);
                Db.Get().Techs.Get("Acoustics").unlockedItemIDs.Add(ConditionerL8.ID);
                LocString.CreateLocStringKeys(typeof(BUILDINGS), "STRINGS.");
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch {
            static void Postfix() {
                Database.Story GravitasMemory = new Database.Story("GravitasMemory", "storytraits/GravitasMemory", -1);
                Db.Get().Stories.AddStoryMod(GravitasMemory);
                LocString.CreateLocStringKeys(typeof(CODEX), "STRINGS.");
            }
        }

        [HarmonyPatch(typeof(MutatedWorldData), MethodType.Constructor, typeof(ProcGen.World),typeof(List<WorldTrait>), typeof(List<WorldTrait>))]
        public static class MutatedWorldData_Constructor_Patch {
            internal static void Postfix(MutatedWorldData __instance) {
                var storyTraits = __instance.storyTraits;
                PUtil.LogDebug(storyTraits);
            }
        }
    }
}
