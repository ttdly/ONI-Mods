using System.Collections.Generic;

namespace WirelessProject.ProwerManager {
    public class StaticVar {
        public static Tag HasProxyTag = new Tag("Project_HasPowerProxy");
        public static List<PowerProxy> PowerProxies = new List<PowerProxy>();
        public static Dictionary<int, PowerProxy.ProxyList> PowerInfoList = new Dictionary<int, PowerProxy.ProxyList>();
    }
}
