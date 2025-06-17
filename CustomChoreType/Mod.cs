using System.Collections.Generic;
using System.IO;
using CustomChoreType.Screen;
using HarmonyLib;
using KMod;

namespace CustomChoreType {
    public class Mod: UserMod2 {
        public static Dictionary<string, string[]> Changes = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> Backup = new Dictionary<string, string[]>();
        public static HashSet<ChoreConsumer> ChoreConsumers = new HashSet<ChoreConsumer>();
        public static string ConfigPath;
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            ModAssets.LoadAssets();
            ConfigPath = Path.Combine(Manager.GetDirectory(), "custom_chore_types_config.json");
        }
    }
}