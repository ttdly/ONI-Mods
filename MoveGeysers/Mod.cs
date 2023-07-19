using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;


namespace MoveGeysers {
    public class Mod :KMod.UserMod2{
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }
    }

    [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
    public static class GeyserGenericConfig_CreateGeyser_Patch {
        public static void Postfix(GameObject __result) {
            __result.AddOrGet<Pickupable>();
            __result.AddOrGet<Movable>();
            //__result.GetComponent<KPrefabID>().RemoveTag(GameTags.Other);
            __result.AddOrGet<DoPost>();
        }
    }

    [HarmonyPatch(typeof(OilWellConfig), nameof(OilWellConfig.CreatePrefab))]
    public class OilWellConfig_CreatePrefab_Patch {
        public static void Postfix(GameObject __result) {
            __result.AddOrGet<Pickupable>();
            __result.AddOrGet<Movable>();
            __result.GetComponent<KPrefabID>().RemoveTag(GameTags.BuildableRaw);
            __result.AddOrGet<DoPost>();
            __result.AddOrGet<OilWellSubscribe>();
        }
    }

    [HarmonyPatch(typeof(OilWellCapConfig), nameof(OilWellCapConfig.DoPostConfigureComplete))]
    public class OilWellCapConfig_DoPostConfigureComplete_Patch {
        public static void Postfix(GameObject go) {
            go.AddOrGet<OilWellTrigger>();
        }
    }
}
