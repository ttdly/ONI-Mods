using STRINGS;

namespace TweaksPack {
    public class TweaksPackStatusItrems {
        public static StatusItem WellPressurizingAuto;

        public static void Init() {
            WellPressurizingAuto = Db.Get().BuildingStatusItems.Add(new StatusItem("WellPressurizingAuto", BUILDING.STATUSITEMS.WELL_PRESSURIZING.NAME, BUILDING.STATUSITEMS.WELL_PRESSURIZING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, status_overlays: 129022));
            WellPressurizingAuto.resolveStringCallback = delegate (string str, object data) {
                OilWellCapAuto.StatesInstance statesInstance6 = (OilWellCapAuto.StatesInstance)data;
                return (statesInstance6 != null) ? string.Format(str, GameUtil.GetFormattedPercent(100f * statesInstance6.GetPressurePercent())) : str;
            };
        }
    }
}
