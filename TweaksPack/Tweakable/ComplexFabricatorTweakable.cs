﻿namespace TweaksPack.Tweakable {
    public class ComplexFabricatorTweakable : OnceTweakable{
        protected override void ToogleTweak() {
            base.ToogleTweak();
            if (isTweaked) {
                if (gameObject.GetComponent<ComplexFabricator>() != null) { gameObject.GetComponent<ComplexFabricator>().duplicantOperated = false; }
            }
        }
        protected override void OnFetchComplete() {
            base.OnFetchComplete();
            SetWorkTime(TweakableStaticVars.WorkTime.Auto);
        }
    }
}
