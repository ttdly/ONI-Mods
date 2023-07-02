using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FixPack.FluidShipping {
    [AddComponentMenu("KMonoBehaviour/Workable/BottleFiller")]
    public class BottleFiller : Workable {



        private BottleFiller.WorkSession session;
        int infoCount = 0;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            this.resetProgressOnStop = true;
            this.showProgressBar = false;
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            this.SetWorkTime(10f);

            foreach (GameObject go in this.GetComponent<Storage>().items) {
                if (!((UnityEngine.Object)go == (UnityEngine.Object)null) && (UnityEngine.Object)go != (UnityEngine.Object)null)
                    go.DeleteObject();
            }
        }

        public override float GetPercentComplete() {
            if (this.session != null)
                return this.session.GetPercentComplete();
            return 0.0f;
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
            Pickupable.PickupableStartWorkInfo startWorkInfo = (Pickupable.PickupableStartWorkInfo)worker.startWorkInfo;
            float amount = startWorkInfo.amount;
            Element element = startWorkInfo.originalPickupable.GetComponent<PrimaryElement>().Element;
            this.session = new BottleFiller.WorkSession(element.id, startWorkInfo.originalPickupable.GetComponent<SubstanceChunk>(), amount, this.gameObject);

        }

        protected override void OnStopWork(Worker worker) {
            base.OnStopWork(worker);
            if (this.session != null) {
                this.session.Cleanup();
                this.session = (BottleFiller.WorkSession)null;
            }
            this.GetComponent<KAnimControllerBase>().Play((HashedString)"on", KAnim.PlayMode.Once, 1f, 0.0f);
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            if (this.session == null)
                return;
            Storage component1 = worker.GetComponent<Storage>();
            float consumedAmount = this.session.GetConsumedAmount();
            if ((double)consumedAmount > 0.0) {
                SubstanceChunk source = this.session.GetSource();
                SimUtil.DiseaseInfo diseaseInfo = this.session != null ? this.session.GetDiseaseInfo() : SimUtil.DiseaseInfo.Invalid;
                PrimaryElement component2 = source.GetComponent<PrimaryElement>();
                Pickupable component3 = LiquidSourceManager.Instance.CreateChunk(component2.Element, consumedAmount, this.session.GetTemperature(), diseaseInfo.idx, diseaseInfo.count, this.transform.GetPosition()).GetComponent<Pickupable>();
                component3.TotalAmount = consumedAmount;
                component3.Trigger(1335436905, (object)source.GetComponent<Pickupable>());
                worker.workCompleteData = (object)component3;
                ///remove the emission
                for (int index = 0; index < component1.items.Count; ++index) {
                    GameObject go = component1.items[index];
                    if (!((UnityEngine.Object)go == (UnityEngine.Object)null) && go.HasTag(component2.Element.tag)) {
                        go.GetComponent<PrimaryElement>().Mass -= consumedAmount;
                    }
                }

                //End remove emission

                if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
                    component1.Store(component3.gameObject, false, false, true, false);
            }
            this.session.Cleanup();
            this.session = (BottleFiller.WorkSession)null;
        }

        protected override bool OnWorkTick(Worker worker, float dt) {
            if (this.session != null) {
                //this.meter.SetPositionPercent(this.session.GetPercentComplete());
                if ((double)this.session.GetLastTickAmount() <= 0.0)
                    return true;
            }
            return false;
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            if (this.session != null) {
                this.session.Cleanup();
                this.session = (BottleFiller.WorkSession)null;
            }
        }


        private class WorkSession {
            private float amountToPickup;
            private float consumedAmount;
            private float temperature;
            private float amountPerTick;
            private SimHashes element;
            private float lastTickAmount;
            private SubstanceChunk source;
            private SimUtil.DiseaseInfo diseaseInfo;
            private GameObject pump;

            public WorkSession(
              SimHashes element,
              SubstanceChunk source,
              float amount_to_pickup,
              GameObject pump) {
                this.element = element;
                this.source = source;
                this.amountToPickup = amount_to_pickup;
                this.temperature = ElementLoader.FindElementByHash(element).defaultValues.temperature;
                this.diseaseInfo = SimUtil.DiseaseInfo.Invalid;
                this.amountPerTick = 40f;
                this.pump = pump;
                this.lastTickAmount = this.amountPerTick;
            }

            public float GetPercentComplete() {
                return this.consumedAmount / this.amountToPickup;
            }

            public float GetLastTickAmount() {
                return this.lastTickAmount;
            }

            public SimUtil.DiseaseInfo GetDiseaseInfo() {
                return this.diseaseInfo;
            }

            public SubstanceChunk GetSource() {
                return this.source;
            }

            public float GetConsumedAmount() {
                return this.consumedAmount;
            }

            public float GetTemperature() {
                if ((double)this.temperature > 0.0)
                    return this.temperature;
                return ElementLoader.FindElementByHash(this.element).defaultValues.temperature;
            }

            public void Cleanup() {
                this.amountPerTick = 0.0f;
                this.diseaseInfo = SimUtil.DiseaseInfo.Invalid;
            }
        }

        private struct LiquidInfo {
            public float amount;
            public Element element;
            public SubstanceChunk source;
        }
    }
}
