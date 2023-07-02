using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FixPack.FluidShipping {
    using UnityEngine;

    public class LiquidBottler : Workable {
        public Storage storage;
        private LiquidBottler.Controller.Instance smi;

        protected override void OnSpawn() {
            base.OnSpawn();
            this.smi = new LiquidBottler.Controller.Instance(this);
            this.smi.StartSM();
            this.UpdateStoredItemState();
        }

        protected override void OnCleanUp() {
            if (this.smi != null)
                this.smi.StopSM(nameof(OnCleanUp));
            base.OnCleanUp();
        }

        private void UpdateStoredItemState() {
            this.storage.allowItemRemoval = this.smi != null && this.smi.GetCurrentState() == this.smi.sm.ready;
            foreach (GameObject go in this.storage.items) {
                if ((Object)go != (Object)null)
                    go.Trigger(-778359855, (object)this.storage);
            }
        }

        private class Controller : GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler> {
            public GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State empty;
            public GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State filling;
            public GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State ready;
            public GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State pickup;

            public override void InitializeStates(out StateMachine.BaseState default_state) {
                default_state = (StateMachine.BaseState)this.empty;
                this.empty.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, this.filling, (StateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.Transition.ConditionCallback)(smi => smi.master.storage.IsFull()));
                this.filling.PlayAnim("working").OnAnimQueueComplete(this.ready);
                this.ready.EventTransition(GameHashes.OnStorageChange, this.pickup, (StateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.Transition.ConditionCallback)(smi => !smi.master.storage.IsFull())).Enter((StateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State.Callback)(smi => {
                    smi.master.storage.allowItemRemoval = true;
                    foreach (GameObject go in smi.master.storage.items)
                        go.Trigger(-778359855, (object)smi.master.storage);
                })).Exit((StateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State.Callback)(smi => {
                    smi.master.storage.allowItemRemoval = false;
                    foreach (GameObject go in smi.master.storage.items)
                        go.Trigger(-778359855, (object)smi.master.storage);
                }));
                this.pickup.PlayAnim("pick_up").OnAnimQueueComplete(this.empty);
            }

            public class Instance : GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.GameInstance {
                public Instance(LiquidBottler master)
                  : base(master) {
                }
            }
        }
    }

}
