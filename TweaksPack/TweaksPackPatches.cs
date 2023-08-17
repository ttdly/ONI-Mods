using HarmonyLib;
using System;
using System.Collections.Generic;
using TweaksPack.Tweakable;
using UnityEngine;

namespace TweaksPack
{
    public class TweaksPackPatches {
        
        public static void BuildingPostfix(GameObject go) {
            go.AddOrGet<ComplexFabricatorTweakable>();
        }

        public static void ManualPatchBuildings(Harmony harmony) {
            List<Type> types = new List<Type>() {
                typeof(AdvancedApothecaryConfig),
                typeof(ApothecaryConfig),
                typeof(ChlorinatorConfig),
                typeof(ClothingAlterationStationConfig),
                typeof(ClothingFabricatorConfig),
                typeof(CookingStationConfig),
                typeof(CraftingTableConfig),
                typeof(DiamondPressConfig),
                typeof(EggCrackerConfig),
                typeof(FossilDigSiteConfig),
                typeof(GenericFabricatorConfig),
                typeof(GlassForgeConfig),
                typeof(GourmetCookingStationConfig),
                typeof(KilnConfig),
                typeof(ManualHighEnergyParticleSpawnerConfig),
                typeof(MetalRefineryConfig),
                typeof(MicrobeMusherConfig),
                typeof(MilkPressConfig),
                typeof(MissileFabricatorConfig),
                typeof(OrbitalResearchCenterConfig),
                typeof(RockCrusherConfig),
                typeof(SludgePressConfig),
                typeof(SuitFabricatorConfig),
                typeof(SupermaterialRefineryConfig),
                typeof(UraniumCentrifugeConfig)
            };

            foreach (Type type in types) {
                harmony.Patch(type.GetMethod("ConfigureBuildingTemplate"), postfix: new HarmonyMethod(typeof(TweaksPackPatches).GetMethod(nameof(BuildingPostfix))));
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
                            string name = Strings.Get("STRINGS.CREATURES.SPECIES.GEYSER." + upper + ".NAME");
                            string desc = Strings.Get("STRINGS.CREATURES.SPECIES.GEYSER." + upper + ".DESC");
                            GeyserChangeDialog.Options.Add(new EnumOption(name, desc, tag));
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToBasicPlant))]
        public class EntityTemplates_Patch {
            public static void Postfix(GameObject __result, string crop_id) {
                if (crop_id != null) {
                    __result.AddOrGet<PlantTweakable>();
                }
            }
        }

        [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
        partial class GeyserGenericConfig_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<GeyserTweakable>();
            }
        }

        [HarmonyPatch(typeof(Db),nameof(Db.Initialize))]
        public class Db_Initialize_Patch {
            public static void Postfix() {
                LocString.CreateLocStringKeys(typeof(TweaksPackStrings),"");
            }
        }

        [HarmonyPatch(typeof(OilWellCapConfig), nameof(OilWellCapConfig.DoPostConfigureComplete))]
        public class OilWellCapConfig_Patch { 
            public static void Postfix(GameObject go) {
                UnityEngine.Object.Destroy(go.AddOrGet<OilWellCap>());
                go.AddOrGet<OilWellCapTweakable>();
                OilWellCapA oilWellCap = go.AddOrGet<OilWellCapA>();
                oilWellCap.gasElement = SimHashes.Methane;
                oilWellCap.gasTemperature = 573.15f;
                oilWellCap.addGasRate = 0.0333333351f;
                oilWellCap.maxGasPressure = 80.00001f;
                oilWellCap.releaseGasRate = 0.444444478f;
            }
        }

        [HarmonyPatch(typeof(OilRefineryConfig), nameof(OilRefineryConfig.DoPostConfigureComplete))]
        public class OilRefineryConfig_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<OilRefineryTweakable>();
            }
        }

        [HarmonyPatch(typeof(CompostConfig), nameof(CompostConfig.DoPostConfigureComplete))]
        public class CompostConfig_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<CompostTweakbale>();
            }
        }
    }
}
