using System;
using KSerialization;

namespace LuckyChallenge {
  public class Manual : KMonoBehaviour, ISidescreenButtonControl {
    [Serialize] private bool isMarkForSurvey;

    string ISidescreenButtonControl.SidescreenButtonText => ButtonText();

    string ISidescreenButtonControl.SidescreenButtonTooltip => ButtonToolTip();

    int ISidescreenButtonControl.ButtonSideScreenSortOrder() {
      return 20;
    }

    int ISidescreenButtonControl.HorizontalGroupID() {
      return -1;
    }

    void ISidescreenButtonControl.OnSidescreenButtonPressed() {
      Toggle();
    }

    void ISidescreenButtonControl.SetButtonTextOverride(ButtonMenuTextOverride textOverride) {
      throw new NotImplementedException();
    }

    bool ISidescreenButtonControl.SidescreenButtonInteractable() {
      return !gameObject.HasTag("Surveyed");
    }


    bool ISidescreenButtonControl.SidescreenEnabled() {
      return true;
    }

    private string ButtonText() {
      if (isMarkForSurvey)
        return "NOOOO";
      return "YEEEEEEEEEEEEES";
    }

    private string ButtonToolTip() {
      if (isMarkForSurvey)
        return "NO";
      return "YES";
    }

    private void Toggle() {
      isMarkForSurvey = !isMarkForSurvey;
      Refresh();
    }

    private void Refresh() {
      if (isLoadingScene)
        return;
      if (isMarkForSurvey) CreateBeacon();
    }

    private void CreateBeacon() {
      var go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)GiftConfig.ID), Grid.CellToPos(Grid.PosToCell(gameObject)),
        Grid.SceneLayer.Creatures, gameObject.name);
      go.GetComponent<KBatchedAnimController>().Play("idle_" + new Random().Next(1, 5));
      go.SetActive(true);
      isMarkForSurvey = false;
    }
  }
}