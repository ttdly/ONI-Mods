using PeterHan.PLib.Core;

namespace WirelessProject.ProwerManager {
  internal class BatteryLinkToProxy : BaseLinkToProxy {
    [MyCmpGet] private readonly Battery battery;

    protected override void AddThisToProxy() {
      base.AddThisToProxy();
      if (proxyList == null) return;
      ProxyListId = proxyList.Connect(battery);
    }

    public override void RemoveThisFromProxy(bool isCleanUp = false) {
      if (proxyList == null) return;
      if (isCleanUp)
        proxyList.Remove(battery);
      else
        proxyList.Disconnect(battery);
      base.RemoveThisFromProxy(isCleanUp);
    }

    public override void ChangeProxy(int newProxyId) {
      if (newProxyId == -1) {
        RemoveThisFromProxy();
        return;
      }

      if (StaticVar.PowerInfoList.TryGetValue(newProxyId, out var proxyList)) {
        if (this.proxyList == null) {
          this.proxyList = proxyList;
          AddThisToProxy();
        } else {
          proxyList.Remove(battery);
          ProxyListId = proxyList.Add(battery);
          this.proxyList = proxyList;
        }
      } else {
        PUtil.LogWarning("Try to add this equipment to a not exist proxyList");
      }
    }
  }
}