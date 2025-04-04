using System;
using HarmonyLib;
using UnityEngine;

namespace GeyserExpandMachine {
    public class GeyserPatch {
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
    }
}