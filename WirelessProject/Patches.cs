using HarmonyLib;
using System;
using UnityEngine;
using WirelessProject.ProwerManager;

namespace WirelessProject {
    public class Patches {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                ModUtil.AddBuildingToPlanScreen("Equipment", ProwerManager.PowerProxyConfig.ID);
                TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(ProwerManager.PowerProxyConfig.ID, "Prower Proxy");
                Db.Get().Techs.Get("AdvancedResearch").unlockedItemIDs.Add(ProwerManager.PowerProxyConfig.ID);
                GlobalVar.ProxyCircuitStatus = new StatusItem("ProxyCircuitStatus", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID).SetResolveStringCallback((Func<string, object, string>)((str, data) => {
                    PowerProxy proxy = (PowerProxy)data;
                    GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Watts;
                    if (proxy.maxWatts >= Wire.WattageRating.Max20000)
                        unit = GameUtil.WattageFormatterUnit.Kilowatts;
                    float neededWhenActive = proxy.GetWattsNeededWhenActive();
                    float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(proxy.maxWatts);
                    float wattsUsedByCircuit = proxy.wattsUsed;
                    string wireLoadColor = GameUtil.GetWireLoadColor(wattsUsedByCircuit, maxWattageAsFloat, neededWhenActive);
                    string str1 = str;
                    string newValue;
                    if (!(wireLoadColor == Color.white.ToHexString()))
                        newValue = "<color=#" + wireLoadColor + ">" + GameUtil.GetFormattedWattage(wattsUsedByCircuit, unit) + "</color>";
                    else
                        newValue = GameUtil.GetFormattedWattage(wattsUsedByCircuit, unit);
                    str = str1.Replace("{CurrentLoadAndColor}", newValue);
                    str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(maxWattageAsFloat, unit));
                    return str;
                }));

                GlobalVar.ProxyMaxWattageStatus = new StatusItem("ProxyMaxWattageStatus", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID).SetResolveStringCallback((str, data) => {
                    PowerProxy proxy = (PowerProxy)data;
                    GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Watts;
                    if (proxy.maxWatts >= Wire.WattageRating.Max20000)
                        unit = GameUtil.WattageFormatterUnit.Kilowatts;
                    float neededWhenActive = proxy.GetWattsNeededWhenActive();
                    float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(proxy.maxWatts);
                    string str2 = str;
                    string newValue;
                    if ((double)neededWhenActive <= (double)maxWattageAsFloat)
                        newValue = GameUtil.GetFormattedWattage(neededWhenActive, unit);
                    else
                        newValue = "<color=#" + new Color(0.9843137f, 0.6901961f, 0.23137255f).ToHexString() + ">" + GameUtil.GetFormattedWattage(neededWhenActive, unit) + "</color>";
                    str = str2.Replace("{TotalPotentialLoadAndColor}", newValue);
                    str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(maxWattageAsFloat, unit));
                    return str;
                });
            }
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Load))]
        public class Game_Load_Patch {
            public static void Prefix() {
                ProwerManager.GlobalVar.PowerProxiesWithCell.Clear();
            }
        }
    }
}
