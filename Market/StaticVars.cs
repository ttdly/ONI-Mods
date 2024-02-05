using HarmonyLib;

namespace Market {
    public class StaticVars {
        public static StatusItem NowCoin;

        public static void Init() {
            NowCoin = (StatusItem)Traverse.Create(Db.Get().MiscStatusItems).Method("CreateStatusItem", new object[] {
                    "Coin",
                    "MISC",
                    string.Empty,
                    StatusItem.IconType.Info,
                    NotificationType.Neutral,
                    false,
                    OverlayModes.None.ID,
                    true,
                    129022 }
                ).GetValue();
            NowCoin.resolveStringCallback = (str, data) => {
                TrueVendingMachineComponent vendingMachineComponent = (TrueVendingMachineComponent)data;
                str = str.Replace("{Coin}", vendingMachineComponent.coin.ToString());
                return str;
            };
        }
    }
}
