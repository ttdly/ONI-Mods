using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using UnityEngine;

namespace WirelessProject.ProwerManager {
    public class AddToProxyDialog {
        public static AddToProxyDialog Instance;
        private readonly GameObject DialogObj;
        private PPanel Preview;

        public AddToProxyDialog(ProxyLink  addToProxy) {
            PPanel mainPanel = new PPanel("MainPanel") {
                FlexSize = Vector2.right,
                BackColor = Color.white,
                Spacing = 8,
            };
            ShowDialog(mainPanel, addToProxy);
            PScrollPane scrollPane = new PScrollPane("ScrollPane") {
                Child = mainPanel,
                FlexSize = Vector2.right,
                ScrollVertical = true,
                AlwaysShowVertical = true,
                TrackSize = 8,
            };

            PDialog main = new PDialog("main") {
                Title =  "选择接入终端",
                MaxSize = new Vector2(0, 400),
                DialogBackColor = Color.white,
                DialogClosed = _ => CloseDialog(),
            };

            main.Body.AddChild(scrollPane);
            DialogObj = main.Build();
            Instance = this;
        }

        public void CloseDialog() {
            DialogObj.SetActive(false);
            DialogObj.DeleteObject();
        }

        public void ShowDialog(PPanel parent, ProxyLink addToProxy) {
            foreach (PowerProxy proxy in GlobalVar.PowerProxies) {
                bool isCurr = proxy == addToProxy.proxy;
                PPanel panel = new PPanel() {
                    Direction = PanelDirection.Horizontal,
                    FlexSize = Vector2.right,
                    BackColor = isCurr 
                    ? PUITuning.Colors.ButtonBlueStyle.activeColor 
                    : PUITuning.Colors.ButtonBlueStyle.inactiveColor,
                };
                if (isCurr) {
                    Preview = panel;
                }
                PLabel label = new PLabel() {
                    Text = proxy.gameObject.GetProperName(),
                };
                PButton btn = new PButton() {
                    Text = "连接",
                    OnClick = delegate {
                        if (isCurr) return;
                        panel.BackColor = PUITuning.Colors.ButtonBlueStyle.activeColor;
                        if (Preview != null) {
                            Preview.BackColor = PUITuning.Colors.ButtonBlueStyle.inactiveColor;
                        }
                        addToProxy.ChangeProxy(proxy);
                        CloseDialog();
                    }
                };
                panel.AddChild(label).AddChild(btn);
                parent.AddChild(panel);
            }
        }
    }
}
