
namespace SpaceStore.SellButtons {
    internal class EntitySellButton: BaseSellButton {

        protected override void OnSpawn() {
            base.OnSpawn();
            coin = 100;
        }

        public override void Sell() {
            StaticVars.AddCoin(coin);
            gameObject.DeleteObject();
            base.Sell();
        }
    }
}
