using UnityEngine;
using TweaksPack.Auto;

namespace TweaksPack.Tweakable {
    public class OilWellCapTweakable : ComplexFabricatorTweakable{
        protected override void ToogleTweak() {
            base.ToogleTweak();
            if (isTweaked) {
                Destroy(GetComponent<OilWellCap>());
                OilWellCapAuto oilWellCapAuto = gameObject.AddOrGet<OilWellCapAuto>();
                oilWellCapAuto.gasElement = SimHashes.Methane;
                oilWellCapAuto.gasTemperature = 573.15f;
                oilWellCapAuto.addGasRate = 0.0333333351f;
                oilWellCapAuto.maxGasPressure = 80.00001f;
                oilWellCapAuto.releaseGasRate = 0.444444478f;
                KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
                if (component != null) component.TintColour = new Color32(190, 255, 190, 255);
            }
        }
    }
}
