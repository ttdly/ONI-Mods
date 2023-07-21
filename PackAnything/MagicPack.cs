using UnityEngine;

namespace PackAnything {
    public class MagicPack : KMonoBehaviour{
        public GameObject storedObject;
        public bool isGeyser = false;

        protected override void OnSpawn() {
            if (storedObject != null) {
                this.gameObject.name = storedObject.name;
            }
        }
    }
}
