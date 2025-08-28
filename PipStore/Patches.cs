using HarmonyLib;
using PipStore.Screen;
using STRINGS;

namespace PipStore {
    public class Patches {
        [HarmonyPatch(typeof(TopLeftControlScreen), "OnActivate")]
        private static class TopLeftControlScreenOnActivatePatch {
            private static void Postfix(TopLeftControlScreen __instance) {
                var templateButton = Traverse.Create(__instance).Field("kleiItemDropButton").GetValue<MultiToggle>();
                var storeToggle = Util.KInstantiateUI(
                    templateButton.gameObject,
                    templateButton.gameObject.transform.parent.gameObject,
                    true);
                if (!storeToggle.TryGetComponent<MultiToggle>(out var storeToggleButton)) {
                    LogUtil.Error("未找到绑定组件");
                }
                storeToggleButton.onClick += PipStoreScreen.ShowWindow;
            }
        }

        [HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
        private static class SaveGameOnPrefabInitPatch {
            internal static void Postfix(SaveGame __instance) {
                __instance.gameObject.AddOrGet<SerializeData>();
                __instance.gameObject.AddOrGet<ResourcesCounter>();
            }
        }
        
    }
}