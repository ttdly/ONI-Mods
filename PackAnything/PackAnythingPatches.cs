using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static DetailsScreen;

namespace PackAnything {
    public class PackAnythingPatches {
        public static void EntityPostfix(GameObject __result) {
            __result.AddOrGet<ObjectCanMove>();
        }

        public static void BuildingPostfix(GameObject go) {
            go.AddOrGet<ObjectCanMove>();
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
        // 所有泉
        [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
        partial class GeyserGenericConfig_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<ObjectCanMove>().objectType = ObjectType.Geyser;
            }
        }

        // 所有遗迹
        [HarmonyPatch(typeof(EntityTemplates), "ConfigPlacedEntity")]
        public class EntityTemplates_ConfigPlacedEntity_Patch {
            public static void Postfix(GameObject __result) {
                if (__result.HasTag(GameTags.Gravitas)) __result.AddOrGet<ObjectCanMove>();
            }
        }
        
        // 梦境合成
        [HarmonyPatch(typeof(MegaBrainTank), "OnSpawn")]
        public class MegaBrainTank_OnSpawn_Patch { 
            public static void Postfix(MegaBrainTank __instance) {
                __instance.gameObject.AddOrGet<ObjectCanMove>();
            }
        }
        // 生物织构
        [HarmonyPatch(typeof(MorbRoverMakerWorkable), "OnSpawn")]
        public class MorbRoverMakerWorkable_OnSpawn_Patch {
            public static void Postfix(MorbRoverMakerWorkable __instance) {
                __instance.gameObject.AddOrGet<ObjectCanMove>();
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
                var moveSoryTool = new GameObject("MoveStory");
                var tool2 = moveSoryTool.AddComponent<MoveStoryTargetTool>();
                moveSoryTool.transform.SetParent(__instance.gameObject.transform);
                moveSoryTool.SetActive(true);
                moveSoryTool.SetActive(false);
                interfaceTools.Add(tool2);
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
