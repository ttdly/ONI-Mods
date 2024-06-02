namespace SpaceStore.SellButtons {
  public class ElementSellButton : BaseSellButton {
    [MyCmpGet] public readonly Pickupable pickupable;

    [MyCmpGet] private readonly PrimaryElement primaryElement;

    protected override void OnSpawn() {
      base.OnSpawn();
      StaticVars.Buttons.Add(this);
    }

    public override void Sell() {
      if (StaticVars.coinSaver == null) return;
      if (coin == 0) CountPrice();
      if (coin < 0) return;
      //StaticVars.AddCoin(coin);
      StaticVars.coinSaver.AddCoin(coin);
      primaryElement.gameObject.DeleteObject();
      base.Sell();
    }

    public override void CountPrice() {
      coin = primaryElement.Units * GetCoinPerUnit();
    }

    private float GetCoinPerUnit() {
      PriceConvter.Instance.sellItems.TryGetValue(gameObject.PrefabID(), out var price);
      return price;
    }
  }
}