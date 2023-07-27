
using System;
using UnityEngine;

namespace LuckyChallenge {
    public class MinionGift : KMonoBehaviour{
        private ChoreDriver driver;

        protected override void OnSpawn() {
            base.OnSpawn();
            this.driver = this.gameObject.GetComponent<ChoreDriver>();
            this.driver.Subscribe((int)GameHashes.SleepFinished, new Action<object>(this.OnStopSleep));
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            this.driver.Unsubscribe((int)GameHashes.SleepFinished, new Action<object>(this.OnStopSleep));
        }

        private void OnStopSleep(object data) {
            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)GiftConfig.ID), Grid.CellToPos(Grid.CellAbove(Grid.PosToCell(this.gameObject))), Grid.SceneLayer.Creatures, name: this.gameObject.name);
            go.GetComponent<KBatchedAnimController>().Queue("idle_" + new System.Random().Next(1, 5));
            go.SetActive(true);
        }
    }
}
