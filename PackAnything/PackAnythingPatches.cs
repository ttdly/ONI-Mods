using System.Collections.Generic;
using HarmonyLib;
using PackAnything.MoveTool;
using UnityEngine;

namespace PackAnything {
  public class PackAnythingPatches {
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
      public static void Prefix() {
        LocString.CreateLocStringKeys(typeof(ModString.Options));
        LocString.CreateLocStringKeys(typeof(ModString.INFO), "STRINGS.CAL.MOD.");
      }
    }

    // 添加工具栏工具
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
        var interfaceTools = new List<InterfaceTool>(__instance.tools) {
          CreateToolInstance<EntityMoveTool>(__instance)
        };
        __instance.tools = interfaceTools.ToArray();
      }
    }
  }
}