using TweaksPack.Auto;

namespace TweaksPack.Tweakable {
    internal class OilRefineryTweakable : OnceTweakable{
        protected override void ToogleTweak() {
            base.ToogleTweak();
            if (isTweaked) {
                Destroy(GetComponent<OilRefinery>());
                OilRefineryAuto oilRefineryAuto = gameObject.AddOrGet<OilRefineryAuto>();
                oilRefineryAuto.overpressureWarningMass = 4.5f;
                oilRefineryAuto.overpressureMass = 5f;
            }
        }
    }
}
