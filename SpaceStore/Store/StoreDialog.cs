using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using SpaceStore.SellButtons;
using UnityEngine;
using UnityEngine.UI;
using static SpaceStore.MyString;

namespace SpaceStore.Store {
  public class StoreDialog {
    public const int SPACE = 6;
    public const int COL = 5;
    public const int DIALOG_HEIGHT = 400;
    public const int ITEM_WIDTH = 130;
    public const float ICON_SIZE = 52;
    public const float COIN_ICON_SIZE = 24;
    public static GameObject DialogObj;
    public static PLabel CoinLabel;
    public static Text CoinText;

    public StoreDialog() {
      var mainPanel = new PPanel("MainPanel") {
        FlexSize = Vector2.right,
        BackColor = Color.white,
        Spacing = 8
      };

      CoinLabel = new PLabel("coin") {
        Text = UI.STORE.STOREDIALOG.DISABLEED,
        Margin = new RectOffset(0, 0, 0, 20),
        TextStyle = PUITuning.Fonts.TextDarkStyle,
        Sprite = StaticVars.CoinIcon,
        SpriteSize = new Vector2(COIN_ICON_SIZE, COIN_ICON_SIZE),
        SpritePosition = TextAnchor.MiddleLeft,
        FlexSize = Vector2.right,
        TextAlignment = TextAnchor.MiddleLeft
      };
      RefreshCoin();

      if (StoreList.marketItems.Count == 0) StoreList.Init();

      CreateItems(mainPanel);

      var scrollPane = new PScrollPane("ScrollPane") {
        Child = mainPanel,
        FlexSize = Vector2.right,
        ScrollVertical = true,
        AlwaysShowVertical = true,
        TrackSize = 8
      };

      var main = new PDialog("main") {
        Title = UI.MENU_TOOL.TITLE,
        DialogClosed = _ => ToolMenu.Instance.ClearSelection(),
        MaxSize = new Vector2(0, SingletonOptions<Options>.Instance.DialogHeight),
        DialogBackColor = Color.white
      };

      var pSpacer = new PSpacer {
        PreferredSize = new Vector2(0, 10)
      };

      var refreshBtn = new PButton("refresh") {
        Text = UI.STORE.STOREDIALOG.REFRESH,
        OnClick = delegate {
          SpaceStoreTool.Instance.DeactivateTool();
          StoreList.marketItems.Clear();
          new PriceConvter();
          DialogObj.DeleteObject();
          DialogObj = null;
        }
      };

      var openConfigFolderBtn = new PButton("openfolder") {
        Text = UI.STORE.STOREDIALOG.OPEN_CONFIG_FOLDER,
        OnClick = delegate { StoreList.OpenConfigFolder(); }
      };

      var openWebPage = new PButton("openwebPage") {
        Text = UI.STORE.STOREDIALOG.OPEN_WEB_PAGE,
        OnClick = delegate { StoreList.OpenWebPage(); }
      };

      var fileActionButton = new PPanel("FileOption") {
        Spacing = SPACE,
        Direction = PanelDirection.Horizontal
      };

      fileActionButton
        .AddChild(refreshBtn)
        .AddChild(openConfigFolderBtn)
        .AddChild(openWebPage);
      main.Body
        .AddChild(CoinLabel)
        .AddChild(scrollPane)
        .AddChild(pSpacer)
        .AddChild(fileActionButton);
      DialogObj = main.Build();
      DialogObj.SetActive(false);
    }

    public void CreateItems(PPanel parent) {
      var rowPane = new PPanel("null");

      var col = 0;
      if (Components.Telepads.Count > 0) {
        for (var i = 0; i < StoreList.marketItems.Count; i++) {
          if (col % SingletonOptions<Options>.Instance.Col == 0) {
            rowPane = new PPanel("rowPanel" + i) {
              Direction = PanelDirection.Horizontal,
              Spacing = SPACE,
              Margin = new RectOffset(0, SPACE + 10, 0, 0)
            };
            parent.AddChild(rowPane);
          }

          var panel = CreateItem(StoreList.marketItems[i]);
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
      var itemContainer = new PRelativePanel($"{marketItem.name}Container") {
        FlexSize = Vector2.right,
        BackImage = PUITuning.Images.ButtonBorder,
        ImageMode = Image.Type.Sliced,
        BackColor = PUITuning.Colors.ButtonBlueStyle.inactiveColor,
        DynamicSize = false
      };

      var label = new PLabel($"{marketItem.name}Label") {
        Sprite = marketItem.sprite.first,
        SpriteTint = marketItem.sprite.second,
        SpriteSize = new Vector2(ICON_SIZE, ICON_SIZE),
        SpritePosition = TextAnchor.UpperCenter,
        Text = marketItem.GetDesc(),
        IconSpacing = SPACE,
        FlexSize = Vector2.right,
        TextAlignment = TextAnchor.LowerCenter,
        Margin = new RectOffset(5, 5, 5, 20)
      };


      var spacer = new PSpacer {
        PreferredSize = new Vector2(SingletonOptions<Options>.Instance.ItemWidth, 0)
      };

      var btn = new PButton($"{marketItem.name}Button") {
        Sprite = StaticVars.CoinIcon,
        SpritePosition = TextAnchor.MiddleLeft,
        SpriteSize = new Vector2(COIN_ICON_SIZE, COIN_ICON_SIZE),
        Text = marketItem.price.ToString(),
        FlexSize = Vector2.right,
        Color = PUITuning.Colors.ButtonBlueStyle,
        OnClick = delegate {
//#if DEBUG
//                    if (StaticVars.Coin < marketItem.price) {
//                        StaticVars.Coin = marketItem.price * 2;
//                    }
//#endif
//                    Telepad telepad = GetCurrTelepad();
//                    if (StaticVars.Coin < marketItem.price || telepad == null) {
//                        return;
//                    }
#if DEBUG
                    if (StaticVars.coinSaver.coin < marketItem.price) {
                        StaticVars.coinSaver.coin = marketItem.price * 2;
                    }
#endif
          var telepad = GetCurrTelepad();
          if (StaticVars.coinSaver == null || StaticVars.coinSaver.coin < marketItem.price || telepad == null) return;
          StaticVars.coinSaver.AddCoin(-marketItem.price);
          //StaticVars.AddCoin(-marketItem.price);
          marketItem.info.Deliver(telepad.transform.position);
          CameraController.Instance.CameraGoTo(telepad.transform.position);
          RefreshCoin();
          SpaceStoreTool.Instance.DeactivateTool();
        }
      };

      itemContainer.AddChild(label).AddChild(spacer).AddChild(btn).SetBottomEdge(btn, 0);
      return itemContainer;
    }

    public static void RefreshCoin() {
      if (StaticVars.coinSaver == null)
        CoinLabel.Text = "NULL";
      else
        CoinLabel.Text = string.Format("{0:0.00}", StaticVars.coinSaver.coin);
      //CoinLabel.Text = string.Format("{0:0.00}", StaticVars.Coin);
    }

    public static Telepad GetCurrTelepad() {
      if (Components.Telepads.Count == 0) return null;
      if (!DlcManager.GetActiveDLCIds().Contains("EXPANSION1_ID")) return Components.Telepads[0];

      var list = Components.Telepads.GetWorldItems(ClusterManager.Instance.activeWorldId);
      if (list.Count == 0) return Components.Telepads[0];
      return list[0];
    }
  }
}