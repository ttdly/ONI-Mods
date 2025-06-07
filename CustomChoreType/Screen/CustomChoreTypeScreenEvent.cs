using UnityEngine.EventSystems;

namespace CustomChoreType.Screen {
    public class CustomChoreTypeScreenEvent : KMonoBehaviour, IPointerExitHandler{
        public void OnPointerExit(PointerEventData eventData) {
            CustomChoreTypeScreen.Hide();
        }
    }
}