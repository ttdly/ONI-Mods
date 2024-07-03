using System;
using System.Collections.Generic;
using HarmonyLib;
using KSerialization;
using TUNING;

namespace AutomaticGeyser {
  public class GeyserLogic : Workable, ISidescreenButtonControl {
    private static readonly EventSystem.IntraObjectHandler<GeyserLogic> OnLogicValueChangedDelegate =
      new EventSystem.IntraObjectHandler<GeyserLogic>(
        (component, data) => component.OnLogicValueChanged(data));
    
    [Serialize] public bool addedLogicPorts;
    private Chore chore;
    private Geyser geyser;
    [Serialize] private bool markForAddLogicPorts;
    [Serialize] public float skipEruptTimes;
    private GeyserLogicStatus.InputLogic currentInputLogic = GeyserLogicStatus.InputLogic.UnusedCase;
    private LogicPorts ports;
    private const string OutputId = "GeyserLogicOutput";
    private const string InputId = "GeyserLogicInput";
    private const string SkipTimesOutputId = "GeyserSkipEruptTimes";
    private Geyser.StatesInstance geyserState;
    private Guid statusItemGuid;
    private Studyable study;

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
      overrideAnims = new KAnimFile[1] {
        Assets.GetAnim((HashedString)"anim_use_machine_kanim")
      };
      faceTargetWhenWorking = true;
      synchronizeAnims = false;
      workerStatusItem = Db.Get().DuplicantStatusItems.Building;
      resetProgressOnStop = false;
      requiredSkillPerk = Db.Get().SkillPerks.IncreaseMachineryLarge.Id;
      attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
      attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
      skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
      skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
      SetWorkTime(3600f);
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      study = gameObject.GetComponent<Studyable>();
      if (!addedLogicPorts && !markForAddLogicPorts) return;
      Refresh();
      if (!addedLogicPorts) return;
      Subscribe(-801688580, OnLogicValueChangedDelegate);
      var output = new List<LogicPorts.Port> {
        LogicPorts.Port.RibbonOutputPort(OutputId, new CellOffset(1, 0),
          ModStrings.UI.Logic.GroupOutput_desc,
          ModStrings.UI.Logic.GroupOutput_active,
          ""),
        LogicPorts.Port.OutputPort(SkipTimesOutputId, new CellOffset(0, 1),
          ModStrings.UI.Logic.Output_desc,
          ModStrings.UI.Logic.Output_active,
          ModStrings.UI.Logic.Output_inactive
        )
      };
      var input = new List<LogicPorts.Port> {
        LogicPorts.Port.RibbonInputPort(InputId, new CellOffset(0, 0),
          ModStrings.UI.Logic.GroupInput_desc,
          ModStrings.UI.Logic.GroupInput_active,
          "")
      };
      GetGeyserAndState();
      ports = gameObject.AddOrGet<LogicPorts>();
      ports.outputPortInfo = output.ToArray();
      ports.inputPortInfo = input.ToArray();
    }

    private void GetGeyserAndState() {
      if (geyser != null && geyserState != null) return;
      geyser = gameObject.GetComponent<Geyser>();
      geyserState = geyser.GetSMI<Geyser.StatesInstance>();
    }

    public void OnLogicValueChanged(object data) {
      var logicValueChanged = (LogicValueChanged)data;
      if (logicValueChanged.portID != InputId)
        return;
      currentInputLogic = GeyserLogicStatus.GetInputLogic(logicValueChanged.newValue);
      Debug.Log($"收到信号：{logicValueChanged.newValue}");
      RefreshStatus();
    }

    #region LogicControl

    private void RefreshStatus() {
      var selectable = gameObject.GetComponent<KSelectable>();
      switch (currentInputLogic) {
        case GeyserLogicStatus.InputLogic.SkipErupt:
          statusItemGuid = selectable.ReplaceStatusItem(statusItemGuid, GeyserLogicStatus.SkipEruptStatusItem, this);
          break;
        case GeyserLogicStatus.InputLogic.SkipDormant:
          statusItemGuid = selectable.ReplaceStatusItem(statusItemGuid, GeyserLogicStatus.SkipDormantStatusItem, this);
          break;
        case GeyserLogicStatus.InputLogic.AlwaysDormant:
          statusItemGuid = selectable.ReplaceStatusItem(statusItemGuid, GeyserLogicStatus.AlwaysDormant);
          break;
        case GeyserLogicStatus.InputLogic.UnusedCase:
        default:
          statusItemGuid = selectable.RemoveStatusItem(statusItemGuid);
          break;
      }
    }

    public void SkipStage(
      GeyserLogicStatus.InputLogic wishInputLogic,
      StateMachine.BaseState fromSate,
      StateMachine.BaseState toState,
      float offsetTime,
      float times
    ) {
      if (!addedLogicPorts || ports == null) return;
      if (wishInputLogic != currentInputLogic) return;
      if (!geyserState.IsInsideState(fromSate)) return;
      if (skipEruptTimes + times < 0) return;
      geyser.AlterTime(offsetTime);
      geyserState.GoTo(toState);
      skipEruptTimes += times;
      ports.SendSignal(SkipTimesOutputId, skipEruptTimes > 0 ? 1 : 0);
    }

    public void SkipErupt() {
      GetGeyserAndState();
      SkipStage(
        GeyserLogicStatus.InputLogic.SkipErupt,
        geyserState.sm.erupt,
        geyserState.sm.post_erupt,
        geyser.timeShift + geyser.RemainingEruptTime(),
        1);
    }

    public void SkipIdle() {
      GetGeyserAndState();
      SkipStage(
        GeyserLogicStatus.InputLogic.SkipDormant,
        geyserState.sm.idle,
        geyserState.sm.pre_erupt,
        geyser.timeShift + geyser.RemainingIdleTime(),
        -1);
    }

    public void SkipDormant() {
      GetGeyserAndState();
      SkipStage(
        GeyserLogicStatus.InputLogic.SkipErupt,
        geyserState.sm.dormant,
        geyserState.sm.pre_erupt,
        geyser.timeShift + geyser.RemainingDormantTime(),
        -1);
    }

    public void AlwaysDormant() {
      GetGeyserAndState();
      SkipStage(
        GeyserLogicStatus.InputLogic.AlwaysDormant,
        geyserState.sm.pre_erupt,
        geyserState.sm.dormant,
        geyser.timeShift + geyser.RemainingActiveTime(),
        0);
    }

    public void SendStatus(GeyserLogicStatus.OutputLogic status) {
      if (!addedLogicPorts || ports == null) return;
      ports.SendSignal(OutputId, GeyserLogicStatus.GetLogicTrueValue(status));
    }

    #endregion

    #region Workable

    public void Refresh() {
      if (isLoadingScene)
        return;
      if (addedLogicPorts) {
        requiredSkillPerk = null;
      } else {
        if (markForAddLogicPorts) {
          if (chore == null)
            chore = new WorkChore<GeyserLogic>(Db.Get().ChoreTypes.Build, this, only_when_operational: false);
        } else {
          CancelChore();
        }
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
      return study.Studied && !addedLogicPorts;
    }

    public int HorizontalGroupID() {
      return -1;
    }

    public int ButtonSideScreenSortOrder() {
      return 21;
    }

    public void SetButtonTextOverride(ButtonMenuTextOverride textOverride) {
    }

    public void OnSidescreenButtonPressed() {
      ToggleChore();
    }

    public string SidescreenButtonText {
      get {
        if (!study.Studied) return ModStrings.UI.SideButton.NeedAnalyze;
        if (addedLogicPorts) return ModStrings.UI.SideButton.LogicAdded;
        return markForAddLogicPorts
          ? ModStrings.UI.SideButton.CancelAddLogic
          : ModStrings.UI.SideButton.MarkForAddLogic;
      }
    }

    public string SidescreenButtonTooltip {
      get {
        if (!study.Studied) return ModStrings.UI.SideButton.NeedAnalyze_Tip;
        if (addedLogicPorts) return ModStrings.UI.SideButton.LogicAdded_Tip;
        return markForAddLogicPorts
          ? ModStrings.UI.SideButton.CancelAddLogic_Tip
          : ModStrings.UI.SideButton.MarkForAddLogic_Tip;
      }
    }

    #endregion
  }
}