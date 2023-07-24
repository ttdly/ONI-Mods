using KSerialization;
using UnityEngine;

namespace PackAnything {
    public class Manual : KMonoBehaviour, ISidescreenButtonControl {
        [Serialize]
        private bool isMarkForSurvey;
        string ISidescreenButtonControl.SidescreenButtonText => ButtonText();

        string ISidescreenButtonControl.SidescreenButtonTooltip => ButtonToolTip();

        private string ButtonText() {
            if (this.isMarkForSurvey) {
                return PackAnythingString.UI.SURVEY.NAME_OFF;
            } else {
                return PackAnythingString.UI.SURVEY.NAME;
            }
        }

        private string ButtonToolTip() {
            if (this.isMarkForSurvey) {
                return PackAnythingString.UI.SURVEY.TOOLTIP_OFF;
            } else {
                return PackAnythingString.UI.SURVEY.TOOLTIP;
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
            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)BeaconConfig.ID), Grid.CellToPos(Grid.PosToCell(this.gameObject)), Grid.SceneLayer.Creatures, name: this.gameObject.name);
            go.SetActive(true);
            Beacon becaon = go.GetComponent<Beacon>();
            becaon.originCell = Grid.PosToCell(this.gameObject);
            if (this.gameObject.HasTag(GameTags.GeyserFeature)) {
                becaon.isGeyser = true;
            }
            string name;
            if (becaon.isGeyser) {
                name = this.gameObject.name;
            } else {
                name = Strings.Get("STRINGS.BUILDINGS.PREFABS." + this.gameObject.name.Replace("Complete", "").ToUpper() + ".NAME");
            }
            go.FindOrAddComponent<UserNameable>().savedName = name;
            this.gameObject.AddTag("Surveyed");
            this.Toggle();
        }
    }
}
