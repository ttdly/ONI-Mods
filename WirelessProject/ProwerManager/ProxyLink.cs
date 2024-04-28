using KSerialization;

namespace WirelessProject.ProwerManager {
    public class ProxyLink : KMonoBehaviour {
        public enum ProwerType {
            Generator,
            Battery,
            Consumer
        }
        [Serialize]
        public bool hasProxy = false;
        public ProwerType type;
        [Serialize]
        public PowerProxy proxy = null;

        private static readonly EventSystem.IntraObjectHandler<ProxyLink> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ProxyLink>((component, data) => component.OnRefreshUserMenu(data));
        

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            if (hasProxy) {
                AddThisToProxy();
            } else {
                RemoveThisFromProxy();
            }
        }

        private void OnRefreshUserMenu(object _) {
            if(GlobalVar.PowerProxies.Count == 0) return; 
            if (hasProxy) {
                Game.Instance.userMenu.AddButton(
                gameObject,
                new KIconButtonMenu.ButtonInfo(
                    "action_follow_cam",
                    "断开连接",
                    RemoveThisFromProxy,
                    tooltipText: "NOO")
                );
            } else {
                Game.Instance.userMenu.AddButton(
                    gameObject,
                    new KIconButtonMenu.ButtonInfo(
                        "action_follow_cam",
                        "接入终端",
                        OpenDialog,
                        tooltipText: "NOO")
                    );
            }
        }

        private void OpenDialog() {
            new AddToProxyDialog(this);
        }

        public void RemoveThisFromProxy() {
            if (proxy == null) return;
            switch (type) {
                case ProwerType.Generator:
                    Generator generator = gameObject.GetComponent<Generator>();
                    proxy.Disconnect(generator);
                    break;
                case ProwerType.Consumer:
                    EnergyConsumer energyConsumer = gameObject.GetComponent<EnergyConsumer>();
                    proxy.Disconnect(energyConsumer, true);
                    break;
                case ProwerType.Battery:
                    Battery battery = gameObject.GetComponent<Battery>();
                    proxy.Disconnect(battery);
                    break;
            }
            gameObject.RemoveTag(GlobalVar.HasProxy);
            hasProxy = false;
            proxy = null;
        }

        private void AddThisToProxy() {
            if (proxy == null) return;
            switch (type) {
                case ProwerType.Generator:
                    Generator generator = gameObject.GetComponent<Generator>();
                    proxy.Connect(generator);
                    break;
                case ProwerType.Consumer:
                    EnergyConsumer energyConsumer = gameObject.GetComponent<EnergyConsumer>();
                    proxy.Connect(energyConsumer);
                    break;
                case ProwerType.Battery:
                    Battery battery = gameObject.GetComponent<Battery>();
                    proxy.Connect(battery);
                    break;
            }
            gameObject.AddTag(GlobalVar.HasProxy);
            hasProxy = true;
        }   

        public void ChangeProxy(PowerProxy new_proxy) {
            if (proxy == null) {
                proxy = new_proxy;
                AddThisToProxy();
            }else{
                switch (type) {
                    case ProwerType.Generator:
                        Generator generator = gameObject.GetComponent<Generator>();
                        proxy.Disconnect(generator);
                        new_proxy.Connect(generator);
                        break;
                    case ProwerType.Consumer:
                        EnergyConsumer energyConsumer = gameObject.GetComponent<EnergyConsumer>();
                        proxy.Disconnect(energyConsumer, true);
                        new_proxy.Connect(energyConsumer);
                        break;
                    case ProwerType.Battery:
                        Battery battery = gameObject.GetComponent<Battery>();
                        proxy.Disconnect(battery);
                        new_proxy.Connect(battery);
                        break;
                }
                proxy = new_proxy;
            };
        }
    }
}
