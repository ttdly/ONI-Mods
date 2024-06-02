using System.Collections.Generic;
using UnityEngine;

namespace SpaceStore.SellButtons {
  public class BrushSellTool : BrushTool {
    public static BrushSellTool instance;
    private readonly string soundPath = GlobalAssets.GetSound("SandboxTool_ClearFloor");

    public static void DestroyInstance() {
      instance = null;
    }

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
      instance = this;
    }

    protected override string GetDragSound() {
      return "";
    }

    public void Activate() {
      PlayerController.Instance.ActivateTool(this);
    }

    protected override void OnActivateTool() {
      base.OnActivateTool();
      SandboxToolParameterMenu.instance.gameObject.SetActive(true);
      SandboxToolParameterMenu.instance.DisableParameters();
      SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(true);
      SandboxToolParameterMenu.instance.brushRadiusSlider.SetValue(1);
    }

    protected override void OnDeactivateTool(InterfaceTool new_tool) {
      base.OnDeactivateTool(new_tool);
      SandboxToolParameterMenu.instance.gameObject.SetActive(false);
    }

    public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors) {
      colors = new HashSet<ToolMenu.CellColorData>();
      foreach (var cellsInRadiu in cellsInRadius)
        colors.Add(new ToolMenu.CellColorData(cellsInRadiu, radiusIndicatorColor));
    }

    public override void OnMouseMove(Vector3 cursorPos) {
      base.OnMouseMove(cursorPos);
    }

    public override void OnLeftClickDown(Vector3 cursor_pos) {
      base.OnLeftClickDown(cursor_pos);
      KFMOD.PlayUISound(GlobalAssets.GetSound("SandboxTool_Click"));
    }

    protected override void OnPaintCell(int cell, int distFromOrigin) {
      base.OnPaintCell(cell, distFromOrigin);
      var flag = false;
      foreach (var sell in StaticVars.Buttons) {
        if (sell == null || !(Grid.PosToCell(sell) == cell) || sell.pickupable.storage != null) continue;
        if (!flag) {
          KFMOD.PlayOneShot(soundPath, sell.gameObject.transform.GetPosition());
          flag = true;
        }

        sell.Sell();
      }

      StaticVars.Buttons.RemoveAll(item => item == null || item.gameObject == null);
    }
  }
}