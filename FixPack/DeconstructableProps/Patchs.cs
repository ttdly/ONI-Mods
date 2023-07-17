using HarmonyLib;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FixPack.DeconstructableProps {
    public class Patchs {

        //[HarmonyPatch(typeof(Deconstructable), "TriggerDestroy")]
        //public class Deconstructable_TriggerDestroy_Patch {
        //    public static void Prefix( Deconstructable __instance, float temperature, byte disease_idx, int disease_count) {
        //        PUtil.LogDebug("不知大干啥的");
        //        __instance.constructionElements = ((IEnumerable<Tag>)__instance.constructionElements).Where<Tag>((Func<Tag, bool>)(x => x.GetHash() != 1838482828)).ToArray<Tag>();
        //    }
        //}

        // 低温箱
        [HarmonyPatch(typeof(CryoTankConfig), nameof(CryoTankConfig.CreatePrefab))]
        public class CryoTankConfig_CreatePrefab_Patch {
            public static void Postfix(ref GameObject __result) {
                if (!SingletonOptions<Option>.Instance.ActiveDeconstructableProps) return;
                __result.AddOrGet<Demolishable>();
            }
        }
        // 安全门
        [HarmonyPatch(typeof(POIBunkerExteriorDoor), nameof(POIBunkerExteriorDoor.DoPostConfigureComplete))]
        public class POIBunkerExteriorDoor_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                if (!SingletonOptions<Option>.Instance.ActiveDeconstructableProps) return;
                go.GetComponent<Deconstructable>().allowDeconstruction = true;
            }
        }
        // 传送仓输入端
        [HarmonyPatch(typeof(WarpConduitReceiverConfig), nameof(WarpConduitReceiverConfig.DoPostConfigureComplete))]
        public class WarpConduitReceiverConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                if (!SingletonOptions<Option>.Instance.ActiveDeconstructableProps) return;
                go.GetComponent<Deconstructable>().SetAllowDeconstruction(true);
            }
        }
        // 传送仓接收端
        [HarmonyPatch(typeof(WarpConduitSenderConfig), nameof(WarpConduitSenderConfig.DoPostConfigureComplete))]
        public class WarpConduitSenderConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                if (!SingletonOptions<Option>.Instance.ActiveDeconstructableProps) return;
                go.GetComponent<Deconstructable>().SetAllowDeconstruction(true);
            }
        }
        // 复制人传送仓输入端
        [HarmonyPatch(typeof(WarpReceiverConfig), nameof(WarpReceiverConfig.CreatePrefab))]
        public class WarpReceiverConfig_CreatePrefab_Patch {
            public static void Postfix(ref GameObject __result) {
                if (!SingletonOptions<Option>.Instance.ActiveDeconstructableProps) return;
                __result.AddOrGet<Demolishable>();
            }
        }
        // 复制人传送仓输出端
        [HarmonyPatch(typeof(WarpPortalConfig), nameof(WarpPortalConfig.CreatePrefab))]
        public class WarpPortalConfig_CreatePrefab_Patch {
            public static void Postfix(ref GameObject __result) {
                if (!SingletonOptions<Option>.Instance.ActiveDeconstructableProps) return;
                __result.AddOrGet<Demolishable>();
            }
        }
        // 时空裂缝开口器
        [HarmonyPatch(typeof(TemporalTearOpenerConfig), nameof(TemporalTearOpenerConfig.DoPostConfigureComplete))]
        public class TemporalTearOpenerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                if (!SingletonOptions<Option>.Instance.ActiveDeconstructableProps) return;
                go.GetComponent<Deconstructable>().allowDeconstruction = true;
            }
        }
    }
}
