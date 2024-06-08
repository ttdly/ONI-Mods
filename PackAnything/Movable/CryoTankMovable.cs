using HarmonyLib;
using PeterHan.PLib.Core;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable {
  public class CryoTankMovable : BaseMovable {
    public override void Move(int targetCell) {
      base.Move(targetCell);
      var cloned = GameUtil.KInstantiate(gameObject, GetBuildingPosCbc(targetCell), Grid.SceneLayer.Building);
      var originComponent = gameObject.GetComponent<CryoTank>();
      if (!originComponent.SidescreenButtonInteractable()) {
        PUtil.LogDebug("CryoTank ACTIVATED");
        var clonedComponent = cloned.AddOrGet<CryoTank>();
        var demolish = cloned.GetComponent<Demolishable>();
        demolish.allowDemolition = true;
        clonedComponent.smi.GoTo(clonedComponent.smi.sm.off);
        if (clonedComponent.smi.IsInsideState(clonedComponent.smi.sm.closed)) PUtil.LogDebug("Closed");
      }

      ToggleLoreBearer(gameObject, cloned);
      MoveOver(gameObject, cloned);
    }

    [HarmonyPatch(typeof(CryoTankConfig), nameof(CryoTankConfig.CreatePrefab))]
    public class Patch_1 {
      private static void Postfix(GameObject __result) {
        __result.AddOrGet<CryoTankMovable>();
      }
    }
  }
}