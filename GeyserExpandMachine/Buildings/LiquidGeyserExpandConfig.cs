using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace GeyserExpandMachine.Buildings {
    public class LiquidGeyserExpandConfig : IBuildingConfig {
        public const string ID = "LiquidGeyserExpand";
        private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;
        public static readonly HashedString OutputRibbonID = new HashedString($"{ID}RibbonOutput");
        public static readonly HashedString OutputPortID = new HashedString($"{ID}Output");

        public override BuildingDef CreateBuildingDef() {
            var tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            var rawMetals = MATERIALS.RAW_METALS;
            var tieR1 = NOISE_POLLUTION.NOISY.TIER1;
            var tieR0 = BUILDINGS.DECOR.PENALTY.TIER0;
            var buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 1, "logic_not_kanim", 30, 10f, tieR3, rawMetals, 1600f, BuildLocationRule.OnFloor, tieR0, tieR1);
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
            buildingDef.OutputConduitType = CONDUIT_TYPE;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "Metal";

            buildingDef.AttachmentSlotTag = GameTags.GeyserFeature;
            buildingDef.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
            buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;

            buildingDef.LogicOutputPorts = new List<LogicPorts.Port> {
                LogicPorts.Port.RibbonOutputPort(OutputRibbonID, new CellOffset(0, 0),
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.LOGIC_PORT,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.INPUT_PORT_ACTIVE,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.INPUT_PORT_INACTIVE),
                LogicPorts.Port.OutputPort(OutputPortID, new CellOffset(1, 0),
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.LOGIC_PORT_OUTPUT,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.OUTPUT_PORT_ACTIVE,
                    STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.OUTPUT_PORT_INACTIVE)
            };
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            // GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            // go.AddOrGet<DevPump>().elementState = Filterable.ElementState.Liquid;
            // go.AddOrGet<GeyserExpand>();
            go.AddOrGet<GeyserLogicExpand>();
            go.AddOrGet<GeyserExpandProxy>();
            var storage = go.AddOrGet<Storage>();
            storage.capacityKg = 50000f;
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            var liquidGeyserExpandDispenser = go.AddOrGet<GeyserExpandDispenser>();            
            liquidGeyserExpandDispenser.conduitType = CONDUIT_TYPE;
            liquidGeyserExpandDispenser.alwaysDispense = true;
            liquidGeyserExpandDispenser.elementFilter = null;
        }

        public override void DoPostConfigureComplete(GameObject go) {
            Object.DestroyImmediate(go.GetComponent<RequireInputs>());
            Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        }
    }
}