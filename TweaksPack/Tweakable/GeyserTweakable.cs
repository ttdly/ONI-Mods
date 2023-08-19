using KSerialization;

namespace TweaksPack.Tweakable 
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class GeyserTweakable : BaseTweakable {
        [Serialize]
        public Geyser.GeyserModification modification;
        [Serialize]
        public bool hasModification = false;

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            ShowGeyserChangeDialog();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            materialNeeds = TweakableStaticVars.MaterialNeeds.Geyser;
            if (hasModification) {
                gameObject.GetComponent<Geyser>().AddModification(modification);
            }
        }

        protected override void OnFetchComplete() {
            base.OnFetchComplete();
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
