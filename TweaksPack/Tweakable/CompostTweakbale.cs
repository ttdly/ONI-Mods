using TweaksPack.Auto;

namespace TweaksPack.Tweakable {
    public class CompostTweakbale : ComplexFabricatorTweakable{
        protected override void ToogleTweak() {
            base.ToogleTweak();
            Destroy(GetComponent<Compost>());
            gameObject.AddOrGet<CompostAuto>().simulatedInternalTemperature = 348.15f;
        }
    }
}
