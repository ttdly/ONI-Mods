namespace FixCrashBMF {
  public class Beacon : KMonoBehaviour {
    protected override void OnSpawn() {
      base.OnSpawn();
      var kBatchedAnimController = gameObject.GetComponent<KBatchedAnimController>();
      kBatchedAnimController.Play("destroy");
      kBatchedAnimController.destroyOnAnimComplete = true;
    }
  }
}