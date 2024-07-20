using HarmonyLib;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable {
  public class GeothermalMovable : BaseMovable {
    public int[] unoOffsets;

    public override void Move(int targetCell) {
      base.Move(targetCell);
      var originCell = Grid.PosToCell(gameObject.transform.position);
      gameObject.transform.SetPosition(GetBuildingPosCbc(targetCell));
      foreach (var unoOffset in unoOffsets) {
        DeleteNeutroniumOneCell(Grid.OffsetCell(originCell, unoOffset, -1));
        AddNeutroniumOneCell(Grid.OffsetCell(targetCell, unoOffset, -1));
      }
    }

    #region Patch

    // 地热排气孔
    [HarmonyPatch(typeof(GeothermalVentConfig), nameof(GeothermalVentConfig.CreatePrefab))]
    public class Patch_1 {
      public static void Postfix(GameObject __result) {
        RemoveGravitiesAndAddMovable<GeothermalMovable>(__result);
        __result.AddOrGet<GeothermalMovable>().unoOffsets = new[] { -1, 0, 1 };
      }
    }

    [HarmonyPatch(typeof(GeothermalController), "OnSpawn")]
    public class Patch_2 {
      private static bool isAdded;

      public static void Postfix(GeothermalController __instance) {
        if (isAdded) return;
        RemoveGravitiesAndAddMovable<GeothermalMovable>(__instance.gameObject);
        __instance.gameObject.GetComponent<GeothermalMovable>().unoOffsets = new[] { -4, -3, -2, -1, 0, 1, 2, 3, 4 };
        isAdded = true;
      }
    }

    #endregion
  }
}