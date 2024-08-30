using System.Reflection;
using HarmonyLib;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable {
  public class CommonMovable : BaseMovable {
    public override void Move(int targetCell) {
      base.Move(targetCell);
      var cloned = GameUtil.KInstantiate(gameObject, GetBuildingPosCbc(targetCell), Grid.SceneLayer.Building);
      MoveOver(gameObject, cloned);
    }

    #region 补丁

    public static void PatchBuildings(Harmony harmony) {
      // 反熵
      var targetMethod_1 = typeof(MassiveHeatSinkConfig).GetMethod("DoPostConfigureComplete");
      // 睡衣柜
      var targetMethod_2 = typeof(GravitasContainerConfig).GetMethod("DoPostConfigureComplete");
      // 柴堆
      var targetMethod_3 = typeof(WoodStorageConfig).GetMethod("DoPostConfigureComplete");

      var postfix = AccessTools.Method(typeof(CommonMovable), nameof(CommonPostfix));
      harmony.Patch(targetMethod_1, postfix: new HarmonyMethod(postfix));
      harmony.Patch(targetMethod_2, postfix: new HarmonyMethod(postfix));
      harmony.Patch(targetMethod_3, postfix: new HarmonyMethod(postfix));
    }

    public static void CommonPostfix(GameObject go) {
      go.AddOrGet<CommonMovable>();
    }

    // 储油石
    [HarmonyPatch(typeof(OilWellConfig), nameof(OilWellConfig.CreatePrefab))]
    public class Patch_1 {
      public static void Postfix(GameObject __result) {
        __result.AddOrGet<CommonMovable>();
      }
    }

    // 干饭树
    [HarmonyPatch(typeof(SapTreeConfig), nameof(SapTreeConfig.CreatePrefab))]
    public class Patch_2 {
      public static void Postfix(GameObject __result) {
        __result.AddOrGet<CommonMovable>();
      }
    }

    // 辐射蜂巢
    [HarmonyPatch(typeof(BaseBeeHiveConfig), nameof(BaseBeeHiveConfig.CreatePrefab))]
    public class Patch_5 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<CommonMovable>();
      }
    }

    // 流明石英
    [HarmonyPatch(typeof(PinkRockConfig), nameof(PinkRockConfig.CreatePrefab))]
    public class Patch_7 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<CommonMovable>();
      }
    }

    #endregion
  }
}