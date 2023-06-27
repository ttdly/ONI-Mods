// Decompiled with JetBrains decompiler
// Type: AirFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2BFC01D3-C011-441F-99ED-A7DA2F70C2DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GeyserC4 : StateMachineComponent<GeyserC4.StatesInstance>, IGameObjectEffectDescriptor
{
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

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public List<Descriptor> GetDescriptors(GameObject go) => (List<Descriptor>) null;

  public class StatesInstance : 
    GameStateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.GameInstance
  {
    public StatesInstance(GeyserC4 smi)
      : base(smi)
    {
    }
  }

  public class States : GameStateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4>
  {
    public GeyserC4.States.ReadyStates hasFilter;
    public GameStateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State waiting;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.waiting;
      this.waiting.EventTransition(GameHashes.OnStorageChange, (GameStateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State) this.hasFilter, (StateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.Transition.ConditionCallback) (smi => smi.master.HasFilter() && smi.master.operational.IsOperational)).EventTransition(GameHashes.OperationalChanged, (GameStateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State) this.hasFilter, (StateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.Transition.ConditionCallback) (smi => smi.master.HasFilter() && smi.master.operational.IsOperational));
      this.hasFilter.EventTransition(GameHashes.OperationalChanged, this.waiting, (StateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational)).Enter("EnableConsumption", (StateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State.Callback) (smi => smi.master.elementConsumer.EnableConsumption(true))).Exit("DisableConsumption", (StateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State.Callback) (smi => smi.master.elementConsumer.EnableConsumption(false))).DefaultState(this.hasFilter.idle);
      this.hasFilter.idle.EventTransition(GameHashes.OnStorageChange, this.hasFilter.converting, (StateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.Transition.ConditionCallback) (smi => smi.master.IsConvertable()));
      this.hasFilter.converting.Enter("SetActive(true)", (StateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Exit("SetActive(false)", (StateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).EventTransition(GameHashes.OnStorageChange, this.hasFilter.idle, (StateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.Transition.ConditionCallback) (smi => !smi.master.IsConvertable()));
    }

    public class ReadyStates : 
      GameStateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State
    {
      public GameStateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State idle;
      public GameStateMachine<GeyserC4.States, GeyserC4.StatesInstance, GeyserC4, object>.State converting;
    }
  }
}
