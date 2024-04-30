using KSerialization;
using PeterHan.PLib.Core;
using static WirelessProject.ProwerManager.StaticVar;
using UnityEngine;

namespace WirelessProject.ProwerManager {
    public class BaseLinkToProxy : KMonoBehaviour {
        [Serialize]
        public bool hasProxy = false;
        [Serialize]
        public int ProxyCell = -1;
        //public PowerProxy proxy = null;
        public PowerProxy.ProxyList proxyList;

        private static readonly EventSystem.IntraObjectHandler<BaseLinkToProxy> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BaseLinkToProxy>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);

            if (ProxyCell != -1) {
                PowerProxiesWithCell.TryGetValue(ProxyCell, out PowerProxy.ProxyList new_proxy);
                if (new_proxy != null) {
                    proxyList = new_proxy;
                } else {
                    PowerProxy.ProxyList new_init_proxy = new PowerProxy.ProxyList {
                        ThisCell = ProxyCell,
                    };
                    PowerProxiesWithCell.Add(ProxyCell, new_init_proxy);
                }
                AddThisToProxy();
            }
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            RemoveThisFromProxy(true);
        }

        private void OnRefreshUserMenu(object _) {
            GameObject go = new GameObject("screen");
            DetailsScreen.Instance.SetSecondarySideScreen(go.AddComponent<PowerProxyScreen>(), "nihao");
            if (PowerProxiesWithCell.Count == 0) return;

            Game.Instance.userMenu.AddButton(
                gameObject,
                new KIconButtonMenu.ButtonInfo(
                    "action_follow_cam",
                    "管理终端",
                    OpenDialog,
                    tooltipText: "NOO")
                );
        }

        private void OpenDialog() {
            new AddToProxyDialog(this);
        }

        public virtual void RemoveThisFromProxy(bool _ = false) {
            gameObject.RemoveTag(HasProxyTag);
            hasProxy = false;
            proxyList = null;
            ProxyCell = -1;
        }

        protected virtual void AddThisToProxy() {
            gameObject.AddTag(HasProxyTag);
            hasProxy = true;
        }

        public virtual void ChangeProxy(PowerProxy.ProxyList new_proxy) { }
    }
}
