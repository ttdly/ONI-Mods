using System.Collections.Generic;
using PeterHan.PLib.Core;
using STRINGS;

namespace AutomaticGeyser {
  public class GeyserLogic : KMonoBehaviour {
    private static readonly EventSystem.IntraObjectHandler<GeyserLogic> OnLogicValueChangedDelegate =
      new EventSystem.IntraObjectHandler<GeyserLogic>(
        (component, data) => component.OnLogicValueChanged(data));

    private Geyser geyser;

    private LogicPorts ports;

    protected override void OnSpawn() {
      base.OnSpawn();

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

      // 信号由二进制组成，value 会转成十进制 范围：0000-1111
      SwitchStage(logicValueChanged.newValue);
    }

    public void SwitchStage(int logicValue) {
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
  }
}