using PeterHan.PLib.Core;
using System.Collections.Generic;


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
#if DEBUG
            PUtil.LogDebug($"卖出 {primaryElement.GetProperName()} {primaryElement.Units}单位，目前{StaticVars.Coin}");
#endif
            primaryElement.gameObject.DeleteObject();
            base.Sell();
        }

        public override void CountPrice() {
            coin = primaryElement.Units * GetCoinPerUnit();
        }

        private float GetCoinPerUnit() {
            if (PriceConvter.Instance == null) {
                new PriceConvter();
            }

            foreach(KeyValuePair<Tag, float> tagAndPrice in PriceConvter.Instance.sellItems) {
                if (gameObject.HasTag(tagAndPrice.Key)) {
                    return tagAndPrice.Value;
                }
            }
            return 0.0001f;
        }
    }
}
