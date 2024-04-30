using System.Collections.Generic;
using STRINGS;
using static CircuitManager;
using static WirelessProject.ProwerManager.GlobalVar;
using UnityEngine;
using PeterHan.PLib.Core;

namespace WirelessProject.ProwerManager {

    public class PowerProxy : KMonoBehaviour, ISim200ms {

        public class ProxyList {
            public PowerProxy proxy;
            public int ThisCell;
            public readonly List<EnergyConsumer> energyConsumers = new List<EnergyConsumer>();
            public readonly List<Battery> batteries = new List<Battery>();
            public readonly List<Generator> generators = new List<Generator>();

            #region DisOrConnect

            public int Connect(Generator generator) {
                if (!Game.IsQuitting()) {
                    Game.Instance.energySim.RemoveGenerator(generator);
                    Game.Instance.circuitManager.Disconnect(generator);
                    generators.Add(generator);
                    return ThisCell;
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
                    return ThisCell;
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
                    return ThisCell;
                }
                return -1;
            }

            public void Disconnect(EnergyConsumer consumer, bool isDestroy) {
                if (!Game.IsQuitting()) {
                    energyConsumers.Remove(consumer);
                    if (!isDestroy) {
                        consumer.SetConnectionStatus(ConnectionStatus.NotConnected);
                    }
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
                    return ThisCell;
                }
                return -1;
            }

            public int Add(Generator generator) {
                if (!Game.IsQuitting()) {
                    generators.Add(generator);
                    return ThisCell;
                }
                return -1;
            }

            public int Add(Battery battery) {
                if (!Game.IsQuitting()) {
                    batteries.Add(battery);
                    return ThisCell;
                }
                return -1;
            }
            #endregion
        }

        public ProxyList proxyList;
        public float minBatteryPercentFull;
        public float wattsUsed;
        public Wire.WattageRating maxWatts = Wire.WattageRating.Max20000;
        private float elapsedTime;
        private readonly List<Generator> activeGenerators = new List<Generator>();
        [MyCmpGet]
        readonly Operational operational;
        public int ThisCell;

        #region LifeCycle
        protected override void OnSpawn() {
            base.OnSpawn();
            ThisCell = Grid.PosToCell(gameObject.transform.GetPosition());
            //PowerProxiesWithCell.Add(ThisCell, this);
            PowerProxiesWithCell.TryGetValue(ThisCell, out ProxyList proxyList);
            if (proxyList != null) {
                this.proxyList = proxyList;
                this.proxyList.proxy = this;
            } else {
                ProxyList new_proxyList = new ProxyList {
                    ThisCell = ThisCell,
                    proxy = this
                };
                PowerProxiesWithCell.Add(ThisCell, new_proxyList);
                this.proxyList = new_proxyList;
            }
            
            GetComponent<KSelectable>().AddStatusItem(ProxyMaxWattageStatus, this);
            GetComponent<KSelectable>().AddStatusItem(ProxyCircuitStatus, this);
        }

        protected override void OnCleanUp() {
            while (proxyList.generators.Count > 0) {
                ClearProxy(proxyList.generators[0].gameObject);
            }
            while (proxyList.batteries.Count > 0) {
                ClearProxy(proxyList.batteries[0].gameObject);
            }
            while (proxyList.energyConsumers.Count > 0) {
                ClearProxy(proxyList.energyConsumers[0].gameObject);
            }
            PowerProxiesWithCell.Remove(ThisCell);
            base.OnCleanUp();
        }
        #endregion



        #region RenderEverTick
        public void Sim200msLast(float dt) {
            elapsedTime += dt;
            if (elapsedTime < 0.2f) {
                return;
            }
            elapsedTime -= 0.2f;
            wattsUsed = 0f;
            activeGenerators.Clear();
            proxyList.batteries.Sort((Battery a, Battery b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
            bool hasAnyWattProvider = false;
            bool hasGenerator = proxyList.generators.Count > 0;
            for (int j = 0; j < proxyList.generators.Count; j++) {
                Generator generator = proxyList.generators[j];
                if (generator.JoulesAvailable > 0f) {
                    hasAnyWattProvider = true;
                    activeGenerators.Add(generator);
                }
            }

            activeGenerators.Sort((Generator a, Generator b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));

            float num = 1f;
            for (int index = 0; index < proxyList.batteries.Count; index++) {
                Battery battery = proxyList.batteries[index];
                if (battery.JoulesAvailable > 0f) {
                    hasAnyWattProvider = true;
                }

                num = Mathf.Min(num, battery.PercentFull);
            }

            minBatteryPercentFull = num;
            if (hasAnyWattProvider) {
                for (int index = 0; index < proxyList.energyConsumers.Count; index++) {
                    EnergyConsumer energyConsumer = proxyList.energyConsumers[index];
                    float consumWattsNum = energyConsumer.WattsUsed * 0.2f;
                    if (consumWattsNum > 0f) {
                        bool canConsume = false;
                        for (int num3 = 0; num3 < activeGenerators.Count; num3++) {
                            Generator g = activeGenerators[num3];
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
                        if (canConsume) {
                            wattsUsed += energyConsumer.WattsUsed;
                        } else {
                            wattsUsed += energyConsumer.WattsUsed - consumWattsNum / 0.2f;
                        }
                        energyConsumer.SetConnectionStatus((!canConsume) ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
                    } else {
                        energyConsumer.SetConnectionStatus((!hasAnyWattProvider) ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
                    }
                }
            } else if (hasGenerator) {
                for (int index = 0; index < proxyList.energyConsumers.Count; index++) {
                    proxyList.energyConsumers[index].SetConnectionStatus(ConnectionStatus.Unpowered);
                }
            } else {
                for (int index = 0; index < proxyList.energyConsumers.Count; index++) {
                    proxyList.energyConsumers[index].SetConnectionStatus(ConnectionStatus.NotConnected);
                }
            }

            proxyList.batteries.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
            proxyList.generators.Sort((Generator a, Generator b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));

            float joules_used = 0f;
            float joules_used2 = 0f;

            ChargeBatteries(proxyList.batteries, proxyList.generators, ref joules_used2);

            minBatteryPercentFull = 1f;
            for (int index = 0; index < proxyList.batteries.Count; index++) {
                float percentFull = proxyList.batteries[index].PercentFull;
                if (percentFull < minBatteryPercentFull) {
                    minBatteryPercentFull = percentFull;
                }
            }

            wattsUsed += joules_used / 0.2f;
            bool is_connected_to_something_useful = proxyList.generators.Count + proxyList.energyConsumers.Count > 0;
            UpdateBatteryConnectionStatus(proxyList.batteries, is_connected_to_something_useful);
            bool flag4 = proxyList.generators.Count > 0;
            if (!flag4) {
                foreach (Battery battery3 in proxyList.batteries) {
                    if (battery3.JoulesAvailable > 0f) {
                        flag4 = true;
                        break;
                    }
                }
            }

            for (int num12 = 0; num12 < proxyList.generators.Count; num12++) {
                Generator generator2 = proxyList.generators[num12];
                ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, 0f - generator2.JoulesAvailable, StringFormatter.Replace(BUILDINGS.PREFABS.GENERATOR.OVERPRODUCTION, "{Generator}", generator2.gameObject.GetProperName()));
            }
        }

        public void Sim200ms(float dt) {
            if (proxyList.energyConsumers.Count == 0 && proxyList.generators.Count == 0) {
                operational.SetActive(false);
                return;
            }
            operational.SetActive(true);
            Sim200msLast(dt);
            EnergySim200ms(dt);
        }

        public void EnergySim200ms(float dt) {
            foreach (Generator generator in proxyList.generators) {
                generator.EnergySim200ms(dt);
            }
            foreach (Battery battery in proxyList.batteries) {
                battery.EnergySim200ms(dt);
            }
            foreach (EnergyConsumer energyConsumer in proxyList.energyConsumers) {
                energyConsumer.EnergySim200ms(dt);
            }
        }
        #endregion

        #region MANAGE_BATTERY
        private float GetBatteryJoulesAvailable(List<Battery> batteries, out int num_powered) {
            float result = 0f;
            num_powered = 0;
            for (int i = 0; i < batteries.Count; i++) {
                if (batteries[i].JoulesAvailable > 0f) {
                    result = batteries[i].JoulesAvailable;
                    num_powered = batteries.Count - i;
                    break;
                }
            }
            return result;
        }

        private void UpdateBatteryConnectionStatus(List<Battery> batteries, bool is_connected_to_something_useful) {
            foreach (Battery battery in batteries) {
                if (!(battery == null)) {
                    if (battery.powerTransformer == null) {
                        battery.SetConnectionStatus(is_connected_to_something_useful ? ConnectionStatus.Powered : ConnectionStatus.NotConnected);
                    } else {
                        battery.SetConnectionStatus((!is_connected_to_something_useful) ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
                    }
                }
            }
        }

        private void ChargeBatteries(List<Battery> sink_batteries, List<Generator> source_generators, ref float joules_used) {
            if (sink_batteries.Count == 0) {
                return;
            }
            foreach (Generator source_generator in source_generators) {
                for (bool flag = true; flag && source_generator.JoulesAvailable >= 1f; flag = ChargeBatteriesFromGenerator(sink_batteries, source_generator, ref joules_used)) {
                }
            }
        }

        private bool ChargeBatteriesFromGenerator(List<Battery> sink_batteries, Generator source_generator, ref float joules_used) {
            float num = source_generator.JoulesAvailable;
            float num2 = 0f;
            for (int i = 0; i < sink_batteries.Count; i++) {
                Battery battery = sink_batteries[i];
                if (battery != null && source_generator != null && battery.gameObject != source_generator.gameObject) {
                    float num3 = battery.Capacity - battery.JoulesAvailable;
                    if (num3 > 0f) {
                        float num4 = Mathf.Min(num3, num / (float)(sink_batteries.Count - i));
                        battery.AddEnergy(num4);
                        num -= num4;
                        num2 += num4;
                    }
                }
            }

            if (num2 > 0f) {
                source_generator.ApplyDeltaJoules(0f - num2);
                joules_used += num2;
                return true;
            }

            return false;
        }
        #endregion

        #region CONSUME_ENERGY

        private float PowerFromBatteries(float joules_needed, List<Battery> batteries, EnergyConsumer c) {
            int num_powered;
            do {
                float num = GetBatteryJoulesAvailable(batteries, out num_powered) * num_powered;
                float num2 = ((num < joules_needed) ? num : joules_needed);
                joules_needed -= num2;
                ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, 0f - num2, c.Name);
                float joules = num2 / num_powered;
                for (int i = batteries.Count - num_powered; i < batteries.Count; i++) {
                    batteries[i].ConsumeEnergy(joules);
                }
            }
            while (joules_needed >= 0.01f && num_powered > 0);
            return joules_needed;
        }

        private float PowerFromGenerator(float joules_needed, Generator g, EnergyConsumer c) {
            float num = Mathf.Min(g.JoulesAvailable, joules_needed);
            joules_needed -= num;
            g.ApplyDeltaJoules(0f - num);
            ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, 0f - num, c.Name);
            return joules_needed;
        }

        #endregion

        public float GetWattsNeededWhenActive() {
            float num = 0f;
            foreach (EnergyConsumer energyConsumer in proxyList.energyConsumers) {
                num += energyConsumer.WattsNeededWhenActive;
            }
            return num;
        }

        private void ClearProxy(GameObject go) {
            go.GetComponent<BaseLinkToProxy>().RemoveThisFromProxy();
        }
    }
}
