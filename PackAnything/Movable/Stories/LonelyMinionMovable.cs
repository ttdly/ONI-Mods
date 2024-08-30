using System.Reflection;
using HarmonyLib;
using PeterHan.PLib.Core;
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

    public static void PatchBuildings(Harmony harmony) {
      var targetMethod_1 = typeof(LonelyMinionMailboxConfig).GetMethod("DoPostConfigureComplete");
      var targetMethod_2 = typeof(LonelyMinionHouseConfig).GetMethod("DoPostConfigureComplete");

      var postfix = AccessTools.Method(typeof(LonelyMinionMovable), nameof(CommonPostfix));
      harmony.Patch(targetMethod_1, postfix: new HarmonyMethod(postfix));
      harmony.Patch(targetMethod_2, postfix: new HarmonyMethod(postfix));
    }

    public static void CommonPostfix(GameObject go) {
      go.AddOrGet<LonelyMinionMovable>();
    }

    #endregion
  }
}