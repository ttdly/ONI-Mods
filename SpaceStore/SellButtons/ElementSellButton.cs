
namespace SpaceStore.SellButtons {
    public class ElementSellButton: BaseSellButton {
        [MyCmpGet]
        readonly PrimaryElement primaryElement;
        [MyCmpGet]
        public readonly Pickupable pickupable;

        protected override void OnSpawn() {
            base.OnSpawn();
            StaticVars.Buttons.Add(this);
        }

        public override void Sell() {
            if (coin == 0) { CountPrice(); }
            if (coin < 0) { return; }
            StaticVars.AddCoin(coin);
            primaryElement.gameObject.DeleteObject();
            base.Sell();
        }

        public override void CountPrice() {
            coin = primaryElement.Units * GetCoinPerUnit();
        }

        private float GetCoinPerUnit() {
            PriceConvter.Instance.sellItems.TryGetValue(gameObject.PrefabID() ,out float price);
            return price;
        }
    }
}
