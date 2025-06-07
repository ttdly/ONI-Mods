using CustomChoreType.Screen;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CustomChoreType {
    public class ChoreLabelEvents: KMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        private ChoreType thisChoreType;
        private LocText targetChoreLabel;
        private Color originalColor;
        
        public void Initialize(ChoreType choreType, LocText choreLabel) {
            thisChoreType = choreType;
            targetChoreLabel = choreLabel;
            originalColor = choreLabel.color;
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            CustomChoreTypeScreen.Show(thisChoreType);
        }
        
        public void OnPointerEnter(PointerEventData eventData) {
            targetChoreLabel.color = new Color(0.4980392f, 0.2392157f, 0.3686275f);
        }
        
        public void OnPointerExit(PointerEventData eventData) {
            targetChoreLabel.color = originalColor;
        }
    }
}