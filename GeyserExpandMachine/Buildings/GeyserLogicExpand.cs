using GeyserExpandMachine.GeyserModify;
using KSerialization;
using PeterHan.PLib.Core;
using UnityEngine;


namespace GeyserExpandMachine.Buildings;

public class GeyserLogicExpand : KMonoBehaviour{
    
    private GeyserLogicController controller;
    public HashedString portID;
    public HashedString ribbonPortID;
    public HashedString ribbonPortInputID;
    private GameObject geyserFeature;
    private static readonly EventSystem.IntraObjectHandler<GeyserLogicExpand> OnLogicValueChangedDelegate =
        new ((component, data) => component.OnLogicValueChanged(data));
    private GeyserExpandDispenser expandDispenser;
    
    [Serialize]
    public float logicMin = 0f; 
    [Serialize]
    public float logicMax = 100f;
    

    public GeyserLogicController.RunMode RunMode {
      get => controller.runMode;
      set => controller.runMode = value;
    }

    public float FlowMass {
        get => expandDispenser.flowMass * 1000f;
        set => expandDispenser.flowMass = value / 1000f;
    }
    
    
    protected override void OnSpawn() {
        base.OnSpawn();
        Unsubscribe((int)GameHashes.LogicEvent, OnLogicValueChangedDelegate);
        var cell = Grid.PosToCell(this);

        geyserFeature = Grid.Objects[cell, (int)ObjectLayer.Building];
        var geyserComponent = geyserFeature.GetComponent<Geyser>();
        if (geyserComponent == null) {
            PUtil.LogError($"Geyser component not found: {geyserFeature}");
        }
        else {
            controller = geyserFeature.gameObject.AddOrGet<GeyserLogicController>();
            controller.ports = GetComponent<LogicPorts>();
            controller.portID = portID;
            controller.ribbonPortID = ribbonPortID;
        }
        expandDispenser = gameObject.GetComponent<GeyserExpandDispenser>();
        geyserFeature.SetActive(false);
        geyserFeature.SetActive(true);
    }

    protected override void OnCleanUp() {
        base.OnCleanUp();
        geyserFeature.SetActive(false);
        geyserFeature.SetActive(true);
    }

    public void OnLogicValueChanged(object data) {
        var logicValueChanged = (LogicValueChanged)data;
        if (logicValueChanged.portID != ribbonPortInputID)
            return;

        switch (logicValueChanged.newValue) {
            case 1:
                RunMode = GeyserLogicController.RunMode.SkipErupt;
                break;
            case 2:
                RunMode = GeyserLogicController.RunMode.SkipIdle;
                break;
            case 4:
                RunMode = GeyserLogicController.RunMode.AlwaysDormant;
                break;
            default:
                RunMode = GeyserLogicController.RunMode.Default;
                break;
        }
        // Debug.Log($"收到信号：{logicValueChanged.newValue}");
        // RefreshStatus();
    }

}