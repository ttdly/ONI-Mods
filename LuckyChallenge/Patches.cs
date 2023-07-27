

using HarmonyLib;
using Klei.CustomSettings;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using ProcGen;
using System.Collections.Generic;
using UnityEngine;

namespace LuckyChallenge {
    public class Patches {
        public static SettingConfig LuckyChallenge = (SettingConfig)new ToggleSettingConfig(nameof(Patches.LuckyChallenge), STRINGS.UI.CONFIG.NAME, STRINGS.UI.CONFIG.DESC, new SettingLevel("Disabled", STRINGS.UI.CONFIG.DISABLED, STRINGS.UI.CONFIG.DISABLED_TIP), new SettingLevel("Enabled", STRINGS.UI.CONFIG.ENABLE, STRINGS.UI.CONFIG.ENABLE_TIP), "Disabled", "Disabled");

        [HarmonyPatch(typeof(Db),nameof(Db.Initialize))]
        public class Db_Initialize_Patch {
            public static void Postfix() {
                LocString.CreateLocStringKeys(typeof(STRINGS), "");
            }
        }

        [HarmonyPatch(typeof(CustomGameSettings), "SetSurvivalDefaults")]
        public class CustomGameSettings_OnPrefabInit_Patch {
            public static void Postfix() {
                CustomGameSettings.Instance.AddQualitySettingConfig(Patches.LuckyChallenge);
            }
        }

        [HarmonyPatch(typeof(MutatedWorldData), MethodType.Constructor, typeof(ProcGen.World),
            typeof(List<WorldTrait>), typeof(List<WorldTrait>))]
        public class MutatedWorldData_Constructor_Patch {
            public static void Postfix(MutatedWorldData __instance) {
                var world = __instance.world;
                var subworlds = __instance.subworlds;
                if (CustomGameSettings.Instance.GetCurrentQualitySetting(LuckyChallenge).id == "Enabled") {
                    world.worldTemplateRules?.Clear();
                    if (subworlds != null)
                        foreach (var subworld in subworlds)
                            subworld.Value.subworldTemplateRules?.Clear();
                }
            }
        }

        [HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.CreatePrefab))]
        public class MinionConfig_CreatePrefab_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddComponent<MinionGift>();
            }
        }
    }
}
