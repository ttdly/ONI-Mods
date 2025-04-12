using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace GeyserExpandMachine.Buildings {
    public class LiquidGeyserExpandConfig : IBuildingConfig {
        public const string ID = "LiquidGeyserExpand";
        private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;
        public static readonly HashedString OutputRibbonID = new HashedString($"{ID}RibbonOutput");
        public static readonly HashedString OutputPortID = new HashedString($"{ID}Output");
        public static readonly HashedString InputRibbonID = new HashedString($"{ID}RibbonInput");

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
                LogicPorts.Port.RibbonInputPort(InputRibbonID, new CellOffset(0, 1),
                    ModString.Logic.GroupInputDesc,
                    ModString.Logic.GroupOutputActive,
                    ""),
                LogicPorts.Port.RibbonOutputPort(OutputRibbonID, new CellOffset(0, 0),
                    ModString.Logic.GroupOutputDesc,
                    ModString.Logic.GroupOutputActive,
                    ""),
                LogicPorts.Port.OutputPort(OutputPortID, new CellOffset(1, 0),
                    ModString.Logic.OutputDesc,
                    ModString.Logic.OutputActive,
                    ModString.Logic.OutputInactive),
                LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(1, 1),
                    STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT,
                    STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE,
                    STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE)
            };
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            var logicExpand = go.AddOrGet<GeyserLogicExpand>();
            logicExpand.portID = OutputPortID;
            logicExpand.ribbonPortID = OutputRibbonID;
            go.AddOrGet<GeyserExpandProxy>();
            var storage = go.AddOrGet<Storage>();
            storage.capacityKg = 50000f;
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            var liquidGeyserExpandDispenser = go.AddOrGet<GeyserExpandDispenser>();            
            liquidGeyserExpandDispenser.conduitType = CONDUIT_TYPE;
            liquidGeyserExpandDispenser.alwaysDispense = true;
            liquidGeyserExpandDispenser.elementFilter = null;
            // go.AddOrGet<SmartReservoir>();
        }

        public override void DoPostConfigureComplete(GameObject go) {
            Object.DestroyImmediate(go.GetComponent<RequireInputs>());
            Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        }
    }
}