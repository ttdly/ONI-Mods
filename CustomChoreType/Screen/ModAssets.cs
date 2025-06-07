using System.IO;
using System.Linq;
using System.Reflection;
using PeterHan.PLib.Core;
using UnityEngine;

namespace CustomChoreType.Screen {
    public class ModAssets {
        public static GameObject CustomChoreTypeDialog;
        
        public static void LoadAssets() {
            var bundle = LoadAssetBundle("custom_chore_group", platformSpecific: true);
            CustomChoreTypeDialog = bundle.LoadAsset<GameObject>("Assets/UIs/CustomChoreGroup.prefab");

            var tmPConverter = new TMPConverter();
            tmPConverter.ReplaceAllText(CustomChoreTypeDialog);
            // ListChildren(CustomChoreTypeDialog.transform);
            CustomChoreTypeScreen.CustomChoreTypePrefab = CustomChoreTypeDialog;

        }

        public static void InitDialog() {
            
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