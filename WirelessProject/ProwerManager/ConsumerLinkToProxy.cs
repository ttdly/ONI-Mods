
using PeterHan.PLib.Core;

namespace WirelessProject.ProwerManager {
    public class ConsumerLinkToProxy : BaseLinkToProxy {
        [MyCmpGet]
        readonly EnergyConsumer consumer;

        protected override void AddThisToProxy() {
            if (proxyList == null) return;
            ProxyInfoId = proxyList.Connect(consumer);
            base.AddThisToProxy();
        }

        public override void RemoveThisFromProxy(bool isCleanUp = false) {
            if (proxyList == null) return;
            if (isCleanUp) {
                proxyList.Remove(consumer);
            } else {
                proxyList.Disconnect(consumer, true);
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
        //        proxyList.Remove(consumer);
        //        ProxyInfoId = new_proxy.Add(consumer);
        //        proxyList = new_proxy;
        //    }
        //}
        public override void ChangeProxy(int newProxyId) {
            if (newProxyId == -1) {
                RemoveThisFromProxy();
                return;
            }
            if (StaticVar.PowerInfoList.TryGetValue(newProxyId, out PowerProxy.ProxyList proxyList)) {
                if (this.proxyList == null) {
                    this.proxyList = proxyList;
                    AddThisToProxy();
                } else {
                    proxyList.Remove(consumer);
                    ProxyInfoId = proxyList.Add(consumer);
                    this.proxyList = proxyList;
                }
            } else {
                PUtil.LogWarning("Try to add this equipment to a not exist proxyList");
            }
        }
    }
}
