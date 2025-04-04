using KSerialization;
using PeterHan.PLib.Options;
using STRINGS;
using UnityEngine;

namespace FragmentThermostat {
  public class BaseTransferHeat : KMonoBehaviour {
    [Serialize] public float targetTemperature;
    private float lastSampleTime = -1f;
    protected HandleVector<int>.Handle structureTemperature;
    protected float TargetTemperature => targetTemperature + 273.15f;

    protected static float CountHeat(PrimaryElement primaryElement, float targetTemp) {
      return (primaryElement.Temperature - targetTemp) * primaryElement.Element.specificHeatCapacity *
             primaryElement.Mass * SingletonOptions<ModOptions>.Instance.HeatMultiply;
    }

    public void TransferHeat(float heat) {
      var display_dt = lastSampleTime > 0.0 ? Time.time - lastSampleTime : 1f;
      lastSampleTime = Time.time;
      GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, heat,
        BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, display_dt);
    }
  }
}