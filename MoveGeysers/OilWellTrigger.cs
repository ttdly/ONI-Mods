namespace MoveGeysers {
  public class OilWellTrigger : KMonoBehaviour {
    public static int OilWellCapBuild = Hash.SDBMLower("OilWellCapBuild");
    public static int OilWellCapDestory = Hash.SDBMLower("OilWellCapDestory");

    protected override void OnSpawn() {
      base.OnSpawn();
      Game.Instance.Trigger(OilWellCapBuild);
    }

    protected override void OnCleanUp() {
      base.OnCleanUp();
      Game.Instance.Trigger(OilWellCapDestory);
    }
  }
}