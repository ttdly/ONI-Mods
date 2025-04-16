using System;
using GeyserExpandMachine.Buildings;
using HarmonyLib;
using PeterHan.PLib.Core;
using UnityEngine;

namespace GeyserExpandMachine.GeyserModify {
    public class Patches {
        [HarmonyPatch(typeof(GeyserGenericConfig))]
        [HarmonyPatch(nameof(GeyserGenericConfig.CreateGeyser))]
        [HarmonyPatch(new [] {
            typeof(string), typeof(string), typeof(int), typeof(int), typeof(string), typeof(string),
            typeof(HashedString), typeof(float), typeof(string[]), typeof(string[])
        })]
        public class GeyserGenericConfigPatch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1] {
                    new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), GameTags.GeyserFeature, null)
                };
            }
        }

        [HarmonyPatch(typeof(Geyser), "OnSpawn")]
        public class GeyserOnSpawnPatch {
            public static void Postfix(Geyser __instance) {
                ModData.Instance.Geysers.Add(Grid.PosToCell(__instance), __instance);
                if (ModData.Instance.BaseGeyserExpands.TryGetValue(Grid.PosToCell(__instance), out var expand)) {
                    if (!expand.safe) {
                        expand.BindGeyser(__instance);
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Geyser), "OnCleanUp")]
        public class GeyserOnCleanUpPatch {
            public static void Postfix(Geyser __instance) {
                ModData.Instance.Geysers.Add(Grid.PosToCell(__instance), __instance);
            }
        }

        #region 逻辑端口

//         [HarmonyPatch(typeof(Geyser.States), "InitializeStates")]
//         public class GeyserStatesInitializeStatesPatch {
//             private static bool GetGeyserExpandByCell(int cell, out GeyserExpand rtnGeyserExpand) {
//                 ModData.Instance.GeyserExpands.TryGetValue(cell, out var geyserExpand);
//                 rtnGeyserExpand = geyserExpand;
// // #if DEBUG
// //                 if (geyserExpand == null) {
// //                     PUtil.LogDebug($"没在 {cell} 找到目标组件");
// //                 }
// //                 else {
// //                     PUtil.LogDebug($"Find yet geyserExpand: {geyserExpand}");
// //                 }
// // #endif
//                 return geyserExpand != null;
//             }
//                 
//             
//             public static void Postfix(Geyser.States __instance) {
//                 __instance.pre_erupt.Enter(smi => {
//                     if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
//                     geyserExpand.SendLogic(GeyserExpand.OutputLogic.PreErupt);
//                     geyserExpand.AlwaysDormant();
//                 });
//       
//                 __instance.erupt.Enter(smi => {
//                     if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
//                     geyserExpand.SkipErupt();
//                 });
//       
//                 __instance.post_erupt.Enter(smi => {
//                     if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
//                     geyserExpand.SendLogic(GeyserExpand.OutputLogic.PostErupt);
//                 });
//       
//                 __instance.erupt.erupting.Enter(smi => {
//                     if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
//                     geyserExpand.SendLogic(GeyserExpand.OutputLogic.Erupting);
//                 });
//       
//                 __instance.erupt.overpressure.Enter(smi => {
//                     if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
//                     geyserExpand.SendLogic(GeyserExpand.OutputLogic.OverPressure);
//                 });
//       
//                 __instance.idle.Enter(smi => {
//                     if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
//                     geyserExpand.SendLogic(GeyserExpand.OutputLogic.Dormant);
//                     geyserExpand.SkipIdle();
//                 });
//
//                 __instance.dormant.Enter(smi => {
//                     if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
//                     geyserExpand.SendLogic(GeyserExpand.OutputLogic.Dormant);
//                     geyserExpand.SkipDormant();
//                 });
//             }
//         }

        #endregion

        #region 输出端口

        [HarmonyPatch(typeof(ElementEmitter), "SetEmitting")]
        public class ElementEmitterSetEmittingPatch {
            public static void Postfix(ElementEmitter __instance, bool emitting) {
                if (!ModData.Instance.BaseGeyserExpands.
                        TryGetValue(Grid.PosToCell(__instance), out var geyserExpandProxy)) return;
                geyserExpandProxy.close = !emitting;
            }
        }

        [HarmonyPatch(typeof(ElementEmitter), "OnSimActivate")]
        public class ElementEmitterOnSimActivatePatch {
            public static bool Prefix(ElementEmitter __instance) {
                if (!ModData.Instance.BaseGeyserExpands.
                        TryGetValue(Grid.PosToCell(__instance), out var geyserExpandProxy)) return true;
                geyserExpandProxy.close = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(ElementEmitter), "OnSimDeactivate")]
        public class ElementEmitterOnSimDeactivatePatch {
            public static bool Prefix(ElementEmitter __instance) {
                if (!ModData.Instance.BaseGeyserExpands.
                        TryGetValue(Grid.PosToCell(__instance), out var geyserExpandProxy)) return true;
                geyserExpandProxy.close = true;
                return false;
            }
        }


        [HarmonyPatch(typeof(Geyser), "ApplyConfigurationEmissionValues")]
        public class GeyserApplyConfigurationEmissionValuesPatch {
            public static bool Prefix(Geyser __instance, GeyserConfigurator.GeyserInstanceConfiguration config) {
                var emitter = __instance.gameObject.GetComponent<ElementEmitter>();
                if (!ModData.Instance.BaseGeyserExpands.
                        TryGetValue(Grid.PosToCell(__instance), out var geyserExpandProxy)) return true;
                var output = new ElementConverter.OutputElement(
                    config.GetEmitRate(),
                    config.GetElement(),
                    config.GetTemperature(),
                    storeOutput: true,
                    addedDiseaseIdx: config.GetDiseaseIdx(),
                    addedDiseaseCount: Mathf.RoundToInt(config.GetDiseaseCount() * config.GetEmitRate())
                );

                emitter.emitRange = 2;
                emitter.maxPressure = config.GetMaxPressure();
                emitter.outputElement = output;

                if (!emitter.IsSimActive)
                    return false;
                if (geyserExpandProxy.close)
                    geyserExpandProxy.close = false;
                emitter.SetSimActive(true);
                return false;
            }
        }
        
        #endregion

    }
}