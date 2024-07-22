using HarmonyLib;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable {
  public class GravitiesMovable : BaseMovable {
    public bool needNew = true;
    public override void Move(int targetCell) {
      base.Move(targetCell);
      var template = needNew ? Assets.GetPrefab(gameObject.PrefabID()) : gameObject;
      var cloned = GameUtil.KInstantiate(template, GetBuildingPosCbc(targetCell), Grid.SceneLayer.Building);
      var loreBearer = gameObject.GetComponent<LoreBearer>();
      if (loreBearer != null) ToggleLoreBearer(loreBearer, cloned.AddOrGet<LoreBearer>());
      var setLocker = gameObject.GetComponent<SetLocker>();
      if (setLocker != null) ToggleSetLocker(setLocker, cloned.AddOrGet<SetLocker>());
      MoveOver(gameObject, cloned);
    }

    #region 补丁

    [HarmonyPatch(typeof(GeneShufflerConfig), nameof(GeneShufflerConfig.CreatePrefab))]
    public class Patch_1 {
      public static void Postfix(GameObject __result) {
        __result.AddOrGet<GravitiesMovable>();
      }
    }

    [HarmonyPatch(typeof(TemporalTearOpenerConfig), nameof(TemporalTearOpenerConfig.DoPostConfigureComplete))]
    public class Patch_2 {
      public static void Postfix(GameObject go) {
        go.AddOrGet<GravitiesMovable>();
      }
    }

    [HarmonyPatch(typeof(WarpConduitSenderConfig), nameof(WarpConduitSenderConfig.DoPostConfigureComplete))]
    public class Patch_3 {
      public static void Postfix(GameObject go) {
        go.AddOrGet<GravitiesMovable>().canCrossMove = false;
      }
    }

    [HarmonyPatch(typeof(WarpConduitReceiverConfig), nameof(WarpConduitReceiverConfig.DoPostConfigureComplete))]
    public class Patch_4 {
      public static void Postfix(GameObject go) {
        go.AddOrGet<GravitiesMovable>().canCrossMove = false;
      }
    }

    [HarmonyPatch(typeof(HeadquartersConfig), nameof(HeadquartersConfig.DoPostConfigureComplete))]
    public class Patch_5 {
      public static void Postfix(GameObject go) {
        var movable = go.AddOrGet<GravitiesMovable>();
        movable.canCrossMove = false;
        movable.needNew = false;
      }
    }

    [HarmonyPatch(typeof(EntityTemplates), "ConfigPlacedEntity")]
    public class EntityTemplates_ConfigPlacedEntity_Patch {
      public static void Postfix(GameObject __result) {
        if (__result.HasTag(GameTags.Gravitas)) __result.AddOrGet<GravitiesMovable>();
      }
    }

    #endregion
  }
}