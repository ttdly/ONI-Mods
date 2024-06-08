using HarmonyLib;
using UnityEngine;
using static PackAnything.Movable.StaticMethods;

namespace PackAnything.Movable {
  public class StoryMovable : BaseMovable {
    public override void Move(int targetCell) {
      base.Move(targetCell);
      gameObject.transform.SetPosition(GetBuildingPosCbc(targetCell));
      if (gameObject.PrefabID().ToString() != LonelyMinionMailboxConfig.ID) return;
      var house = gameObject.GetComponent<LonelyMinionMailbox>().House.gameObject;
      var houseCell = Grid.OffsetCell(targetCell, new CellOffset(3, 0));
      house.transform.SetPosition(GetBuildingPosCbc(houseCell));
    }

    private static void AddMovable(GameObject go) {
      if (go.TryGetComponent(out GravitiesMovable component)) DestroyImmediate(component);
      go.AddOrGet<StoryMovable>();
    }

    #region 补丁

    [HarmonyPatch(typeof(GravitasCreatureManipulatorConfig),
      nameof(GravitasCreatureManipulatorConfig.DoPostConfigureComplete))]
    public class Patch_1 {
      private static void Postfix(GameObject go) {
        go.AddOrGet<StoryMovable>();
      }
    }

    [HarmonyPatch(typeof(FossilDigSiteConfig), nameof(FossilDigSiteConfig.DoPostConfigureComplete))]
    public class Patch_2 {
      private static void Postfix(GameObject go) {
        go.AddOrGet<StoryMovable>();
      }
    }

    [HarmonyPatch(typeof(FossilSiteConfig_Ice), nameof(FossilSiteConfig_Ice.CreatePrefab))]
    public class Patch_3 {
      private static void Postfix(GameObject __result) {
        AddMovable(__result);
      }
    }

    [HarmonyPatch(typeof(FossilSiteConfig_Resin), nameof(FossilSiteConfig_Ice.CreatePrefab))]
    public class Patch_4 {
      private static void Postfix(GameObject __result) {
        AddMovable(__result);
      }
    }

    [HarmonyPatch(typeof(FossilSiteConfig_Rock), nameof(FossilSiteConfig_Ice.CreatePrefab))]
    public class Patch_5 {
      private static void Postfix(GameObject __result) {
        AddMovable(__result);
      }
    }

    [HarmonyPatch(typeof(LonelyMinionHouseConfig), nameof(LonelyMinionHouseConfig.DoPostConfigureComplete))]
    public class Patch_6 {
      private static void Postfix(GameObject go) {
        go.AddOrGet<StoryMovable>();
      }
    }

    [HarmonyPatch(typeof(LonelyMinionMailboxConfig), nameof(LonelyMinionMailboxConfig.DoPostConfigureComplete))]
    public class Patch_7 {
      private static void Postfix(GameObject go) {
        go.AddOrGet<StoryMovable>();
      }
    }

    // 梦境合成
    [HarmonyPatch(typeof(MegaBrainTank), "OnSpawn")]
    public class MegaBrainTank_OnSpawn_Patch {
      public static void Postfix(MegaBrainTank __instance) {
        __instance.gameObject.AddOrGet<StoryMovable>();
      }
    }

    // 生物织构
    [HarmonyPatch(typeof(MorbRoverMakerWorkable), "OnSpawn")]
    public class MorbRoverMakerWorkable_OnSpawn_Patch {
      public static void Postfix(MorbRoverMakerWorkable __instance) {
        __instance.gameObject.AddOrGet<StoryMovable>();
      }
    }

    #endregion
  }
}