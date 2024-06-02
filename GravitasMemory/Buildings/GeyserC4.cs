using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GeyserC4 : StateMachineComponent<GeyserC4.StatesInstance>, IGameObjectEffectDescriptor {
  public Tag consumeTag;

  [MyCmpGet] private ElementConsumer elementConsumer;

  [MyCmpGet] private ElementConverter elementConverter;

  [MyCmpGet] private Operational operational;

  [MyCmpGet] private Storage storage;

  public List<Descriptor> GetDescriptors(GameObject go) {
    return null;
  }

  public bool HasFilter() {
    return elementConverter.HasEnoughMass(consumeTag);
  }

  public bool IsConvertable() {
    return elementConverter.HasEnoughMassToStartConverting();
  }

  protected override void OnSpawn() {
    base.OnSpawn();
    smi.StartSM();
  }

  public class StatesInstance :
    GameStateMachine<States, StatesInstance, GeyserC4, object>.GameInstance {
    public StatesInstance(GeyserC4 smi)
      : base(smi) {
    }
  }

  public class States : GameStateMachine<States, StatesInstance, GeyserC4> {
    public ReadyStates hasFilter;
    public State waiting;

    public override void InitializeStates(out BaseState default_state) {
      default_state = waiting;
      waiting.EventTransition(GameHashes.OnStorageChange, hasFilter,
        smi => smi.master.HasFilter() && smi.master.operational.IsOperational).EventTransition(
        GameHashes.OperationalChanged, hasFilter,
        smi => smi.master.HasFilter() && smi.master.operational.IsOperational);
      hasFilter.EventTransition(GameHashes.OperationalChanged, waiting, smi => !smi.master.operational.IsOperational)
        .Enter("EnableConsumption", smi => smi.master.elementConsumer.EnableConsumption(true))
        .Exit("DisableConsumption", smi => smi.master.elementConsumer.EnableConsumption(false))
        .DefaultState(hasFilter.idle);
      hasFilter.idle.EventTransition(GameHashes.OnStorageChange, hasFilter.converting,
        smi => smi.master.IsConvertable());
      hasFilter.converting.Enter("SetActive(true)", smi => smi.master.operational.SetActive(true))
        .Exit("SetActive(false)", smi => smi.master.operational.SetActive(false))
        .EventTransition(GameHashes.OnStorageChange, hasFilter.idle, smi => !smi.master.IsConvertable());
    }

    public class ReadyStates :
      State {
      public State converting;
      public State idle;
    }
  }
}