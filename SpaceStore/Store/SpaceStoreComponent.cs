using KSerialization;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using System;
using UnityEngine;
using static SpaceStore.Store.StoreList;
namespace SpaceStore.Store
{
    public class SpaceStoreComponent : StateMachineComponent<SpaceStoreComponent.StatesInstance>
    {
        private KSelectable kSelectable;
        [Serialize]
        public float coin = 0;
        private Guid statuesItemGuid;
        private HandleVector<int>.Handle pickupablesChangedEntry;
        [MyCmpReq]
        public readonly KBatchedAnimController kBatchedAnimController;
        [MyCmpReq]
        private readonly Storage storage;
        int x = 2;
        public List<MarketItem> marketItems = new List<MarketItem>();
        public float needConsume;

        private FilteredStorage filteredStorage;


        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            filteredStorage = new FilteredStorage(this, null, null, false, Db.Get().ChoreTypes.StorageFetch);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            coin = 10000;
            kSelectable = GetComponent<KSelectable>();
            smi.StartSM();
        }


        protected override void OnCleanUp() {
            base.OnCleanUp();
            kSelectable?.RemoveStatusItem(statuesItemGuid);
            GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
        }


        public void SpawnItems()
        {
            foreach (MarketItem item in marketItems)
            {
                if (item.count == 0) continue;
                GameObject go = null;
#if DEBUG
                PUtil.LogDebug(item.ToString());
#endif
                Vector3 posCbc = gameObject.transform.position;
                posCbc.y += 1f;
                switch (item.type)
                {
                    case ObjectType.Element:
                        Element element = ElementLoader.FindElementByHash((SimHashes)item.tag.GetHashCode());
                        go = element.substance.SpawnResource(posCbc, item.unit * item.count, 0f, byte.MaxValue, 0);
                        break;
                    case ObjectType.Object:
                        go = GameUtil.KInstantiate(Assets.GetPrefab(item.tag), posCbc, Grid.SceneLayer.Front);
                        go.SetActive(true);
                        break;
                    //case ObjectType.NeedPack:
                    //    go = GameUtil.KInstantiate(Assets.GetPrefab(item.tag), posCbc, Grid.SceneLayer.Front);
                    //    go.SetActive(true);
                    //    go.AddOrGet<ItemSpawnPackComponent>().spawnTag = item.innerTag;
                    //    go.AddOrGet<UserNameable>().savedName = item.name;
                    //    go.name = item.name;
                    //    break;
                }
                if (go == null)
                {
                    needConsume -= item.count * item.price;
                    continue;
                }
                Fall(go);
            }
        }

        public void ClearBuffer()
        {
            if (marketItems.Count > 0)
            {
                foreach (MarketItem item in marketItems)
                {
                    item.count = 0;
                }
                marketItems.Clear();
            }
            needConsume = 0;
        }

        private void Fall(GameObject go)
        {
            if (GameComps.Fallers.Has(go)) GameComps.Fallers.Remove(go);
            GameComps.Fallers.Add(go, new Vector2(x, UnityEngine.Random.Range(4f, 8f)));
            x *= -1;
        }

        public class StatesInstance :
          GameStateMachine<States, StatesInstance, SpaceStoreComponent, object>.GameInstance
        {
            public StatesInstance(SpaceStoreComponent master)
              : base(master)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, SpaceStoreComponent>
        {
            public State closed;
            public State open;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = closed;
                closed.PlayAnim("on");
                open.PlayAnim("working").OnAnimQueueComplete(closed).Exit(smi => smi.master.SpawnItems());
            }
        }
    }

}
