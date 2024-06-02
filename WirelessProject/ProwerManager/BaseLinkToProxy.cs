using KSerialization;
using static WirelessProject.ProwerManager.StaticVar;

namespace WirelessProject.ProwerManager {
  public class BaseLinkToProxy : KMonoBehaviour {
    private static readonly EventSystem.IntraObjectHandler<BaseLinkToProxy> OnSelectedObject =
      new EventSystem.IntraObjectHandler<BaseLinkToProxy>((component, data) => component.OnSelectObject());

    [Serialize] public bool hasProxy;

    [Serialize] public int ProxyListId = -1;

    public PowerProxy.ProxyList proxyList;

    protected override void OnSpawn() {
      base.OnSpawn();
      Subscribe((int)GameHashes.RefreshUserMenu, OnSelectedObject);
      if (ProxyListId != -1) {
        PowerInfoList.TryGetValue(ProxyListId, out var new_proxy);
        // just for update old info
        if (new_proxy == null) {
          ProxyListId = gameObject.GetMyWorldId();
          PowerInfoList.TryGetValue(ProxyListId, out new_proxy);
        }

        if (new_proxy != null) {
          proxyList = new_proxy;
        } else {
          var new_init_proxy = new PowerProxy.ProxyList {
            ProxyInfoId = ProxyListId
          };
          PowerInfoList.Add(ProxyListId, new_init_proxy);
          proxyList = new_init_proxy;
        }

        AddThisToProxy();
      }
    }

    protected override void OnCleanUp() {
      base.OnCleanUp();
      RemoveThisFromProxy(true);
    }

    private void OnSelectObject() {
      if (PowerInfoList.Count == 0 || !PowerInfoList.ContainsKey(gameObject.GetMyWorldId())) return;
      if (!hasProxy)
        Game.Instance.userMenu.AddButton(
          gameObject,
          new KIconButtonMenu.ButtonInfo(
            "action_follow_cam",
            Strings.PowerManager.AddtoProxy,
            AddThisToProxy
          )
        );
      else
        Game.Instance.userMenu.AddButton(
          gameObject,
          new KIconButtonMenu.ButtonInfo(
            "action_follow_cam",
            Strings.PowerManager.RemoveFromProxy,
            delegate { RemoveThisFromProxy(); }
          )
        );
    }


    public virtual void RemoveThisFromProxy(bool _ = false) {
      gameObject.RemoveTag(HasProxyTag);
      hasProxy = false;
      proxyList = null;
      ProxyListId = -1;
    }

    protected virtual void AddThisToProxy() {
      gameObject.AddTag(HasProxyTag);
      hasProxy = true;
      ProxyListId = gameObject.GetMyWorldId();
      PowerInfoList.TryGetValue(ProxyListId, out var new_proxy);
      if (new_proxy != null) proxyList = new_proxy;
    }

    public virtual void ChangeProxy(int newProxyId) {
    }
  }
}