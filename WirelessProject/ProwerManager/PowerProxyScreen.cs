using PeterHan.PLib.UI;
using UnityEngine;

namespace WirelessProject.ProwerManager {
    public class PowerProxyScreen: KScreen{
        GameObject Container;

        protected override void OnPrefabInit() {
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

            panel.AddChild(toggle).AddChild(label).AddChild(button);
            Container = panel.Build();
            Container.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 20);
        }
    }
}
