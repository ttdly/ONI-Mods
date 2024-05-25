using KSerialization;
using STRINGS;
using UnityEngine;

namespace FragmentThermostat {
#pragma warning disable CS0649
  public class FragmentThermostatComponent : KMonoBehaviour, ISingleSliderControl {
    [Serialize] public float targetTemperature = 0f;
    [MyCmpGet] private readonly Operational operational;
    private float TargetTemperature => targetTemperature + 273.15f;
    private HandleVector<int>.Handle structureTemperature;
    private float lastSampleTime = -1f;

    private static readonly EventSystem.IntraObjectHandler<FragmentThermostatComponent> OnstorageChange =
      new EventSystem.IntraObjectHandler<FragmentThermostatComponent>(
        (component, data) => component.StorageChange(data));

    public string SliderTitleKey => "STRINGS.UI_FTMOD.TITLE";

    public string SliderUnits => UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;

    protected override void OnSpawn() {
      base.OnSpawn();
      Subscribe((int)GameHashes.OnStorageChange, OnstorageChange);
      structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
    }

    private void StorageChange(object data) {
      var go = data as GameObject;
      if (go == null) return;
      var primaryElement = go.GetComponent<PrimaryElement>();
      if (primaryElement == null || primaryElement.Mass == 0) return;
      TransferHeat(CountHeat(primaryElement));
      primaryElement.Temperature = TargetTemperature;
      operational.SetActive(operational.IsOperational);
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

    public int SliderDecimalPlaces(int index) => 0;

    public float GetSliderMin(int index) => -20f;

    public float GetSliderMax(int index) => 30f;

    public float GetSliderValue(int index) => targetTemperature;

    public void SetSliderValue(float percent, int index) => targetTemperature = percent;

    public string GetSliderTooltipKey(int index) => "STRINGS.UI_FTMOD.TITLE";

    public string GetSliderTooltip(int index) => $"{targetTemperature}";
  }
}