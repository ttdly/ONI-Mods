using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WirelessProject.ConduitManger.StaticVar;

namespace WirelessProject.ConduitManger {
    public class BaseLinkToProxy : KMonoBehaviour {
        [Serialize]
        public bool hasProxy = false;
        [Serialize]
        public int ProxyListId = -1;
        protected ConduitProxyContentList proxyList = null;


        protected override void OnSpawn() {
            base.OnSpawn();
            if (ProxyListId != -1) {
                GlobalIdAndProxyList.TryGetValue(ProxyListId, out ConduitProxyContentList new_proxy);
                if (new_proxy != null) {
                    proxyList = new_proxy;
                } else {
                    ConduitProxyContentList new_init_proxy = new ConduitProxyContentList {
                        ProxyListId = ProxyListId,
                    };
                    GlobalIdAndProxyList.Add(ProxyListId, new_init_proxy);
                    proxyList = new_init_proxy;
                }
                AddThisToProxy();
            }
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
        }

        protected virtual void RestoreLink() {

        }

        public virtual void ChangeProxy(int newProxyId) { }
    }
}
