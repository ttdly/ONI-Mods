namespace MoveGeysers {
    public class DoPost : KMonoBehaviour {
        
        protected override void OnSpawn() {
            base.OnSpawn();
            Destroy(gameObject.GetComponent<Clearable>());
            Pickupable pickupable = gameObject.GetComponent<Pickupable>();
            pickupable.SetWorkTime(10f);
        }
        
    }
}
