using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PipStore.Screen {
    public class ModAssets {
        public static GameObject PipStoreScreenPrefab;
        
        
        public static void LoadAssets() {
            var bundle = LoadAssetBundle("pipsqueakstore", platformSpecific: true);
            var prefab = bundle.LoadAsset<GameObject>("Assets/UIs/PipStore.prefab");
            PipStoreScreenPrefab = prefab;

            var tmPConverter = new TMPConverter();
            tmPConverter.ReplaceAllText(prefab);
            ListChildren(PipStoreScreenPrefab.transform);
        }

        /// <summary>
        ///     Credit: Sgt_Imalas
        /// </summary>
        private static AssetBundle LoadAssetBundle(string assetBundleName, string path = null,
            bool platformSpecific = false) {
            foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles())
                if (bundle.name == assetBundleName)
                    return bundle;

            if (path.IsNullOrWhiteSpace()) {
                var exec = Assembly.GetExecutingAssembly().Location;
                var execDir = Path.GetDirectoryName(exec);
                if (execDir == null) throw new Exception($"Could not find executable at {exec}.");

                path = Path.Combine(execDir, "assets");
            }

            if (platformSpecific) {
                if (path == null) throw new Exception($"Could not find asset dir at {assetBundleName}.");
                switch (Application.platform) {
                    case RuntimePlatform.WindowsPlayer:
                        path = Path.Combine(path, "windows");
                        break;
                    case RuntimePlatform.LinuxPlayer:
                        path = Path.Combine(path, "linux");
                        break;
                    case RuntimePlatform.OSXPlayer:
                        path = Path.Combine(path, "mac");
                        break;
                    default:
                        throw new Exception($"Not supported platform: {Application.platform}");
                }
            }

            if (path == null) throw new Exception($"Could not find platform dir at {path}.");

            path = Path.Combine(path, assetBundleName);

            var assetBundle = AssetBundle.LoadFromFile(path);

            if (assetBundle != null) return assetBundle;

            LogUtil.Warning($"Failed to load AssetBundle from path {path}");
            return null;
        }

        /// <summary>
        ///     Credit: Sgt_Imalas
        /// </summary>
        public static void ListChildren(Transform parent, int level = 0, int maxDepth = 10) {
            if (level >= maxDepth) return;

            foreach (Transform child in parent) {
                LogUtil.Info(string.Concat(Enumerable.Repeat('-', level)) + child.name);
                ListChildren(child, level + 1);
            }
        }
    }
}