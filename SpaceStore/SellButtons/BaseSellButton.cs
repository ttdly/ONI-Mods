using SpaceStore.Store;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
namespace SpaceStore.SellButtons {
    public class BaseSellButton :KMonoBehaviour {
        public float coin = 0;

        private static readonly EventSystem.IntraObjectHandler<BaseSellButton> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BaseSellButton>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
        }

        private void OnRefreshUserMenu(object _) {
            if (coin == 0) { CountPrice(); }
            if (coin < 0) { return; }
            Game.Instance.userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo("action_move_to_storage", MyString.UI.SELL.TITLE, new System.Action(Sell),
                tooltipText: MyString.UI.SELL.TOOL_TIP.Replace("{coin}", coin.ToString())
                .Replace("{item}", gameObject.GetProperName())));
        }

        public virtual void Sell() {
            PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource,
                $"{MyString.UI.COIN_NAME} {coin}", gameObject.transform, Vector3.zero);
            StoreDialog.RefreshCoin();
        }

        public virtual void CountPrice() {
            
        }
    }
}
