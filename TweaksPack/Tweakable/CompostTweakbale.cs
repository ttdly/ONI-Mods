using TweaksPack.Auto;

namespace TweaksPack.Tweakable {
    public class CompostTweakbale : OnceTweakable{
        protected override void ToogleTweak() {
            base.ToogleTweak();
            if (isTweaked) {
                Destroy(GetComponent<Compost>());
                gameObject.AddOrGet<CompostAuto>().simulatedInternalTemperature = 348.15f;
            }
        }
    }
}
