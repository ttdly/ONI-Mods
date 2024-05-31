using KSerialization;
using STRINGS;
using UnityEngine;

namespace FragmentThermostat {
  public class BaseTransferHeat : KMonoBehaviour {
    [Serialize] public float targetTemperature = 0f;
    protected float TargetTemperature => targetTemperature + 273.15f;
    protected HandleVector<int>.Handle structureTemperature;
    private float lastSampleTime = -1f;

    protected static float CountHeat(PrimaryElement primaryElement, float targetTemp) {
      return (primaryElement.Temperature - targetTemp) * primaryElement.Element.specificHeatCapacity *
             primaryElement.Mass;
    }

    public void TransferHeat(float heat) {
      var display_dt = lastSampleTime > 0.0 ? Time.time - lastSampleTime : 1f;
      lastSampleTime = Time.time;
      GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, heat,
        BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, display_dt);
    }
  }
}