using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElectricCooling : StateMachineComponent<ElectricCooling.StatesInstance>, IGameObjectEffectDescriptor {
    [MyCmpGet]
    private Operational operational;
    [MyCmpGet]
    private Storage storage;
    [MyCmpGet]
    private ElementConverter elementConverter;
    [MyCmpGet]
    private ElementConsumer elementConsumer;
    public Tag consumeTag;

    public bool HasFilter() => this.elementConverter.HasEnoughMass(this.consumeTag);

    public bool IsConvertable() => this.elementConverter.HasEnoughMassToStartConverting();

    protected override void OnSpawn() {
        base.OnSpawn();
        this.smi.StartSM();
    }

    public List<Descriptor> GetDescriptors(GameObject go) => (List<Descriptor>)null;

    public class StatesInstance :
      GameStateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.GameInstance {
        public StatesInstance(ElectricCooling smi)
          : base(smi) {
        }
    }

    public class States : GameStateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling> {
        public ElectricCooling.States.ReadyStates hasFilter;
        public GameStateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State waiting;

        public override void InitializeStates(out StateMachine.BaseState default_state) {
            default_state = (StateMachine.BaseState)this.waiting;
            this.waiting.EventTransition(GameHashes.OnStorageChange, (GameStateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State)this.hasFilter, (StateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.Transition.ConditionCallback)(smi => smi.master.HasFilter() && smi.master.operational.IsOperational)).EventTransition(GameHashes.OperationalChanged, (GameStateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State)this.hasFilter, (StateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.Transition.ConditionCallback)(smi => smi.master.HasFilter() && smi.master.operational.IsOperational));
            this.hasFilter.EventTransition(GameHashes.OperationalChanged, this.waiting, (StateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.Transition.ConditionCallback)(smi => !smi.master.operational.IsOperational)).Enter("EnableConsumption", (StateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State.Callback)(smi => smi.master.elementConsumer.EnableConsumption(true))).Exit("DisableConsumption", (StateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State.Callback)(smi => smi.master.elementConsumer.EnableConsumption(false))).DefaultState(this.hasFilter.idle);
            this.hasFilter.idle.EventTransition(GameHashes.OnStorageChange, this.hasFilter.converting, (StateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.Transition.ConditionCallback)(smi => smi.master.IsConvertable()));
            this.hasFilter.converting.Enter("SetActive(true)", (StateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State.Callback)(smi => smi.master.operational.SetActive(true))).Exit("SetActive(false)", (StateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State.Callback)(smi => smi.master.operational.SetActive(false))).EventTransition(GameHashes.OnStorageChange, this.hasFilter.idle, (StateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.Transition.ConditionCallback)(smi => !smi.master.IsConvertable()));
        }

        public class ReadyStates :
          GameStateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State {
            public GameStateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State idle;
            public GameStateMachine<ElectricCooling.States, ElectricCooling.StatesInstance, ElectricCooling, object>.State converting;
        }
    }
}
