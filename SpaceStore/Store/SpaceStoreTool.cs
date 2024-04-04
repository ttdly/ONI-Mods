namespace SpaceStore.Store {
    public class SpaceStoreTool: InterfaceTool {
        public static void DestroyInstance() {
            StoreScreen.ScreenInstance = null;
        }

        protected override void OnActivateTool() {
            base.OnActivateTool();
            PauseAndDisable(true);
            if (StoreScreen.ScreenInstance == null) { 
                StoreScreen.CreateScreenInstance();
            } else {
                StoreScreen.ScreenInstance.Show();
            }
        }

        protected override void OnDeactivateTool(InterfaceTool new_tool) {
            base.OnDeactivateTool(new_tool);
            PauseAndDisable(false);
        }

        private void PauseAndDisable(bool show) {
            if (SpeedControlScreen.Instance != null) {
                if (show)
                    SpeedControlScreen.Instance.Pause(false);
                else if (!show)
                    SpeedControlScreen.Instance.Unpause(false);
            }
            if (!(CameraController.Instance != null))
                return;
            CameraController.Instance.DisableUserCameraControl = show;
        }
    }
}
