using KSerialization;
using ProcGenGame;
using UnityEngine;

using static WirelessProject.ProwerManager.StaticVar;

namespace WirelessProject.ProwerManager {
    public class BaseLinkToProxy : KMonoBehaviour {
        [Serialize]
        public bool hasProxy = false;
        [Serialize]
        public int ProxyInfoId = -1;
        public PowerProxy.ProxyList proxyList;

        private static readonly EventSystem.IntraObjectHandler<BaseLinkToProxy> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BaseLinkToProxy>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            if (ProxyInfoId != -1) {
                PowerInfoList.TryGetValue(ProxyInfoId, out PowerProxy.ProxyList new_proxy);
                if (new_proxy != null) {
                    proxyList = new_proxy;
                } else {
                    PowerProxy.ProxyList new_init_proxy = new PowerProxy.ProxyList {
                        ProxyInfoId = ProxyInfoId,
                    };
                    PowerInfoList.Add(ProxyInfoId, new_init_proxy);
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
            LinkToProxyScreen.Instance.SetTarget(this);
            DetailsScreen.Instance.SetSecondarySideScreen(LinkToProxyScreen.Instance, "第二窗口");
            //Game.Instance.userMenu.AddButton(
            //    gameObject,
            //    new KIconButtonMenu.ButtonInfo(
            //        "action_follow_cam",
            //        "管理终端",
            //        OpenDialog,
            //        tooltipTex: "NOO")
            //    );
        }
        

        //private void OpenDialog() {
        //    new AddToProxyDialog(this);
        //}

        public virtual void RemoveThisFromProxy(bool _ = false) {
            gameObject.RemoveTag(HasProxyTag);
            hasProxy = false;
            proxyList = null;
            ProxyInfoId = -1;
        }

        protected virtual void AddThisToProxy() {
            gameObject.AddTag(HasProxyTag);
            hasProxy = true;
        }

        public virtual void ChangeProxy(int newProxyId) { }
        //public virtual void ChangeProxy(PowerProxy.ProxyList new_proxy) { }
    }
}
