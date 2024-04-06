namespace SpaceStore.Store {
    public class SpaceStoreTool: InterfaceTool {
        public static SpaceStoreTool Instance;


        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            Instance = this;
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            Instance = null;
        }

        protected override void OnActivateTool() {
            base.OnActivateTool();
            PauseAndDisable(true);
            if (StoreDialog.DialogObj == null) {
                new StoreDialog();
            }
            StoreDialog.DialogObj.SetActive(true);
  
            //if (StoreScreen.ScreenInstance == null) { 
            //    StoreScreen.CreateScreenInstance();
            //} else {
            //    StoreScreen.DialogObject.SetActive(true);
            //}
        }

        protected override void OnDeactivateTool(InterfaceTool new_tool) {
            base.OnDeactivateTool(new_tool);
            PauseAndDisable(false);
            StoreDialog.DialogObj.TryGetComponent(out KScreen screen);
            screen?.Deactivate();
            StoreDialog.DialogObj.SetActive(false);
            //StoreScreen.DialogObject.SetActive(false);
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
