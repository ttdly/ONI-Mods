using KSerialization;
using UnityEngine;

namespace PackAnything {
    [SerializationConfig(MemberSerialization.OptIn)]
    public class MagicPack : KMonoBehaviour{
        public GameObject storedObject;
        [Serialize]
        public bool isGeyser = false;
        [Serialize]
        public int originCell;

        protected override void OnSpawn() {
            base.OnSpawn();
            if (storedObject != null) {
                storedObject.SetActive(false);
                storedObject.FindOrAddComponent<OccupyArea>().ApplyToCells = false;
            }
        }
    }
}
