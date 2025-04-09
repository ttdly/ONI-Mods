using PeterHan.PLib.Core;

namespace GeyserExpandMachine.Buildings;

public class GeyserLogicExpand : KMonoBehaviour{
    
    private GeyserLogicController controller;
    
    
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
        }
    }

    public void SetRunMode(GeyserLogicController.RunMode runMode) {
        controller.runMode = runMode;
    }

    public void GetRunMode(out GeyserLogicController.RunMode runMode) {
        runMode = controller.runMode;
    }
}