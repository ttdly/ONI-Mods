using KSerialization;
using STRINGS;
using UnityEngine;
using static ConduitFlow;

namespace WirelessProject.ConduitManger {
    public class ConduitConsumerProxy : BaseLinkToProxy {
        [MyCmpGet]
        readonly ConduitConsumer conduitConsumer;
        [MyCmpGet]
        readonly Operational operational;
        [MyCmpGet]
        readonly Storage storage;
        public bool IsSatisfied;
        private bool consumedLastTick;
        private bool AlwaysConsume => conduitConsumer.alwaysConsume;
        public float ConsumptionRate => conduitConsumer.consumptionRate;
        public int proxyListIndex;
        public SimHashes lastConsumedElement = SimHashes.Vacuum;
        [SerializeField]
        public bool keepZeroMassObject = true;

        //protected override void OnSpawn() {
        //    base.OnSpawn();
            
        //}

        protected override void AddThisToProxy() {
            base.AddThisToProxy();
            ConduitProxyContentList.ListIdAndIndex idAndIndex = proxyList.AddConduitUpdater(ConduitUpdate);
            ProxyListId = idAndIndex.Id;
            proxyListIndex = idAndIndex.Index;
        }

        public override void RemoveThisFromProxy(bool _ = false) {
            base.RemoveThisFromProxy(_);
            proxyList.RemoveConduitUpdater(ConduitUpdate, proxyListIndex);
            ProxyListId = -1;
        }

        protected override void RestoreLink() {
            base.RestoreLink();
        }

        public void ConduitUpdate(float dt) {
            Consume(dt);
        }

        private void Consume(float dt) {
            IsSatisfied = false;
            consumedLastTick = false;

            if (!StaticVar.GetContents(ProxyListId, proxyListIndex, out ConduitContents contents)) {
                return;
            }
            if (contents.mass <= 0f) {
                return;
            }

            IsSatisfied = true;
            if (!AlwaysConsume && !operational.MeetsRequirements(conduitConsumer.OperatingRequirement)) {
                return;
            }

            float a = ConsumptionRate * dt;
            a = Mathf.Min(a,conduitConsumer.space_remaining_kg);
            Element element = ElementLoader.FindElementByHash(contents.element);
            if (contents.element != lastConsumedElement) {
                DiscoveredResources.Instance.Discover(element.tag, element.materialCategory);
            }

            float num = 0f;
            if (a > 0f) {
                ConduitContents conduitContents = RemoveElement(contents, a);
                num = conduitContents.mass;
                lastConsumedElement = conduitContents.element;
            }

            bool flag = element.HasTag(conduitConsumer.capacityTag);
            if (num > 0f && conduitConsumer.capacityTag != GameTags.Any && !flag) {
                Trigger(-794517298, new BuildingHP.DamageSourceInfo {
                    damage = 1,
                    source = BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
                    popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
                });
            }

            if (flag || conduitConsumer.wrongElementResult == ConduitConsumer.WrongElementResult.Store || contents.element == SimHashes.Vacuum || conduitConsumer.capacityTag == GameTags.Any) {
                if (!(num > 0f)) {
                    return;
                }

                consumedLastTick = true;
                int disease_count = (int)(contents.diseaseCount * (num / contents.mass));
                Element element2 = ElementLoader.FindElementByHash(contents.element);
                switch (conduitConsumer.conduitType) {
                    case ConduitType.Liquid:
                        if (element2.IsLiquid) {
                            storage.AddLiquid(contents.element, num, contents.temperature, contents.diseaseIdx, disease_count, keepZeroMassObject, do_disease_transfer: false);
                        } else {
                            Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element2.id);
                        }

                        break;
                    case ConduitType.Gas:
                        if (element2.IsGas) {
                            storage.AddGasChunk(contents.element, num, contents.temperature, contents.diseaseIdx, disease_count, keepZeroMassObject, do_disease_transfer: false);
                        } else {
                            Debug.LogWarning("Gas conduit consumer consuming non gas: " + element2.id);
                        }

                        break;
                }
            } else if (num > 0f) {
                consumedLastTick = true;
                if (conduitConsumer.wrongElementResult == ConduitConsumer.WrongElementResult.Dump) {
                    int disease_count2 = (int)(contents.diseaseCount * (num / contents.mass));
                    SimMessages.AddRemoveSubstance(Grid.PosToCell(base.transform.GetPosition()), contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num, contents.temperature, contents.diseaseIdx, disease_count2);
                }
            }
        }

        public ConduitContents RemoveElement(ConduitContents contents, float delta) {
            float num = Mathf.Min(contents.mass, delta);
            float num2 = contents.mass - num;
            if (num2 <= 0f) {
                return contents;
            }

            ConduitContents result = contents;
            result.RemoveMass(num2);
            int num3 = (int)(num2 / contents.mass * contents.diseaseCount);
            result.diseaseCount = contents.diseaseCount - num3;
            ConduitContents contents2 = contents;
            contents2.RemoveMass(num);
            contents2.diseaseCount = num3;
            if (num3 <= 0) {
                contents2.diseaseIdx = byte.MaxValue;
                contents2.diseaseCount = 0;
            }
            return result;
        }

    }
}
