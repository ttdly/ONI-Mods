using System;
using PackAnything.Movable;
using UnityEngine;

namespace PackAnything.MoveTool {
  public class EntityMoveTool : InterfaceTool {
    public static EntityMoveTool Instance;
    private BaseMovable targetMovable;
    private KBatchedAnimController kBatchedAnimController;

    protected override void OnPrefabInit() {
      Instance = this;
    }

    public override void OnLeftClickDown(Vector3 cursor_pos) {
      base.OnLeftClickDown(cursor_pos);
      if (!(targetMovable != null))
        return;
      var mouseCell = DebugHandler.GetMouseCell();
      if (targetMovable.CanMoveTo(mouseCell)) {
        PlaySound(GlobalAssets.GetSound("HUD_Click"));
        targetMovable.Move(mouseCell);
        SelectTool.Instance.Activate();
      } else {
        PlaySound(GlobalAssets.GetSound("Negative"));
      }
    }

    public override void OnMouseMove(Vector3 cursor_pos) {
      base.OnMouseMove(cursor_pos);
      RefreshColor();
    }

    protected override void OnActivateTool() {
      base.OnActivateTool();
      if (targetMovable == null) return;
      visualizer = GameUtil.KInstantiate(CreateVisualizer(), Grid.SceneLayer.Ore,
        gameLayer: LayerMask.NameToLayer("Place"));
      visualizer.SetActive(true);
      // 显示鼠标周围的网格效果
      GridCompositor.Instance.ToggleMajor(true);
    }

    protected override void OnDeactivateTool(InterfaceTool new_tool) {
      Destroy(visualizer);
      GridCompositor.Instance.ToggleMajor(false);
      if (new_tool == SelectTool.Instance)
        Game.Instance.Trigger(-1190690038);
      base.OnDeactivateTool(new_tool);
    }

    public void Activate(BaseMovable movable) {
      targetMovable = movable;
      PlayerController.Instance.ActivateTool(this);
      OnActivateTool();
    }

    private void RefreshColor() {
      if (targetMovable == null) return;
      var c = Color.red;
      if (targetMovable.CanMoveTo(DebugHandler.GetMouseCell()))
        c = Color.white;
      if (kBatchedAnimController == null) return;
      kBatchedAnimController.TintColour = c;
    }

    // 创建一个工具视图对象
    private GameObject CreateVisualizer() {
      var visualBuffer = new GameObject(targetMovable.gameObject.name + "Proxy");
      visualBuffer.SetActive(false);
      visualBuffer.AddOrGet<KPrefabID>();
      visualBuffer.AddOrGet<KSelectable>();
      visualBuffer.AddOrGet<StateMachineController>();
      var primaryElement = visualBuffer.AddOrGet<PrimaryElement>();
      primaryElement.Mass = 1f;
      primaryElement.Temperature = 293f;
      DontDestroyOnLoad(visualBuffer);
      BuildingLoader.AddID(visualBuffer, targetMovable.gameObject.PrefabID() + "Moving");
      var visualAnimController = visualBuffer.AddOrGet<KBatchedAnimController>();
      var gameObjectController = targetMovable.gameObject.GetComponent<KBatchedAnimController>();
      visualAnimController.AnimFiles = gameObjectController.AnimFiles;
      visualAnimController.initialAnim = gameObjectController.initialAnim;
      kBatchedAnimController = visualAnimController;
      return visualBuffer;
    }
  }
}