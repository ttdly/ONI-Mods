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
            public static void Postfix() {
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

                foreach (var change in Mod.Changes) {
                    var choreType = Db.Get().ChoreTypes.TryGet(change.Key);
                    if (choreType == null) continue;
                    var newChoreGroup = change.Value
                        .Select(group => Db.Get().ChoreGroups.TryGet(group)).ToArray();
                    Mod.Backup[choreType.Id] = choreType.groups.Select(g => g.Id).ToArray();  
                    CustomChoreTypeScreen.ApplyChoreGroup(choreType.Id, newChoreGroup);
                }
            }
        }
        
        [HarmonyPatch(typeof(BuildingChoresPanel), "GetChoreEntry")]
        public class BuildingChoresPanelGetChoreEntryPatch {
            static void Postfix(ChoreType choreType, HierarchyReferences __result) {
                var choreLabel = __result.GetReference<LocText>("ChoreLabel");
                if (choreLabel.gameObject.GetComponent<ChoreLabelEvents>()) return;
                var choreLabelEvents = choreLabel.gameObject.AddComponent<ChoreLabelEvents>();
                choreLabelEvents.Initialize(choreType, choreLabel);
            }
        }

        // [HarmonyPatch(typeof(ChoreType), "groups", MethodType.Setter)]
        // public class ChoreTypeGroupPatch {
        //     static void Postfix(ChoreType __instance) {
        //         // Debug.Log($"设置{__instance.Name}:{__instance.groups}");
        //     }
        // }
    }
}