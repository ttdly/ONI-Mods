using KSerialization;
using System;
using UnityEngine;

namespace TweaksPack.Auto {
    [SerializationConfig(MemberSerialization.OptIn)]
    public class OilRefineryAuto : StateMachineComponent<OilRefineryAuto.StatesInstance> {
        private bool wasOverPressure;
        [SerializeField]
        public float overpressureWarningMass = 4.5f;
        [SerializeField]
        public float overpressureMass = 5f;
        private float maxSrcMass;
        private float envPressure;
        private float cellCount;
        [MyCmpGet]
        private Storage storage;
        [MyCmpReq]
        private Operational operational;
        [MyCmpReq]
        private OccupyArea occupyArea;
        private const bool hasMeter = true;
        private MeterController meter;
        private static readonly EventSystem.IntraObjectHandler<OilRefineryAuto> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<OilRefineryAuto>((component, data) => component.OnStorageChanged(data));

        protected override void OnSpawn() {
            Subscribe(-1697596308, OnStorageChangedDelegate);
            meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, null);
            smi.StartSM();
            maxSrcMass = GetComponent<ConduitConsumer>().capacityKG;
        }

        private void OnStorageChanged(object _) => meter.SetPositionPercent(Mathf.Clamp01(storage.GetMassAvailable(SimHashes.CrudeOil) / maxSrcMass));

        private static bool UpdateStateCb(int cell, object data) {
            OilRefineryAuto oilRefinery = data as OilRefineryAuto;
            if (Grid.Element[cell].IsGas) {
                ++oilRefinery.cellCount;
                oilRefinery.envPressure += Grid.Mass[cell];
            }
            return true;
        }

        private void TestAreaPressure() {
            envPressure = 0.0f;
            cellCount = 0.0f;
            if (!(occupyArea != null) || !(gameObject != null))
                return;
            occupyArea.TestArea(Grid.PosToCell(gameObject), this, new Func<int, object, bool>(UpdateStateCb));
            envPressure /= cellCount;
        }

        private bool CanStartConvert() => Mathf.Clamp01(storage.GetMassAvailable(SimHashes.CrudeOil) / maxSrcMass) >= 0.5f;

        private bool CanStopConvert() => !gameObject.GetComponent<ElementConverter>().HasEnoughMassToStartConverting();

        private bool IsOverPressure() => envPressure >= (double)overpressureMass;

        private bool IsOverWarningPressure() => envPressure >= (double)overpressureWarningMass;

        public class StatesInstance :
          GameStateMachine<States, StatesInstance, OilRefineryAuto, object>.GameInstance {
            public StatesInstance(OilRefineryAuto smi)
              : base(smi) {
            }

            public void TestAreaPressure() {
                smi.master.TestAreaPressure();
                int num = smi.master.IsOverPressure() ? 1 : 0;
                bool flag = smi.master.IsOverWarningPressure();
                if (num != 0) {
                    smi.master.wasOverPressure = true;
                    sm.isOverPressure.Set(true, this);
                } else {
                    if (!smi.master.wasOverPressure || flag)
                        return;
                    sm.isOverPressure.Set(false, this);
                }
            }

        }

        public class States : GameStateMachine<States, StatesInstance, OilRefineryAuto> {
            public BoolParameter isOverPressure;
            public BoolParameter isOverPressureWarning;
            public State idle;
            public State pre;
            public State loop;
            public State pst;
            public State waiting;

            public override void InitializeStates(out BaseState default_state) {
                default_state = waiting;
                idle.PlayAnim("off").OnAnimQueueComplete(waiting);
                waiting.PlayAnim("on").EventTransition(GameHashes.OnStorageChange, pre, smi => smi.master.CanStartConvert());
                pre.PlayAnim("working_pre").OnAnimQueueComplete(loop);
                loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).Enter(smi => smi.master.operational.SetActive(true)).Exit(smi => smi.master.operational.SetActive(false)).Update("Test Pressure Update", (smi, dt) => smi.TestAreaPressure(), UpdateRate.SIM_1000ms).ParamTransition(isOverPressure, pst, IsTrue).Transition(idle, smi => smi.master.CanStopConvert());
                pst.PlayAnim("working_pst").Update("Test Pressure Update", (smi, dt) => smi.TestAreaPressure(), UpdateRate.SIM_1000ms).ParamTransition(isOverPressure, idle, IsFalse);
            }
        }
    }
}