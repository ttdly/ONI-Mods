using HarmonyLib;
using PeterHan.PLib.Core;
using UnityEngine;

namespace PackAnything {
    public class PackAnythingPatches {

        public static void EntityPostfix(GameObject __result) {
            __result.AddOrGet<Packable>();
        }

        public static void BuildingPostfix(GameObject go) {
            go.AddOrGet<Packable>();
        }

        [HarmonyPatch(typeof(Geyser), "OnSpawn")]
        public static class Geyser_OnSpawn_Patch {
            public static void Postfix(Geyser __instance) {
                if (__instance.GetType() != typeof(Geyser)) {
                    Packable packable = __instance.GetComponent<Packable>();
                    if ((UnityEngine.Object)packable != (UnityEngine.Object)null) {
                        UnityEngine.Object.Destroy(__instance.GetComponent<Packable>());
                    }
                    return;
                } else {
                    __instance.FindOrAddComponent<Packable>();
                }
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                PUtil.LogDebug("测试" + STRINGS.DUPLICANTS.STATUSITEMS.PACKINGITEM.NAME);
                new MixStatusItem(Db.Get().Root);
                new PackAnythingChoreTypes(Db.Get().Root);
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
