

using PeterHan.PLib.Core;

namespace WirelessProject.ProwerManager {
    public class GeneratorLinkToProxy:BaseLinkToProxy {
        [MyCmpGet]
        readonly Generator generator;

        protected override void AddThisToProxy() {
            base.AddThisToProxy();
            if (proxyList == null) return;
            ProxyListId = proxyList.Connect(generator);
        }

        public override void RemoveThisFromProxy(bool isCleanUp = false) {
            if (proxyList == null) return;
            if (isCleanUp) {
                proxyList.Remove(generator);
                PUtil.LogDebug("HFHFHFHFHHF");
            } else {
                proxyList.Disconnect(generator);
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
                    proxyList.Remove(generator);
                    ProxyListId = proxyList.Add(generator);
                    this.proxyList = proxyList;
                }
            } else {
                PUtil.LogWarning("Try to add this equipment to a not exist proxyList");
            }
        }
    }
}
