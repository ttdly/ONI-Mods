using System;
using System.Collections.Generic;
using KSerialization;
using Market.ItemSpwan;
using PeterHan.PLib.Core;
using UnityEngine;
using static Market.MarketList;
using Random = UnityEngine.Random;

namespace Market {
  public class TrueVendingMachineComponent : StateMachineComponent<TrueVendingMachineComponent.StatesInstance> {
    private static readonly EventSystem.IntraObjectHandler<TrueVendingMachineComponent> OnStorageChangedDelegate =
      new EventSystem.IntraObjectHandler<TrueVendingMachineComponent>((component, data) =>
        component.OnStorageChanged(data));

    [Serialize] public float coin;

    public float needConsume;

    [MyCmpReq] public readonly KBatchedAnimController kBatchedAnimController;

    [MyCmpReq] private readonly Storage storage;

    private FilteredStorage filteredStorage;
    private KSelectable kSelectable;
    public List<MarketItem> marketItems = new List<MarketItem>();
    private MeterController meter;
    private HandleVector<int>.Handle pickupablesChangedEntry;
    private Guid statuesItemGuid;
    private int x = 2;

    private void OnStorageChanged(object _) {
      UpdateMeter();
      if (storage.MassStored() == storage.capacityKg) GainCoin();
    }

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
      filteredStorage = new FilteredStorage(this, null, null, false, Db.Get().ChoreTypes.StorageFetch);
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      coin = 10000;
      kSelectable = GetComponent<KSelectable>();
      RefreshStatuesItem();
      Subscribe((int)GameHashes.OnStorageChange, OnStorageChangedDelegate);
      meter = new MeterController(this, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
      filteredStorage.FilterChanged();
      UpdateMeter();
      smi.StartSM();
    }

    private void UpdateMeter() {
      meter.SetPositionPercent(storage.MassStored() / storage.capacityKg);
    }

    protected override void OnCleanUp() {
      base.OnCleanUp();
      kSelectable?.RemoveStatusItem(statuesItemGuid);
      GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
    }

    private void GainCoin() {
      storage.ConsumeAllIgnoringDisease();
      coin += 100;
    }

    private void RefreshStatuesItem() {
      kSelectable?.RemoveStatusItem(statuesItemGuid);
      statuesItemGuid = kSelectable.AddStatusItem(StaticVars.NowCoin, this);
    }

    public void SpawnItems() {
      foreach (var item in marketItems) {
        if (item.count == 0) continue;
        GameObject go = null;
#if DEBUG
                PUtil.LogDebug(item.ToString());
#endif
        var posCbc = gameObject.transform.position;
        posCbc.y += 1f;
        switch (item.type) {
          case ObjectType.Element:
            var element = ElementLoader.FindElementByHash((SimHashes)item.tag.GetHashCode());
            go = element.substance.SpawnResource(posCbc, item.unit * item.count, 0f, byte.MaxValue, 0);
            break;
          case ObjectType.Object:
            go = GameUtil.KInstantiate(Assets.GetPrefab(item.tag), posCbc, Grid.SceneLayer.Front);
            go.SetActive(true);
            break;
          case ObjectType.NeedPack:
            go = GameUtil.KInstantiate(Assets.GetPrefab(item.tag), posCbc, Grid.SceneLayer.Front);
            go.SetActive(true);
            go.AddOrGet<ItemSpawnPackComponent>().spawnTag = item.innerTag;
            go.AddOrGet<UserNameable>().savedName = item.name;
            go.name = item.name;
            break;
        }

        if (go == null) {
          needConsume -= item.count * item.price;
          continue;
        }

        Fall(go);
      }

      ConsumeCoins(needConsume);
    }

    private void ConsumeCoins(float needConsume) {
      coin -= needConsume;
      RefreshStatuesItem();
    }

    public void ClearBuffer() {
      if (marketItems.Count > 0) {
        foreach (var item in marketItems) item.count = 0;
        marketItems.Clear();
      }

      needConsume = 0;
    }

    private void Fall(GameObject go) {
      if (GameComps.Fallers.Has(go)) GameComps.Fallers.Remove(go);
      GameComps.Fallers.Add(go, new Vector2(x, Random.Range(4f, 8f)));
      x *= -1;
    }

    public class StatesInstance :
      GameStateMachine<States, StatesInstance, TrueVendingMachineComponent, object>.GameInstance {
      public StatesInstance(TrueVendingMachineComponent master)
        : base(master) {
      }
    }

    public class States : GameStateMachine<States, StatesInstance, TrueVendingMachineComponent> {
      public State closed;
      public State open;

      public override void InitializeStates(out BaseState default_state) {
        default_state = closed;
        closed.PlayAnim("on");
        open.PlayAnim("working").OnAnimQueueComplete(closed).Exit(smi => smi.master.SpawnItems());
      }
    }
  }
}