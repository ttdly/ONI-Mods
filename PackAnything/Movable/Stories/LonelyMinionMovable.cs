using HarmonyLib;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable.Stories {
  public class LonelyMinionMovable : BaseMovable {
    public override void Move(int targetCell) {
      base.StableMove(targetCell);
      if (!gameObject.TryGetComponent(out LonelyMinionMailbox mailbox)) return;
      var house = mailbox.House.gameObject;
      if (house == null) return;
      var houseCell = Grid.OffsetCell(targetCell, new CellOffset(3, 0));
      house.transform.SetPosition(GetBuildingPosCbc(houseCell));
    }

    #region 补丁

    [HarmonyPatch(typeof(LonelyMinionMailboxConfig), nameof(LonelyMinionMailboxConfig.DoPostConfigureComplete))]
    public class Patch_1 {
      private static void Postfix(GameObject go) {
        go.AddOrGet<LonelyMinionMovable>();
      }
    }

    [HarmonyPatch(typeof(LonelyMinionHouseConfig), nameof(LonelyMinionHouseConfig.DoPostConfigureComplete))]
    public class Patch_2 {
      private static void Postfix(GameObject go) {
        go.AddOrGet<LonelyMinionMovable>();
      }
    }

    #endregion
  }
}