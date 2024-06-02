using PeterHan.PLib.Options;
using STRINGS;

namespace FragmentThermostat {
#pragma warning disable CS0649
  public class FragmentThermostatComponent : BaseTransferHeat, ISingleSliderControl {
    [MyCmpGet] private readonly Operational operational;
    [MyCmpGet] private readonly SolidConduitBridge solidConduitBridge;

    protected override void OnSpawn() {
      base.OnSpawn();
      structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
      solidConduitBridge.OnMassTransfer += ConduitBridgeEvent;
    }

    private void ConduitBridgeEvent(
      SimHashes element,
      float mass,
      float temperature,
      byte disease_idx,
      int disease_count,
      Pickupable pickupable) {
      if (pickupable == null) {
        operational.SetActive(false);
        return;
      }

      TransferHeat(CountHeat(pickupable.PrimaryElement, TargetTemperature));
      operational.SetActive(solidConduitBridge.IsDispensing);
      pickupable.PrimaryElement.Temperature = TargetTemperature;
    }

    #region Slider

    public string SliderTitleKey => "STRINGS.UI_FTMOD.TITLE";

    public string SliderUnits => UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;

    public int SliderDecimalPlaces(int index) {
      return 0;
    }

    public float GetSliderMin(int index) {
      return SingletonOptions<ModOptions>.Instance.MinTemperature;
    }

    public float GetSliderMax(int index) {
      return SingletonOptions<ModOptions>.Instance.MaxTemperature;
    }

    public float GetSliderValue(int index) {
      return targetTemperature;
    }

    public void SetSliderValue(float percent, int index) {
      targetTemperature = percent;
    }

    public string GetSliderTooltipKey(int index) {
      return "STRINGS.UI_FTMOD.TITLE";
    }

    public string GetSliderTooltip(int index) {
      return $"{targetTemperature}";
    }

    #endregion
  }
}