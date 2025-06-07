using UnityEngine;
using UnityEngine.UI;

namespace CustomChoreType.Screen {
    public class ChoreGroupEntry : KMonoBehaviour {
        public bool selected;
        public ChoreGroup ChoreGroup;
        public Toggle toggle;

        public override string ToString() {
            return $"Selected: {selected}; choreGroup: {ChoreGroup.Name}; toggle: {toggle==null}";
        }
    }
}