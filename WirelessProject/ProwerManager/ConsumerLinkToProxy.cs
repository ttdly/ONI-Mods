
using PeterHan.PLib.Core;

namespace WirelessProject.ProwerManager {
    public class ConsumerLinkToProxy : BaseLinkToProxy {
        [MyCmpGet]
        readonly EnergyConsumer consumer;

        protected override void AddThisToProxy() {
            base.AddThisToProxy();
            if (proxyList == null) return;
            ProxyListId = proxyList.Connect(consumer);
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
                    ProxyListId = proxyList.Add(consumer);
                    this.proxyList = proxyList;
                }
            } else {
                PUtil.LogWarning("Try to add this equipment to a not exist proxyList");
            }
        }
    }
}
