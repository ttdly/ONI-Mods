using System;
using System.Collections.Generic;

namespace MoreTiles {
    public class AutoLightTile : KMonoBehaviour, ISim1000ms {
        private HandleVector<int>.Handle pickupablesChangedEntry;
        private Extents pickupableExtents;
        private readonly HashSet<Worker> workers = new HashSet<Worker>();
        Worker targetWorker;
        [MyCmpReq]
        public readonly Operational operational;
        [MyCmpReq]
        readonly Light2D light2D;
        [MyCmpReq]
        readonly KBatchedAnimController batchedAnimController;
        IEnergyConsumer energy;
        bool animStatues;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            energy = GetComponent<IEnergyConsumer>();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Vector2I xy = Grid.CellToXY(this.NaturalBuildingCell());
            int cell = Grid.XYToCell(xy.x, xy.y + 1);
            CellOffset offset = new CellOffset(0, 1);
            pickupableExtents = new Extents(cell, 1);
            pickupablesChangedEntry = GameScenePartitioner.Instance.Add("DuplicantSensor.PickupablesChanged", gameObject, pickupableExtents, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(OnPickupablesChanged));
            RefreshLight();
        }

        protected override void OnCleanUp() {
            GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
            MinionGroupProber.Get().ReleaseProber(this);
            base.OnCleanUp();
        }

        private void OnPickupablesChanged(object data) {
            if (targetWorker != null) return;
            Pickupable pickupable = data as Pickupable;
            if (pickupable == null) return;
            if (!pickupable.KPrefabID.HasTag(GameTags.DupeBrain)) return;
            Worker worker = pickupable.gameObject.GetComponent<Worker>();
            if (worker != null) {
                workers.Add(worker);
            }
            RefreshLight();
        }

        private bool CanLight() {
            if (targetWorker == null) return false;
            if (!energy.IsConnected || !energy.IsPowered) return false;
            if (targetWorker.state == Worker.State.Working) return true;
            targetWorker = null;
            return false;
        }

        private void RefreshLight() {
            if (workers.Count > 0 || targetWorker != null) SelectTargetWorker();
            bool canlight = CanLight();
            if (operational.IsActive != canlight) {
                operational.SetActive(canlight);
            }
            if (light2D.enabled != canlight) {
                light2D.enabled = canlight;
            }
            if (animStatues != canlight) {
                string animName = canlight ? "on" : "off";
                animStatues = canlight;
                batchedAnimController.Play(animName);
            }
        }

        private void SelectTargetWorker() {
            foreach (Worker worker in workers) {
                if (worker.state == Worker.State.Working) {
                    targetWorker = worker;
                    workers.Clear();
                    break;
                }
            }
        }

        public void Sim1000ms(float dt) {
            RefreshLight();
        }
    }

}