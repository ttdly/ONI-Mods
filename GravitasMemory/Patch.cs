using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using ProcGen;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using static ModInfo;

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
                Db.Get().Techs.Get("RenewableEnergy").unlockedItemIDs.Add(GeyserV2Config.ID);
                ModUtil.AddBuildingToPlanScreen("Equipment", GeyserC4Config.ID);
                Db.Get().Techs.Get("ImprovedCombustion").unlockedItemIDs.Add(GeyserC4Config.ID);
                ModUtil.AddBuildingToPlanScreen("Utilities", ConditionerL8Config.ID);
                Db.Get().Techs.Get("LiquidTemperature").unlockedItemIDs.Add(ConditionerL8Config.ID);
                ModUtil.AddBuildingToPlanScreen("Refining", PressD16Config.ID);
                Db.Get().Techs.Get("Catalytics").unlockedItemIDs.Add(PressD16Config.ID);
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

        [HarmonyPatch(typeof(Assets), "SubstanceListHookup")]
        public class Assets_SubstanceListHookup_Patch {
            private static void Prefix() {
                ElementUtil.RegisterElementStrings(Crystal.SOLID_ID, ELEMENTS.CRYSTAL.NAME, ELEMENTS.CRYSTAL.DESC);
            }
            private static void Postfix() {
                Crystal.RegisterCrystalSubstance();
            }
        }

        [HarmonyPatch(typeof(Enum), "Parse", new System.Type[] { typeof(System.Type), typeof(string), typeof(bool) })]
        internal class SimHashes_Parse_Patch {
            private static bool Prefix(System.Type enumType, string value, ref object __result) => !enumType.Equals(typeof(SimHashes)) || !SimHashUtil.ReverseSimHashNameLookup.TryGetValue(value, out __result);
        }

        [HarmonyPatch(typeof(Enum), "ToString", new System.Type[] { })]
        internal class SimHashes_ToString_Patch {
            private static bool Prefix(ref Enum __instance, ref string __result) => !(__instance is SimHashes) || !SimHashUtil.SimHashNameLookup.TryGetValue((SimHashes)__instance, out __result);
        }

        //[HarmonyPatch(typeof(MutatedWorldData), MethodType.Constructor, typeof(ProcGen.World),typeof(List<WorldTrait>), typeof(List<WorldTrait>))]
        //public static class MutatedWorldData_Constructor_Patch {
        //    internal static void Postfix(MutatedWorldData __instance) {
        //        var storyTraits = __instance.storyTraits;
        //        PUtil.LogDebug(storyTraits.);
        //    }
        //}
    }
}
