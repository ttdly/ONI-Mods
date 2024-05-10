using HarmonyLib;
using PeterHan.PLib.Actions;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using SpaceStore.SellButtons;
using SpaceStore.Store;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SpaceStore {
    public sealed class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
#if DEBUG
            ModUtil.RegisterForTranslation(typeof(MyString));
#endif
            // 初始化 PUtil 的文件
            PUtil.InitLibrary();
            // 检查模组版本是否更新
            new PVersionCheck().Register(this, new SteamVersionChecker());
            new POptions().RegisterOptions(this, typeof(Options));
            LocString.CreateLocStringKeys(typeof(MyString.OPTIONS));
            new PLocalization().Register();
            StaticVars.Action = new PActionManager().CreateAction("OpenSpaceStore", MyString.UI.MENU_TOOL.TITLE, new PKeyBinding());
            StaticVars.CoinIcon = PUIUtils.LoadSprite("SpaceStore.images.coin.png");
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods) {
            base.OnAllModsLoaded(harmony, mods);
            LocString.CreateLocStringKeys(typeof(MyString.OPTIONS));
            new PriceConvter();
        }

    }

    public static class StaticVars {
        public static string ToolName = "SpaceStoreTool";
        public static float Coin = 0;
        public static PAction Action;
        public static Sprite CoinIcon;
        public static string LOCAL_FILE_DIR = Path.GetFullPath(Path.Combine(KMod.Manager.GetDirectory(), "spacestore"));
        public static List<ElementSellButton> Buttons = new List<ElementSellButton>();


        public static void AddCoin(float amount) {
            Coin += amount;
        }
    }
}
