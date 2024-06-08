using HarmonyLib;
using PeterHan.PLib.Detours;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable {
  public class WarpMovable : BaseMovable {
    private static readonly IDetouredField<WarpPortal, bool> discovered =
      PDetours.DetourField<WarpPortal, bool>("discovered");

    public bool isReceiver;

    protected override void OnSpawn() {
      base.OnSpawn();
      canCrossMove = false;
    }

    public override void Move(int targetCell) {
      base.Move(targetCell);
      var cloned = GameUtil.KInstantiate(gameObject, gameObject.transform.position, Grid.SceneLayer.Building);
      // 同步接收器/发送器探索状态
      switch (isReceiver) {
        case true:
          if (!gameObject.GetComponent<WarpReceiver>().Used) break;
          cloned.AddOrGet<WarpReceiver>().Used = true;
          break;
        case false:
          if (!discovered.Get(gameObject.GetComponent<WarpPortal>())) break;
          discovered.Set(cloned.AddOrGet<WarpPortal>(), true);
          break;
      }

      // 同步数据库条目状态
      ToggleLoreBearer(gameObject, cloned);
      var posCbc = GetBuildingPosCbc(targetCell);
      cloned.transform.SetPosition(posCbc);
      MoveOver(gameObject, cloned);
    }

    #region 补丁

    [HarmonyPatch(typeof(WarpPortalConfig), nameof(WarpPortalConfig.CreatePrefab))]
    public class Patch_Portal {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<WarpMovable>();
      }
    }

    [HarmonyPatch(typeof(WarpReceiverConfig), nameof(WarpReceiverConfig.CreatePrefab))]
    public class Patch_Receiver {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<WarpMovable>().isReceiver = true;
      }
    }

    #endregion
  }
}