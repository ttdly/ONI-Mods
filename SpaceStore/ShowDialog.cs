using SpaceStore.Store;
using System;


namespace SpaceStore {
    internal class ShowDialog :KMonoBehaviour {
        private static readonly EventSystem.IntraObjectHandler<ShowDialog> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ShowDialog>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
        }

        private void OnRefreshUserMenu(object _) {
            Game.Instance.userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo("action_repair", "hhhh", new System.Action(new System.Action(() => { StoreScreen.CreateScreenInstance(); }))));
        }
    }
}
