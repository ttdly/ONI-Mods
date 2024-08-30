using System.Reflection;
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

    public static void PatchBuildings(Harmony harmony) {
      var targetMethod_common_1 = typeof(TemporalTearOpenerConfig).GetMethod("DoPostConfigureComplete");
      var targetMethod_noCrossMove_1 = typeof(WarpConduitSenderConfig).GetMethod("DoPostConfigureComplete");
      var targetMethod_noCrossMove_2 = typeof(WarpConduitReceiverConfig).GetMethod("DoPostConfigureComplete");
      var targetMethod_headquarters = typeof(HeadquartersConfig).GetMethod("DoPostConfigureComplete");
      var postfix_common = AccessTools.Method(typeof(GravitiesMovable), nameof(PostfixCommon)); 
      var postfix_noCrossMove = AccessTools.Method(typeof(GravitiesMovable), nameof(PostfixNoCrossMove));
      var postfix_headquarters = AccessTools.Method(typeof(GravitiesMovable), nameof(PostfixHeadquarters));
      harmony.Patch(targetMethod_common_1, postfix: new HarmonyMethod(postfix_common));
      harmony.Patch(targetMethod_noCrossMove_1, postfix: new HarmonyMethod(postfix_noCrossMove));
      harmony.Patch(targetMethod_noCrossMove_2, postfix: new HarmonyMethod(postfix_noCrossMove));
      harmony.Patch(targetMethod_headquarters, postfix: new HarmonyMethod(postfix_headquarters));
    }

    public static void PostfixNoCrossMove(GameObject go) {
      go.AddOrGet<GravitiesMovable>().canCrossMove = false;
    }

    public static void PostfixHeadquarters(GameObject go) {
      var movable = go.AddOrGet<GravitiesMovable>();
      movable.canCrossMove = false;
      movable.needNew = false;
    }

    public static void PostfixCommon(GameObject go) {
      go.AddOrGet<GravitiesMovable>();
    }

    [HarmonyPatch(typeof(GeneShufflerConfig), nameof(GeneShufflerConfig.CreatePrefab))]
    public class Patch_1 {
      public static void Postfix(GameObject __result) {
        __result.AddOrGet<GravitiesMovable>();
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