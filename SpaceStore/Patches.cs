using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using SpaceStore.SellButtons;
using SpaceStore.Store;
using PeterHan.PLib.UI;

namespace SpaceStore
{
    public class Patches {

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
                var brushSellTool = new GameObject("BrushSellTool");
                var tool2 = brushSellTool.AddComponent<BrushSellTool>();
                brushSellTool.transform.SetParent(__instance.gameObject.transform);
                brushSellTool.SetActive(true);
                brushSellTool.SetActive(false);
                interfaceTools.Add(tool2);
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
            public static void Prefix(ToolMenu __instance) {
                new StoreDialog();
                if (!StaticVars.Loaded) {
                    Assets.Sprites.Add(new HashedString("icon_space_store"), PUIUtils.LoadSprite("SpaceStore.images.icon_space_store.png"));
                    StaticVars.Loaded = true;
                }
                __instance.basicTools.Add(ToolMenu.CreateToolCollection(MyString.UI.MENU_TOOL.TITLE, "icon_space_store", StaticVars.Action.GetKAction(),
                    StaticVars.ToolName, MyString.UI.MENU_TOOL.TOOL_TIP, false));
                __instance.basicTools.Add(ToolMenu.CreateToolCollection(MyString.UI.MENU_TOOL_2.TITLE, "icon_action_sweep", StaticVars.Action.GetKAction(),
                    "BrushSellTool", MyString.UI.MENU_TOOL_2.TOOL_TIP, false));
            }
        }

        [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.CreateOreEntity))]
        public static class EntityTemplates_CreateSolidOreEntity_Patch {
            public static void Postfix(GameObject __result) {
                AddSellButton<ElementSellButton>(__result);
            }
        }

        [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.CreateAndRegisterSeedForPlant))]
        public static class EntityTemplates_CreateAndRegisterSeedForPlant_Patch {
            public static void Postfix(GameObject __result) {
                AddSellButton<ElementSellButton>(__result);
            }
        }

        [HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
        public static class SaveGame_OnPrefabInit_Patch {
            internal static void Postfix(SaveGame __instance) {
                __instance.gameObject.AddOrGet<StoreData>();
            }
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Load))]
        public class Game_Load_Patch {
            public static void Prefix() {
                
                StaticVars.Coin = SaveGame.Instance.gameObject.AddOrGet<StoreData>().coin;
            }
        }

        public static void AddSellButton<T>(GameObject go) where T: KMonoBehaviour{
            if (PriceConvter.Instance.sellItems.ContainsKey(go.PrefabID())) {
                go.AddOrGet<T>();
            }
        }
    }
}
