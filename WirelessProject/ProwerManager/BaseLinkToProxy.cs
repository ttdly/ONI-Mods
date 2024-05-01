using KSerialization;
using UnityEngine;
using static WirelessProject.ProwerManager.StaticVar;

namespace WirelessProject.ProwerManager {
    public class BaseLinkToProxy : KMonoBehaviour {
        [Serialize]
        public bool hasProxy = false;
        [Serialize]
        public int ProxyCell = -1;
        public PowerProxy.ProxyList proxyList;

        private static readonly EventSystem.IntraObjectHandler<BaseLinkToProxy> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BaseLinkToProxy>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            if (ProxyCell != -1) {
                PowerInfoList.TryGetValue(ProxyCell, out PowerProxy.ProxyList new_proxy);
                if (new_proxy != null) {
                    proxyList = new_proxy;
                } else {
                    PowerProxy.ProxyList new_init_proxy = new PowerProxy.ProxyList {
                        ThisCell = ProxyCell,
                    };
                    PowerInfoList.Add(ProxyCell, new_init_proxy);
                    proxyList = new_init_proxy;
                }
                AddThisToProxy();
            }
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            RemoveThisFromProxy(true);
        }

        private void OnRefreshUserMenu(object _) {
            if (PowerInfoList.Count == 0) return;
            GameObject go = new GameObject("screen");
            DetailsScreen.Instance.SetSecondarySideScreen(go.AddComponent<PowerProxyScreen>(), "nihao");
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
