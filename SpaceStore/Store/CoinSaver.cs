using KSerialization;

namespace SpaceStore.Store {
    public class CoinSaver :KMonoBehaviour{
        [Serialize]
        public float coin = 0;
        [Serialize]
        public bool sync = true;

        protected override void OnSpawn() {
            base.OnSpawn();
            if (sync) {
                StoreData storeData = SaveGame.Instance.gameObject.AddOrGet<StoreData>();
                if (storeData != null && storeData.coin != 0) {
                    coin = storeData.coin;
                }
                sync = false;
            }
            StaticVars.coinSaver = this;
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            StaticVars.coinSaver = null;
        }

        public void AddCoin(float amount) { 
            coin += amount;
        }
    }
}
