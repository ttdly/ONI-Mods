using HarmonyLib;
using UnityEngine;

namespace PackAnything.Movable.Stories {
  public class GravitasCreatureManipulatorMovable : BaseMovable {
    public override void Move(int targetCell) {
      base.StableMove(targetCell);
      ConvertToVacuum();
    }

    private void ConvertToVacuum() {
      if (!gameObject.TryGetComponent(out BuildingComplete buildingComplete)) return;
      var def = gameObject.GetDef<MakeBaseSolid.Def>();
      var component = gameObject.GetComponent<Building>();
      foreach (var solidOffset in def.solidOffsets) {
        var rotatedOffset = component.GetRotatedOffset(solidOffset);
        var num = Grid.OffsetCell(originCell, rotatedOffset);
        SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn,
          0.0f);
        Grid.Objects[num, 9] = null;
        Grid.Foundation[num] = false;
        Grid.SetSolid(num, false, CellEventLogger.Instance.SimCellOccupierDestroy);
        SimMessages.ClearCellProperties(num, 103);
        Grid.RenderedByWorld[num] = true;
        World.Instance.OnSolidChanged(num);
        GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
      }
    }

    #region 补丁

    [HarmonyPatch(typeof(GravitasCreatureManipulatorConfig),
      nameof(GravitasCreatureManipulatorConfig.DoPostConfigureComplete))]
    public class Patch_1 {
      private static void Postfix(GameObject go) {
        go.AddOrGet<GravitasCreatureManipulatorMovable>();
      }
    }

    #endregion
  }
}