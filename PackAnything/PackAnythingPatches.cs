using HarmonyLib;
using PeterHan.PLib.UI;
using UnityEngine;

namespace PackAnything {
    public class PackAnythingPatches {

        public static void EntityPostfix(GameObject __result) {
            __result.AddOrGet<Surveyable>();
        }

        public static void BuildingPostfix(GameObject go) {
            go.AddOrGet<Surveyable>();
        }

        [HarmonyPatch(typeof(Geyser), "OnSpawn")]
        public static class Geyser_OnSpawn_Patch {
            public static void Postfix(Geyser __instance) {
                if (__instance.GetType() != typeof(Geyser)) {
                    Surveyable packable = __instance.GetComponent<Surveyable>();
                    if (packable != null) {
                        Object.Destroy(__instance.GetComponent<Surveyable>());
                    }
                    return;
                } else {
                    __instance.FindOrAddComponent<Surveyable>();
                }
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                LocString.CreateLocStringKeys(typeof(STRINGS), "");
                Sprite skillbadge_role_building4 = PUIUtils.LoadSprite("PackAnything.images.skillbadge_role_building4.png");
                Assets.Sprites.Add("skillbadge_role_building4", skillbadge_role_building4);
                PackAnythingStaticVars.Init();
            }
        }

        [HarmonyPatch(typeof(WarpPortalConfig),nameof(WarpPortalConfig.CreatePrefab))]
        public class WarpPortalConfig_CreatePrefab_Patch { 
            public static void Postfix(GameObject __result) {
                __result.AddComponent<Manual>();
                __result.AddTag("DontShowSurveyable");
            }
        }

        [HarmonyPatch(typeof(GeneShufflerConfig), nameof(GeneShufflerConfig.CreatePrefab))]
        public class GeneShufflerConfig_CreatePrefab_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddComponent<Manual>();
                __result.AddTag("DontShowSurveyable");
            }
        }

        [HarmonyPatch(typeof(MegaBrainTank), "OnSpawn")]
        public class MegaBrainTank_OnSpawn_Patch { 
            public static void Postfix(MegaBrainTank __instance) {
                __instance.gameObject.AddOrGet<Surveyable>();
            }
        }
    }
}
