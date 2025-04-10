using GeyserExpandMachine.GeyserModify;
using PeterHan.PLib.Core;


namespace GeyserExpandMachine.Buildings;

public class GeyserLogicExpand : KMonoBehaviour{
    
    private GeyserLogicController controller;
    public HashedString portID;
    public HashedString ribbonPortID;

    public GeyserLogicController.RunMode RunMode {
      get => controller.runMode;
      set => controller.runMode = value;
    }
    
    
    protected override void OnSpawn() {
        base.OnSpawn();
        var cell = Grid.PosToCell(this);

        var geyserFeature = Grid.Objects[cell, (int)ObjectLayer.Building];
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
    }


}