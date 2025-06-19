using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;

namespace CustomChoreType {
    public class Patches {
        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public static class DbInitializePatch {
            public static void Prefix() {
                if (!File.Exists(Mod.ConfigPath)) return;
                try
                {
                    var json = File.ReadAllText(Mod.ConfigPath);
                    Mod.Changes = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    throw;
                }
            }
        }
        
        [HarmonyPatch(typeof(ChoreType), MethodType.Constructor, typeof(string), typeof(ResourceSet), typeof(string[]),
            typeof(string), typeof(string), typeof(string), typeof(string), typeof(IEnumerable<Tag>), typeof(int), typeof(int))]
        public class ChoreTypePatch {
            public static void Prefix(string id, ref string[] chore_groups) {
                if (!Mod.Changes.ContainsKey(id)) return;
                Mod.Backup[id] = chore_groups;
                chore_groups = Mod.Changes[id];
            }
        }

        [HarmonyPatch(typeof(BuildingChoresPanel), "GetChoreEntry")]
        public class BuildingChoresPanelGetChoreEntryPatch {
            static void Postfix(ChoreType choreType, HierarchyReferences __result) {
                var choreLabel = __result.GetReference<LocText>("ChoreLabel");
                var choreLabelEvents = choreLabel.gameObject.AddOrGet<ChoreLabelEvents>();
                choreLabelEvents.Initialize(choreType, choreLabel);
            }
        }
    }
}