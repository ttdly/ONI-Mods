namespace LuckyChallenge {
  public class MinionGift : KMonoBehaviour {
    private ChoreDriver driver;

    protected override void OnSpawn() {
      base.OnSpawn();
      driver = gameObject.GetComponent<ChoreDriver>();
      driver.Subscribe((int)GameHashes.SleepFinished, OnStopSleep);
    }

    protected override void OnCleanUp() {
      base.OnCleanUp();
      driver.Unsubscribe((int)GameHashes.SleepFinished, OnStopSleep);
    }

    private void OnStopSleep(object data) {
      var position = Grid.CellToPos(Grid.CellAbove(Grid.PosToCell(gameObject)));
      var go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)GiftConfig.ID), position, Grid.SceneLayer.Move,
        gameObject.name);
      var gift = go.GetComponent<Gift>();
      gift.count = 2;
      gift.anim = "normal_min";
      go.SetActive(true);
    }
  }
}