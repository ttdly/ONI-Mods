using System.Collections.Generic;
using GeyserExpandMachine.Buildings;
using HarmonyLib;

namespace GeyserExpandMachine {
    public class ModData {
        public static ModData Instance;
        
        public readonly Dictionary<int, BaseGeyserExpand> BaseGeyserExpands = new Dictionary<int, BaseGeyserExpand>();
        
        public readonly Dictionary<int, Geyser> Geysers = new Dictionary<int, Geyser>();
    }


    [HarmonyPatch(typeof(Game), nameof(Game.Load))]
    public static class GameLoadPatch {
        public static void Postfix() {
            if (ModData.Instance != null) {
                ModData.Instance.BaseGeyserExpands.Clear();
                ModData.Instance.Geysers.Clear();
            }
            ModData.Instance = new ModData();
        }
    }
}