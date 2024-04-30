

namespace WirelessProject.ProwerManager {
    public class GeneratorLinkToProxy:BaseLinkToProxy {
        [MyCmpGet]
        readonly Generator generator;

        protected override void AddThisToProxy() {
            if (proxyList == null) return;
            ProxyCell = proxyList.Connect(generator);
            base.AddThisToProxy();
        }

        public override void RemoveThisFromProxy(bool isCleanUp = false) {
            if (proxyList == null) return;
            if (isCleanUp) {
                proxyList.Remove(generator);
            } else {
                proxyList.Disconnect(generator);
            }
            base.RemoveThisFromProxy(isCleanUp);
        }


        public override void ChangeProxy(PowerProxy.ProxyList new_proxy) {
            if (new_proxy == null) {
                RemoveThisFromProxy();
                return;
            }
            if (proxyList == null) {
                proxyList = new_proxy;
                AddThisToProxy();
            } else {
                proxyList.Remove(generator);
                ProxyCell = new_proxy.Add(generator);
                proxyList = new_proxy;
            }
        }
    }
}
