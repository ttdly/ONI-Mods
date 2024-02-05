using KSerialization;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using System;
using UnityEngine;
using Market.ItemSpwan;
using static Market.MarketList;
namespace Market {
    public class TrueVendingMachineComponent : StateMachineComponent<TrueVendingMachineComponent.StatesInstance> {
        private KSelectable kSelectable;
        [Serialize]
        public float coin = 0;
        private Guid statuesItemGuid;
        private Extents pickupableExtents;
        private HandleVector<int>.Handle pickupablesChangedEntry;
        [MyCmpReq]
        public readonly KBatchedAnimController kBatchedAnimController;
        [MyCmpReq]
        private readonly Storage storage;
        int x = 2;
        private MeterController meter;
        private bool canStore = true;
        public List<MarketItem> marketItems = new List<MarketItem>();
        public float needConsume;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            kBatchedAnimController.Play("on");
            kSelectable = GetComponent<KSelectable>();
            RefreshStatuesItem();
            CellOffset[] cellOffsets = {
                new CellOffset(-1, 0),
                new CellOffset(0, 0),
                new CellOffset(1, 0),
            };
            pickupableExtents = new Extents(this.NaturalBuildingCell(), cellOffsets);
            pickupablesChangedEntry = GameScenePartitioner.Instance.Add("TrueVendingMachine", gameObject, pickupableExtents, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(OnPickupablesChanged));
            meter = new MeterController(gameObject.GetComponent<KAnimControllerBase>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
            meter.SetPositionPercent(storage.MassStored() / storage.capacityKg);
            smi.StartSM();
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            kSelectable?.RemoveStatusItem(statuesItemGuid);
            GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
        }

        private void OnPickupablesChanged(object data) {
            Pickupable pickupable = data as Pickupable;
            if (!canStore || !(bool)pickupable || pickupable.HasTag(GameTags.Stored))
                return;
            if (pickupable.HasAnyTags(new Tag[3] { GameTags.Seed, GameTags.BuildableRaw, GameTags.RefinedMetal })) {
                if (storage.MassStored() >= 1000f) {
                    canStore = false;
                    GainCoin();
                } else {
                    storage.Store(pickupable.gameObject);
                }
                meter.SetPositionPercent(storage.MassStored() / storage.capacityKg);
            }
        }

        private void GainCoin() {
            for (int i = 0; i < storage.items.Count; i++) {
                if (storage.items[i] == null) continue;
                storage.ConsumeIgnoringDisease(storage.items[i]);
            }
            coin += 100;
            RefreshStatuesItem();
            canStore = true;
        }

        private void RefreshStatuesItem() {
            kSelectable?.RemoveStatusItem(statuesItemGuid);
            statuesItemGuid = kSelectable.AddStatusItem(StaticVars.NowCoin, this);
        }

        public void SpawnItems() {
            foreach (MarketItem item in marketItems) {
                if (item.count == 0) continue;
                GameObject go = null;
#if DEBUG
                PUtil.LogDebug(item.ToString());
#endif
                Vector3 posCbc = gameObject.transform.position;
                posCbc.y += 1f;
                switch (item.type) {
                    case ObjectType.Element:
                        Element element = ElementLoader.FindElementByHash((SimHashes)item.tag.GetHashCode());
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
                foreach (MarketItem item in marketItems) {
                    item.count = 0;
                }
                marketItems.Clear();
            }
            needConsume = 0;
        }

        private void Fall(GameObject go) {
            if (GameComps.Fallers.Has(go)) GameComps.Fallers.Remove(go);
            GameComps.Fallers.Add(go, new Vector2(x, UnityEngine.Random.Range(4f, 8f)));
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
