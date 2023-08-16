using System.Collections.Generic;
using UnityEngine;

namespace TweaksPack.Auto {
    public class CompostAuto : StateMachineComponent<CompostAuto.StatesInstance>, IGameObjectEffectDescriptor {
        [MyCmpGet]
        private Operational operational;
        [MyCmpGet]
        private Storage storage;
        [SerializeField]
        public float flipInterval = 600f;
        [SerializeField]
        public float simulatedInternalTemperature = 323.15f;
        [SerializeField]
        public float simulatedInternalHeatCapacity = 400f;
        [SerializeField]
        public float simulatedThermalConductivity = 1000f;
        private SimulatedTemperatureAdjuster temperatureAdjuster;
        private static readonly EventSystem.IntraObjectHandler<CompostAuto> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<CompostAuto>((component, data) => component.OnStorageChanged(data));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            Subscribe(-1697596308, OnStorageChangedDelegate);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            GetComponent<ManualDeliveryKG>().ShowStatusItem = false;
            temperatureAdjuster = new SimulatedTemperatureAdjuster(simulatedInternalTemperature, simulatedInternalHeatCapacity, simulatedThermalConductivity, GetComponent<Storage>());
            smi.StartSM();
        }

        protected override void OnCleanUp() => temperatureAdjuster.CleanUp();

        private void OnStorageChanged(object data) {
            int _ = (UnityEngine.Object)data == null ? 1 : 0;
        }

        public List<Descriptor> GetDescriptors(GameObject go) => SimulatedTemperatureAdjuster.GetDescriptors(simulatedInternalTemperature);

        public class StatesInstance :
          GameStateMachine<States, StatesInstance, CompostAuto, object>.GameInstance {
            public StatesInstance(CompostAuto master)
              : base(master) {
            }

            public bool CanStartConverting() => master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting();

            public bool CanContinueConverting() => master.GetComponent<ElementConverter>().CanConvertAtAll();

            public bool IsEmpty() => master.storage.IsEmpty();

            public void ResetWorkable() {
                CompostWorkable component = master.GetComponent<CompostWorkable>();
                component.ShowProgressBar(false);
                component.WorkTimeRemaining = component.GetWorkTime();
            }
        }

        public class States : GameStateMachine<States, StatesInstance, CompostAuto> {
            public State empty;
            public State insufficientMass;
            public State disabled;
            public State disabledEmpty;
            public State inert;
            public State composting;

            public override void InitializeStates(out BaseState default_state) {
                default_state = empty;
                serializable = SerializeType.Both_DEPRECATED;
                empty.Enter("empty", smi => smi.ResetWorkable()).EventTransition(GameHashes.OnStorageChange, insufficientMass, smi => !smi.IsEmpty()).EventTransition(GameHashes.OperationalChanged, disabledEmpty, smi => !smi.GetComponent<Operational>().IsOperational).ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingWaste).PlayAnim("off");
                insufficientMass.Enter("empty", smi => smi.ResetWorkable()).EventTransition(GameHashes.OnStorageChange, empty, smi => smi.IsEmpty()).EventTransition(GameHashes.OnStorageChange, inert, smi => smi.CanStartConverting()).ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingWaste).PlayAnim("idle_half");
                inert.EventTransition(GameHashes.OperationalChanged, disabled, smi => !smi.GetComponent<Operational>().IsOperational).PlayAnim("on").OnAnimQueueComplete(composting);
                composting.QueueAnim("composting_loop", true).Enter("Composting", smi => smi.master.operational.SetActive(true)).EventTransition(GameHashes.OnStorageChange, empty, smi => !smi.CanContinueConverting()).EventTransition(GameHashes.OperationalChanged, disabled, smi => !smi.GetComponent<Operational>().IsOperational).ScheduleGoTo(smi => smi.master.flipInterval, inert).Exit(smi => smi.master.operational.SetActive(false));
                disabled.Enter("disabledEmpty", smi => smi.ResetWorkable()).PlayAnim("on").EventTransition(GameHashes.OperationalChanged, inert, smi => smi.GetComponent<Operational>().IsOperational);
                disabledEmpty.Enter("disabledEmpty", smi => smi.ResetWorkable()).PlayAnim("off").EventTransition(GameHashes.OperationalChanged, empty, smi => smi.GetComponent<Operational>().IsOperational);
            }
        }
    }

}