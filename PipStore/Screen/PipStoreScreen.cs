namespace PipStore.Screen {
    public class PipStoreScreen : FScreen {
        public static PipStoreScreen Instance;

        public static void ShowWindow()
        {
            if (Instance == null)
            {
                LogUtil.Info($"{ModAssets.PipStoreScreenPrefab == null}--{PauseScreen.Instance.transform.parent.gameObject}");
                var screen = Util.KInstantiateUI(ModAssets.PipStoreScreenPrefab,
                    PauseScreen.Instance.transform.parent.gameObject, true);
                Instance = screen.AddOrGet<PipStoreScreen>();
                LogUtil.Info("初始化窗口");
                Instance.Init();
            }

            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
        }

        public void Init() {
            
        }
    }
}