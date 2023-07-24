using HarmonyLib;
using PeterHan.PLib.Core;
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
                    if ((UnityEngine.Object)packable != (UnityEngine.Object)null) {
                        UnityEngine.Object.Destroy(__instance.GetComponent<Surveyable>());
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
                LocString.CreateLocStringKeys(typeof(PackAnything.STRINGS), "");
                new MixStatusItem(Db.Get().Root);
                new PackAnythingChoreTypes(Db.Get().Root);
                Sprite skillbadge_role_building4 = PUIUtils.LoadSprite("PackAnything.images.skillbadge_role_building4.png");
                Assets.Sprites.Add("skillbadge_role_building4", skillbadge_role_building4);
                PackAnythingStaticVars.Init();
            }
        }

        //[HarmonyPatch(typeof(MegaBrainTankConfig), nameof(MegaBrainTankConfig.ConfigureBuildingTemplate))]
        //public class MegaBrainTankConfig_DoPostConfigureComplete_Patch {
        //    public static void Prefix(GameObject go) {
        //        go.AddOrGet<Packable>();
        //    }
        //}
    }
}
