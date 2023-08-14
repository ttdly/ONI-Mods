using HarmonyLib;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using UnityEngine;

namespace TweaksPack {
    public class AblePatches {
        [HarmonyPatch(typeof(Geyser), "OnSpawn")]
        public static class Geyser_OnSpawn_Patch {
            public static void Postfix(Geyser __instance) {
                if (__instance.GetType() != typeof(Geyser)) {
                    GeyserTweakable updatable = __instance.gameObject.GetComponent<GeyserTweakable>();
                    if (updatable != null) {
                        Object.Destroy(__instance.GetComponent<GeyserTweakable>());
                    }
                    return;
                } else {
                    GeyserTweakable updatable = __instance.gameObject.FindOrAddComponent<GeyserTweakable>();
                    updatable.materialNeeds.Add(SimHashes.Katairite.CreateTag(), 100);
                    updatable.materialNeeds.Add(SimHashes.Ceramic.CreateTag(), 800);
                }
            }
        }

        [HarmonyPatch(typeof(CodexCache), nameof(CodexCache.CodexCacheInit))]
        public class CodexCache_CodexInit_Patch {
            public static void Postfix() {
                List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Geyser>();
                if (prefabsWithComponent != null) {
                    foreach (GameObject go in prefabsWithComponent) {
                        if (!go.GetComponent<KPrefabID>().HasTag(GameTags.DeprecatedContent)) {
                            Tag tag = go.PrefabID();
                            string upper = tag.ToString().ToUpper();
                            string str1 = "GEYSERGENERIC_";
                            upper = upper.Replace(str1, "");
                            PUtil.LogDebug(upper);
                            string name = Strings.Get("STRINGS.CREATURES.SPECIES.GEYSER." + upper + ".NAME");
                            string desc = Strings.Get("STRINGS.CREATURES.SPECIES.GEYSER." + upper + ".DESC");
                            GeyserChangeDialog.Options.Add(new EnumOption(name, desc, tag));
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Db),nameof(Db.Initialize))]
        public class Db_Initialize_Patch {
            public static void Postfix() {
                LocString.CreateLocStringKeys(typeof(AbleStrings),"");
            }
        }
    }
}
