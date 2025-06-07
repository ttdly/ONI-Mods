using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace CustomChoreType.Screen //Source: Aki
{
	// There is an issue with how TMP imports itself and alighnment has to be reapplied
	class TMPFixer : KMonoBehaviour
	{
		[SerializeField]
		public TextAlignmentOptions alignment;

		[MyCmpReq]
		private LocText text;

		protected override void OnSpawn()
		{
			base.OnSpawn();
			text.alignment = alignment;
			Destroy(this);
		}
	}
}
