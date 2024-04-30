using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using System.Collections.Generic;
using UnityEngine;
using static Database.MonumentPartResource;

namespace WirelessProject.ProwerManager {
    public class AddToProxyDialog {
        public static AddToProxyDialog Instance;
        private readonly GameObject DialogObj;

        public AddToProxyDialog(BaseLinkToProxy addToProxy) {
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
                Title = "选择接入终端",
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

        public void AddNull(PPanel parent, BaseLinkToProxy addToProxy) {
            PPanel panel = new PPanel() {
                Direction = PanelDirection.Horizontal,
                FlexSize = Vector2.right,
            };

            PLabel label = new PLabel() {
                Text = "NULL",
                TextStyle = PUITuning.Fonts.TextDarkStyle
            };
            PButton btn = new PButton() {
                Text = "连接",
                OnClick = delegate {
                    panel.BackColor = PUITuning.Colors.ButtonBlueStyle.activeColor;
                    addToProxy.ChangeProxy(null);
                    CloseDialog();
                }
            };
            panel.AddChild(label).AddChild(btn);
            parent.AddChild(panel);
        }

        public void ShowDialog(PPanel parent, BaseLinkToProxy addToProxy) {
            AddNull(parent, addToProxy);
            foreach (KeyValuePair<int, PowerProxy.ProxyList> valuePair in StaticVar.PowerProxiesWithCell) {
                PPanel panel = new PPanel() {
                    Direction = PanelDirection.Horizontal,
                    FlexSize = Vector2.right,
                };

                PLabel label = new PLabel() {
                    Text = valuePair.Value.ThisCell.ToString(),
                    FlexSize = Vector2.right,
                    TextStyle = PUITuning.Fonts.TextDarkStyle
                };
                PButton btn = new PButton() {
                    Text = "连接",
                    OnClick = delegate {
                        panel.BackColor = PUITuning.Colors.ButtonBlueStyle.activeColor;
                        addToProxy.ChangeProxy(valuePair.Value);
                        CloseDialog();
                    }
                };
                panel.AddChild(label).AddChild(btn);
                parent.AddChild(panel);
            }
        }
    }
}
