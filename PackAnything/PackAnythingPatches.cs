using System.Collections.Generic;
using HarmonyLib;
using PackAnything.Movable;
using PackAnything.MoveTool;
using UnityEngine;


namespace PackAnything {
  public class PackAnythingPatches {
    public static void EntityPostfix(GameObject __result) {
      __result.AddOrGet<ObjectCanMove>();
    }

    public static void BuildingPostfix(GameObject go) {
      go.AddOrGet<ObjectCanMove>();
    }

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
      public static void Prefix() {
        LocString.CreateLocStringKeys(typeof(PackAnythingString.STRINGS), "");
        PackAnythingStaticVars.Init();
      }
    }
    
    

    // 所有泉
    [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
    private class GeyserGenericConfig_Patch {
      public static void Postfix(GameObject __result) {
        __result.AddOrGet<GeyserMovable>();
      }
    }

    // 所有遗迹
    [HarmonyPatch(typeof(EntityTemplates), "ConfigPlacedEntity")]
    public class EntityTemplates_ConfigPlacedEntity_Patch {
      public static void Postfix(GameObject __result) {
        if (__result.HasTag(GameTags.Gravitas)) __result.AddOrGet<ObjectCanMove>();
      }
    }

    // 梦境合成
    [HarmonyPatch(typeof(MegaBrainTank), "OnSpawn")]
    public class MegaBrainTank_OnSpawn_Patch {
      public static void Postfix(MegaBrainTank __instance) {
        __instance.gameObject.AddOrGet<ObjectCanMove>();
      }
    }

    // 生物织构
    [HarmonyPatch(typeof(MorbRoverMakerWorkable), "OnSpawn")]
    public class MorbRoverMakerWorkable_OnSpawn_Patch {
      public static void Postfix(MorbRoverMakerWorkable __instance) {
        __instance.gameObject.AddOrGet<ObjectCanMove>();
      }
    }

    [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
    public static class PlayerController_OnPrefabInit_Patch {
      private static T CreateToolInstance<T>(PlayerController playerController)
        where T : InterfaceTool {
        var proxyGameObject = new GameObject(typeof(T).Name);
        var tool = proxyGameObject.AddComponent<T>();
        proxyGameObject.transform.SetParent(playerController.gameObject.transform);
        proxyGameObject.SetActive(true);
        proxyGameObject.SetActive(false);
        return tool;
      }

      internal static void Postfix(PlayerController __instance) {
        var interfaceTools = new List<InterfaceTool>() {
          CreateToolInstance<EntityMoveTool>(__instance)
        };
        __instance.tools = interfaceTools.ToArray();
      }
    }
  }
}