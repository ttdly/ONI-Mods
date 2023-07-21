using System;
using System.IO;
using System.Reflection;

namespace PackAnything {
    public static class Utils {
        public static void Localize(Type root) {
            ModUtil.RegisterForTranslation(root);
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string name = executingAssembly.GetName().Name;
            string path = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "translations");
            Localization.Locale locale = Localization.GetLocale();
            if (locale != null) {
                try {
                    string text = Path.Combine(path, locale.Code + ".po");
                    Debug.LogWarning(name + " lang file: " + text);
                    if (File.Exists(text)) {
                        Debug.Log(name + ": Localize file found " + text);
                        Localization.OverloadStrings(Localization.LoadStringsFile(text, false));
                    }
                }
                catch {
                    Debug.LogWarning(name + " Failed to load localization.");
                }
            }
            LocString.CreateLocStringKeys(root, "");
        }
    }
}
