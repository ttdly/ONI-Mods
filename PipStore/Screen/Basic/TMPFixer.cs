using TMPro;
using UnityEngine;

#pragma warning disable CS0649

namespace PipStore.Screen.Basic //Source: Aki
{
    // There is an issue with how TMP imports itself and alignment has to be reapplied
    internal class TMPFixer : KMonoBehaviour {
        [SerializeField]
        public TextAlignmentOptions alignment;

        [MyCmpReq] private LocText text;

        protected override void OnSpawn() {
            base.OnSpawn();
            text.alignment = alignment;
            Destroy(this);
        }
    }
}