using System;
using GeyserExpandMachine.Buildings;
using HarmonyLib;
using PeterHan.PLib.Core;
using UnityEngine;

namespace GeyserExpandMachine.GeyserModify {
    public class Patches {
        [HarmonyPatch(typeof(GeyserGenericConfig))]
        [HarmonyPatch(nameof(GeyserGenericConfig.CreateGeyser))]
        [HarmonyPatch(new Type[] {
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
        
        [HarmonyPatch(typeof(Geyser.States), "InitializeStates")]
        public class GeyserStatesInitializeStatesPatch {
            private static bool GetGeyserExpandByCell(int cell, out GeyserExpand rtnGeyserExpand) {
                ModData.Instance.GeyserExpands.TryGetValue(cell, out var geyserExpand);
                rtnGeyserExpand = geyserExpand;
                if (geyserExpand == null) {
                    PUtil.LogDebug($"没在 {cell} 找到目标组件");
                }
                else {
                    PUtil.LogDebug($"Find yet geyserExpand: {geyserExpand}");
                }
                return geyserExpand != null;
            }
                
            
            public static void Postfix(Geyser.States __instance) {
                __instance.pre_erupt.Enter(smi => {
                    if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
                    geyserExpand.SendLogic(GeyserExpand.OutputLogic.PreErupt);
                    geyserExpand.AlwaysDormant();
                });
      
                __instance.erupt.Enter(smi => {
                    if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
                    geyserExpand.SkipErupt();
                });
      
                __instance.post_erupt.Enter(smi => {
                    if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
                    geyserExpand.SendLogic(GeyserExpand.OutputLogic.PostErupt);
                });
      
                __instance.erupt.erupting.Enter(smi => {
                    if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
                    geyserExpand.SendLogic(GeyserExpand.OutputLogic.Erupting);
                });
      
                __instance.erupt.overpressure.Enter(smi => {
                    if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
                    geyserExpand.SendLogic(GeyserExpand.OutputLogic.OverPressure);
                });
      
                __instance.idle.Enter(smi => {
                    if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
                    geyserExpand.SendLogic(GeyserExpand.OutputLogic.Dormant);
                    geyserExpand.SkipIdle();
                });

                __instance.dormant.Enter(smi => {
                    if (!GetGeyserExpandByCell(Grid.PosToCell(smi.master), out var geyserExpand)) return;
                    geyserExpand.SendLogic(GeyserExpand.OutputLogic.Dormant);
                    geyserExpand.SkipDormant();
                });
            }
        }
    }
}