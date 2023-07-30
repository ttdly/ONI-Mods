using KSerialization;
using UnityEngine;

namespace LuckyChallenge {
    public class Manual : KMonoBehaviour, ISidescreenButtonControl {
        [Serialize]
        private bool isMarkForSurvey;
        string ISidescreenButtonControl.SidescreenButtonText => ButtonText();

        string ISidescreenButtonControl.SidescreenButtonTooltip => ButtonToolTip();

        private string ButtonText() {
            if (this.isMarkForSurvey) {
                return "NOOOO";
            } else {
                return "YEEEEEEEEEEEEES";
            }
        }

        private string ButtonToolTip() {
            if (this.isMarkForSurvey) {
                return "NO";
            } else {
                return "YES";
            }
        }

        int ISidescreenButtonControl.ButtonSideScreenSortOrder() => 20;

        int ISidescreenButtonControl.HorizontalGroupID() => -1;

        void ISidescreenButtonControl.OnSidescreenButtonPressed() {
            this.Toggle();
        }

        void ISidescreenButtonControl.SetButtonTextOverride(ButtonMenuTextOverride textOverride) {
            throw new System.NotImplementedException();
        }

        bool ISidescreenButtonControl.SidescreenButtonInteractable() => !this.gameObject.HasTag("Surveyed");


        bool ISidescreenButtonControl.SidescreenEnabled() => true;

        private void Toggle() {
            this.isMarkForSurvey = !this.isMarkForSurvey;
            this.Refresh();
        }

        private void Refresh() {
            if (KMonoBehaviour.isLoadingScene)
                return;
            if (this.isMarkForSurvey) {
                CreateBeacon();
            }
        }

        private void CreateBeacon() {
            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)GiftConfig.ID), Grid.CellToPos(Grid.PosToCell(this.gameObject)), Grid.SceneLayer.Creatures, name: this.gameObject.name);
            go.GetComponent<KBatchedAnimController>().Play("idle_"+new System.Random().Next(1,5));
            go.SetActive(true);
            this.isMarkForSurvey = false;
        }
    }
}
