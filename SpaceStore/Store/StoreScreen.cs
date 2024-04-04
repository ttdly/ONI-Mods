using PeterHan.PLib.UI;
using UnityEngine;
using static SpaceStore.MyString;

namespace SpaceStore.Store {
    public class StoreScreen {
        public static PDialog ScreenInstance;
        public static PLabel CoinLabel;
        public static int COL = 5;

        public static void CreateScreenInstance() {
            if (StoreList.marketItems.Count == 0) StoreList.Init();
            PDialog storeDialog = new PDialog("StoreDialog") {
                Title = "SpaceStore",
                DialogClosed = _ => ToolMenu.Instance.ClearSelection(),
            };
            storeDialog.Title = UI.MENU_TOOL.TITLE;
            ScreenInstance = storeDialog;
            CoinLabel = new PLabel("coin") {
                Text = UI.STORE.STOREDIALOG.DISABLEED,
                Margin = new RectOffset(0, 0, 0, 20),
            };
            RefreshCoin();
            PPanel mainPanel = new PPanel("MainPanel") {
                FlexSize = Vector2.right,
                Spacing = 5,
            };

            PButton btn = new PButton("Button") {
                Text = UI.STORE.STOREDIALOG.BUY,
                Margin = new RectOffset(0, 5, 0, 0),
            };
            GameObject btnObj = btn.Build();
            int offset = (int)btnObj.GetComponent<RectTransform>().sizeDelta.x + 16;
            btnObj.DeleteObject();
            
            if (Components.Telepads.Count > 0) {
                for (int i = 0; i < StoreList.marketItems.Count; i++) {
                    PRelativePanel panel = CreateContainer(StoreList.marketItems[i], offset);
                    mainPanel.AddChild(panel);
                }
            } else {
                CoinLabel.Text = UI.STORE.STOREDIALOG.DISABLEED;
            }
            PScrollPane scrollPane = new PScrollPane("ScrollMain") {
                Child = mainPanel,
                FlexSize = Vector2.right,
                AlwaysShowVertical = true,
            };
            storeDialog.Body.AddChild(CoinLabel);
            storeDialog.Body.AddChild(scrollPane);
            storeDialog.Build();
        }

        public static void RefreshCoin() {
            CoinLabel.Text = UI.STORE.STOREDIALOG.CURR_COIN + StaticVars.Coin.ToString();
        }

        private static PRelativePanel CreateContainer(StoreList.MarketItem marketItem, int offset) {
            string text = marketItem.name;
            Sprite sprite = marketItem.sprite;
            PRelativePanel relativePanel = new PRelativePanel(text + "Panel") {
                FlexSize = Vector2.right,
                BackImage = PUITuning.Images.ButtonBorder,
                ImageMode = UnityEngine.UI.Image.Type.Sliced,
                BackColor = PUITuning.Colors.ButtonBlueStyle.activeColor,
            };

            PButton btn = new PButton(text + "Button") {
                Text = UI.STORE.STOREDIALOG.BUY,
                Margin = new RectOffset(0, 5, 0, 0),
                FlexSize = new Vector2(0, offset),
            };
            RectTransform btnObj = btn.Build().GetComponent<RectTransform>();
            btnObj.DeleteObject();

            PLabel label = new PLabel(text + "Label") {
                Sprite = sprite,
                SpriteSize = new Vector2(32f, 32f),
                SpritePosition = TextAnchor.MiddleLeft,
                Text = marketItem.GetDesc(),
                IconSpacing = 8,
                FlexSize = Vector2.right,
                TextAlignment = TextAnchor.MiddleLeft,
                Margin = new RectOffset(5, offset, 5, 5)
            };

            btn.OnClick = delegate {
#if DEBUG
                if (StaticVars.Coin < marketItem.price) {
                    StaticVars.Coin = marketItem.price * 2;
                }
#endif
                if (StaticVars.Coin < marketItem.price || Components.Telepads.Count == 0) {
                    return;
                }
                StaticVars.AddCoin(-marketItem.price);
                marketItem.info.Deliver(Components.Telepads[0].transform.position);
                CameraController.Instance.CameraGoTo(Components.Telepads[0].transform.position);
                RefreshCoin();
            };
            relativePanel.AddChild(label).AddChild(btn).SetLeftEdge(label, 0).SetRightEdge(btn, 1).SetMargin(btn, new RectOffset(5, 5, 6, 6));
            return relativePanel;
        }
    }
}
