using HarmonyLib;
using Klei.AI;
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

    // 反熵
    [HarmonyPatch(typeof(MassiveHeatSinkConfig), nameof(MassiveHeatSinkConfig.DoPostConfigureComplete))]
    public class Patch_3 {
      public static void Postfix(GameObject go) {
        go.AddOrGet<CommonMovable>();
      }
    }

    // 睡衣柜
    [HarmonyPatch(typeof(GravitasContainerConfig), nameof(GravitasContainerConfig.DoPostConfigureComplete))]
    public class Patch_4 {
      public static void Postfix(GameObject go) {
        go.AddOrGet<CommonMovable>();
      }
    }

    // 辐射蜂巢
    [HarmonyPatch(typeof(BaseBeeHiveConfig), nameof(BaseBeeHiveConfig.CreatePrefab))]
    public class Patch_5 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<CommonMovable>();
      }
    }

    #endregion
  }
}