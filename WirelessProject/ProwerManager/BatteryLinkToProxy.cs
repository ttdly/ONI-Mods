namespace WirelessProject.ProwerManager {
    internal class BatteryLinkToProxy:BaseLinkToProxy {
        [MyCmpGet]
        readonly Battery battery;

        protected override void AddThisToProxy() {
            if (proxyList == null) return;
            ProxyCell = proxyList.Connect(battery);
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

        public override void ChangeProxy(PowerProxy.ProxyList new_proxy) {
            if (new_proxy == null) {
                RemoveThisFromProxy();
                return;
            }
            if (proxyList == null) {
                proxyList = new_proxy;
                AddThisToProxy();
            } else {
                proxyList.Remove(battery);
                ProxyCell = new_proxy.Add(battery);
                proxyList = new_proxy;
            }
        }
    }
}
