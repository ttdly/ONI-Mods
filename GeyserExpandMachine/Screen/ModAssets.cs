using System.IO;
using System.Linq;
using System.Reflection;
using PeterHan.PLib.Core;
using UnityEngine;
using UtilLibs.UIcmp;

namespace GeyserExpandMachine.Screen {
    public class ModAssets {
        public static GameObject ExpandSideSecondScreenPrefab;


        public static void LoadAssets() {
            var bundle = LoadAssetBundle("canvas", platformSpecific: true);
            ExpandSideSecondScreenPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/GeyserExpandSideScreen.prefab");
            // PUtil.LogDebug($"bundle == null : {bundle == null}:{ExpandSideSecondScreenPrefab == null}:{bundle.GetAllAssetNames().Length}");
            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(ExpandSideSecondScreenPrefab);
            // PUtil.LogDebug($"Loaded geyser_expand_ui.prefab {ExpandSideSecondScreenPrefab == null}");
            // ListChildren(ExpandSideSecondScreenPrefab.transform, 0, 10);
        }
        /// <summary>
        /// Credit: Sgt_Imalas
        /// </summary>
        private static AssetBundle LoadAssetBundle(string assetBundleName, string path = null, bool platformSpecific = false)
        {
            foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles())
            {
                if (bundle.name == assetBundleName)
                {
                    return bundle;
                }
            }

            if (path.IsNullOrWhiteSpace())
            {
                path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "assets");
            }

            if (platformSpecific)
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsPlayer:
                        path = Path.Combine(path, "windows");
                        break;
                    case RuntimePlatform.LinuxPlayer:
                        path = Path.Combine(path, "linux");
                        break;
                    case RuntimePlatform.OSXPlayer:
                        path = Path.Combine(path, "mac");
                        break;
                }
            }

            path = Path.Combine(path, assetBundleName);

            var assetBundle = AssetBundle.LoadFromFile(path);

            if (assetBundle == null)
            {
                PUtil.LogWarning($"Failed to load AssetBundle from path {path}");
                return null;
            }

            return assetBundle;
        }
        /// <summary>
        /// Credit: Sgt_Imalas
        /// </summary>
        public static void ListChildren(Transform parent, int level = 0, int maxDepth = 10)
        {
            if (level >= maxDepth) return;

            foreach (Transform child in parent)
            {
                PUtil.LogDebug(string.Concat(Enumerable.Repeat('-', level)) + child.name);
                ListChildren(child, level + 1);
            }
        }
    }
}