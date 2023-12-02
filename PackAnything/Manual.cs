using KSerialization;

namespace PackAnything {
    public class Manual : Surveyable, ISidescreenButtonControl {
        [Serialize]
        private bool isMarkForSurvey;

        protected override void OnSpawn() {
            base.OnSpawn();
            if (isSurveyed) {
                PackAnythingStaticVars.SurveableCmps.Add(this);
            }
        }

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

        bool ISidescreenButtonControl.SidescreenButtonInteractable() => !isSurveyed;

        bool ISidescreenButtonControl.SidescreenEnabled() => true;

        private void Toggle() {
            isMarkForSurvey = !isMarkForSurvey;
            Refresh();
        }

        private void Refresh() {
            if (isLoadingScene)
                return;
            if (isMarkForSurvey) {
                isSurveyed = true;
                PackAnythingStaticVars.SurveableCmps.Add(this);
            }
        }

    }
}
