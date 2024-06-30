using System;
using System.Collections.Generic;
using HarmonyLib;
using KSerialization;
using PeterHan.PLib.Core;
using TUNING;
using BUILDINGS = STRINGS.BUILDINGS;

namespace AutomaticGeyser {
  public class GeyserLogic : Workable, ISidescreenButtonControl {
    private static readonly EventSystem.IntraObjectHandler<GeyserLogic> OnLogicValueChangedDelegate =
      new EventSystem.IntraObjectHandler<GeyserLogic>(
        (component, data) => component.OnLogicValueChanged(data));

    [Serialize] public bool addedLogicPorts;
    private Chore chore;
    private Geyser geyser;
    [Serialize] private bool markForAddLogicPorts;
    [Serialize] private int skipEruptionTimes;
    public int savedLogicValue;
    private LogicPorts ports;

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
      overrideAnims = new KAnimFile[1] {
        Assets.GetAnim((HashedString)"anim_use_machine_kanim")
      };
      faceTargetWhenWorking = true;
      synchronizeAnims = false;
      workerStatusItem = Db.Get().DuplicantStatusItems.Studying;
      resetProgressOnStop = false;
      requiredSkillPerk = Db.Get().SkillPerks.CanStudyWorldObjects.Id;
      attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
      attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
      skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
      skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
      SetWorkTime(3600f);
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      if (!addedLogicPorts &&!markForAddLogicPorts) return;
      Refresh();
      if (!addedLogicPorts) return;
      Subscribe(-801688580, OnLogicValueChangedDelegate);
      var output = new List<LogicPorts.Port> {
        LogicPorts.Port.RibbonOutputPort("GeyserLogicOutput", new CellOffset(1, 0),
          BUILDINGS.PREFABS.LOGICRIBBONWRITER.LOGIC_PORT_OUTPUT,
          BUILDINGS.PREFABS.LOGICRIBBONWRITER.OUTPUT_PORT_ACTIVE,
          BUILDINGS.PREFABS.LOGICRIBBONWRITER.OUTPUT_PORT_INACTIVE, true)
      };
      var input = new List<LogicPorts.Port> {
        LogicPorts.Port.RibbonInputPort("GeyserLogicInput", new CellOffset(0, 0),
          BUILDINGS.PREFABS.PIXELPACK.LOGIC_PORT,
          BUILDINGS.PREFABS.PIXELPACK.INPUT_PORT_ACTIVE,
          BUILDINGS.PREFABS.PIXELPACK.INPUT_PORT_INACTIVE)
      };
      ports = gameObject.AddOrGet<LogicPorts>();
      ports.outputPortInfo = output.ToArray();
      ports.inputPortInfo = input.ToArray();
      geyser = gameObject.GetComponent<Geyser>();
    }
    
    public void OnLogicValueChanged(object data) {
      var logicValueChanged = (LogicValueChanged)data;
      if (logicValueChanged.portID != "GeyserLogicInput")
        return;
      savedLogicValue = logicValueChanged.newValue;
      // 信号由二进制组成，value 会转成十进制 范围：0000-1111
      SwitchStage(logicValueChanged.newValue);
    }

    public void SwitchStage(int logicValue) {
      skipEruptionTimes++;
      if (LogicCircuitNetwork.IsBitActive(3, logicValue)) {
        geyser.ShiftTimeTo(Geyser.TimeShiftStep.PreviousIteration);
        PUtil.LogDebug($"信号 {logicValue} : 4");
        return;
      }

      if (LogicCircuitNetwork.IsBitActive(2, logicValue)) {
        geyser.ShiftTimeTo(Geyser.TimeShiftStep.NextIteration);
        PUtil.LogDebug($"信号 {logicValue} : 3");
        return;
      }

      if (LogicCircuitNetwork.IsBitActive(1, logicValue)) {
        geyser.ShiftTimeTo(Geyser.TimeShiftStep.DormantState);
        PUtil.LogDebug($"信号 {logicValue} : 2");
        return;
      }

      if (LogicCircuitNetwork.IsBitActive(0, logicValue)) {
        geyser.ShiftTimeTo(Geyser.TimeShiftStep.ActiveState);
        PUtil.LogDebug($"信号 {logicValue} : 1");
      }
    }
    
    #region Workable

    public void Refresh() {
      if (isLoadingScene)
        return;
      var kSelectable = GetComponent<KSelectable>();
      if (addedLogicPorts) {
        // this.statusItemGuid = kSelectable.ReplaceStatusItem(this.statusItemGuid, Db.Get().MiscStatusItems.Studied);
        // this.studiedIndicator.gameObject.SetActive(true);
        // this.studiedIndicator.meterController.Play((HashedString) this.meterAnim, KAnim.PlayMode.Loop);
        // this.requiredSkillPerk = (string) null;
        // UpdateStatusItem();
      } else {
        if (markForAddLogicPorts) {
          if (chore == null)
            chore = new WorkChore<GeyserLogic>(Db.Get().ChoreTypes.Build, this, only_when_operational: false);
          // this.statusItemGuid = kSelectable.ReplaceStatusItem(this.statusItemGuid, Db.Get().MiscStatusItems.AwaitingStudy);
        } else {
          CancelChore();
          // this.statusItemGuid = kSelectable.RemoveStatusItem(this.statusItemGuid);
        }
        // this.studiedIndicator.gameObject.SetActive(false);
      }
    }

    private void ToggleChore() {
      if (DebugHandler.InstantBuildMode) {
        AddLogicPort();
        if (chore == null) return;
        chore.Cancel("debug");
        chore = null;
      }

      markForAddLogicPorts = !markForAddLogicPorts;
      Refresh();
    }

    private void CancelChore() {
      if (chore == null)
        return;
      chore.Cancel("AddLogicPorts.CancelChore");
      chore = null;
      Trigger(1488501379);
    }

    protected override void OnCompleteWork(Worker curr_worker) {
      base.OnCompleteWork(curr_worker);
      chore = null;
      AddLogicPort();
    }

    private void AddLogicPort() {
      // 克隆一份
      var cloned = Util.KInstantiate(Assets.GetPrefab(gameObject.PrefabID()));
      // 同步研究状态
      if (gameObject.GetComponent<Studyable>().Studied)
        AccessTools.Field(typeof(Studyable), "studied").SetValue(cloned.AddOrGet<Studyable>(), true);
      // 同步名称以及泉水配置
      if (gameObject.TryGetComponent(out UserNameable userNameable))
        cloned.AddOrGet<UserNameable>().savedName = userNameable.savedName;
      if (!gameObject.TryGetComponent(out Geyser origin_geyser)) return;
      var clonedGeyser = cloned.AddOrGet<Geyser>();
      clonedGeyser.configuration = origin_geyser.configuration;
      cloned.AddOrGet<GeyserLogic>().addedLogicPorts = true;
      // 设置克隆体位置
      cloned.transform.SetPosition(gameObject.transform.position);
      cloned.SetActive(true);
      gameObject.DeleteObject();
    }

    #endregion
    
    #region SideButton

    public bool SidescreenEnabled() {
      return true;
    }

    public bool SidescreenButtonInteractable() {
      return !addedLogicPorts;
    }

    public int HorizontalGroupID() {
      return -1;
    }

    public int ButtonSideScreenSortOrder() {
      return 21;
    }

    // public string SidescreenTitleKey => "STRINGS.UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.TITLE";

    public void SetButtonTextOverride(ButtonMenuTextOverride textOverride) {
      return;
    }

    public void OnSidescreenButtonPressed() {
      ToggleChore();
    }

    public string SidescreenButtonText {
      get {
        if (addedLogicPorts) return "端口已添加";
        return markForAddLogicPorts ? "取消端口添加" : "添加端口";
      }
    }

    public string SidescreenButtonTooltip => "提示信息";

    #endregion
  }
}