using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using static CircuitManager;
using static WirelessProject.ProwerManager.StaticVar;


namespace WirelessProject.ProwerManager {
  public class PowerProxy : KMonoBehaviour, ISim200ms {
    public float wattsUsed;
    public Wire.WattageRating maxWatts = Wire.WattageRating.Max20000;
    public int WorldId;
    private readonly List<Generator> activeGenerators = new List<Generator>();

    [MyCmpGet] private readonly Building building;

    [MyCmpGet] private readonly Operational operational;

    private float elapsedTime;
    public ProxyList proxyList;

    public float GetWattsNeededWhenActive() {
      var num = 0f;
      foreach (var energyConsumer in proxyList.energyConsumers) num += energyConsumer.WattsNeededWhenActive;
      return num;
    }

    private void ClearProxy(GameObject go) {
      go.GetComponent<BaseLinkToProxy>().RemoveThisFromProxy();
    }

    public class ProxyList {
      public readonly List<Battery> batteries = new List<Battery>();
      public readonly List<EnergyConsumer> energyConsumers = new List<EnergyConsumer>();
      public readonly List<Generator> generators = new List<Generator>();
      public PowerProxy proxy;
      public int ProxyInfoId;
      public string ProxyName;

      #region DisOrConnect

      public int Connect(Generator generator) {
        if (!Game.IsQuitting()) {
          Game.Instance.energySim.RemoveGenerator(generator);
          Game.Instance.circuitManager.Disconnect(generator);
          generators.Add(generator);
          return ProxyInfoId;
        }

        return -1;
      }

      public void Disconnect(Generator generator) {
        if (!Game.IsQuitting()) {
          generators.Remove(generator);
          Game.Instance.circuitManager.Connect(generator);
          Game.Instance.energySim.AddGenerator(generator);
        }
      }

      public int Connect(Battery battery) {
        if (!Game.IsQuitting()) {
          Game.Instance.energySim.RemoveBattery(battery);
          batteries.Add(battery);
          return ProxyInfoId;
        }

        return -1;
      }

      public void Disconnect(Battery battery) {
        if (!Game.IsQuitting()) {
          batteries.Remove(battery);
          Game.Instance.energySim.AddBattery(battery);
        }
      }

      public int Connect(EnergyConsumer consumer) {
        if (!Game.IsQuitting()) {
          Game.Instance.energySim.RemoveEnergyConsumer(consumer);
          Game.Instance.circuitManager.Disconnect(consumer, true);
          energyConsumers.Add(consumer);
          return ProxyInfoId;
        }

        return -1;
      }

      public void Disconnect(EnergyConsumer consumer, bool isDestroy) {
        if (!Game.IsQuitting()) {
          energyConsumers.Remove(consumer);
          if (!isDestroy) consumer.SetConnectionStatus(ConnectionStatus.NotConnected);
          Game.Instance.circuitManager.Connect(consumer);
          Game.Instance.energySim.AddEnergyConsumer(consumer);
        }
      }

      #endregion

      #region AddOrRemove

      public void Remove(EnergyConsumer consumer) {
        energyConsumers.Remove(consumer);
      }

      public void Remove(Generator generator) {
        generators.Remove(generator);
      }

      public void Remove(Battery battery) {
        batteries.Remove(battery);
      }

      public int Add(EnergyConsumer consumer) {
        if (!Game.IsQuitting()) {
          energyConsumers.Add(consumer);
          return ProxyInfoId;
        }

        return -1;
      }

      public int Add(Generator generator) {
        if (!Game.IsQuitting()) {
          generators.Add(generator);
          return ProxyInfoId;
        }

        return -1;
      }

      public int Add(Battery battery) {
        if (!Game.IsQuitting()) {
          batteries.Add(battery);
          return ProxyInfoId;
        }

        return -1;
      }

      #endregion
    }

    #region LifeCycle

    protected override void OnSpawn() {
      base.OnSpawn();
      WorldId = gameObject.GetMyWorldId();
      PowerInfoList.TryGetValue(WorldId, out var proxyList);
      if (proxyList != null) {
        if (proxyList.proxy == null) {
          this.proxyList = proxyList;
          this.proxyList.proxy = this;
        }
      } else {
        var new_proxyList = new ProxyList {
          ProxyInfoId = WorldId,
          proxy = this
        };
        PowerInfoList.Add(WorldId, new_proxyList);
        this.proxyList = new_proxyList;
      }
    }

    protected override void OnCleanUp() {
      if (proxyList == null) return;
      while (proxyList.generators.Count > 0) ClearProxy(proxyList.generators[0].gameObject);
      while (proxyList.batteries.Count > 0) ClearProxy(proxyList.batteries[0].gameObject);
      while (proxyList.energyConsumers.Count > 0) ClearProxy(proxyList.energyConsumers[0].gameObject);
      PowerInfoList.Remove(WorldId);
      base.OnCleanUp();
    }

    #endregion

    #region RenderEverTick

    public void Sim200msLast(float dt) {
      elapsedTime += dt;
      if (elapsedTime < 0.2f) return;
      elapsedTime -= 0.2f;
      wattsUsed = 0f;
      activeGenerators.Clear();
      proxyList.batteries.Sort((a, b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
      var hasAnyWattProvider = false;
      var hasGenerator = proxyList.generators.Count > 0;
      for (var j = 0; j < proxyList.generators.Count; j++) {
        var generator = proxyList.generators[j];
        if (generator.JoulesAvailable > 0f) {
          hasAnyWattProvider = true;
          activeGenerators.Add(generator);
        }
      }

      activeGenerators.Sort((a, b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));

      for (var index = 0; index < proxyList.batteries.Count; index++) {
        var battery = proxyList.batteries[index];
        if (battery.JoulesAvailable > 0f) hasAnyWattProvider = true;
      }

      if (hasAnyWattProvider)
        for (var index = 0; index < proxyList.energyConsumers.Count; index++) {
          var energyConsumer = proxyList.energyConsumers[index];
          var consumWattsNum = energyConsumer.WattsUsed * 0.2f;
          if (consumWattsNum > 0f) {
            var canConsume = false;
            for (var num3 = 0; num3 < activeGenerators.Count; num3++) {
              var g = activeGenerators[num3];
              consumWattsNum = PowerFromGenerator(consumWattsNum, g, energyConsumer);
              if (consumWattsNum <= 0f) {
                canConsume = true;
                break;
              }
            }

            if (!canConsume) {
              consumWattsNum = PowerFromBatteries(consumWattsNum, proxyList.batteries, energyConsumer);
              canConsume = consumWattsNum <= 0.01f;
            }

            if (canConsume)
              wattsUsed += energyConsumer.WattsUsed;
            else
              wattsUsed += energyConsumer.WattsUsed - consumWattsNum / 0.2f;
            energyConsumer.SetConnectionStatus(!canConsume ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
          } else {
            energyConsumer.SetConnectionStatus(!hasAnyWattProvider
              ? ConnectionStatus.Unpowered
              : ConnectionStatus.Powered);
          }
        }
      else if (hasGenerator)
        for (var index = 0; index < proxyList.energyConsumers.Count; index++)
          proxyList.energyConsumers[index].SetConnectionStatus(ConnectionStatus.Unpowered);
      else
        for (var index = 0; index < proxyList.energyConsumers.Count; index++)
          proxyList.energyConsumers[index].SetConnectionStatus(ConnectionStatus.NotConnected);

      proxyList.batteries.Sort((a, b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
      proxyList.generators.Sort((a, b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));

      ChargeBatteries(proxyList.batteries, proxyList.generators);

      UpdateBatteryConnectionStatus(proxyList.batteries, true);

      for (var index = 0; index < proxyList.generators.Count; index++) {
        var generator2 = proxyList.generators[index];
        ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, 0f - generator2.JoulesAvailable,
          StringFormatter.Replace(BUILDINGS.PREFABS.GENERATOR.OVERPRODUCTION, "{Generator}",
            generator2.gameObject.GetProperName()));
      }
    }

    public void Sim200ms(float dt) {
      if (proxyList == null) return;
      if (proxyList.energyConsumers.Count == 0 && proxyList.generators.Count == 0) {
        operational.SetActive(false);
        return;
      }

      operational.SetActive(true);
      Sim200msLast(dt);
      building.Def.ExhaustKilowattsWhenActive = wattsUsed;
      EnergySim200ms(dt);
    }

    public void EnergySim200ms(float dt) {
      foreach (var generator in proxyList.generators) generator.EnergySim200ms(dt);
      foreach (var battery in proxyList.batteries) battery.EnergySim200ms(dt);
      foreach (var energyConsumer in proxyList.energyConsumers) energyConsumer.EnergySim200ms(dt);
    }

    #endregion

    #region MANAGE_BATTERY

    private float GetBatteryJoulesAvailable(List<Battery> batteries, out int num_powered) {
      var result = 0f;
      num_powered = 0;
      for (var i = 0; i < batteries.Count; i++)
        if (batteries[i].JoulesAvailable > 0f) {
          result = batteries[i].JoulesAvailable;
          num_powered = batteries.Count - i;
          break;
        }

      return result;
    }

    private void UpdateBatteryConnectionStatus(List<Battery> batteries, bool is_connected_to_something_useful) {
      foreach (var battery in batteries)
        if (!(battery == null)) {
          if (battery.powerTransformer == null)
            battery.SetConnectionStatus(is_connected_to_something_useful
              ? ConnectionStatus.Powered
              : ConnectionStatus.NotConnected);
          else
            battery.SetConnectionStatus(!is_connected_to_something_useful
              ? ConnectionStatus.Unpowered
              : ConnectionStatus.Powered);
        }
    }

    private void ChargeBatteries(List<Battery> sink_batteries, List<Generator> source_generators) {
      if (sink_batteries.Count == 0) return;
      foreach (var source_generator in source_generators)
        for (var flag = true;
             flag && source_generator.JoulesAvailable >= 1f;
             flag = ChargeBatteriesFromGenerator(sink_batteries, source_generator)) {
        }
    }

    private bool ChargeBatteriesFromGenerator(List<Battery> sink_batteries, Generator source_generator) {
      var num = source_generator.JoulesAvailable;
      var num2 = 0f;
      for (var i = 0; i < sink_batteries.Count; i++) {
        var battery = sink_batteries[i];
        if (battery != null && source_generator != null && battery.gameObject != source_generator.gameObject) {
          var num3 = battery.Capacity - battery.JoulesAvailable;
          if (num3 > 0f) {
            var num4 = Mathf.Min(num3, num / (sink_batteries.Count - i));
            battery.AddEnergy(num4);
            num -= num4;
            num2 += num4;
          }
        }
      }

      if (num2 > 0f) {
        source_generator.ApplyDeltaJoules(0f - num2);
        return true;
      }

      return false;
    }

    #endregion

    #region CONSUME_ENERGY

    private float PowerFromBatteries(float joules_needed, List<Battery> batteries, EnergyConsumer c) {
      int num_powered;
      do {
        var num = GetBatteryJoulesAvailable(batteries, out num_powered) * num_powered;
        var num2 = num < joules_needed ? num : joules_needed;
        joules_needed -= num2;
        ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, 0f - num2, c.Name);
        var joules = num2 / num_powered;
        for (var i = batteries.Count - num_powered; i < batteries.Count; i++) batteries[i].ConsumeEnergy(joules);
      } while (joules_needed >= 0.01f && num_powered > 0);

      return joules_needed;
    }

    private float PowerFromGenerator(float joules_needed, Generator g, EnergyConsumer c) {
      var num = Mathf.Min(g.JoulesAvailable, joules_needed);
      joules_needed -= num;
      g.ApplyDeltaJoules(0f - num);
      ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, 0f - num, c.Name);
      return joules_needed;
    }

    #endregion
  }
}