
namespace WirelessProject.ProwerManager {
    public class ConsumerLinkToProxy : BaseLinkToProxy{
        [MyCmpGet]
        readonly EnergyConsumer consumer;

        protected override void AddThisToProxy() {
            if (proxy == null) return;
            ProxyCell = proxy.Connect(consumer);
            base.AddThisToProxy();
        }

        public override void RemoveThisFromProxy() {
            if (proxy == null) return;
            proxy.Disconnect(consumer, true);
            base.RemoveThisFromProxy();
        }

        //public override void ChangeProxy(PowerProxy new_proxy) {
        //    if (new_proxy == null) {
        //        RemoveThisFromProxy(); 
        //        return;
        //    }
        //    if (proxy == null) {
        //        proxy = new_proxy;
        //        AddThisToProxy();
        //    } else {
        //        proxy.Remove(consumer);
        //        ProxyCell = new_proxy.Add(consumer);
        //        proxy = new_proxy;
        //    }
        //}

        public override void ChangeProxy(PowerProxy.ProxyList new_proxy) {
            if (new_proxy == null) {
                RemoveThisFromProxy();
                return;
            }
            if (proxy == null) {
                proxy = new_proxy;
                AddThisToProxy();
            } else {
                proxy.Remove(consumer);
                ProxyCell = new_proxy.Add(consumer);
                proxy = new_proxy;
            }
        }
    }
}
