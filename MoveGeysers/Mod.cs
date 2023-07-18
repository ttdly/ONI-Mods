using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Movable movable = __result.AddOrGet<Movable>();
            movable.requiredSkillPerk = Db.Get().SkillPerks.IncreaseCarryAmountMedium.Id;
            __result.GetComponent<KPrefabID>().RemoveTag(GameTags.Other);

        }
    }

    //[HarmonyPatch(typeof(OilWellConfig), nameof(OilWellConfig.CreatePrefab))]
    //public class OilWellConfig_CreatePrefab_Patch {
    //    public static void Postfix(GameObject __result) {
    //        Pickupable pickupable = __result.AddOrGet<Pickupable>();
    //        pickupable.SetWorkTime(50f);
    //        pickupable.sortOrder = 0;
    //        pickupable.deleteOffGrid = false;
    //        __result.AddOrGet<Movable>();
    //        __result.GetComponent<KPrefabID>().RemoveTag(GameTags.BuildableRaw);
    //    }
    //}
}
