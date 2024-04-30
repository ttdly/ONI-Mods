
namespace WirelessProject.ProwerManager {
    public class ConsumerLinkToProxy : BaseLinkToProxy{
        [MyCmpGet]
        readonly EnergyConsumer consumer;

        protected override void AddThisToProxy() {
            if (proxyList == null) return;
            ProxyCell = proxyList.Connect(consumer);
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

        public override void ChangeProxy(PowerProxy.ProxyList new_proxy) {
            if (new_proxy == null) {
                RemoveThisFromProxy();
                return;
            }
            if (proxyList == null) {
                proxyList = new_proxy;
                AddThisToProxy();
            } else {
                proxyList.Remove(consumer);
                ProxyCell = new_proxy.Add(consumer);
                proxyList = new_proxy;
            }
        }
    }
}
