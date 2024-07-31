namespace SpaceStore.SellButtons {
  internal class EntitySellButton : BaseSellButton {
    protected override void OnSpawn() {
      base.OnSpawn();
      if (coin == 0) coin = 100;
    }

    public override void Sell() {
      //StaticVars.AddCoin(coin);
      if (StaticVars.coinSaver == null) return;
      StaticVars.coinSaver.AddCoin(coin);
      base.Sell();
      NeutroniumMover.Delete(Grid.PosToCell(gameObject.transform.position));
      gameObject.DeleteObject();
    }
  }
}