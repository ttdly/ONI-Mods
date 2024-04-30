using System.Collections.Generic;

namespace WirelessProject.ProwerManager {
    public class StaticVar {
        public static Tag HasProxyTag = new Tag("Project_HasProxy");
        public static List<PowerProxy> PowerProxies = new List<PowerProxy>();
        //public static Dictionary<int, PowerProxy> PowerProxiesWithCell = new Dictionary<int, PowerProxy>();
        public static Dictionary<int, PowerProxy.ProxyList> PowerProxiesWithCell = new Dictionary<int, PowerProxy.ProxyList>();
        public static List<ushort> IDs = new List<ushort>();
        public static StatusItem ProxyCircuitStatus;
        public static StatusItem ProxyMaxWattageStatus;
    }
}
