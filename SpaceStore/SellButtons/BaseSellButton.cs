using SpaceStore.Store;
namespace SpaceStore.SellButtons {
    public class BaseSellButton :KMonoBehaviour {
        public int coin;

        private static readonly EventSystem.IntraObjectHandler<BaseSellButton> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BaseSellButton>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
        }

        private void OnRefreshUserMenu(object _) {
            Game.Instance.userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo("action_move_to_storage", MyString.UI.SELL.TITLE, new System.Action(Sell),
                tooltipText: MyString.UI.SELL.TOOL_TIP.Replace("{coin}", coin.ToString())));
        }

        public virtual void Sell() {
            StoreScreen.RefreshCoin();
        }
    }
}
