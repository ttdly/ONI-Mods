using PeterHan.PLib.UI;
using System.Collections.Generic;
using UnityEngine;

namespace WirelessProject.ProwerManager {
    public class AddToProxyDialog {
        public static AddToProxyDialog Instance;
        private readonly GameObject DialogObj;
        private PCheckBox checkBox1;
        private GameObject previewObj = null;

        public AddToProxyDialog(BaseLinkToProxy addToProxy) {
            PPanel mainPanel = new PPanel("MainPanel") {
                FlexSize = Vector2.right,
                Spacing = 8,
                Alignment = TextAnchor.MiddleLeft,
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
            GameObject checkBoxObject = null;
            PCheckBox checkBox = new PCheckBox() {
                Text = "不接入终端",
                InitialState = addToProxy.ProxyCell == -1 ? 1 : 0,
                FlexSize = Vector2.right,
            }.AddOnRealize((obj) => { checkBoxObject = obj; });
            checkBox.OnChecked = delegate {
                addToProxy.ChangeProxy(null);
                if (previewObj != null) {
                    PCheckBox.SetCheckState(previewObj, 0);
                    previewObj = checkBoxObject;
                    PCheckBox.SetCheckState(checkBoxObject, 1);
                }
            };
            parent.AddChild(checkBox);
        }

        public void ShowDialog(PPanel parent, BaseLinkToProxy addToProxy) {
            AddNull(parent, addToProxy);
            
            foreach (KeyValuePair<int, PowerProxy.ProxyList> valuePair in StaticVar.PowerInfoList) {
                GameObject checkBoxObject = null;
                bool isCurr = addToProxy.ProxyCell == valuePair.Key;
                PCheckBox checkBox = new PCheckBox() {
                    Text = valuePair.Value.proxy.gameObject.GetProperName(),
                    InitialState = isCurr ? 1 : 0,
                    FlexSize = Vector2.right,
                }.AddOnRealize((obj) => { checkBoxObject = obj; }); ;
                checkBox.OnChecked = delegate {
                    addToProxy.ChangeProxy(valuePair.Value);
                    if (previewObj != null) {
                        PCheckBox.SetCheckState(previewObj, 0);
                        previewObj = checkBoxObject;
                        PCheckBox.SetCheckState(checkBoxObject, 1);
                    }
                };
                parent.AddChild(checkBox);
            }
        }
    }
}
