using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using UnityEngine;
using static SpaceStore.MyString;

namespace SpaceStore.Store {
    public class StoreDialog {
        public static GameObject DialogObj;
        public static PLabel CoinLabel;
        public const int SPACE = 6;
        public const int COL = 5;
        public const int DIALOG_HEIGHT = 400;
        public const int ITEM_WIDTH = 130;
        public const float ICON_SIZE = 52;
        public const float COIN_ICON_SIZE = 24;

        public StoreDialog() {
            PPanel mainPanel = new PPanel("MainPanel") {
                FlexSize = Vector2.right,
                BackColor = Color.white,
                Spacing = 8,
            };

            CoinLabel = new PLabel("coin") {
                Text = UI.STORE.STOREDIALOG.DISABLEED,
                Margin = new RectOffset(0, 0, 0, 20),
                TextStyle = PUITuning.Fonts.TextDarkStyle,
                Sprite = StaticVars.CoinIcon,
                SpriteSize = new Vector2(COIN_ICON_SIZE, COIN_ICON_SIZE),
                SpritePosition = TextAnchor.MiddleLeft,
                FlexSize = Vector2.right,
                TextAlignment = TextAnchor.MiddleLeft,
            };
            RefreshCoin();
            CreateItems(mainPanel);

            PScrollPane scrollPane = new PScrollPane("ScrollPane") {
                Child = mainPanel,
                FlexSize = Vector2.right,
                ScrollVertical = true,
                AlwaysShowVertical = true,
                TrackSize = 8,
            };

            PDialog main = new PDialog("main") {
                Title = UI.MENU_TOOL.TITLE,
                DialogClosed = _ => ToolMenu.Instance.ClearSelection(),
                MaxSize = new Vector2(0, SingletonOptions<Options>.Instance.DialogHeight),
                DialogBackColor = Color.white,
            };
            main.Body.AddChild(CoinLabel);
            main.Body.AddChild(scrollPane);
            DialogObj = main.Build();
            DialogObj.SetActive(false);
        }

        public void CreateItems(PPanel parent) {
            PPanel rowPane = new PPanel("null");

            int col = 0;
            if (Components.Telepads.Count > 0) {
                for (int i = 0; i < StoreList.marketItems.Count; i++) {
                    if (col % SingletonOptions<Options>.Instance.Col == 0) {
                        rowPane = new PPanel("rowPanel" + i.ToString()) {
                            Direction = PanelDirection.Horizontal,
                            Spacing = SPACE,
                            Margin = new RectOffset(0, SPACE + 10, 0, 0),
                        };
                        parent.AddChild(rowPane);
                    }
                    PRelativePanel panel = CreateItem(StoreList.marketItems[i]);
                    rowPane.AddChild(panel);
                    col++;
                }
            } else {
                CoinLabel.Text = UI.STORE.STOREDIALOG.DISABLEED;
                CoinLabel.Sprite = null;
                CoinLabel.TextAlignment = TextAnchor.MiddleCenter;
            }
        }

        public PRelativePanel CreateItem(StoreList.MarketItem marketItem) {
            PRelativePanel itemContainer = new PRelativePanel($"{marketItem.name}Container") {
                FlexSize = Vector2.right,
                BackImage = PUITuning.Images.ButtonBorder,
                ImageMode = UnityEngine.UI.Image.Type.Sliced,
                BackColor = PUITuning.Colors.ButtonBlueStyle.inactiveColor,
                DynamicSize = false,
            };


            PLabel label = new PLabel($"{marketItem.name}Label") {
                Sprite = marketItem.sprite,
                SpriteSize = new Vector2(ICON_SIZE, ICON_SIZE),
                SpritePosition = TextAnchor.UpperCenter,
                Text = marketItem.GetDesc(),
                IconSpacing = SPACE,
                FlexSize = Vector2.right,
                TextAlignment = TextAnchor.LowerCenter,
                Margin = new RectOffset(5, 5, 5, 20),
            };


            PSpacer spacer = new PSpacer() {
                PreferredSize = new Vector2(SingletonOptions<Options>.Instance.ItemWidth, 0),
            };

            PButton btn = new PButton($"{marketItem.name}Button") {
                Sprite = StaticVars.CoinIcon,
                SpritePosition = TextAnchor.MiddleLeft,
                SpriteSize = new Vector2(COIN_ICON_SIZE, COIN_ICON_SIZE),
                Text = marketItem.price.ToString(),
                FlexSize = Vector2.right,
                Color = PUITuning.Colors.ButtonBlueStyle,
                OnClick = delegate {
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
                    SpaceStoreTool.Instance.DeactivateTool();
                }
            };

            itemContainer.AddChild(label).AddChild(spacer).AddChild(btn).SetBottomEdge(btn, 0);
            return itemContainer;
        }

        public static void RefreshCoin() {
            CoinLabel.Text = ((int)StaticVars.Coin).ToString();
        }
    }
}
