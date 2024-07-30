using HarmonyLib;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable.Stories {
  public class StoryMovable : BaseMovable {
    public override void Move(int targetCell) {
      base.StableMove(targetCell);
    }

    #region 补丁

    [HarmonyPatch(typeof(FossilDigSiteConfig), nameof(FossilDigSiteConfig.DoPostConfigureComplete))]
    public class Patch_2 {
      private static void Postfix(GameObject go) {
        go.AddOrGet<StoryMovable>();
      }
    }

    [HarmonyPatch(typeof(FossilSiteConfig_Ice), nameof(FossilSiteConfig_Ice.CreatePrefab))]
    public class Patch_3 {
      private static void Postfix(GameObject __result) {
        RemoveGravitiesAndAddMovable<StoryMovable>(__result);
      }
    }

    [HarmonyPatch(typeof(FossilSiteConfig_Resin), nameof(FossilSiteConfig_Ice.CreatePrefab))]
    public class Patch_4 {
      private static void Postfix(GameObject __result) {
        RemoveGravitiesAndAddMovable<StoryMovable>(__result);
      }
    }

    [HarmonyPatch(typeof(FossilSiteConfig_Rock), nameof(FossilSiteConfig_Ice.CreatePrefab))]
    public class Patch_5 {
      private static void Postfix(GameObject __result) {
        RemoveGravitiesAndAddMovable<StoryMovable>(__result);
      }
    }

    // 梦境合成
    [HarmonyPatch(typeof(MegaBrainTankConfig), "OnSpawn")]
    public class MegaBrainTank_OnSpawn_Patch {
      public static void Postfix(MegaBrainTank __instance) {
        __instance.gameObject.AddOrGet<StoryMovable>();
      }
    }

    // 生物织构
    [HarmonyPatch(typeof(MorbRoverMakerWorkable), "OnSpawn")]
    public class MorbRoverMakerWorkable_OnSpawn_Patch {
      public static void Postfix(MorbRoverMakerWorkable __instance) {
        __instance.gameObject.AddOrGet<StoryMovable>();
      }
    }

    #endregion
  }
}