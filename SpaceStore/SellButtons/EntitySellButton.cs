namespace SpaceStore.SellButtons {
    internal class EntitySellButton: BaseSellButton {
        protected override void OnSpawn() {
            base.OnSpawn();
            if (coin == 0) coin = 100;
        }

        public override void Sell() {
            StaticVars.AddCoin(coin);
            base.Sell();
            gameObject.DeleteObject();
            
        }
    }
}
