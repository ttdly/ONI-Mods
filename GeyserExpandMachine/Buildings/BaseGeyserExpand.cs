using GeyserExpandMachine.GeyserModify;
using KSerialization;
using UnityEngine;


namespace GeyserExpandMachine.Buildings;

public class BaseGeyserExpand : KMonoBehaviour, ISim1000ms{
    
    private GeyserLogicController controller;
    public HashedString portID;
    public HashedString ribbonPortID;
    public HashedString ribbonPortInputID;
    public bool close;
    public bool safe;
    private GameObject geyserFeature;
    private static readonly EventSystem.IntraObjectHandler<BaseGeyserExpand> OnLogicValueChangedDelegate =
        new ((component, data) => component.OnLogicValueChanged(data));
    private GeyserExpandDispenser expandDispenser;
    private LogicPorts ports;
    private Storage storage;
    private float PercentFull => storage.MassStored() / storage.Capacity();
    private bool activated;
    private ProgressBar progressBar;
    
    
    [Serialize]
    public float logicMin = 0f; 
    [Serialize]
    public float logicMax = 100f;
    
    private Geyser geyser;
    private ElementConverter.OutputElement outputElement;
    

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
        Subscribe((int)GameHashes.LogicEvent, OnLogicValueChangedDelegate);
        BindGeyser();
    }

    private void DoBind() {
        geyserFeature = geyser.gameObject;
        ports = GetComponent<LogicPorts>();
        controller = geyserFeature.gameObject.AddOrGet<GeyserLogicController>();
        controller.ports = ports;
        controller.portID = portID;
        controller.ribbonPortID = ribbonPortID;
        expandDispenser = gameObject.GetComponent<GeyserExpandDispenser>();
        storage = gameObject.GetComponent<Storage>();

        outputElement = new ElementConverter.OutputElement(
            geyser.configuration.GetEmitRate(),
            geyser.configuration.GetElement(),
            geyser.configuration.GetTemperature(),
            storeOutput: true,
            addedDiseaseIdx: geyser.configuration.GetDiseaseIdx(),
            addedDiseaseCount: Mathf.RoundToInt(
                geyser.configuration.GetDiseaseCount() * geyser.configuration.GetEmitRate())
        );
        
        
        if (progressBar == null) {
            progressBar = ProgressBar.CreateProgressBar(gameObject, GetPercentFull);
            progressBar.barColor = Color.green;
            progressBar.transform
                .SetPosition(progressBar.transform.position + Vector3.up * 0.5f);
            progressBar.SetVisibility(true);
        }
        geyserFeature.SetActive(false);
        geyserFeature.SetActive(true);
        safe = true;
    }
    private void BindGeyser() {
        var cell = Grid.PosToCell(this);
        ModData.Instance.BaseGeyserExpands.Add(cell, this);
        if (!ModData.Instance.Geysers.TryGetValue(cell, out geyser)) {
            LogUtil.Warn($"未能找到指定的间歇泉组件 {cell}");
            foreach (var keyValue in ModData.Instance.Geysers) {
                LogUtil.Info($"- {keyValue.Key}: {keyValue.Value}");
            }
            return;
        }
        DoBind();
    }

    public void BindGeyser(Geyser geyserCmp) {
        geyser = geyserCmp;
        DoBind();
    }

    private float GetPercentFull() {
        return storage.MassStored() / storage.Capacity();
    }

    protected override void OnCleanUp() {
        base.OnCleanUp();
        geyserFeature.SetActive(false);
        geyserFeature.SetActive(true);
        ModData.Instance.BaseGeyserExpands.Remove(Grid.PosToCell(this));
        if (progressBar == null) return; 
        progressBar.gameObject.DeleteObject();
        progressBar = null;
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


    public void Sim1000ms(float dt) {
        if (!safe) return;
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
        
        if (close || storage == null || outputElement.elementHash == 0) return;

        if (ElementLoader.FindElementByHash(outputElement.elementHash).IsLiquid) {
            storage.AddLiquid(
                outputElement.elementHash, 
                outputElement.massGenerationRate, 
                outputElement.minOutputTemperature,
                outputElement.addedDiseaseIdx,
                outputElement.addedDiseaseCount);
        }
        else {
            storage.AddGasChunk(
                outputElement.elementHash,
                outputElement.massGenerationRate,
                outputElement.minOutputTemperature,
                outputElement.addedDiseaseIdx,
                outputElement.addedDiseaseCount,
                false
            );
        }
    }
}