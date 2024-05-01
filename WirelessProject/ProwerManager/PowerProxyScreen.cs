using PeterHan.PLib.UI;
using UnityEngine;

namespace WirelessProject.ProwerManager {
    public class PowerProxyScreen: KScreen{
        GameObject Container;

        protected override void OnPrefabInit() {
            var margin = new RectOffset(8, 8, 8, 8);
            if (gameObject.TryGetComponent(out BoxLayoutGroup baseLayout))
                baseLayout.Params = new BoxLayoutParams {
                    Margin = margin,
                    Direction = PanelDirection.Vertical,
                    Alignment =
                    TextAnchor.UpperCenter,
                    Spacing = 8
                };
            base.OnPrefabInit();
            PPanel panel = new PPanel() {
            };
            PLabel label = new PLabel() {
                Text = "NULLLLLLLLLL"
            };
            PButton button = new PButton() {
                Text = "aaaa"
            };
            PToggle toggle = new PToggle() {
                ToolTip = "dddd",
                Size = new Vector2(10, 20)
            };
            panel.AddChild(toggle).AddChild(label).AddChild(button).AddTo(gameObject);
            Container = gameObject;
        }
    }
}
