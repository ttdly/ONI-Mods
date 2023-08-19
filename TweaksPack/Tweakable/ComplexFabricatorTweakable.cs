namespace TweaksPack.Tweakable 
{
    public class ComplexFabricatorTweakable : OnceTweakable{
        protected override void OnSpawn() {
            base.OnSpawn();
            materialNeeds = TweakableStaticVars.MaterialNeeds.ComplexFabricator;
        }

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
