namespace TweaksPack.Tweakable {
  public class ComplexFabricatorTweakable : OnceTweakable {
    protected override void OnSpawn() {
      base.OnSpawn();
      SetWorkTime(TweakableStaticVars.WorkTime.Auto);
    }

    protected override void ToogleTweak() {
      base.ToogleTweak();
      if (isTweaked)
        if (gameObject.GetComponent<ComplexFabricator>() != null)
          gameObject.GetComponent<ComplexFabricator>().duplicantOperated = false;
    }
  }
}