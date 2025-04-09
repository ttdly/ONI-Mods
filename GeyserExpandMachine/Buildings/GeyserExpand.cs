using System;
using KSerialization;
using PeterHan.PLib.Core;


namespace GeyserExpandMachine.Buildings {
    public class GeyserExpand : KMonoBehaviour{
        [Serialize]
        public float skipEruptTimes;
        [Serialize]
        public RunMode runMode = RunMode.Default;
        public static readonly Tag ComponentTag = "GeyserExpand";
        public Storage storage;
        private Geyser geyser;
        private LogicPorts ports;
        private int thisCell;
        private Geyser.StatesInstance geyserState;
        
        public enum OutputLogic {
            Dormant = 0,
            PreErupt = 1,
            Erupting = 2,
            PostErupt = 4,
            OverPressure = 8
        }
        
        public enum RunMode {
            SkipErupt = 1,
            SkipIdle = 2,
            Dormant = 3,
            Default = 4
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            
            GetGeyserAndState();
            geyser.gameObject.AddTag(ComponentTag);
            geyserState.master.AddTag(ComponentTag);
            
            ports = GetComponent<LogicPorts>();
            storage = GetComponent<Storage>();
            ModData.Instance.GeyserExpands.Add(thisCell, this);
            
            geyser.gameObject.SetActive(false);
            geyser.gameObject.SetActive(true);
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            ModData.Instance.GeyserExpands.Remove(thisCell);
            geyser.gameObject.RemoveTag(ComponentTag);
            
            geyser.gameObject.SetActive(false);
            geyser.gameObject.SetActive(true);
        }
        
        private void GetGeyserAndState() {
            PUtil.LogDebug($"RunMode: {runMode}");
            if (geyser != null && geyserState != null) return;
            var cell = Grid.PosToCell(this);
            var geyserFeature = Grid.Objects[cell, (int)ObjectLayer.Building];
            var geyserComponent = geyserFeature.GetComponent<Geyser>();
            if (geyserComponent == null) {
                PUtil.LogError($"Geyser component not found: {geyserFeature}");
            }
            
            geyser = geyserComponent;
            geyser.gameObject.AddComponent<GeyserLogicController>();
            geyserState = geyserFeature.GetSMI<Geyser.StatesInstance>();
            thisCell = cell;
        }

        public void SendLogic(OutputLogic status) {
            if (ports == null) return;
            ports.SendSignal(LiquidGeyserExpandConfig.OutputRibbonID, (int) status);
        }

        #region 设置泉的状态

        public void SkipStage(
            StateMachine.BaseState fromSate,
            StateMachine.BaseState toState,
            float offsetTime,
            float times
        ) {
            // PUtil.LogDebug($"Skip time: {GameClock.Instance.GetTime()}");
            // if (ports == null 
            //     || !geyserState.IsInsideState(fromSate)
            //     || skipEruptTimes + times < 0) return;
            if (ports == null 
                || !geyserState.IsInsideState(fromSate)) return;

        
            // if (GameClock.Instance.GetTime() - skipEruptTimes < 5f) return;
            geyser.AlterTime(offsetTime);
            geyserState.GoTo(toState);
            skipEruptTimes += times;
            // elapsedTime = GameClock.Instance.GetTime();
            ports.SendSignal(LiquidGeyserExpandConfig.OutputPortID, skipEruptTimes > 0 ? 1 : 0); //TODO
            
        }

        private bool CheckMode(RunMode targetRunMode) {
            // PUtil.LogDebug($"Checking mode: {targetRunMode}; {runMode}");
            return runMode == targetRunMode;
        } 
        
        public void SkipErupt() {
            GetGeyserAndState();
            if (!CheckMode(RunMode.SkipErupt)) return;
            SkipStage(
                geyserState.sm.erupt,
                geyserState.sm.post_erupt,
                geyser.timeShift + geyser.RemainingEruptTime(),
                1);
        }

        public void SkipIdle() {
            GetGeyserAndState();
            if (!CheckMode(RunMode.SkipIdle)) return;
            SkipStage(
                geyserState.sm.idle,
                geyserState.sm.pre_erupt,
                geyser.timeShift + geyser.RemainingIdleTime(),
                -1);
        }

        public void SkipDormant() {
            GetGeyserAndState();
            if (!CheckMode(RunMode.SkipIdle)) return;
            SkipStage(
                geyserState.sm.dormant,
                geyserState.sm.pre_erupt,
                geyser.timeShift + geyser.RemainingDormantTime(),
                -1);
        }

        public void AlwaysDormant() {
            GetGeyserAndState();
            if (!CheckMode(RunMode.Dormant)) return;
            SkipStage(
                geyserState.sm.pre_erupt,
                geyserState.sm.dormant,
                geyser.timeShift + geyser.RemainingActiveTime(),
                0);
        }

        #endregion
    }
}