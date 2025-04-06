using System.Collections.Generic;
using GeyserExpandMachine.Buildings;
using HarmonyLib;

namespace GeyserExpandMachine {
    public class ModData {
        public static ModData Instance;
        
        public readonly Dictionary<int, GeyserExpand> GeyserExpands = new Dictionary<int, GeyserExpand>();
        public readonly Dictionary<int, GeyserExpandProxy> GeyserExpandProxies = new Dictionary<int, GeyserExpandProxy>();
    }


    [HarmonyPatch(typeof(Game), nameof(Game.Load))]
    public static class GameLoadPatch {
        public static void Postfix() {
            if (ModData.Instance != null) {
                ModData.Instance.GeyserExpandProxies.Clear();
                ModData.Instance.GeyserExpands.Clear();
            }
            ModData.Instance = new ModData();
        }
    }
}