using GeyserExpandMachine.GeyserModify;
using KSerialization;
using UnityEngine;


namespace GeyserExpandMachine.Buildings;

public class GeyserLogicExpand : KMonoBehaviour, ISim200ms{
    
    private GeyserLogicController controller;
    public HashedString portID;
    public HashedString ribbonPortID;
    public HashedString ribbonPortInputID;
    private GameObject geyserFeature;
    private static readonly EventSystem.IntraObjectHandler<GeyserLogicExpand> OnLogicValueChangedDelegate =
        new ((component, data) => component.OnLogicValueChanged(data));
    private GeyserExpandDispenser expandDispenser;
    private LogicPorts ports;
    private Storage storage;
    private float PercentFull => storage.MassStored() / storage.Capacity();
    private bool activated;
    
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
        ports = GetComponent<LogicPorts>();

        geyserFeature = Grid.Objects[cell, (int)ObjectLayer.Building];
        var geyserComponent = geyserFeature.GetComponent<Geyser>();
        if (geyserComponent == null) {
            Debug.LogError($"Geyser component not found: {geyserFeature}");
        }
        else {
            controller = geyserFeature.gameObject.AddOrGet<GeyserLogicController>();
            controller.ports = ports;
            controller.portID = portID;
            controller.ribbonPortID = ribbonPortID;
        }
        expandDispenser = gameObject.GetComponent<GeyserExpandDispenser>();
        storage = gameObject.GetComponent<Storage>();
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
            case 3:
                RunMode = GeyserLogicController.RunMode.SkipDormant;
                break;
            case 4:
                RunMode = GeyserLogicController.RunMode.AlwaysDormant;
                break;
            default:
                RunMode = GeyserLogicController.RunMode.Default;
                break;
        }
    }


    public void Sim200ms(float dt) {
        var num = PercentFull * 100f;
        if (activated) {
            if (num >= (double)logicMax)
                activated = false;
        }
        else if (num <= (double)logicMin) {
            activated = true;
        }
        var newActivated = activated;
        ports.SendSignal(SmartReservoir.PORT_ID, newActivated ? 1 : 0);
    }
}