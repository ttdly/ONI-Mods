using KSerialization;
using System.Collections.Generic;
using UnityEngine;

namespace TweaksPack.Tweakable {
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ComplexFabricatorTweakable : BaseTweakable {
        [Serialize]
        public bool isTweaked = false;

        protected override void OnSpawn() {
            base.OnSpawn();
            materialNeeds = new Dictionary<Tag, float>() {
                { SimHashes.Glass.CreateTag(), 200f },
                { SimHashes.Steel.CreateTag(), 100f }
            };
            ToogleTweak();
        }

        protected virtual void ToogleTweak() {
            if (isTweaked) {
                gameObject.AddTag("NotShowTweak");
                KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
                if (component != null) component.TintColour = new Color32(190, 255, 190, 255);
                if (gameObject.GetComponent<ComplexFabricator>() != null) { gameObject.GetComponent<ComplexFabricator>().duplicantOperated = false; }
            }
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            isTweaked = true;
            ToogleTweak();
        }

        protected override void OnFetchComplete() {
            base.OnFetchComplete();
            SetWorkTime(100f);
        }
    }
}
