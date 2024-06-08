using HarmonyLib;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable {
  public class RadiationMovable : BaseMovable {
    public override void Move(int targetCell) {
      base.Move(targetCell);
      var cloned = Util.KInstantiate(Assets.GetPrefab(gameObject.PrefabID()));
      cloned.transform.SetPosition(GetBuildingPosCbc(targetCell));
      ToggleLoreBearer(gameObject, cloned);
      ToggleSetLocker(gameObject, cloned);
      MoveOver(gameObject, cloned);
    }

    private static void AddRadiation(GameObject go) {
      if (go.TryGetComponent(out BaseMovable movable)) DestroyImmediate(movable);

      go.AddOrGet<RadiationMovable>();
    }

    [HarmonyPatch(typeof(PropSurfaceSatellite1Config), nameof(PropSurfaceSatellite1Config.CreatePrefab))]
    public class Patch_1 {
      private static void Postfix(GameObject __result) {
        AddRadiation(__result);
      }
    }

    [HarmonyPatch(typeof(PropSurfaceSatellite2Config), nameof(PropSurfaceSatellite2Config.CreatePrefab))]
    public class Patch_2 {
      private static void Postfix(GameObject __result) {
        AddRadiation(__result);
      }
    }

    [HarmonyPatch(typeof(PropSurfaceSatellite3Config), nameof(PropSurfaceSatellite3Config.CreatePrefab))]
    public class Patch_3 {
      private static void Postfix(GameObject __result) {
        AddRadiation(__result);
      }
    }
  }
}