using UnityEngine;
using TweaksPack.Auto;

namespace TweaksPack.Tweakable {
    public class OilWellCapTweakable : OnceTweakable{
        protected override void ToogleTweak() {
            base.ToogleTweak();
            if (isTweaked) {
                gameObject.AddOrGet<OilWellCapAuto>().duplicantOperated = false;
            }
        }
    }
}
