using KSerialization;
using UnityEngine;

namespace PackAnything {
    public class Manual : KMonoBehaviour, ISidescreenButtonControl {
        [Serialize]
        private bool isMarkForSurvey;
        string ISidescreenButtonControl.SidescreenButtonText => ButtonText();

        string ISidescreenButtonControl.SidescreenButtonTooltip => ButtonToolTip();

        private string ButtonText() {
            if (isMarkForSurvey) {
                return PackAnythingString.UI.SURVEY.NAME_OFF;
            } else {
                return PackAnythingString.UI.SURVEY.NAME;
            }
        }

        private string ButtonToolTip() {
            if (isMarkForSurvey) {
                return PackAnythingString.UI.SURVEY.TOOLTIP_OFF;
            } else {
                return PackAnythingString.UI.SURVEY.TOOLTIP;
            }
        }

        int ISidescreenButtonControl.ButtonSideScreenSortOrder() => 20;

        int ISidescreenButtonControl.HorizontalGroupID() => -1;

        void ISidescreenButtonControl.OnSidescreenButtonPressed() {
            Toggle();
        }

        void ISidescreenButtonControl.SetButtonTextOverride(ButtonMenuTextOverride textOverride) {
            throw new System.NotImplementedException();
        }

        bool ISidescreenButtonControl.SidescreenButtonInteractable() => !gameObject.HasTag("Surveyed");
        

        bool ISidescreenButtonControl.SidescreenEnabled() => true;

        private void Toggle() {
            isMarkForSurvey = !isMarkForSurvey;
            Refresh();
        }

        private void Refresh() {
            if (isLoadingScene)
                return;
            if (isMarkForSurvey) {
                CreateBeacon();
            }
        }

        private void CreateBeacon() {
            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)BeaconConfig.ID), Grid.CellToPos(Grid.PosToCell(gameObject)), Grid.SceneLayer.Creatures, name: gameObject.name);
            go.SetActive(true);
            Beacon becaon = go.GetComponent<Beacon>();
            becaon.originCell = Grid.PosToCell(gameObject);
            if (gameObject.HasTag(GameTags.GeyserFeature)) {
                becaon.isGeyser = true;
            }
            string name;
            if (becaon.isGeyser) {
                name = gameObject.name;
            } else {
                name = Strings.Get("STRINGS.BUILDINGS.PREFABS." + gameObject.name.Replace("Complete", "").ToUpper() + ".NAME");
            }
            go.FindOrAddComponent<UserNameable>().savedName = name;
            gameObject.AddTag("Surveyed");
            Toggle();
        }
    }
}
