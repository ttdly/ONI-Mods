using HarmonyLib;
using PeterHan.PLib.Options;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace PlantPulverizerEnhancement {
    public class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(Options));
            LocString.CreateLocStringKeys(typeof(Strings), "");
        }
    }

    public class Patches {
        [HarmonyPatch(typeof(MilkPressConfig), nameof(MilkPressConfig.CreateBuildingDef))]
        public class MilkPressConfig_CreateBuildingDef_Patch {
            public static void Postfix(BuildingDef __result) {
                __result.InputConduitType = ConduitType.Liquid;
                __result.UtilityInputOffset = new CellOffset(1, 1);
            }
        }

        [HarmonyPatch(typeof(MilkPressConfig), nameof(MilkPressConfig.DoPostConfigureComplete))]
        public class MilkPressConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
                conduitConsumer.conduitType = ConduitType.Liquid;
                conduitConsumer.consumptionRate = 10f;
                conduitConsumer.capacityKG = 20f;
                conduitConsumer.capacityTag = GameTags.Water;
                conduitConsumer.forceAlwaysSatisfied = true;
                conduitConsumer.storage = go.AddOrGet<ComplexFabricator>().inStorage;
                conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            }
        }

        [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.CreateAndRegisterSeedForPlant))]
        public class EntityTemplates_CreateAndRegisterSeedForPlant_Patch {
            public static void Postfix(string id) {
                if (!SingletonOptions<Options>.Instance.AddRecipe) return;
                if (id != "ColdWheatSeed" && id != "BeanPlantSeed") {
                    ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]{
                        new ComplexRecipe.RecipeElement(id, 10f),
                        new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 15f)
                    };
                    ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]{
                        new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
                    };
                    new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array, array2), array, array2, 0, 0) {
                        time = 40f,
                        description = string.Format(BUILDINGS.PREFABS.MILKPRESS.WHEAT_MILK_RECIPE_DESCRIPTION, ITEMS.FOOD.COLDWHEATSEED.NAME, SimHashes.Milk.CreateTag().ProperName()),
                        nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
                        fabricators = new List<Tag> { TagManager.Create("MilkPress") }
                    };
                }
            }
        }
    }
}
