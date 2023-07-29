
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
            Vector3 position = Grid.CellToPos(Grid.CellAbove(Grid.PosToCell(this.gameObject)));
            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)GiftConfig.ID), position, Grid.SceneLayer.Move, name: this.gameObject.name);
            Gift gift = go.GetComponent<Gift>();
            gift.count = 2;
            gift.anim = "normal_min";
            go.SetActive(true);
        }
    }
}
