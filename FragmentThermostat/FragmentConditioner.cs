using KSerialization;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using STRINGS;
using UnityEngine;

namespace FragmentThermostat {
#pragma warning disable CS0649
  public class FragmentConditioner : BaseTransferHeat {
    [MyCmpGet] private readonly Operational operational;
    [MyCmpGet] private readonly SolidConduitBridge solidConduitBridge;

    protected override void OnSpawn() {
      base.OnSpawn();
      targetTemperature = SingletonOptions<ModOptions>.Instance.Mode2Temp;
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

      if (pickupable.PrimaryElement.Temperature <= TargetTemperature) return;
      var temp = pickupable.PrimaryElement.Temperature - 14;
      TransferHeat(CountHeat(pickupable.PrimaryElement, temp));
      pickupable.PrimaryElement.Temperature = temp;
    }
  }
}