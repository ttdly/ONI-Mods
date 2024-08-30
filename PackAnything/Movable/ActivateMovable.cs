using System.Reflection;
using HarmonyLib;
using PeterHan.PLib.Detours;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable {
  public class ActivateMovable : BaseMovable {
    private static readonly IDetouredField<Activatable, bool> activated =
      PDetours.DetourField<Activatable, bool>("activated");

    private static readonly MethodInfo updateFlag =
      typeof(Activatable).GetMethod("UpdateFlag", BindingFlags.NonPublic | BindingFlags.Instance);

    public override void Move(int targetCell) {
      base.Move(targetCell);
      var cloned = GameUtil.KInstantiate(gameObject, GetBuildingPosCbc(targetCell), Grid.SceneLayer.Building);
      // 同步激活组件
      var originActivate = gameObject.GetComponent<Activatable>();
      if (originActivate == null) {
      }

      if (originActivate != null && originActivate.IsActivated) {
        var clonedActivate = cloned.AddOrGet<Activatable>();
        if (clonedActivate != null) {
          activated.Set(clonedActivate, true);
          if (updateFlag != null) updateFlag.Invoke(clonedActivate, null);
        }
      }

      ToggleLoreBearer(gameObject, cloned);
      ToggleSetLocker(gameObject, cloned);
      MoveOver(gameObject, cloned);
    }

    #region 补丁

    public static void PatchBuildings(Harmony harmony) {
      var targetMethod = typeof(ExobaseHeadquartersConfig).GetMethod("DoPostConfigureComplete");
      
      var postfix_noCrossMove = AccessTools.Method(typeof(GravitiesMovable), nameof(PostfixNoCrossMove));
      harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfix_noCrossMove));
    }

    public static void PostfixNoCrossMove(GameObject go) {
      go.AddOrGet<ActivateMovable>().canCrossMove = false;
    }

    #endregion
  }
}