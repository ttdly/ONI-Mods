using TUNING;
using UnityEngine;

namespace MoreTiles {
    public class AutoLightTileConfig : IBuildingConfig {
        public static string ID = "AutoLightTile";

        public override BuildingDef CreateBuildingDef() {
            float[] tieR2 = { 100f, 10f};
            string[] farmable = { "Glass", "RefinedMetal"};
            EffectorValues none1 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = BUILDINGS.DECOR.NONE;
            EffectorValues noise = none1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "cellingLightTile_kanim", 100, 30f, tieR2, farmable, 1600f, BuildLocationRule.Tile, none2, noise);
            BuildingTemplates.CreateFoundationTileDef(buildingDef);
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            buildingDef.DragBuild = true;
            buildingDef.BlockTileIsTransparent = true;
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 10f;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.doReplaceElement = true;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<TileTemperature>();
            BuildingTemplates.CreateDefaultStorage(go).SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            go.AddOrGet<AnimTileable>();
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go) {
            GeneratedBuildings.RemoveLoopingSounds(go);
            go.GetComponent<KPrefabID>().AddTag(GameTags.FarmTiles);
            go.AddOrGet<AutoLightTile>();
            Light2D light2d = go.AddOrGet<Light2D>();
            light2d.Offset = new Vector2(0, 1);
            light2d.Range = 1;
            light2d.Lux = 4000;
            light2d.drawOverlay = true;
            light2d.shape = LightShape.Circle;
            light2d.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
            light2d.Color = LIGHT2D.CEILINGLIGHT_COLOR;
            light2d.enabled = false;
            RangeVisualizer rangeVisualizer = go.AddOrGet<RangeVisualizer>();
            rangeVisualizer.OriginOffset = new Vector2I(0, 0);
            rangeVisualizer.RangeMin.x = 0;
            rangeVisualizer.RangeMin.y = 1;
            rangeVisualizer.RangeMax.x = 0;
            rangeVisualizer.RangeMax.y = 1;
            rangeVisualizer.BlockingTileVisible = true;
        }
    }
}
