using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static DetailsScreen;

namespace PackAnything {
    public class PackAnythingPatches {

        public static void EntityPostfix(GameObject __result) {
            __result.AddOrGet<Surveyable>();
        }

        public static void BuildingPostfix(GameObject go) {
            go.AddOrGet<Surveyable>();
        }

        [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
        partial class GeyserGenericConfig_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<Surveyable>();
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                LocString.CreateLocStringKeys(typeof(STRINGS), "");
                ModUtil.AddBuildingToPlanScreen("Equipment", WorldModifierConfig.ID);
                TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(WorldModifierConfig.ID, "World Modifier");
                Db.Get().Techs.Get("AdvancedResearch").unlockedItemIDs.Add(WorldModifierConfig.ID);
                PackAnythingStaticVars.Init();
            }
        }

        [HarmonyPatch(typeof(WarpPortalConfig),nameof(WarpPortalConfig.CreatePrefab))]
        public class WarpPortalConfig_CreatePrefab_Patch { 
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<Manual>();
                __result.AddTag("DontShowSurveyable");
            }
        }

        [HarmonyPatch(typeof(GeneShufflerConfig), nameof(GeneShufflerConfig.CreatePrefab))]
        public class GeneShufflerConfig_CreatePrefab_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<Manual>();
                __result.AddTag("DontShowSurveyable");
            }
        }

        [HarmonyPatch(typeof(MegaBrainTank), "OnSpawn")]
        public class MegaBrainTank_OnSpawn_Patch { 
            public static void Postfix(MegaBrainTank __instance) {
                __instance.gameObject.AddOrGet<Surveyable>();
            }
        }

        [HarmonyPatch(typeof(MorbRoverMakerWorkable), "OnSpawn")]
        public class MorbRoverMakerWorkable_OnSpawn_Patch {
            public static void Postfix(MorbRoverMakerWorkable __instance) {
                __instance.gameObject.AddOrGet<Surveyable>();
            }
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Load))]
        public class Game_Load_Patch {
            public static void Prefix() {
                PackAnythingStaticVars.SurveableCmps.Clear();
            }
        }

        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public static class PlayerController_OnPrefabInit_Patch {
            internal static void Postfix(PlayerController __instance) {
                var interfaceTools = new List<InterfaceTool>(__instance.tools);
                var moveBeaconTool = new GameObject("MoveBeacon");
                var tool = moveBeaconTool.AddComponent<MoveTargetTool>();
                moveBeaconTool.transform.SetParent(__instance.gameObject.transform);
                moveBeaconTool.SetActive(true);
                moveBeaconTool.SetActive(false);
                interfaceTools.Add(tool);
                __instance.tools = interfaceTools.ToArray();
            }
        }

        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class DetailsScreen_OnPrefabInit_Patch {
            internal static void Postfix(List<SideScreenRef> ___sideScreens, GameObject ___sideScreenContentBody) {
                ModifierSideScreen.AddSideScreen(___sideScreens, ___sideScreenContentBody);
            }
        }

    }
}
