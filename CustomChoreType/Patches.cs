using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomChoreType.Screen;
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
                // foreach (var change in Mod.Changes) {
                //     var choreType = Db.Get().ChoreTypes.TryGet(change.Key);
                //     if (choreType == null) continue;
                //     var newChoreGroup = change.Value
                //         .Select(group => Db.Get().ChoreGroups.TryGet(group)).ToArray();
                //     Mod.Backup[choreType.Id] = choreType.groups.Select(g => g.Id).ToArray();  
                //     CustomChoreTypeScreen.ApplyChoreGroup(choreType.Id, newChoreGroup);
                // }
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

        [HarmonyPatch(typeof(ChoreConsumer), "OnSpawn")]
        public class ChoreConsumerOnSpawnPatch {
            public static void Postfix(ChoreConsumer __instance) {
                Debug.Log(__instance.gameObject.GetComponent<KPrefabID>().Tags);
                // Mod.ChoreConsumers.Add(__instance);
            }
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Load))]
        public class GameLoadPatch {
            public static void Prefix() {
                // Mod.ChoreConsumers.Clear();
            }
        }
    }
}