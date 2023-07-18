

namespace MoreMovable {
    public class RemoveClearable : KMonoBehaviour {
        protected override void OnSpawn() {
            base.OnSpawn();
            Destroy(gameObject.GetComponent<Clearable>());
        }
    }
}
