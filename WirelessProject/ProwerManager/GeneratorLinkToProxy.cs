

namespace WirelessProject.ProwerManager {
    public class GeneratorLinkToProxy:BaseLinkToProxy {
        [MyCmpGet]
        readonly Generator generator;

        protected override void AddThisToProxy() {
            if (proxy == null) return;
            ProxyCell = proxy.Connect(generator);
            base.AddThisToProxy();
        }

        public override void RemoveThisFromProxy() {
            if (proxy == null) return;
            proxy.Disconnect(generator);
            base.RemoveThisFromProxy();
        }

        public override void ChangeProxy(PowerProxy new_proxy) {
            if (new_proxy == null) {
                RemoveThisFromProxy();
                return;
            }
            if (proxy == null) {
                proxy = new_proxy;
                AddThisToProxy();
            } else {
                proxy.Remove(generator);
                ProxyCell = new_proxy.Add(generator);
                proxy = new_proxy;
            }
        }
    }
}
