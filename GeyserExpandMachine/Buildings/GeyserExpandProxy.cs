using System;
using System.Collections.Generic;
using PeterHan.PLib.Core;
using UnityEngine;

namespace GeyserExpandMachine.Buildings {
    public class GeyserExpandProxy : KMonoBehaviour, ISim1000ms{
        private int thisCell;
        public Storage storage;
        public ElementConverter.OutputElement outputElement;
        public bool close = true;
        private Geyser geyser;

        protected override void OnSpawn() {
            base.OnSpawn();

            thisCell = Grid.PosToCell(this);
            ModData.Instance.GeyserExpandProxies.Add(thisCell, this);
            var cell = Grid.PosToCell(this);
            var geyserFeature = Grid.Objects[cell, (int)ObjectLayer.Building];
            var geyserComponent = geyserFeature.GetComponent<Geyser>();
            if (geyserComponent == null) {
                PUtil.LogError($"Geyser component not found: {geyserFeature}");
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
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            ModData.Instance.GeyserExpandProxies.Remove(thisCell);
        }
        
        public void Sim1000ms(float dt) {
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