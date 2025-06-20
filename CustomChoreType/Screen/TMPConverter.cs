﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using PeterHan.PLib.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomChoreType.Screen //Source: Aki
{
	// IMPORTANT: This class will use data provided from my special unity asset bundle exporter script.
	public class TMPConverter
	{
		static TMP_FontAsset NotoSans;
		static TMP_FontAsset GrayStroke;

		public TMPConverter() {
			Initialize();
		}

		public void Initialize()
		{
			var fonts = new List<TMP_FontAsset>(Resources.FindObjectsOfTypeAll<TMP_FontAsset>());
			NotoSans = fonts.FirstOrDefault(f => f.name == "NotoSans-Regular");
			GrayStroke = fonts.FirstOrDefault(f => f.name == "GRAYSTROKE REGULAR SDF");
		}

		public void ReplaceAllText(GameObject parent, bool realign = true)
		{
			var textComponents = parent.GetComponentsInChildren(typeof(Text), true);
			// PUtil.LogDebug($"Found {textComponents.Length} text components");
			foreach (Text text in textComponents)
			{
				if (text.gameObject.name == "SettingsDialogData") continue;

				string TMPData = text.text;
				GameObject obj = text.gameObject;
				TMPSettings data = ExtractTMPData(TMPData, text);

				if (data != null)
				{
					var LT = obj.AddComponent<LocText>();
					LT.font = data.Font.Contains("GRAYSTROKE") ? GrayStroke : NotoSans;
					LT.fontStyle = data.FontStyle;
					LT.fontSize = data.FontSize;
					LT.maxVisibleLines = data.MaxVisibleLines;
					LT.enableWordWrapping = data.EnableWordWrapping;
					LT.autoSizeTextContainer = data.AutoSizeTextContainer;
					LT.text = "";
					LT.color = new Color(data.Color[0], data.Color[1], data.Color[2]);
					LT.key = data.Content;
					// alignment isn't carried over instantiation, so it's applied later
					if (realign)
					{
						LT.gameObject.AddComponent<TMPFixer>().alignment = data.Alignment;
					}
				}
			}
		}
		private static TMPSettings ExtractTMPData(string TMPData, Text text)
		{
			TMPSettings data = null;

			if (isValidJSon(TMPData))
			{
				try
				{
					data = JsonConvert.DeserializeObject<TMPSettings>(TMPData);
				}
				catch (JsonReaderException e)
				{
					PUtil.LogWarning("Not valid Json format" + e);
				}

				Object.DestroyImmediate(text);
			}

			return data;
		}
		private static bool isValidJSon(string data)
		{
			if (string.IsNullOrWhiteSpace(data)) return false;
			return data.StartsWith("{") && data.EndsWith("}");
		}


	}
}
