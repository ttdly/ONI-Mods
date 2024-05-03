using KSerialization;
using static WirelessProject.ProwerManager.StaticVar;

namespace WirelessProject.ProwerManager {
    public class BaseLinkToProxy : KMonoBehaviour {
        [Serialize]
        public bool hasProxy = false;
        [Serialize]
        public int ProxyListId = -1;
        public PowerProxy.ProxyList proxyList;

        private static readonly EventSystem.IntraObjectHandler<BaseLinkToProxy> OnSelectedObject = new EventSystem.IntraObjectHandler<BaseLinkToProxy>((component, data) => component.OnSelectObject(data));

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.SelectObject, OnSelectedObject);
            if (ProxyListId != -1) {
                PowerInfoList.TryGetValue(ProxyListId, out PowerProxy.ProxyList new_proxy);
                if (new_proxy != null) {
                    proxyList = new_proxy;
                } else {
                    PowerProxy.ProxyList new_init_proxy = new PowerProxy.ProxyList {
                        ProxyInfoId = ProxyListId,
                    };
                    PowerInfoList.Add(ProxyListId, new_init_proxy);
                    proxyList = new_init_proxy;
                }
                AddThisToProxy();
            }
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            RemoveThisFromProxy(true);
        }

        private void OnSelectObject(object selected) {
            if (PowerInfoList.Count == 0) return;
            if ((bool) selected) {
                LinkToProxyScreen.Instance.SetTarget(this);
                DetailsScreen.Instance.SetSecondarySideScreen(LinkToProxyScreen.Instance, Strings.PowerManager.WindowName);
            } else {
                DetailsScreen.Instance.ClearSecondarySideScreen();
            }
            //Game.Instance.userMenu.AddButton(
            //    gameObject,
            //    new KIconButtonMenu.ButtonInfo(
            //        "action_follow_cam",
            //        "管理终端",
            //        OpenDialog,
            //        tooltipTex: "NOO")
            //    );
        }

        public virtual void RemoveThisFromProxy(bool _ = false) {
            gameObject.RemoveTag(HasProxyTag);
            hasProxy = false;
            proxyList = null;
            ProxyListId = -1;
        }

        protected virtual void AddThisToProxy() {
            gameObject.AddTag(HasProxyTag);
            hasProxy = true;
        }

        public virtual void ChangeProxy(int newProxyId) { }
    }
}
