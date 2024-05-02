using PeterHan.PLib.Core;

namespace WirelessProject.ProwerManager {
    internal class BatteryLinkToProxy:BaseLinkToProxy {
        [MyCmpGet]
        readonly Battery battery;

        protected override void AddThisToProxy() {
            if (proxyList == null) return;
            ProxyInfoId = proxyList.Connect(battery);
            base.AddThisToProxy();
        }

        public override void RemoveThisFromProxy(bool isCleanUp =false) {
            if (proxyList == null) return;
            if (isCleanUp) {
                proxyList.Remove(battery);
            } else {
                proxyList.Disconnect(battery);
            }
            base.RemoveThisFromProxy(isCleanUp);
        }

        //public override void ChangeProxy(PowerProxy.ProxyList new_proxy) {
        //    if (new_proxy == null) {
        //        RemoveThisFromProxy();
        //        return;
        //    }
        //    if (proxyList == null) {
        //        proxyList = new_proxy;
        //        AddThisToProxy();
        //    } else {
        //        proxyList.Remove(battery);
        //        ProxyInfoId = new_proxy.Add(battery);
        //        proxyList = new_proxy;
        //    }
        //}
        public override void ChangeProxy(int newProxyId) {
            if ( newProxyId == -1){
                RemoveThisFromProxy();
                return;
            }
            if (StaticVar.PowerInfoList.TryGetValue(newProxyId, out PowerProxy.ProxyList proxyList)) {
                if (this.proxyList == null) {
                    this.proxyList = proxyList;
                    AddThisToProxy();
                } else {
                    proxyList.Remove(battery);
                    ProxyInfoId = proxyList.Add(battery);
                    this.proxyList = proxyList;
                }
            } else {
                PUtil.LogWarning("Try to add this equipment to a not exist proxyList");
            }
        }
    }
}
