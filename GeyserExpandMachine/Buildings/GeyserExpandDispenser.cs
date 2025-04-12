using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace GeyserExpandMachine.Buildings {
    public class GeyserExpandDispenser : KMonoBehaviour, ISaveLoadable, IConduitDispenser {
        [SerializeField]
        public ConduitType conduitType;

        [SerializeField]
        public SimHashes[] elementFilter;

        [SerializeField]
        public bool invertElementFilter;

        [SerializeField]
        public bool alwaysDispense;

        [SerializeField]
        public bool isOn = true;

        [SerializeField]
        public bool blocked;

        [SerializeField]
        public bool empty = true;

        [SerializeField]
        public bool useSecondaryOutput;

        [SerializeField]
        public CellOffset noBuildingOutputCellOffset;

        [Serialize]
        public float flowMass = 10f;

        private static readonly Operational.Flag OutputConduitFlag =
            new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

        [MyCmpGet] private Operational operational;

        [MyCmpReq] public Storage storage;

        [MyCmpGet] private Building building;

        private HandleVector<int>.Handle partitionerEntry;
        private int utilityCell = -1;
        private int elementOutputOffset;

        public Storage Storage => storage;

        public ConduitType ConduitType => conduitType;

        public ConduitFlow.ConduitContents ConduitContents => GetConduitManager().GetContents(utilityCell);

        public void SetConduitData(ConduitType type) {
            conduitType = type;
        }

        public ConduitFlow GetConduitManager() {
            switch (conduitType) {
                case ConduitType.Gas:
                    return Game.Instance.gasConduitFlow;
                case ConduitType.Liquid:
                    return Game.Instance.liquidConduitFlow;
                default:
                    return null;
            }
        }

        private void OnConduitConnectionChanged(object data) {
            Trigger(-2094018600, IsConnected);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            GameScheduler.Instance.Schedule("PlumbingTutorial", 2f,
                obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing));
            utilityCell = GetOutputCell(GetConduitManager().conduitType);
            partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn",
                gameObject, utilityCell,
                GameScenePartitioner.Instance.objectLayers[conduitType == ConduitType.Gas ? 12 : 16],
                OnConduitConnectionChanged);
            GetConduitManager()
                .AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Dispense);
            OnConduitConnectionChanged(null);
        }

        protected override void OnCleanUp() {
            GetConduitManager().RemoveConduitUpdater(ConduitUpdate);
            GameScenePartitioner.Instance.Free(ref partitionerEntry);
            base.OnCleanUp();
        }

        public void SetOnState(bool onState) {
            isOn = onState;
        }

        private void ConduitUpdate(float dt) {
            if (operational != null)
                operational.SetFlag(OutputConduitFlag, IsConnected);
            blocked = false;
            if (!isOn)
                return;
            Dispense(dt);
        }

        private void Dispense(float dt) {
            if ((!(operational != null) ||
                 !operational.IsOperational) && !alwaysDispense)
                return;
            if (building != null && building.Def.CanMove)
                utilityCell = GetOutputCell(GetConduitManager().conduitType);
            var suitableElement = FindSuitableElement();
            if (suitableElement != null) {
                suitableElement.KeepZeroMassObject = true;
                empty = false;
                var num1 = GetConduitManager().AddElement(utilityCell, suitableElement.ElementID,
                    flowMass, suitableElement.Temperature, suitableElement.DiseaseIdx,
                    suitableElement.DiseaseCount);
                if (num1 > 0.0) {
                    var num2 = (int)(num1 / (double)suitableElement.Mass *
                                     suitableElement.DiseaseCount);
                    suitableElement.ModifyDiseaseCount(-num2, "ConduitDispenser.ConduitUpdate");
                    suitableElement.Mass -= num1;
                    storage.Trigger(-1697596308, suitableElement.gameObject);
                }
                else {
                    blocked = true;
                }
            }
            else {
                empty = true;
            }
        }

        private PrimaryElement FindSuitableElement() {
            var items = storage.items;
            var count = items.Count;
            for (var index1 = 0; index1 < count; ++index1) {
                var index2 = (index1 + elementOutputOffset) % count;
                var component = items[index2].GetComponent<PrimaryElement>();
                if (component != null && component.Mass > 0.0 &&
                    (conduitType == ConduitType.Liquid ? component.Element.IsLiquid ? 1 : 0
                        : component.Element.IsGas
                            ? 1
                            : 0) != 0 && (elementFilter == null ||
                                          elementFilter.Length == 0 ||
                                          (!invertElementFilter &&
                                           IsFilteredElement(component.ElementID)) ||
                                          (invertElementFilter &&
                                           !IsFilteredElement(component.ElementID)))) {
                    elementOutputOffset = (elementOutputOffset + 1) % count;
                    return component;
                }
            }

            return null;
        }

        private bool IsFilteredElement(SimHashes element) {
            for (var index = 0; index != elementFilter.Length; ++index)
                if (elementFilter[index] == element)
                    return true;

            return false;
        }

        public bool IsConnected {
            get {
                var conduit = Grid.Objects[utilityCell, conduitType == ConduitType.Gas ? 12 : 16];
                return conduit != null &&
                       conduit.GetComponent<BuildingComplete>() != null;
            }
        }

        private int GetOutputCell(ConduitType outputConduitType) {
            var component = GetComponent<Building>();
            if (!(component != null))
                return Grid.OffsetCell(Grid.PosToCell(this), noBuildingOutputCellOffset);
            if (!useSecondaryOutput)
                return component.GetUtilityOutputCell();
            var components = GetComponents<ISecondaryOutput>();
            foreach (var secondaryOutput in components)
                if (secondaryOutput.HasSecondaryConduitType(outputConduitType))
                    return Grid.OffsetCell(component.NaturalBuildingCell(),
                        secondaryOutput.GetSecondaryConduitOffset(outputConduitType));

            return Grid.OffsetCell(component.NaturalBuildingCell(),
                components[0].GetSecondaryConduitOffset(outputConduitType));
        }

        #region 滑动条
        //
        // private float flowMass = 10f;
        //
        // public int SliderDecimalPlaces(int index) => 0;
        //
        // public float GetSliderMin(int index) => 0f;
        //
        // public float GetSliderMax(int index) => 10000f;
        //
        // public float GetSliderValue(int index) => flowMass * 1000f;
        //
        // public void SetSliderValue(float percent, int index) => flowMass = percent / 1000f;
        //
        // public string GetSliderTooltipKey(int index) => "克";
        //
        // public string GetSliderTooltip(int index) => "克描述";
        //
        // public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.VALVESIDESCREEN.TITLE";
        // public string SliderUnits => "g";

        #endregion


    }
}