using System;
using UnityEngine;

namespace GeyserExpandMachine.Buildings {
    public class GeyserExpandProxy : KMonoBehaviour, ISim1000ms{
        public Storage storage;
        public ElementConverter.OutputElement outputElement;
        public bool close = true;
        private Geyser geyser;
        private bool safe;
        private int thisCell;

        protected override void OnSpawn() {
            base.OnSpawn();
            safe = false;
            var cell = Grid.PosToCell(this);
            // ModData.Instance.BaseGeyserExpands.Add(cell, this);
            thisCell = cell;

            var geyserFeature = Grid.Objects[cell, (int)ObjectLayer.Building];
            if (geyserFeature == null) {
                LogUtil.Error($"没有在指定的位置找到实体 {cell}");
                return;
            }

            var geyserComponent = geyserFeature.GetComponent<Geyser>();
            if (geyserComponent == null) {
                LogUtil.Error($"没有在指定的位置找到间歇泉；实体：{geyserFeature}；");
                return;
            }

            geyser = geyserComponent;
            outputElement = new ElementConverter.OutputElement(
                geyser.configuration.GetEmitRate(),
                geyser.configuration.GetElement(),
                geyser.configuration.GetTemperature(),
                storeOutput: true,
                addedDiseaseIdx: geyser.configuration.GetDiseaseIdx(),
                addedDiseaseCount: Mathf.RoundToInt(
                    geyser.configuration.GetDiseaseCount() * geyser.configuration.GetEmitRate())
            );
            storage = GetComponent<Storage>();
            safe = true;
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            ModData.Instance.BaseGeyserExpands.Remove(thisCell);
        }
        
        public void Sim1000ms(float dt) {
            if (!safe) return;
            if (close || storage == null || outputElement.elementHash == 0) return;

            if (ElementLoader.FindElementByHash(outputElement.elementHash).IsLiquid) {
                storage.AddLiquid(
                    outputElement.elementHash, 
                    outputElement.massGenerationRate, 
                    outputElement.minOutputTemperature,
                    outputElement.addedDiseaseIdx,
                    outputElement.addedDiseaseCount);
            }
            else {
                storage.AddGasChunk(
                    outputElement.elementHash,
                    outputElement.massGenerationRate,
                    outputElement.minOutputTemperature,
                    outputElement.addedDiseaseIdx,
                    outputElement.addedDiseaseCount,
                    false
                );
            }
        }
    }
}