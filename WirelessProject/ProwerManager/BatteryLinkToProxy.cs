namespace WirelessProject.ProwerManager {
    internal class BatteryLinkToProxy:BaseLinkToProxy {
        [MyCmpGet]
        readonly Battery battery;

        protected override void AddThisToProxy() {
            if (proxy == null) return;
            ProxyCell = proxy.Connect(battery);
            base.AddThisToProxy();
        }

        public override void RemoveThisFromProxy() {
            if (proxy == null) return;
            proxy.Disconnect(battery);
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
        //        proxy.Remove(battery);
        //        ProxyCell = new_proxy.Add(battery);
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
                proxy.Remove(battery);
                ProxyCell = new_proxy.Add(battery);
                proxy = new_proxy;
            }
        }
    }
}
