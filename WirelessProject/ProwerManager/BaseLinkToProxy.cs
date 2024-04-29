using KSerialization;
using PeterHan.PLib.Core;
using static WirelessProject.ProwerManager.GlobalVar;

namespace WirelessProject.ProwerManager {
    public class BaseLinkToProxy :KMonoBehaviour{
        [Serialize]
        public bool hasProxy = false;
        [Serialize]
        public int ProxyCell = -1;
        public PowerProxy proxy = null;

        private static readonly EventSystem.IntraObjectHandler<BaseLinkToProxy> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BaseLinkToProxy>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            if (ProxyCell != -1) {
                PowerProxiesWithCell.TryGetValue(ProxyCell, out PowerProxy new_proxy);
                if (new_proxy == null) {
                    hasProxy = false;
                    ProxyCell = -1;
                } else {
                    proxy = new_proxy;
                }
                if (hasProxy) {
                    AddThisToProxy();
                } else {
                    RemoveThisFromProxy();
                }
            }
        }

        private void OnRefreshUserMenu(object _) {
            if (PowerProxiesWithCell.Count == 0) return;
            if (hasProxy) {
                Game.Instance.userMenu.AddButton(
                gameObject,
                new KIconButtonMenu.ButtonInfo(
                    "action_follow_cam",
                    "断开连接",
                    RemoveThisFromProxy,
                    tooltipText: "NOO")
                );
            } else {
                Game.Instance.userMenu.AddButton(
                    gameObject,
                    new KIconButtonMenu.ButtonInfo(
                        "action_follow_cam",
                        "接入终端",
                        OpenDialog,
                        tooltipText: "NOO")
                    );
            }
        }

        private void OpenDialog() {
            new AddToProxyDialog(this);
        }

        public virtual void RemoveThisFromProxy() {
            gameObject.RemoveTag(HasProxyTag);
            hasProxy = false;
            proxy = null;
            ProxyCell = -1;
        }

        protected virtual void AddThisToProxy() {
            gameObject.AddTag(HasProxyTag);
            hasProxy = true;
        }

        public virtual void ChangeProxy(PowerProxy new_proxy) {}
    }
}
