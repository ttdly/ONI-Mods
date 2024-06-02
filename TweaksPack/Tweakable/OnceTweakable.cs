using KSerialization;
using UnityEngine;

namespace TweaksPack.Tweakable {
  [SerializationConfig(MemberSerialization.OptIn)]
  public class OnceTweakable : BaseTweakable {
    [Serialize] public bool isTweaked;

    protected override void OnSpawn() {
      base.OnSpawn();
      ToogleTweak();
    }

    protected virtual void ToogleTweak() {
      if (isTweaked) {
        gameObject.AddTag(TweakableStaticVars.Tags.DontTweak);
        var component = gameObject.GetComponent<KBatchedAnimController>();
        if (component != null) component.TintColour = new Color32(220, 255, 220, 255);
      }
    }

    protected override void OnCompleteWork(Worker worker) {
      base.OnCompleteWork(worker);
      isTweaked = true;
      ToogleTweak();
    }
  }
}