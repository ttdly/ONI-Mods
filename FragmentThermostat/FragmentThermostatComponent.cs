using KSerialization;
using PeterHan.PLib.Options;
using STRINGS;
using UnityEngine;

namespace FragmentThermostat {
#pragma warning disable CS0649
  public class FragmentThermostatComponent : KMonoBehaviour, ISingleSliderControl {
    [Serialize] public float targetTemperature = 0f;
    [MyCmpGet] private readonly Operational operational;
    [MyCmpGet] private readonly SolidConduitBridge solidConduitBridge;
    private float TargetTemperature => targetTemperature + 273.15f;
    private HandleVector<int>.Handle structureTemperature;
    private float lastSampleTime = -1f;
    
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
      Pickupable pickupable){
      if (pickupable == null) {
        operational.SetActive(false);
        return;
      }
      TransferHeat(CountHeat(pickupable.PrimaryElement));
      operational.SetActive(solidConduitBridge.IsDispensing);
      pickupable.PrimaryElement.Temperature = TargetTemperature;
    }
    
    private float CountHeat(PrimaryElement primaryElement) {
      return (primaryElement.Temperature - TargetTemperature) * primaryElement.Element.specificHeatCapacity *
             primaryElement.Mass;
    }

    private void TransferHeat(float heat) {
      var display_dt = lastSampleTime > 0.0 ? Time.time - lastSampleTime : 1f;
      lastSampleTime = Time.time;
      GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, heat,
        BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, display_dt);
    }

    #region Slider
    public string SliderTitleKey => "STRINGS.UI_FTMOD.TITLE";

    public string SliderUnits => UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;

    public int SliderDecimalPlaces(int index) => 0;

    public float GetSliderMin(int index) => SingletonOptions<ModOptions>.Instance.MinTemperature;

    public float GetSliderMax(int index) => SingletonOptions<ModOptions>.Instance.MaxTemperature;

    public float GetSliderValue(int index) => targetTemperature;

    public void SetSliderValue(float percent, int index) => targetTemperature = percent;

    public string GetSliderTooltipKey(int index) => "STRINGS.UI_FTMOD.TITLE";

    public string GetSliderTooltip(int index) => $"{targetTemperature}";
    #endregion
  }
}