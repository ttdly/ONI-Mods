using HarmonyLib;
using System.Collections.Generic;
using static DetailsScreen;
using UnityEngine;
using SpaceStore.MyGeyser;
using SpaceStore.StoreRoboPanel;
using SpaceStore.SellButtons;
using SpaceStore.Store;

namespace SpaceStore
{
    public class Patches {

        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class DetailsScreen_OnPrefabInit_Patch {
            public static void Postfix(List<SideScreenRef> ___sideScreens, GameObject ___sideScreenConfigContentBody) {
                GeoActivatorSideScreen.AddSideScreen(___sideScreens, ___sideScreenConfigContentBody);
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                LocString.CreateLocStringKeys(typeof(MyString));
            }
        }

        [HarmonyPatch(typeof(RockCrusherConfig), nameof(RockCrusherConfig.DoPostConfigureComplete))]
        public static class RockCrusherConfig_Patch {
            public static void Postfix(GameObject go) {
                Autoable.MakeDupTinkerable(go, true);
                go.AddOrGet<RoboPanel>();
            }
        }

        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public static class PlayerController_OnPrefabInit_Patch {
            internal static void Postfix(PlayerController __instance) {
                var interfaceTools = new List<InterfaceTool>(__instance.tools);
                var spaceStoreTool = new GameObject(StaticVars.ToolName);
                var tool = spaceStoreTool.AddComponent<SpaceStoreTool>();
                spaceStoreTool.transform.SetParent(__instance.gameObject.transform);
                spaceStoreTool.SetActive(true);
                spaceStoreTool.SetActive(false);
                interfaceTools.Add(tool);
                __instance.tools = interfaceTools.ToArray();
            }
        }

        [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
        partial class GeyserGenericConfig_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<EntitySellButton>();
            }
        }

        [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
        public static class ToolMenu_CreateBasicTools_Patch {
            internal static void Prefix(ToolMenu __instance) {
                if (StoreScreen.ScreenInstance == null) { 
                    StoreScreen.CreateScreenInstance();
                }
                __instance.basicTools.Add(ToolMenu.CreateToolCollection(MyString.UI.MENU_TOOL.TITLE, "dreamIcon_earth", StaticVars.Action.GetKAction(),
                    StaticVars.ToolName, MyString.UI.MENU_TOOL.TOOL_TIP, false));
            }
        }

        [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.CreateOreEntity))]
        public static class EntityTemplates_CreateOreEntity_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<ElementSellButton>();
            }
        }

        [HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
        public static class SaveGame_OnPrefabInit_Patch {
            internal static void Postfix(SaveGame __instance) {
                __instance.gameObject.AddOrGet<StoreData>();
            }
        }
    }
}
