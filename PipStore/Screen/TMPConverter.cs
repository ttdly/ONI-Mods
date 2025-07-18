using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PipStore.Screen //Source: Aki
{
    // IMPORTANT: This class will use data provided from my special unity asset bundle exporter script.
    public class TMPConverter {
        private static TMP_FontAsset _notoSans;
        private static TMP_FontAsset _grayStroke;

        public TMPConverter() {
            Initialize();
        }

        private static void Initialize() {
            var fonts = new List<TMP_FontAsset>(Resources.FindObjectsOfTypeAll<TMP_FontAsset>());
            _notoSans = fonts.FirstOrDefault(f => f.name == "NotoSans-Regular");
            _grayStroke = fonts.FirstOrDefault(f => f.name == "GRAYSTROKE REGULAR SDF");
        }

        public void ReplaceAllText(GameObject parent, bool realign = true) {
            var textComponents = parent.GetComponentsInChildren(typeof(Text), true);

            foreach (Text text in textComponents) {
                if (text.gameObject.name == "SettingsDialogData") continue;

                var TMPData = text.text;
                var obj = text.gameObject;
                var data = ExtractTMPData(TMPData, text);

                if (data != null) {
                    var LT = obj.AddComponent<LocText>();
                    LT.font = data.Font.Contains("GRAYSTROKE") ? _grayStroke : _notoSans;
                    LT.fontStyle = data.FontStyle;
                    LT.fontSize = data.FontSize;
                    LT.maxVisibleLines = data.MaxVisibleLines;
                    LT.enableWordWrapping = data.EnableWordWrapping;
                    LT.autoSizeTextContainer = data.AutoSizeTextContainer;
                    LT.text = "";
                    LT.color = new Color(data.Color[0], data.Color[1], data.Color[2]);
                    LT.key = data.Content;
                    // alignment isn't carried over instantiation, so it's applied later
                    if (realign) LT.gameObject.AddComponent<TMPFixer>().alignment = data.Alignment;
                }
            }
        }

        private static TMPSettings ExtractTMPData(string TMPData, Text text) {
            TMPSettings data = null;

            if (isValidJSon(TMPData)) {
                try {
                    data = JsonConvert.DeserializeObject<TMPSettings>(TMPData);
                }
                catch (JsonReaderException e) {
                    LogUtil.Warning("Not valid Json format" + e);
                }

                Object.DestroyImmediate(text);
            }

            return data;
        }

        private static bool isValidJSon(string data) {
            if (string.IsNullOrWhiteSpace(data)) return false;
            return data.StartsWith("{") && data.EndsWith("}");
        }
    }
}