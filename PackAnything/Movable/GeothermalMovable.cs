using HarmonyLib;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable {
  public class GeothermalMovable : BaseMovable {
    public int[] neutroniumOffsets;
    private NeutroniumMover neutroniumMover;

    protected override void OnSpawn() {
      base.OnSpawn();
      neutroniumMover = new NeutroniumMover() {
        neutroniumOffsets = neutroniumOffsets
      };
    }

    public override void StableMove(int targetCell) {
      base.StableMove(targetCell);
      neutroniumMover.Move(originCell, targetCell);
    }

    public override void Move(int targetCell) {
      base.Move(targetCell);
      gameObject.transform.SetPosition(GetBuildingPosCbc(targetCell));
      neutroniumMover.Move(originCell, targetCell);
    }

    #region Patch

    // 地热排气孔
    [HarmonyPatch(typeof(GeothermalVentConfig), nameof(GeothermalVentConfig.CreatePrefab))]
    public class Patch_1 {
      public static void Postfix(GameObject __result) {
        RemoveGravitiesAndAddMovable<GeothermalMovable>(__result);
        __result.AddOrGet<GeothermalMovable>().neutroniumOffsets = new[] { -1, 0, 1 };
      }
    }

    [HarmonyPatch(typeof(GeothermalController), "OnSpawn")]
    public class Patch_2 {
      private static bool isAdded;

      public static void Postfix(GeothermalController __instance) {
        if (isAdded) return;
        RemoveGravitiesAndAddMovable<GeothermalMovable>(__instance.gameObject);
        __instance.gameObject.GetComponent<GeothermalMovable>().neutroniumOffsets =
          new[] { -4, -3, -2, -1, 0, 1, 2, 3, 4 };
        isAdded = true;
      }
    }

    #endregion
  }
}