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
    // TODO 在某一状态过程中跳过
    // TODO 技能修改
    // TODO 只有分析之后才能添加端口
    [Serialize] public bool addedLogicPorts;
    private Chore chore;
    private Geyser geyser;
    [Serialize] private bool markForAddLogicPorts;
    [Serialize] public int skipEruptTimes;
    public int savedLogicValue;
    private GeyserLogicStatus.InputLogic geyserLogicStatus;
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
      requiredSkillPerk = Db.Get().SkillPerks.CanStudyWorldObjects.Id;
      attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
      attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
      skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
      skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
      SetWorkTime(3600f);
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      if (!addedLogicPorts && !markForAddLogicPorts) return;
      Refresh();
      if (!addedLogicPorts) return;
      Subscribe(-801688580, OnLogicValueChangedDelegate);
      var output = new List<LogicPorts.Port> {
        LogicPorts.Port.RibbonOutputPort(OutputId, new CellOffset(1, 0),
          "输出间歇泉状态",
          "按位描述，1 为绿色 0 为红色\n信号输出 0000：闲置/休眠\n信号输出 1000：压力上升\n信号输出 0100：喷发\n信号输出 0010：喷发结束\n信号输出 0001：压力过大",
          ""),
        LogicPorts.Port.OutputPort(SkipTimesOutputId, new CellOffset(0, 1),
          "是否可以跳过休眠/闲置状态",
          "至少可以跳过一次休眠/闲置状态",
          "无法跳过休眠/闲置状态"
        )
      };
      var input = new List<LogicPorts.Port> {
        LogicPorts.Port.RibbonInputPort(InputId, new CellOffset(0, 0),
          "输入以控制间歇泉行为",
          "按位描述，1 为绿色 0 为红色\n信号输入 1000：跳过喷发模式\n信号输入 0100：跳过闲置/休眠模式",
          "")
      };
      ports = gameObject.AddOrGet<LogicPorts>();
      ports.outputPortInfo = output.ToArray();
      ports.inputPortInfo = input.ToArray();
      geyser = gameObject.GetComponent<Geyser>();
      geyserState = geyser.GetSMI<Geyser.StatesInstance>();
      study = geyser.GetComponent<Studyable>();
    }

    public void OnLogicValueChanged(object data) {
      var logicValueChanged = (LogicValueChanged)data;
      if (logicValueChanged.portID != "GeyserLogicInput")
        return;
      savedLogicValue = logicValueChanged.newValue;
      geyserLogicStatus = GeyserLogicStatus.GetInputLogic(logicValueChanged.newValue);
      RefreshStatus();
    }

    #region LogicControl

    private void RefreshStatus() {
      var selectable = gameObject.GetComponent<KSelectable>();
      switch (geyserLogicStatus) {
        case GeyserLogicStatus.InputLogic.SkipErupt:
          statusItemGuid = selectable.ReplaceStatusItem(statusItemGuid, GeyserLogicStatus.SkipEruptStatusItem, this);
          break;
        case GeyserLogicStatus.InputLogic.SkipDormant:
          statusItemGuid = selectable.ReplaceStatusItem(statusItemGuid, GeyserLogicStatus.SkipDormantStatusItem, this);
          break;
        case GeyserLogicStatus.InputLogic.UnusedCase:
        default:
          statusItemGuid = selectable.RemoveStatusItem(statusItemGuid);
          break;
      }
    }

    public void SkipErupt() {
      if (!addedLogicPorts || ports == null || geyserLogicStatus != GeyserLogicStatus.InputLogic.SkipErupt) return;
      geyser.AlterTime(geyser.timeShift + geyser.RemainingEruptTime());
      geyserState.GoTo(geyserState.sm.post_erupt);
      skipEruptTimes++;
      ports.SendSignal(SkipTimesOutputId, 1);
    }

    public bool SkipIdle() {
      if (!addedLogicPorts || ports == null) return true;
      if (geyserLogicStatus != GeyserLogicStatus.InputLogic.SkipDormant || skipEruptTimes <= 0) return false;
      if (geyser.ShouldGoDormant()) {
        geyserState.GoTo(geyserState.sm.dormant);
        return true;
      }

      geyser.AlterTime(geyser.timeShift + geyser.RemainingIdleTime());
      geyserState.GoTo(geyserState.sm.pre_erupt);
      skipEruptTimes--;
      if (skipEruptTimes > 0) return true;
      skipEruptTimes = 0;
      ports.SendSignal(SkipTimesOutputId, skipEruptTimes);
      return true;
    }

    public bool SkipDormant() {
      if (!addedLogicPorts || ports == null) return true;
      if (geyserLogicStatus != GeyserLogicStatus.InputLogic.SkipDormant || skipEruptTimes <= 0) return false;
      geyser.AlterTime(geyser.timeShift + geyser.RemainingDormantTime());
      geyserState.GoTo(geyserState.sm.erupt);
      skipEruptTimes--;
      if (skipEruptTimes > 0) return true;
      skipEruptTimes = 0;
      ports.SendSignal(SkipTimesOutputId, skipEruptTimes);
      return true;
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
      return !addedLogicPorts;
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
        if (addedLogicPorts) return "自动化端口已添加";
        return markForAddLogicPorts ? "取消端口添加" : "添加端口";
      }
    }

    public string SidescreenButtonTooltip {
      get {
        if (addedLogicPorts) return "无需任何操作";
        return markForAddLogicPorts ? "取消添加端口的任务" : "添加自动化端口之后可以通过信号线控制间歇泉的行为\n或是获取间歇泉状态信息";
      }
    }

    #endregion
  }
}