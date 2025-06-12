using System;
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

        public void Refresh() {
            toggle.image.color = Color.white;
            toggle.isOn = false;
            selected = false;
        }

        public void Select() {
            toggle.image.color = Color.green;
            toggle.isOn = true;
            selected = true;
        }
    }
}