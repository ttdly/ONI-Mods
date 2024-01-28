// Decompiled with JetBrains decompiler
// Type: FarmTileConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E519DD73-DA90-48A8-894A-B7F073F3EAD7
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

namespace MoreTiles {
    public class FarmTileLightableConfig : IBuildingConfig {
        public const string ID = "FarmTileLightable";

        public override BuildingDef CreateBuildingDef() {
            float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] farmable = MATERIALS.FARMABLE;
            EffectorValues none1 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = BUILDINGS.DECOR.NONE;
            EffectorValues noise = none1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "farmtilerotating_kanim", 100, 30f, tieR2, farmable, 1600f, BuildLocationRule.Tile, none2, noise);
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
            buildingDef.PermittedRotations = PermittedRotations.FlipV;
            buildingDef.DragBuild = true;
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 1f;
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
            PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
            plantablePlot.occupyingObjectRelativePosition = new Vector3(0.0f, 1f, 0.0f);
            plantablePlot.AddDepositTag(GameTags.CropSeed);
            plantablePlot.AddDepositTag(GameTags.WaterSeed);
            plantablePlot.SetFertilizationFlags(true, false);
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Farm;
            go.AddOrGet<AnimTileable>();
            Light2D light2d = go.AddOrGet<Light2D>();
            light2d.Offset = new Vector2(0, 1);
            light2d.Range = 1;
            light2d.Lux = 200;
            light2d.drawOverlay = true;
            light2d.shape = LightShape.Circle;
            light2d.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
            light2d.Color = LIGHT2D.CEILINGLIGHT_COLOR;
            light2d.enabled = false;
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go) {
            GeneratedBuildings.RemoveLoopingSounds(go);
            go.GetComponent<KPrefabID>().AddTag(GameTags.FarmTiles);
            SetUpFarmPlotTags(go);
            go.AddOrGet<FarmTileLightable>();
        }

        public static void SetUpFarmPlotTags(GameObject go) {
            go.GetComponent<KPrefabID>().prefabSpawnFn += inst => {
                Rotatable component1 = inst.GetComponent<Rotatable>();
                PlantablePlot component2 = inst.GetComponent<PlantablePlot>();
                switch (component1.GetOrientation()) {
                    case Orientation.Neutral:
                    case Orientation.FlipH:
                        component2.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Top);
                        break;
                    case Orientation.R90:
                    case Orientation.R270:
                        component2.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Side);
                        break;
                    case Orientation.R180:
                    case Orientation.FlipV:
                        component2.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Bottom);
                        break;
                }
            };
        }
    }

}