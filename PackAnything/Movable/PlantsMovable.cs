using System;
using HarmonyLib;
using UnityEngine;

namespace PackAnything.Movable {
  public class PlantsMovable : BaseMovable {
    private Vector2 box = new Vector2(0f, 0f);

    protected override void OnSpawn() {
      base.OnSpawn();
      canCrossMove = true;
    }

    public override void Move(int targetCell) {
      base.StableMove(targetCell);
    }

    private void GetBox() {
      if (box.x != 0f) return;
      if (gameObject.TryGetComponent(out KBoxCollider2D kBoxCollider2D)) box = kBoxCollider2D.size;
    }

    public override bool CanMoveTo(int targetCell) {
      try {
        var offsetY = gameObject.HasTag(GameTags.Hanging) ? 1 : -1;
        var needValidCell = Grid.OffsetCell(targetCell, 0, offsetY);
        if (!Grid.IsValidCell(targetCell)) return false;
        if (Grid.Solid[targetCell]) return false;
        if (!Grid.Solid[needValidCell]) return false;
        var hasOtherEntity = Grid.ObjectLayers[(int)ObjectLayer.Plants].ContainsKey(targetCell) ||
                    Grid.ObjectLayers[(int)ObjectLayer.Building].ContainsKey(targetCell);
        return !hasOtherEntity;
      } catch (Exception) {
        return false;
      }
    }

    #region 补丁

    [HarmonyPatch(typeof(EntityTemplates), "ExtendEntityToBasicPlant")]
    public class Patch_1 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<PlantsMovable>();
      }
    }
    
    [HarmonyPatch(typeof(BasicForagePlantPlantedConfig), nameof(BasicForagePlantPlantedConfig.CreatePrefab))]
    public class Patch_2 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<PlantsMovable>();
      }
    }
    
    [HarmonyPatch(typeof(ForestForagePlantPlantedConfig), nameof(ForestForagePlantPlantedConfig.CreatePrefab))]
    public class Patch_3 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<PlantsMovable>();
      }
    }
    
    [HarmonyPatch(typeof(IceCavesForagePlantPlantedConfig), nameof(IceCavesForagePlantPlantedConfig.CreatePrefab))]
    public class Patch_4 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<PlantsMovable>();
      }
    }
    
    [HarmonyPatch(typeof(SwampForagePlantPlantedConfig), nameof(SwampForagePlantPlantedConfig.CreatePrefab))]
    public class Patch_5 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<PlantsMovable>();
      }
    }
    
    [HarmonyPatch(typeof(ColdBreatherConfig), nameof(ColdBreatherConfig.CreatePrefab))]
    public class Patch_6 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<PlantsMovable>();
      }
    }

    #endregion
  }
}