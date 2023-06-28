
using System.Collections.Generic;
using TUNING;
using UnityEngine;


public class OverriderConfig : IBuildingConfig {
    
    public const string ID = "Overrider";
    private const int RANGEX = 10;
    private const int RANGEY = 4;
    private string[] materials = new string[2] {"Special", "Other" };
    private float[] mass = new float[2] { 100f, 50f };
    
    public override BuildingDef CreateBuildingDef() {
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Overrider", 1, 1, "overrider_kanim", 30, 30f, mass, materials, 1600f, BuildLocationRule.OnFoundationRotatable, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER4);

        buildingDef.Floodable = false;
        buildingDef.Overheatable = false;
        buildingDef.Entombable = false;
        buildingDef.AudioCategory = "Metal";
        buildingDef.SceneLayer = Grid.SceneLayer.Building;
        buildingDef.PermittedRotations = PermittedRotations.R360;
        buildingDef.AlwaysOperational = true;

        return buildingDef;
    }
    
    public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => OverriderConfig.AddVisualizer(go, true);
    
    public override void DoPostConfigureComplete(GameObject go) {
        Overrider overrider = go.AddOrGet<Overrider>();
        overrider.wasOn = false;
        overrider.detectionRangeY = RANGEY;
        overrider.detectionRangeX = RANGEX;
        OverriderConfig.AddVisualizer(go, false);
    }
    
    private static void AddVisualizer(GameObject prefab, bool movable) {
        RangeVisualizer rangeVisualizer = prefab.AddOrGet<RangeVisualizer>();
        rangeVisualizer.OriginOffset = new Vector2I(0, 0);
        rangeVisualizer.RangeMin.x = - (RANGEX / 2);
        rangeVisualizer.RangeMin.y = 0;
        rangeVisualizer.RangeMax.x = (RANGEX / 2);
        rangeVisualizer.RangeMax.y = RANGEY;
        rangeVisualizer.BlockingTileVisible = true;
    }

    public override void DoPostConfigureUnderConstruction(GameObject go) {
        base.DoPostConfigureUnderConstruction(go);
        go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
    }
}
