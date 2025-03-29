using PeterHan.PLib.Core;

namespace PipeGeyser {
    public class PipeGeyser : KMonoBehaviour, ISim1000ms{

        public Storage storage;
            
        public ElementConverter.OutputElement outputElement;
        public bool close = true;
        


        public void Sim1000ms(float dt) {
            if (close || storage == null) return;
            
            if (ElementLoader.FindElementByHash(outputElement.elementHash).IsLiquid) {
                PUtil.LogDebug("执行液体");
                var result = storage.AddLiquid(
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

            foreach (var item in storage.items) {
                PUtil.LogDebug(item);
            }
        }
    }
}