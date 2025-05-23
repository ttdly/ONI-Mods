﻿using System;
using GeyserExpandMachine.Buildings;
using KSerialization;
using PeterHan.PLib.Core;
using YamlDotNet.Core;

namespace GeyserExpandMachine.GeyserModify {
    public class GeyserLogicController: StateMachineComponent<GeyserLogicController.StatesInstance> {
        public LogicPorts ports;
        public HashedString portID;
        public HashedString ribbonPortID;
        [Serialize]
        public RunMode runMode = RunMode.Default;
        private Geyser geyser;
        private Geyser.StatesInstance geyserState;
        private ElementEmitter emitter;
        private float skipEruptTimes;
        
        public enum RunMode {
            SkipErupt = 1,
            SkipIdle = 2,
            SkipDormant = 3,
            AlwaysDormant = 4,
            Default = 5
        }
        
        public enum OutputLogic {
            Dormant = 0,
            PreErupt = 1,
            Erupting = 2,
            PostErupt = 4,
            OverPressure = 8
        }
        
        protected override void OnSpawn() {
            base.OnSpawn();
            geyser = GetComponent<Geyser>();
            emitter = GetComponent<ElementEmitter>();
            geyserState = geyser.GetSMI<Geyser.StatesInstance>();
            
            geyser.gameObject.SetActive(false);
            geyser.gameObject.SetActive(true);
            smi.StartSM();
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            geyser.gameObject.SetActive(false);
            geyser.gameObject.SetActive(true);
        }

        #region 设置泉的状态

        public void ImmediatelySkip() {
            switch (runMode) {
                case RunMode.SkipIdle:
                    SkipIdle();
                    break;
                case RunMode.SkipErupt:
                    SkipErupt();
                    break;
                case RunMode.SkipDormant:
                    SkipDormant();
                    break;
                case RunMode.AlwaysDormant:
                    AlwaysDormant();
                    break;
            }
        }
        
        public void SkipStage(
            StateMachine.BaseState fromSate,
            StateMachine.BaseState toState,
            float offsetTime,
            float times
        ) {
            // PUtil.LogDebug($"RunMode{runMode}; " +
            //                $"\n\t from:{fromSate.name}; " +
            //                $"\n\t to:{toState.name}; " +
            //                $"\n\t offsetTime:{offsetTime}" +
            //                $"\n\t times:{times}" +
            //                $"\n\t IsInsideState: {geyserState.IsInsideState(fromSate)}" +
            //                $"\n\t GeyserState: {geyserState.GetCurrentState().name}; ");

            // if (ports == null 
            //     || !geyserState.IsInsideState(fromSate)
            // //     || skipEruptTimes + times < 0) return;
            // if (ports == null 
            //     || !geyserState.IsInsideState(fromSate)) return;

        
            // if (GameClock.Instance.GetTime() - skipEruptTimes < 5f) return;
            geyser.AlterTime(offsetTime);
            geyserState.GoTo(toState);
            skipEruptTimes += times;
            // elapsedTime = GameClock.Instance.GetTime();
            ports.SendSignal(LiquidGeyserExpandConfig.OutputPortID, skipEruptTimes > 0 ? 1 : 0); //TODO
            
        }

        private bool CheckMode(RunMode targetRunMode) {
            PUtil.LogDebug($"Checking mode: {targetRunMode}; real mod{runMode}");
            return runMode == targetRunMode;
        } 
        
        public void SkipErupt() {

            if (!CheckMode(RunMode.SkipErupt)) return;
            SkipStage(
                geyserState.sm.erupt,
                geyserState.sm.post_erupt,
                geyser.timeShift + geyser.RemainingEruptTime(),
                1);
        }

        public void SkipIdle() {

            if (!CheckMode(RunMode.SkipIdle)) return;
            SkipStage(
                geyserState.sm.idle,
                geyserState.sm.pre_erupt,
                geyser.timeShift + geyser.RemainingIdleTime(),
                -1);
        }

        public void SkipDormant() {

            if (!CheckMode(RunMode.SkipDormant)) return;
            SkipStage(
                geyserState.sm.dormant,
                geyserState.sm.pre_erupt,
                geyser.timeShift + geyser.RemainingDormantTime(),
                -1);
        }

        public void AlwaysDormant() {

            if (!CheckMode(RunMode.AlwaysDormant)) return;
            SkipStage(
                geyserState.sm.pre_erupt,
                geyserState.sm.dormant,
                geyser.timeShift + geyser.RemainingActiveTime(),
                0);
        }

        #endregion
        
        
        public void SendLogic(OutputLogic status) {
            if (ports == null) return;
            ports.SendSignal(ribbonPortID, (int) status);
        }
        

        public class StatesInstance(GeyserLogicController smi) :
            GameStateMachine<States, StatesInstance, GeyserLogicController, object>.GameInstance(smi) {
        }

        public class States : GameStateMachine<States, StatesInstance, GeyserLogicController> {
            public State dormant;
            public State idle;
            public State pre_erupt;
            public EruptState erupt;
            public State post_erupt;

            public override void InitializeStates(out BaseState default_state) {
                default_state = idle;
                serializable = SerializeType.Both_DEPRECATED;
                root
                    .DefaultState(idle)
                    .Enter(smi => smi.master.emitter.SetEmitting(false));
                dormant
                    .Enter(smi => {
                        smi.master.SendLogic(OutputLogic.Dormant);
                        smi.master.SkipDormant();
                    })
                    .ScheduleGoTo(smi => smi.master.geyser.RemainingDormantTime(), pre_erupt);
                idle
                    .Enter(smi => {
                        if (!smi.master.geyser.ShouldGoDormant())
                            return;
                        smi.GoTo(dormant);
                        smi.master.SendLogic(OutputLogic.Dormant);
                        smi.master.SkipIdle();
                    })
                    .ScheduleGoTo(smi => smi.master.geyser.RemainingIdleTime(), pre_erupt);
                pre_erupt
                    .Enter(smi => {
                        smi.master.SendLogic(OutputLogic.PreErupt);
                        smi.master.AlwaysDormant();
                    })
                    .ScheduleGoTo(smi => smi.master.geyser.RemainingEruptPreTime(), erupt);
                erupt
                    .TriggerOnEnter(GameHashes.GeyserEruption, smi => true)
                    .TriggerOnExit(GameHashes.GeyserEruption, smi => false).DefaultState(erupt.erupting)
                    .ScheduleGoTo(smi => smi.master.geyser.RemainingEruptTime(), post_erupt)
                    .Enter(smi => {
                        smi.master.emitter.SetEmitting(true);
                        smi.master.SkipErupt();
                    })
                    .Exit(smi => smi.master.emitter.SetEmitting(false));
                erupt.erupting
                    .EventTransition(GameHashes.EmitterBlocked, erupt.overpressure,
                    smi => smi.GetComponent<ElementEmitter>().isEmitterBlocked)
                    .Enter(smi => smi.master.SendLogic(OutputLogic.Erupting));
                erupt.overpressure
                    .EventTransition(GameHashes.EmitterUnblocked, erupt.erupting,
                    smi => !smi.GetComponent<ElementEmitter>().isEmitterBlocked)
                    .Enter(smi => smi.master.SendLogic(OutputLogic.OverPressure));
                post_erupt
                    .ScheduleGoTo(smi => smi.master.geyser.RemainingEruptPostTime(), idle)
                    .Enter(smi => smi.master.SendLogic(OutputLogic.PostErupt));
            }

            public class EruptState :
                State {
                public State erupting;
                public State overpressure;
            }
        }
    }
}