using PeterHan.PLib.Core;

namespace SpaceStore.SellButtons {
    public class ElementSellButton: BaseSellButton {
        [MyCmpGet]
        readonly PrimaryElement primaryElement;

        protected override void OnSpawn() {
            base.OnSpawn();
            coin = CountValue();
        }

        public override void Sell() {
            StaticVars.AddCoin(coin);
#if DEBUG
            PUtil.LogDebug($"卖出 {primaryElement.GetProperName()} {primaryElement.Units}单位，目前{StaticVars.Coin}");
#endif
            primaryElement.gameObject.DeleteObject();
            base.Sell();
        }

        public float CountValue() {
            return primaryElement.Units * GetCoinPerUnit();
        }

        private float GetCoinPerUnit() {
            if (primaryElement.HasTag(GameTags.PreciousMetal)) {
                return 0.02f;
            }
            if (primaryElement.HasTag(GameTags.RefinedMetal)) {
                return 0.05f;
            }
            return 0.0001f;
        }
    }
}
