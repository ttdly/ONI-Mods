using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BaseP1Config : IBuildingConfig {
  public const string ID = "BaseP1";
  private const float INPUT_KG = 100f;
  private const float OUTPUT_METAL = 90f;
  private const float OUTPUT_UNOBTANIUM = 10f;

  private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers =
    new List<Storage.StoredItemModifier> {
      Storage.StoredItemModifier.Hide,
      Storage.StoredItemModifier.Preserve,
      Storage.StoredItemModifier.Insulate,
      Storage.StoredItemModifier.Seal
    };

  public override BuildingDef CreateBuildingDef() {
    var tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    var minerals = MATERIALS.REFINED_METALS;
    var tieR6 = NOISE_POLLUTION.NOISY.TIER6;
    var tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    var noise = tieR6;
    var buildingDef = BuildingTemplates.CreateBuildingDef("BaseP1", 4, 4, "basep32_kanim", 30, 60f, tieR5, minerals,
      2400f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.RequiresPowerInput = false;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.AudioSize = "large";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
    go.AddOrGet<DropAllWorkable>();
    var fabricator = go.AddOrGet<ComplexFabricator>();
    fabricator.heatedTemperature = 313.15f;
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    go.AddOrGet<ComplexFabricator>().duplicantOperated = false;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<ComplexFabricatorWorkable>();
    go.AddOrGet<CopyBuildingSettings>();
    BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
    fabricator.outStorage.capacityKg = 2000f;
    fabricator.inStorage.capacityKg = 200f;
    fabricator.inStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
    fabricator.buildStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
    fabricator.outStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
    fabricator.outputOffset = new Vector3(1f, 0.5f);
    foreach (var element in ElementLoader.elements.FindAll(e => e.IsSolid && e.HasTag(GameTags.Metal)))
      if (!element.HasTag(GameTags.Noncrushable) && element.tag != SimHashes.FoolsGold.CreateTag() &&
          element.tag != SimHashes.Electrum.CreateTag()) {
        var lowTempTransition = element.highTempTransition.lowTempTransition;
        if (lowTempTransition != element) {
          var recipeElementArray1 = new ComplexRecipe.RecipeElement[1] {
            new ComplexRecipe.RecipeElement(lowTempTransition.tag, INPUT_KG)
          };
          var recipeElementArray2 = new ComplexRecipe.RecipeElement[2] {
            new ComplexRecipe.RecipeElement(element.tag, OUTPUT_METAL,
              ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
            new ComplexRecipe.RecipeElement(SimHashes.Unobtanium.CreateTag(), OUTPUT_UNOBTANIUM,
              ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
          };
          var obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("BaseP1", lowTempTransition.tag);
          var str = ComplexRecipeManager.MakeRecipeID("BaseP1", recipeElementArray1, recipeElementArray2);
          var complexRecipe = new ComplexRecipe(str, recipeElementArray1, recipeElementArray2) {
            time = 20f,
            description = string.Format(GravitasMemory.BUILDINGS.PREFABS.BASEP1.RECIPE_DESCRIPTION, element.name,
              lowTempTransition.name),
            nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
            fabricators = new List<Tag> {
              TagManager.Create("BaseP1")
            }
          };
          ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, str);
        }
      }

    Prioritizable.AddRef(go);
    var light2D = go.AddOrGet<Light2D>();
    light2D.Color = LIGHT2D.LIGHT_PURPLE;
    light2D.Range = 5f;
    light2D.Offset = LIGHT2D.HEADQUARTERS_OFFSET;
    light2D.overlayColour = LIGHT2D.LIGHT_PURPLE;
    light2D.shape = LightShape.Circle;
    light2D.drawOverlay = true;
  }

  public override void DoPostConfigureComplete(GameObject go) {
    SymbolOverrideControllerUtil.AddToPrefab(go);
    go.GetComponent<KPrefabID>().prefabSpawnFn += game_object => {
      var component = game_object.AddOrGet<ComplexFabricator>();
      var meter = new MeterController(component.GetComponent<KBatchedAnimController>(), "meter_target", "meter",
        Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
      component.Subscribe(-1697596308,
        data => meter.SetPositionPercent(
          Mathf.Clamp01(component.inStorage.MassStored() / component.inStorage.capacityKg)));
      meter.SetPositionPercent(component.inStorage.MassStored() / component.inStorage.capacityKg);
    };
  }

  public override void DoPostConfigureUnderConstruction(GameObject go) {
    base.DoPostConfigureUnderConstruction(go);
    go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
  }
}