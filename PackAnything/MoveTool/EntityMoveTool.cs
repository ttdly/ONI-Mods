﻿using PackAnything.Movable;
using PeterHan.PLib.Options;
using UnityEngine;

namespace PackAnything.MoveTool {
  public class EntityMoveTool : InterfaceTool {
    public static EntityMoveTool Instance;
    private static readonly Color red = new Color(0.91f, 0.21f, 0.2f);
    private BaseMovable targetMovable;

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
        if (SingletonOptions<Options>.Instance.StableMode) {
          targetMovable.StableMove(mouseCell);
        } else {
          targetMovable.Move(mouseCell);
        }
        targetMovable.SetOriginCell(mouseCell);
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
      var (visualBuffer, needOffset) = CreateVisualizer();
      visualizer = GameUtil.KInstantiate(visualBuffer, Grid.SceneLayer.Building,
        gameLayer: LayerMask.NameToLayer("Place"));
      var animController = visualizer.GetComponent<KBatchedAnimController>();
      if (needOffset && animController != null) {
        var offset = animController.Offset;
        offset.x += 0.5f;
        animController.Offset = offset;
      }

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
      var c = red;
      if (targetMovable.CanMoveTo(DebugHandler.GetMouseCell()))
        c = Color.white;
      if (visualizer.TryGetComponent(out KBatchedAnimController controller)) controller.TintColour = c;
    }

    // 创建一个工具视图对象
    private (GameObject visualizer, bool needOffset) CreateVisualizer() {
      var visualBuffer = new GameObject(targetMovable.gameObject.name + "Proxy");
      visualBuffer.SetActive(false);
      visualBuffer.AddOrGet<KPrefabID>();
      visualBuffer.AddOrGet<KSelectable>();
      visualBuffer.AddOrGet<StateMachineController>();
      var primaryElement = visualBuffer.AddOrGet<PrimaryElement>();
      primaryElement.Mass = 1f;
      primaryElement.Temperature = 293f;
      DontDestroyOnLoad(visualBuffer);
      var visualAnimController = visualBuffer.AddOrGet<KBatchedAnimController>();
      var gameObjectController = targetMovable.gameObject.GetComponent<KBatchedAnimController>();
      visualAnimController.AnimFiles = gameObjectController.AnimFiles;
      visualAnimController.initialAnim = gameObjectController.initialAnim;
      var needOffset = targetMovable.gameObject.TryGetComponent(out KBoxCollider2D kBoxCollider2D) &&
                       kBoxCollider2D.size.x % 2 == 0;
      return (visualBuffer, needOffset);
    }
  }
}