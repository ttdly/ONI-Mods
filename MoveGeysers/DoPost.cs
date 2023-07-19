namespace MoveGeysers {
    public class DoPost : KMonoBehaviour {
        
        protected override void OnSpawn() {
            base.OnSpawn();
            gameObject.GetComponent<Clearable>().isClearable = false;
            Pickupable pickupable = gameObject.GetComponent<Pickupable>();
            pickupable.SetWorkTime(10f);
        }
        
    }
}
