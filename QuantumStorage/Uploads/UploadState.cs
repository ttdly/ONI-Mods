
using Klei;
using KSerialization;
using PeterHan.PLib.Core;
using STRINGS;
using System;
using TUNING;
using UnityEngine;
using static Door;
using static QuantumStorage.Database.DatabaseQ;


namespace QuantumStorage.Uploads {
    [SerializationConfig(MemberSerialization.OptIn)]
    public class UploadState : KMonoBehaviour {
        private StatesInstance smi;
        [MyCmpReq]
        private readonly Operational operational;
        [MyCmpReq]
        private readonly Storage storage;
        [MyCmpReq]
        private readonly KBatchedAnimController controller;
        private MeterController meter;
        public float uploadSpeed = 0.5f;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Prioritizable.AddRef(gameObject);
            meter = new MeterController(controller, "meter_target_counter", "meter_counter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]{
                    "meter_target_counter"
                });
            smi = new StatesInstance(this);
            smi.StartSM();
        }

        protected override void OnCleanUp() {
            Prioritizable.RemoveRef(gameObject);
            base.OnCleanUp();
        }

        private void UpdateMeter() {
            float percent = (storage.MassStored() + uploadSpeed) / storage.capacityKg;
            if (percent >= 1) {
                meter.meterController.Play((HashedString)"meter_on", KAnim.PlayMode.Paused);
            } else {
                meter.meterController.Play((HashedString)"meter_counter", KAnim.PlayMode.Paused);
                meter.SetPositionPercent(storage.MassStored() / storage.capacityKg);
            }
            if (storage.MassStored() > 0) {
                smi.sm.storageEmpty.Set(false, smi);
            } else {
                smi.sm.storageEmpty.Set(true, smi);
            }
            
        }

        private void DoUpload() {
            float needUploadMass = uploadSpeed;
            for (int i = 0; i < storage.items.Count; i++) {
                GameObject go = storage.items[i];
                PrimaryElement primaryElement = go.GetComponent<PrimaryElement>();
                if (primaryElement.Mass > needUploadMass) {
                    StaticVar.database.UpdateItems(new DatabaseItem(go.PrefabID(), (double)needUploadMass));
                    storage.ConsumeIgnoringDisease(go.PrefabID(), needUploadMass);
                    break;
                } else {
                    StaticVar.database.UpdateItems(new DatabaseItem(go.PrefabID(), (double)primaryElement.Mass));
                    storage.ConsumeIgnoringDisease(go.PrefabID(), primaryElement.Mass);
                    needUploadMass -= primaryElement.Mass;
                }
            }
            UpdateMeter();
        }

        private void CanWork() {
            smi.sm.canWork.Set(storage.MassStored() > 0 && StaticVar.database != null, smi);
        }

        public class StatesInstance :GameStateMachine<States, StatesInstance, UploadState, object>.GameInstance {
            public StatesInstance(UploadState master) : base(master) {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, UploadState> {
            public State off;
            public WorkingState on;
            public BoolParameter storageEmpty;
            public BoolParameter canWork;

            public override void InitializeStates(out BaseState default_state) {
                default_state = off;
                off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, smi => smi.master.operational.IsOperational);
                on.DefaultState(on.waiting)
                    .EventTransition(GameHashes.OperationalChanged, off, smi => !smi.master.operational.IsOperational);
                on.waiting.PlayAnim("on", KAnim.PlayMode.Once)
                    .Update((smi, dt) => smi.master.CanWork())
                    .ParamTransition(canWork, on.working_pre, IsTrue);
                on.working_pre.PlayAnim("working_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(on.working_loop);
                on.working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop)
                    .Update((smi, dt) => { smi.master.DoUpload(); }, UpdateRate.SIM_1000ms)
                    .ParamTransition(storageEmpty, on.waiting, IsTrue)
                    .Enter(smi => { smi.master.operational.SetActive(true);})
                    .Exit(smi => { smi.master.operational.SetActive(false);});
                on.working_pst.PlayAnim("working_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(on.waiting);
            }
        }
    }
}
