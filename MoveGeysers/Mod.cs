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
            Pickupable pickupable = __result.AddOrGet<Pickupable>();
            pickupable.SetWorkTime(50f);
            pickupable.sortOrder = 0;
            __result.AddOrGet<Movable>();
        }
    }
}
