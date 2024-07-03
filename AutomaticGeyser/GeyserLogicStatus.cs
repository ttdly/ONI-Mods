namespace AutomaticGeyser {
  public static class GeyserLogicStatus {
    public enum InputLogic {
      SkipErupt = 1,
      SkipDormant = 2,
      AlwaysDormant = 4,
      UnusedCase = -1
    }

    public static InputLogic GetInputLogic(int logicValue) {
      switch (logicValue) {
        case 1:
          return InputLogic.SkipErupt;
        case 2:
          return InputLogic.SkipDormant;
        case 4:
          return InputLogic.AlwaysDormant;
        default:
          return InputLogic.UnusedCase;
      }
    }

    public enum OutputLogic {
      Dormant = 0,
      PreErupt = 1,
      Erupting = 2,
      PostErupt = 4,
      OverPressure = 8
    }

    public static int GetLogicTrueValue(OutputLogic logic) {
      return (int)logic;
    }

    public static StatusItem SkipEruptStatusItem = new StatusItem(nameof(SkipEruptStatusItem), "GEYSER", "",
      StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID).SetResolveStringCallback(
      (str, data) => {
        var geyserLogic = (GeyserLogic)data;
        return str.Replace("{0}", geyserLogic.skipEruptTimes.ToString());
      });

    public static StatusItem SkipDormantStatusItem = new StatusItem(nameof(SkipDormantStatusItem), "GEYSER", "",
      StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID).SetResolveStringCallback(
      (str, data) => {
        var geyserLogic = (GeyserLogic)data;
        return str.Replace("{0}", geyserLogic.skipEruptTimes.ToString());
      });

    public static StatusItem AlwaysDormant = new StatusItem(nameof(AlwaysDormant), "GEYSER", "",
      StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
  }
}