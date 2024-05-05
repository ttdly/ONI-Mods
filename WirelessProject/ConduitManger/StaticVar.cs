using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using System.Collections.Generic;
using System.Reflection;

namespace WirelessProject.ConduitManger {
    internal class StaticVar {
        public static Tag HasProxyTag = new Tag("Project_HasConduitProxy");
        public static Dictionary<int, ConduitProxyContentList> GlobalIdAndProxyList = new Dictionary<int, ConduitProxyContentList>();
        public static MethodInfo ConduitUpdateC = typeof(ConduitConsumer).GetMethod("ConduitUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo ConduitUpdateD = typeof(ConduitDispenser).GetMethod("ConduitUpdate", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool GetContents(int proxy_list_id, int proxy_list_index, out ConduitFlow.ConduitContents contents) {
            if (!GlobalIdAndProxyList.ContainsKey(proxy_list_id)) {
                PUtil.LogWarning($"Try to get a conduitInfoList that not exists. [ID:{proxy_list_id}]");
                contents = ConduitFlow.ConduitContents.Empty;
                return false;
            }
            if (GlobalIdAndProxyList.TryGetValue(proxy_list_id, out ConduitProxyContentList contentList)) {
                contents = contentList.contents[proxy_list_index];
                return true;
            } else {
                PUtil.LogError($"Fiald to get ContentList. [ID:{proxy_list_id}]");
                contents = ConduitFlow.ConduitContents.Empty;
                return false;
            }
        }
    }
}
