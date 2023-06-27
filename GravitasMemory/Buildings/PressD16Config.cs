using GravitasMemory;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class PressD16Config : IBuildingConfig {
    public const string ID = "PressD16";
    public List<Tag> tags = new List<Tag>() {
        SimHashes.Fossil.CreateTag(),
        SimHashes.Isoresin.CreateTag(),
        SimHashes.Ceramic.CreateTag(),
        SimHashes.SuperInsulator.CreateTag(),
        new Tag("PlanBuildableRaw")
    };
    private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>(){
        Storage.StoredItemModifier.Hide,
        Storage.StoredItemModifier.Preserve,
        Storage.StoredItemModifier.Insulate,
        Storage.StoredItemModifier.Seal
    };

    public override BuildingDef CreateBuildingDef() {
        float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
        string[] allMinerals = { "Special" };
        EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
        EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
        EffectorValues noise = tieR6;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("PressD16", 3, 6, "pressd16_kanim", 30, 60f, tieR5, allMinerals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
        buildingDef.RequiresPowerInput = true;
        buildingDef.EnergyConsumptionWhenActive = 2000f;
        buildingDef.SelfHeatKilowattsWhenActive = 0f;
        buildingDef.OverheatTemperature = 3000f;
        buildingDef.ViewMode = OverlayModes.Power.ID;
        buildingDef.AudioCategory = "HollowMetal";
        buildingDef.AudioSize = "large";
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
        go.AddOrGet<DropAllWorkable>();
        go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
        ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
        fabricator.duplicantOperated = true;
        fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
        fabricator.keepExcessLiquids = true;
        go.AddOrGet<FabricatorIngredientStatusManager>();
        go.AddOrGet<CopyBuildingSettings>();
        ComplexFabricatorWorkable fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
        BuildingTemplates.CreateComplexFabricatorStorage(go, (ComplexFabricator)fabricator);
        fabricator.outStorage.capacityKg = 2000f;
        fabricator.inStorage.capacityKg = 2002f;
        fabricator.inStorage.SetDefaultStoredItemModifiers(PressD16Config.RefineryStoredItemModifiers);
        fabricator.buildStorage.SetDefaultStoredItemModifiers(PressD16Config.RefineryStoredItemModifiers);
        fabricator.outStorage.SetDefaultStoredItemModifiers(PressD16Config.RefineryStoredItemModifiers);
        fabricator.outputOffset = new Vector3(0f, 2f);
        KAnimFile[] kanimFileArray = new KAnimFile[1]{
            Assets.GetAnim((HashedString) "anim_interacts_supermaterial_refinery_kanim")
        };
        fabricatorWorkable.overrideAnims = kanimFileArray;
        foreach (Element element in ElementLoader.elements.FindAll((Predicate<Element>)(e => e.IsSolid && e.HasTag(GameTags.BuildableRaw)))) {
            if (!element.HasTag(new Tag("PlanBuildableRaw"))) {
                Element lowTempTransition = element.highTempTransition.lowTempTransition;
                if (lowTempTransition != element) {
                    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[2]{
                        new ComplexRecipe.RecipeElement(element.tag, 1000f),
                        new ComplexRecipe.RecipeElement(SimHashes.Unobtanium.CreateTag(), 1f),
                    };
                    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]{
                        new ComplexRecipe.RecipeElement(Crystal.TAG, 200f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
                    };
                    string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("PressD16", element.tag);
                    string str = ComplexRecipeManager.MakeRecipeID("PressD16", (IList<ComplexRecipe.RecipeElement>)recipeElementArray1, (IList<ComplexRecipe.RecipeElement>)recipeElementArray2);
                    ComplexRecipe complexRecipe = new ComplexRecipe(str, recipeElementArray1, recipeElementArray2) {
                        time = 40f,
                        description = string.Format((string)GravitasMemory.BUILDINGS.PREFABS.PRESSD16.RECIPE_DESCRIPTION, (object)element.name, (object)Crystal.SOLID_ID),
                        nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
                        fabricators = new List<Tag>(){
                            TagManager.Create("PressD16")
                        }
                    };
                    ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, str);
                }
            }
        }

        Prioritizable.AddRef(go);
    }

    public override void DoPostConfigureComplete(GameObject go) {
        SymbolOverrideControllerUtil.AddToPrefab(go);
        go.AddOrGetDef<PoweredActiveStoppableController.Def>();
        go.GetComponent<KPrefabID>().prefabSpawnFn += (KPrefabID.PrefabFn)(game_object => {
            ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
            component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
            component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
            component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
            component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
            component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            MeterController meter = new MeterController((KAnimControllerBase)component.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[4]{
                "meter_target",
                "meter_fill",
                "meter_frame",
                "meter_OL"
            });
            ComplexFabricator fabricator = component.GetComponent<ComplexFabricator>();
            component.Subscribe(-1697596308, (Action<object>)(data => meter.SetPositionPercent(Mathf.Clamp01(fabricator.inStorage.MassStored() / fabricator.inStorage.capacityKg))));
            meter.SetPositionPercent(fabricator.inStorage.MassStored() / fabricator.inStorage.capacityKg);
        });
    }
}