using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using TweaksPack.Tweakable;
using UnityEngine;

namespace TweaksPack {
    public class TweaksPackPatches {

        public static void FabricatorBuildingPostfix(GameObject go) {
            go.AddOrGet<ComplexFabricatorTweakable>();
        }

        public static void BuildingPrefix(GameObject go) {
            go.AddOrGet<AutoTweakable>();
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
                harmony.Patch(type.GetMethod("ConfigureBuildingTemplate"), postfix: new HarmonyMethod(typeof(TweaksPackPatches).GetMethod(nameof(FabricatorBuildingPostfix))));
            }

            types = new List<Type>() {
                typeof(OilWellCapConfig),
                typeof(OilRefineryConfig),
                typeof(EggIncubatorConfig),
                typeof(DesalinatorConfig),
                typeof(CompostConfig)
            };

            foreach (Type type in types) {
                harmony.Patch(type.GetMethod("ConfigureBuildingTemplate"), prefix: new HarmonyMethod(typeof(TweaksPackPatches).GetMethod(nameof(BuildingPrefix))));
            }

            types.Clear();
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

        //[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToBasicPlant))]
        //public class EntityTemplates_Patch {
        //    public static void Postfix(GameObject __result, string crop_id) {
        //        if (crop_id != null) {
        //            __result.AddOrGet<PlantTweakable>();
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
        partial class GeyserGenericConfig_Patch {
            public static void Postfix(GameObject __result) {
                __result.AddOrGet<GeyserTweakable>();
            }
        }

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch {
            public static void Postfix() {
                LocString.CreateLocStringKeys(typeof(TweaksPackStrings), "");
            }
        }

        [HarmonyPatch(typeof(EggIncubator), "UpdateChore")]
        public class EggIncubator_UpdateChore_Patch {
            public static void Prefix(EggIncubator __instance) {
                if (__instance.gameObject.HasTag(TweakableStaticVars.Tags.AutoTweaked)) {
                    Chore chore = (Chore)Traverse.Create(__instance).Field("chore").GetValue();
                    chore?.Cancel("NoOp");
                    if (__instance.Occupant) {
                        __instance.Occupant.GetSMI<IncubationMonitor.Instance>()?.ApplySongBuff();
                    }
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(OilRefinery.States), nameof(OilRefinery.States.InitializeStates))]
        public class OilRefinery_States_InitializeStateMachine_Patch {
            public static void Postfix(OilRefinery.States __instance) {
                __instance.disabled.ClearTransitions();
                __instance.disabled.EventTransition(GameHashes.OperationalChanged, __instance.needResources, (OilRefinery.StatesInstance smi) => smi.master.gameObject.AddOrGet<Operational>().IsOperational).Enter(delegate (OilRefinery.StatesInstance smi) {
                    if (smi.master.gameObject.HasTag(TweakableStaticVars.Tags.AutoTweaked)) {
                        smi.master.GetComponent<KBatchedAnimController>().Play("on");
                    }
                });
                __instance.needResources.ClearTransitions();
                __instance.needResources.EventTransition(GameHashes.OnStorageChange, __instance.ready, (OilRefinery.StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting()).Enter(delegate (OilRefinery.StatesInstance smi) {
                    if (smi.master.gameObject.HasTag(TweakableStaticVars.Tags.AutoTweaked)) {
                        smi.master.gameObject.AddOrGet<Operational>().SetActive(false);
                        smi.master.GetComponent<KBatchedAnimController>().Play("on");
                    }
                }).Exit(delegate (OilRefinery.StatesInstance smi) {
                    if (smi.master.gameObject.HasTag(TweakableStaticVars.Tags.AutoTweaked)) {
                        smi.master.gameObject.AddOrGet<Operational>().SetActive(true);
                        smi.master.GetComponent<KBatchedAnimController>().Play("working_loop", KAnim.PlayMode.Loop);
                    }
                });
            }
        }

        [HarmonyPatch(typeof(Compost.States), nameof(Compost.States.InitializeStates))]
        public class Compost_States_InitializeStates_Patch {
            public static void Postfix(Compost.States __instance) {
                __instance.inert.ClearTransitions();
                __instance.inert.ClearEnterActions();
                __instance.inert.ClearExitActions();
                __instance.inert.EventTransition(GameHashes.OperationalChanged, __instance.disabled, smi => !smi.GetComponent<Operational>().IsOperational).PlayAnim("on").ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingCompostFlip).Enter(delegate (Compost.StatesInstance smi) {
                    if (smi.master.gameObject.HasTag(TweakableStaticVars.Tags.AutoTweaked)) {
                        smi.GoTo(__instance.composting);
                    }
                }).ToggleChore(delegate (Compost.StatesInstance smi) {
                    return new WorkChore<CompostWorkable>(Db.Get().ChoreTypes.FlipCompost, smi.master);
                }, __instance.composting);
            }
        }

        [HarmonyPatch(typeof(OilWellCap), nameof(OilWellCap.AddGasPressure))]
        public class OilWellCap_AddGasPressure_Patch {
            public static void Postfix(OilWellCap __instance) {
                if (__instance.gameObject.HasTag(TweakableStaticVars.Tags.AutoTweaked) && __instance.NeedsDepressurizing()) {
                    __instance.GetSMI<OilWellCap.StatesInstance>()?.sm.working.Set(true, __instance.GetSMI<OilWellCap.StatesInstance>());
                }
            }
        }

        [HarmonyPatch(typeof(OilWellCap), nameof(OilWellCap.ReleaseGasPressure))]
        public class OilWellCap_ReleaseGasPressure_Patch {
            public static void Postfix(OilWellCap __instance) {
                if (__instance.gameObject.HasTag(TweakableStaticVars.Tags.AutoTweaked) && !__instance.NeedsDepressurizing()) {
                    __instance.GetSMI<OilWellCap.StatesInstance>()?.sm.working.Set(false, __instance.GetSMI<OilWellCap.StatesInstance>());
                }
            }
        }

        [HarmonyPatch(typeof(Desalinator.StatesInstance), nameof(Desalinator.StatesInstance.CreateEmptyChore))]
        public class Desalinator_StatesInstance_CreateEmptyChore {
            public static void Prefix(Desalinator.StatesInstance __instance) {
                if (__instance.master.gameObject.HasTag(TweakableStaticVars.Tags.AutoTweaked)) {
                    Tag tag = GameTagExtensions.Create(SimHashes.Salt);
                    ListPool<GameObject, Desalinator>.PooledList pooledList = ListPool<GameObject, Desalinator>.Allocate();
                    __instance.master.gameObject.GetComponent<Storage>().Find(tag, pooledList);
                    foreach (GameObject item in pooledList) {
                        __instance.master.GetComponent<Storage>().Drop(item);
                    }
                    pooledList.Recycle();
                    return;
                }
            }
        }


        //[HarmonyPatch(typeof(Growing.States), nameof(Growing.States.InitializeStates))]
        //public class Growing_StatesInstance_InitializeStates_Patch {
        //    public static void Postfix(Growing.States __instance) {
        //        __instance.grown.idle.updateActions.Clear();
        //        __instance.grown.idle.Update("CheckNotGrown", delegate (Growing.StatesInstance smi, float dt) {
        //            if (smi.master.shouldGrowOld) {
        //                AmountInstance oldAge = (AmountInstance)Traverse.Create(smi.master).Field("oldAge").GetValue();
        //                if (oldAge.value >= oldAge.GetMax() || smi.master.gameObject.HasTag(TweakableStaticVars.Tags.AutoHarvest)) {
        //                    smi.GoTo(__instance.grown.try_self_harvest);
        //                }
        //            }
        //        }, UpdateRate.SIM_4000ms);
        //    }
        //}
    }
}
