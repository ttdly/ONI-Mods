using Klei;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using UnityEngine;

namespace WirelessProject.ConduitManger {
    public class ConduitDispenserProxy :  KMonoBehaviour{
        [MyCmpGet]
        readonly Operational operational;
        [MyCmpGet]
        readonly Storage storage;
        [MyCmpGet]
        private readonly float MaxMass = 10f;
        readonly ConduitDispenser dispenser;
        public SimHashes[] elementFilter;
        private ConduitType conduitType;
        private bool invertElementFilter = false;
        private bool alwaysDispense;
        private bool empty = true;
        private bool blocked;
        private int elementOutputOffset;
        public int proxyListId = -2;
        public int proxyListIndex = -1;

        protected override void OnSpawn() {
            base.OnSpawn();
            elementFilter = dispenser.elementFilter;
            conduitType = dispenser.conduitType;
            invertElementFilter = dispenser.invertElementFilter;
            alwaysDispense = dispenser.alwaysDispense;
        }

        public void ConduitUpdate(float dt) {
            //operational.SetFlag(ConduitDispenser, IsConnected);
            blocked = false;
            Dispense();
        }

        private void Dispense() {
            if (!operational.IsOperational && !alwaysDispense) {
                return;
            }

            PrimaryElement primaryElement = FindSuitableElement();
            if (primaryElement != null) {
                primaryElement.KeepZeroMassObject = true;
                empty = false;
                float num = AddElement(proxyListId, proxyListIndex, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                if (num > 0f) {
                    int num2 = (int)(num / primaryElement.Mass * primaryElement.DiseaseCount);
                    primaryElement.ModifyDiseaseCount(-num2, "ConduitDispenser.ConduitUpdate");
                    primaryElement.Mass -= num;
                    storage.Trigger(GameHashes.OnStorageChange.GetHashCode(), primaryElement.gameObject);
                } else {
                    blocked = true;
                }
            } else {
                empty = true;
            }
        }

        private bool IsFilteredElement(SimHashes element) {
            for (int i = 0; i != elementFilter.Length; i++) {
                if (elementFilter[i] == element) {
                    return true;
                }
            }
            return false;
        }

        private PrimaryElement FindSuitableElement() {
            List<GameObject> items = storage.items;
            int count = items.Count;
            for (int i = 0; i < count; i++) {
                int index = (i + elementOutputOffset) % count;
                PrimaryElement component = items[index].GetComponent<PrimaryElement>();
                // 检查组件是否存在且质量大于0
                if (component == null || component.Mass <= 0f) continue;
                // 检查元素类型是否与输送系统类型相匹配
                if ((conduitType == ConduitType.Liquid) ? component.Element.IsLiquid : component.Element.IsGas) continue;
                // 检查元素是否通过过滤器条件
                bool isFiltered = 
                    elementFilter == null || 
                    elementFilter.Length == 0 || 
                    (!invertElementFilter && IsFilteredElement(component.ElementID)) || 
                    (invertElementFilter && !IsFilteredElement(component.ElementID));
                if (!isFiltered) continue;
                elementOutputOffset = (elementOutputOffset + 1) % count;
                return component;
            }
            return null;
        }

        public float AddElement(int proxy_list_id, int index, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count) {

            if (!StaticVar.GetContents(proxy_list_id, index, out ConduitFlow.ConduitContents contents)) {
                return 0f;
            }
            if (contents.element != element && contents.element != SimHashes.Vacuum && mass > 0f) {
                return 0f;
            }

            float num = Mathf.Min(mass, MaxMass - contents.mass);
            float num2 = num / mass;
            if (num <= 0f) {
                return 0f;
            }

            contents.temperature = GameUtil.GetFinalTemperature(temperature, num, contents.temperature, contents.mass);
            contents.AddMass(num);
            contents.element = element;
            contents.ConsolidateMass();
            int num3 = (int)(num2 * disease_count);
            if (num3 > 0) {
                SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, num3, contents.diseaseIdx, contents.diseaseCount);
                contents.diseaseIdx = diseaseInfo.idx;
                contents.diseaseCount = diseaseInfo.count;
            }

            return num;
        }
    }
}
