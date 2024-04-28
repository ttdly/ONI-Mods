using System.Collections.Generic;
using STRINGS;
using static CircuitManager;
using UnityEngine;
using KSerialization;

namespace WirelessProject.ProwerManager {
    [SerializationConfig(MemberSerialization.OptIn)]
    public class PowerProxy : KMonoBehaviour, ISim200ms {
        public float minBatteryPercentFull;
        public float wattsUsed;
        public Wire.WattageRating maxWatts = Wire.WattageRating.Max20000;
        private float elapsedTime;
        private readonly List<EnergyConsumer> energyConsumers = new List<EnergyConsumer>();
        private readonly List<Battery> batteries = new List<Battery>();
        private readonly List<Generator> generators = new List<Generator>();
        private readonly List<Generator> activeGenerators = new List<Generator>();
        [MyCmpGet]
        readonly Operational operational;

        #region LifeCycle
        protected override void OnSpawn() {
            base.OnSpawn();
            GlobalVar.PowerProxies.Add(this);
            GetComponent<KSelectable>().AddStatusItem(GlobalVar.ProxyMaxWattageStatus,this);
            GetComponent<KSelectable>().AddStatusItem(GlobalVar.ProxyCircuitStatus, this);
        }

        protected override void OnCleanUp() {
            while(generators.Count > 0) {
                ClearProxy(generators[0].gameObject);
                Disconnect(generators[0]);
            }
            while (energyConsumers.Count > 0) {
                ClearProxy(batteries[0].gameObject);
                Disconnect(energyConsumers[0], true);
            }
            while (batteries.Count > 0) {
                ClearProxy(batteries[0].gameObject);
                Disconnect(batteries[0]);
            }
            GlobalVar.PowerProxies.Remove(this);
            base.OnCleanUp();
        }
        #endregion

        #region DisOrConnect

        public void Connect(Generator generator) {
            if (!Game.IsQuitting()) {
                Game.Instance.energySim.RemoveGenerator(generator);
                Game.Instance.circuitManager.Disconnect(generator);
                generators.Add(generator);
            }
        }

        public void Disconnect(Generator generator) {
            if (!Game.IsQuitting()) {
                generators.Remove(generator);
                Game.Instance.circuitManager.Connect(generator);
                Game.Instance.energySim.AddGenerator(generator);
            }
        }

        public void Connect(Battery battery) {
            if (!Game.IsQuitting()) {
                Game.Instance.energySim.RemoveBattery(battery);
                batteries.Add(battery);
            }
        }

        public void Disconnect(Battery battery) {
            if (!Game.IsQuitting()) {
                batteries.Remove(battery);
                Game.Instance.energySim.AddBattery(battery);
            }
        }

        public void Connect(EnergyConsumer consumer) {
            if (!Game.IsQuitting()) {
                Game.Instance.energySim.RemoveEnergyConsumer(consumer);
                Game.Instance.circuitManager.Disconnect(consumer, true);
                energyConsumers.Add(consumer);
            }
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

        #region RenderEverTick
        public void Sim200msLast(float dt) {
            elapsedTime += dt;
            if (elapsedTime < 0.2f) {
                return;
            }
            elapsedTime -= 0.2f;
            wattsUsed = 0f;
            activeGenerators.Clear();
            batteries.Sort((Battery a, Battery b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
            bool hasAnyWattProvider = false;
            bool hasGenerator = generators.Count > 0;
            for (int j = 0; j < generators.Count; j++) {
                Generator generator = generators[j];
                if (generator.JoulesAvailable > 0f) {
                    hasAnyWattProvider = true;
                    activeGenerators.Add(generator);
                }
            }

            activeGenerators.Sort((Generator a, Generator b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));

            float num = 1f;
            for (int index = 0; index < batteries.Count; index++) {
                Battery battery = batteries[index];
                if (battery.JoulesAvailable > 0f) {
                    hasAnyWattProvider = true;
                }

                num = Mathf.Min(num, battery.PercentFull);
            }

            minBatteryPercentFull = num;
            if (hasAnyWattProvider) {
                for (int index = 0; index < energyConsumers.Count; index++) {
                    EnergyConsumer energyConsumer = energyConsumers[index];
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
                            consumWattsNum = PowerFromBatteries(consumWattsNum, batteries, energyConsumer);
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
                for (int index = 0; index < energyConsumers.Count; index++) {
                    energyConsumers[index].SetConnectionStatus(ConnectionStatus.Unpowered);
                }
            } else {
                for (int index = 0; index < energyConsumers.Count; index++) {
                    energyConsumers[index].SetConnectionStatus(ConnectionStatus.NotConnected);
                }
            }

            batteries.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
            generators.Sort((Generator a, Generator b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));

            float joules_used = 0f;
            float joules_used2 = 0f;

            ChargeBatteries(batteries, generators, ref joules_used2);

            minBatteryPercentFull = 1f;
            for (int index = 0; index < batteries.Count; index++) {
                float percentFull = batteries[index].PercentFull;
                if (percentFull < minBatteryPercentFull) {
                    minBatteryPercentFull = percentFull;
                }
            }

            wattsUsed += joules_used / 0.2f;
            bool is_connected_to_something_useful = generators.Count + energyConsumers.Count > 0;
            UpdateBatteryConnectionStatus(batteries, is_connected_to_something_useful);
            bool flag4 = generators.Count > 0;
            if (!flag4) {
                foreach (Battery battery3 in batteries) {
                    if (battery3.JoulesAvailable > 0f) {
                        flag4 = true;
                        break;
                    }
                }
            }

            for (int num12 = 0; num12 < generators.Count; num12++) {
                Generator generator2 = generators[num12];
                ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, 0f - generator2.JoulesAvailable, StringFormatter.Replace(BUILDINGS.PREFABS.GENERATOR.OVERPRODUCTION, "{Generator}", generator2.gameObject.GetProperName()));
            }
        }

        public void Sim200ms(float dt) {
            if (energyConsumers.Count == 0 && generators.Count == 0 && batteries.Count == 0) {
                operational.SetActive(false);
                return;
            }
            operational.SetActive(true);
            Sim200msLast(dt);
            EnergySim200ms(dt);
        }

        public void EnergySim200ms(float dt) {
            foreach (Generator generator in generators) {
                generator.EnergySim200ms(dt);
            }
            foreach (Battery battery in batteries) {
                battery.EnergySim200ms(dt);
            }
            foreach (EnergyConsumer energyConsumer in energyConsumers) {
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
            foreach(EnergyConsumer energyConsumer in energyConsumers) {
                num += energyConsumer.WattsNeededWhenActive;
            }
            return num;
        }

        private void ClearProxy(GameObject go) {
            ProxyLink proxy = go.GetComponent<ProxyLink>();
            if (proxy != null) {
                proxy.proxy = null;
                proxy.hasProxy = false;
                proxy.gameObject.RemoveTag(GlobalVar.HasProxy);
            }
        }
    }
}
