﻿// Decompiled with JetBrains decompiler
// Type: OilWellCap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AD882E55-D8AC-4937-9773-540EE16F428F
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

namespace TweaksPack.Auto {
    [SerializationConfig(MemberSerialization.OptIn)]
    [AddComponentMenu("KMonoBehaviour/Workable/OilWellCapClear")]
    public class OilWellCapAuto : Workable, ISingleSliderControl, ISliderControl, IElementEmitter {
        private StatesInstance smi;
        [MyCmpReq]
        private Operational operational;
        [MyCmpReq]
        private Storage storage;
        public SimHashes gasElement;
        public float gasTemperature;
        public float addGasRate = 1f;
        public float maxGasPressure = 10f;
        public float releaseGasRate = 10f;
        [Serialize]
        public bool duplicantOperated = true;
        [Serialize]
        private float depressurizePercent = 0.75f;
        private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
        private MeterController pressureMeter;
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        private static readonly EventSystem.IntraObjectHandler<OilWellCapAuto> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<OilWellCapAuto>((component, data) => component.OnCopySettings(data));
        private WorkChore<OilWellCapAuto> DepressurizeChore;
        private static readonly Chore.Precondition AllowedToDepressurize = new Chore.Precondition() {
            id = nameof(AllowedToDepressurize),
            description = (string)DUPLICANTS.CHORES.PRECONDITIONS.ALLOWED_TO_DEPRESSURIZE,
            fn = (ref Chore.Precondition.Context context, object data) => ((OilWellCapAuto)data).NeedsDepressurizing()
        };

        public SimHashes Element => gasElement;

        public float AverageEmitRate => Game.Instance.accumulators.GetAverageRate(accumulator);

        public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TITLE";

        public string SliderUnits => (string)UI.UNITSUFFIXES.PERCENT;

        public int SliderDecimalPlaces(int index) => 0;

        public float GetSliderMin(int index) => 0.0f;

        public float GetSliderMax(int index) => 100f;

        public float GetSliderValue(int index) => depressurizePercent * 100f;

        public void SetSliderValue(float value, int index) => depressurizePercent = value / 100f;

        public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TOOLTIP";

        string ISliderControl.GetSliderTooltip() => string.Format((string)Strings.Get("STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TOOLTIP"), (float)(depressurizePercent * 100.0));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            Subscribe(-905833192, OnCopySettingsDelegate);
        }

        private void OnCopySettings(object data) {
            OilWellCapAuto component = ((GameObject)data).GetComponent<OilWellCapAuto>();
            if (!(component != null))
                return;
            depressurizePercent = component.depressurizePercent;
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Prioritizable.AddRef(gameObject);
            accumulator = Game.Instance.accumulators.Add("pressuregas", this);
            showProgressBar = false;
            SetWorkTime(float.PositiveInfinity);
            overrideAnims = new KAnimFile[1]
            {
      Assets.GetAnim((HashedString) "anim_interacts_oil_cap_kanim")
            };
            workingStatusItem = Db.Get().BuildingStatusItems.ReleasingPressure;
            attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
            attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
            skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
            skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            pressureMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new Vector3(0.0f, 0.0f, 0.0f), null);
            smi = new StatesInstance(this);
            smi.StartSM();
            UpdatePressurePercent();
        }

        protected override void OnCleanUp() {
            Game.Instance.accumulators.Remove(accumulator);
            Prioritizable.RemoveRef(gameObject);
            base.OnCleanUp();
        }

        public void AddGasPressure(float dt) {
            storage.AddGasChunk(gasElement, addGasRate * dt, gasTemperature, 0, 0, true);
            if (!duplicantOperated && NeedsDepressurizing()) {
                CancelChore("NotOperate");
                smi.sm.working.Set(true, smi);
            }
            UpdatePressurePercent();
        }

        public void ReleaseGasPressure(float dt) {
            PrimaryElement primaryElement = storage.FindPrimaryElement(gasElement);
            if (primaryElement != null && (double)primaryElement.Mass > 0.0) {
                if (!duplicantOperated) {
                    dt = 60f;
                }
                float a = releaseGasRate * dt;
                if (worker != null)
                    a *= GetEfficiencyMultiplier(worker);
                float num = Mathf.Min(a, primaryElement.Mass);
                SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(primaryElement, num / primaryElement.Mass);
                primaryElement.Mass -= num;
                Game.Instance.accumulators.Accumulate(accumulator, num);
                SimMessages.AddRemoveSubstance(Grid.PosToCell(this), ElementLoader.GetElementIndex(gasElement), null, num, primaryElement.Temperature, percentOfDisease.idx, percentOfDisease.count);
            }
            if (!duplicantOperated && !NeedsDepressurizing()) {
                smi.sm.working.Set(false, smi);
            }
            UpdatePressurePercent();
        }

        private void UpdatePressurePercent() {
            float percent_full = Mathf.Clamp01(storage.GetMassAvailable(gasElement) / maxGasPressure);
            double _ = (double)smi.sm.pressurePercent.Set(percent_full, smi);
            pressureMeter.SetPositionPercent(percent_full);
        }

        public bool NeedsDepressurizing() => (double)smi.GetPressurePercent() >= depressurizePercent;

        private WorkChore<OilWellCapAuto> CreateWorkChore() {
            DepressurizeChore = new WorkChore<OilWellCapAuto>(Db.Get().ChoreTypes.Depressurize, this);
            DepressurizeChore.AddPrecondition(AllowedToDepressurize, this);
            return DepressurizeChore;
        }

        private void CancelChore(string reason) {
            if (DepressurizeChore == null)
                return;
            DepressurizeChore.Cancel(reason);
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
            smi.sm.working.Set(true, smi);
        }

        protected override void OnStopWork(Worker worker) {
            base.OnStopWork(worker);
            smi.sm.working.Set(false, smi);
            DepressurizeChore = null;
        }

        protected override bool OnWorkTick(Worker worker, float dt) => (double)smi.GetPressurePercent() <= 0.0;

        public override bool InstantlyFinish(Worker worker) {
            ReleaseGasPressure(60f);
            return true;
        }

        public class StatesInstance :
          GameStateMachine<States, StatesInstance, OilWellCapAuto, object>.GameInstance {
            public StatesInstance(OilWellCapAuto master)
              : base(master) {
            }

            public float GetPressurePercent() => sm.pressurePercent.Get(smi);
        }

        public class States : GameStateMachine<States, StatesInstance, OilWellCapAuto> {
            public FloatParameter pressurePercent;
            public BoolParameter working;
            public State inoperational;
            public OperationalStates operational;

            public override void InitializeStates(out BaseState default_state) {
                default_state = inoperational;
                inoperational.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, operational, new Transition.ConditionCallback(IsOperational));
                operational.DefaultState(operational.idle).ToggleRecurringChore(smi => smi.master.CreateWorkChore(), smi => smi.master.duplicantOperated).EventHandler(GameHashes.WorkChoreDisabled, smi => smi.master.CancelChore("WorkChoreDisabled"));
                operational.idle.PlayAnim("off").ToggleStatusItem(TweaksPackStatusItrems.WellPressurizingAuto).ParamTransition(pressurePercent, operational.overpressure, IsGTEOne).ParamTransition(working, operational.releasing_pressure, IsTrue).EventTransition(GameHashes.OperationalChanged, inoperational, Not(new Transition.ConditionCallback(IsOperational))).EventTransition(GameHashes.OnStorageChange, operational.active, new Transition.ConditionCallback(IsAbleToPump));
                operational.active.DefaultState(operational.active.pre).ToggleStatusItem(TweaksPackStatusItrems.WellPressurizingAuto).Enter(smi => smi.master.operational.SetActive(true)).Exit(smi => smi.master.operational.SetActive(false)).Update((smi, dt) => smi.master.AddGasPressure(dt));
                operational.active.pre.PlayAnim("working_pre").ParamTransition(pressurePercent, operational.overpressure, IsGTEOne).ParamTransition(working, operational.releasing_pressure, IsTrue).OnAnimQueueComplete(operational.active.loop);
                operational.active.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).ParamTransition(pressurePercent, operational.active.pst, IsGTEOne).ParamTransition(working, operational.active.pst, IsTrue).EventTransition(GameHashes.OperationalChanged, operational.active.pst, new Transition.ConditionCallback(MustStopPumping)).EventTransition(GameHashes.OnStorageChange, operational.active.pst, new Transition.ConditionCallback(MustStopPumping));
                operational.active.pst.PlayAnim("working_pst").OnAnimQueueComplete(operational.idle);
                operational.overpressure.PlayAnim("over_pressured_pre", KAnim.PlayMode.Once).QueueAnim("over_pressured_loop", true).ToggleStatusItem(Db.Get().BuildingStatusItems.WellOverpressure).ParamTransition(pressurePercent, operational.idle, (smi, p) => (double)p <= 0.0).ParamTransition(working, operational.releasing_pressure, IsTrue);
                operational.releasing_pressure.DefaultState(operational.releasing_pressure.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingElement, smi => smi.master);
                operational.releasing_pressure.pre.PlayAnim("steam_out_pre").OnAnimQueueComplete(operational.releasing_pressure.loop);
                operational.releasing_pressure.loop.PlayAnim("steam_out_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, operational.releasing_pressure.pst, Not(new Transition.ConditionCallback(IsOperational))).ParamTransition(working, operational.releasing_pressure.pst, IsFalse).Update((smi, dt) => smi.master.ReleaseGasPressure(dt));
                operational.releasing_pressure.pst.PlayAnim("steam_out_pst").OnAnimQueueComplete(operational.idle);
            }

            private bool IsOperational(StatesInstance smi) => smi.master.operational.IsOperational;

            private bool IsAbleToPump(StatesInstance smi) {
                return smi.master.operational.IsOperational && smi.GetComponent<ElementConverter>().HasEnoughMassToStartConverting();
            }

            private bool MustStopPumping(StatesInstance smi) => !smi.master.operational.IsOperational || !smi.GetComponent<ElementConverter>().CanConvertAtAll();

            public class OperationalStates :
              State {
                public State idle;
                public PreLoopPostState active;
                public State overpressure;
                public PreLoopPostState releasing_pressure;
            }
        }
    }
}