using HarmonyLib;
using PipStore.Screen;

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
                MultiToggle storeToggleButton;
                if (storeToggle.TryGetComponent<MultiToggle>(out storeToggleButton)) {
                    LogUtil.Info("找到组件");
                    storeToggleButton.onClick += delegate {
                        LogUtil.Info("点击事件");
                        PipStoreScreen.ShowWindow();
                    };
                }
            }
        }
    }
}