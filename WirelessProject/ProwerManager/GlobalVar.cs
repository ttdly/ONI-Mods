using FMOD;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static STRINGS.UI.OUTFITS;

namespace WirelessProject.ProwerManager {
    public class GlobalVar {
        public static Tag HasProxy = new Tag("Project_HasProxy");
        public static List<PowerProxy> PowerProxies = new List<PowerProxy>();
        public static List<ushort> IDs = new List<ushort>();
        public static StatusItem ProxyCircuitStatus;
        public static StatusItem ProxyMaxWattageStatus;
        //public static StatusItem ProxyCircuitStatus = new StatusItem("ProxyCircuitStatus", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID).SetResolveStringCallback((Func<string, object, string>) ((str, data) =>{
        //    PowerProxy proxy = (PowerProxy)data;
        //    GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Watts;
        //    if (proxy.maxWatts >= Wire.WattageRating.Max20000)
        //        unit = GameUtil.WattageFormatterUnit.Kilowatts;
        //    float neededWhenActive = proxy.GetWattsNeededWhenActive();
        //    float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(proxy.maxWatts);
        //    float wattsUsedByCircuit = proxy.wattsUsed;
        //    string wireLoadColor = GameUtil.GetWireLoadColor(wattsUsedByCircuit, maxWattageAsFloat, neededWhenActive);
        //    string str1 = str;
        //    string newValue;
        //    if (!(wireLoadColor == Color.white.ToHexString()))
        //        newValue = "<color=#" + wireLoadColor + ">" + GameUtil.GetFormattedWattage(wattsUsedByCircuit, unit) + "</color>";
        //    else
        //        newValue = GameUtil.GetFormattedWattage(wattsUsedByCircuit, unit);
        //    str = str1.Replace("{CurrentLoadAndColor}", newValue);
        //    str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(maxWattageAsFloat, unit));
        //    return str;
        //}));

        //public static StatusItem ProxyMaxWattageStatus = new StatusItem("ProxyMaxWattageStatus", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID).SetResolveStringCallback((str, data) =>{
        //    PowerProxy proxy = (PowerProxy)data;
        //    GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Watts;
        //    if (proxy.maxWatts >= Wire.WattageRating.Max20000)
        //        unit = GameUtil.WattageFormatterUnit.Kilowatts;
        //    float neededWhenActive = proxy.GetWattsNeededWhenActive();
        //    float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(proxy.maxWatts);
        //    string str2 = str;
        //    string newValue;
        //    if ((double)neededWhenActive <= (double)maxWattageAsFloat)
        //        newValue = GameUtil.GetFormattedWattage(neededWhenActive, unit);
        //    else
        //        newValue = "<color=#" + new Color(0.9843137f, 0.6901961f, 0.23137255f).ToHexString() + ">" + GameUtil.GetFormattedWattage(neededWhenActive, unit) + "</color>";
        //    str = str2.Replace("{TotalPotentialLoadAndColor}", newValue);
        //    str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(maxWattageAsFloat, unit));
        //    return str;
        //});
        //public static Operational.Flag wireConnectedFlag = null;

        //public static void InitFlagMy() {
        //    FieldInfo field = typeof(Generator).GetField("wireConnectedFlag", BindingFlags.Static | BindingFlags.NonPublic);
        //    Operational.Flag flagValue = (Operational.Flag)field.GetValue(null);
        //    wireConnectedFlag = flagValue;
        //}
    }
}
