using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using UnityEngine;

namespace SpaceStore {
    public class StoreScreen {
        public static PDialog ScreenInstance;

        public static void CreateScreenInstance() {
            PDialog mainScreen = new PDialog("StoreDialog") {
                Title = "SpaceStore"
            };
            PGridPanel mainGridPanel = new PGridPanel("MainPanel");
            mainGridPanel.AddRow(new GridRowSpec());
            mainGridPanel.AddColumn(new GridColumnSpec());
            mainGridPanel.AddColumn(new GridColumnSpec());
            
            PPanel one = CreateContainer(Def.GetUISprite(SimHashes.Salt.CreateTag()).first, "盐");
            PPanel two = CreateContainer(Def.GetUISprite(SimHashes.SandStone.CreateTag()).first, "砂岩");
            mainGridPanel.AddChild(one , new GridComponentSpec(0, 0));
            mainGridPanel.AddChild(two , new GridComponentSpec(0, 1));

            PScrollPane scrollPane = new PScrollPane("ScrollMain") {
                Child = mainGridPanel
            };
            mainScreen.Body.AddChild(scrollPane);
            mainScreen.Build();
            ScreenInstance = mainScreen;
            ScreenInstance.Show();
        }

        public static PPanel CreateContainer(Sprite sprite, string text) {
            PPanel itemContainer = new PPanel(text + "Panel");
            PButton btn = new PButton(text + "Button") {
                Text = "购买"
            };
            PLabel label = new PLabel(text + "Label") {
                Sprite = sprite,
                SpritePosition = TextAnchor.UpperCenter,
                Text = text
            };
            itemContainer.AddChild(label);
            itemContainer.AddChild(btn);
            btn.OnClick = delegate {
                PUtil.LogDebug("Click" + text);
            };
            return itemContainer;
        }
    }
}
