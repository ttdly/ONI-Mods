using KSerialization;

namespace TweaksPack.Tweakable {
  [SerializationConfig(MemberSerialization.OptIn)]
  public class GeyserTweakable : BaseTweakable {
    protected override void OnCompleteWork(Worker worker) {
      base.OnCompleteWork(worker);
      ShowGeyserChangeDialog();
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      SetWorkTime(TweakableStaticVars.WorkTime.Geyser);
    }

    private void ShowGeyserChangeDialog() {
      SpeedControlScreen.Instance?.Pause(false);
      if (!(CameraController.Instance != null))
        return;
      CameraController.Instance.DisableUserCameraControl = true;
      new GeyserChangeDialog(gameObject);
    }
  }
}