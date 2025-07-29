using UnityEngine;

namespace PipStore.Screen.Basic {
    public class DialogCreator {
        
        /// <summary>
        ///     Credit: Aki
        /// </summary>
        public static T CreateFDialog<T>(GameObject prefab, string name = null, bool show = true)
            where T : PipStoreScreen {
            if (prefab == null) {
                LogUtil.Warning($"Could not display UI ({name}): screen prefab is null.");
                return null;
            }

            if (name == null) name = prefab.name;

            var parent = GetACanvas(name).transform;

            var gameObject = Object.Instantiate(prefab, parent);


            var screen = gameObject.AddComponent(typeof(T)) as T;

            if (show) {
                gameObject.SetActive(true);
                // screen.ShowDialog();
            }

            return screen;
        }

        /// <summary>
        ///     Credit: Aki
        /// </summary>
        private static Canvas GetACanvas(string name) {
            GameObject parent;
            if (FrontEndManager.Instance != null) {
                parent = FrontEndManager.Instance.gameObject;
            }
            else {
                if (GameScreenManager.Instance != null && GameScreenManager.Instance.ssOverlayCanvas != null) {
                    parent = GameScreenManager.Instance.ssOverlayCanvas;
                }
                else {
                    parent = new GameObject {
                        name = name + "Canvas"
                    };
                    Object.DontDestroyOnLoad(parent);
                    var canvas = parent.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
                    canvas.sortingOrder = 3000;
                }
            }

            return parent.GetComponent<Canvas>();
        }
    }
}