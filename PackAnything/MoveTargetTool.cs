using System;
using UnityEngine;

namespace PackAnything {
  public class MoveTargetTool : InterfaceTool {
    public static MoveTargetTool Instance;
    public static SpriteRenderer PlaserSpriteRenderer;
    public int targetCell;
    private ObjectCanMove waittingMoveObject;

    public WorldModifier TargetWorldModifier => PackAnythingStaticVars.MoveStatus.worldModifier;

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
      Instance = this;
      visualizer = new GameObject("MoveBeaconToolVisualizer");
      var offs = new GameObject("MoveBeaconToolOffset");
      var offsTransform = offs.transform;
      offsTransform.SetParent(visualizer.transform);
      offsTransform.SetLocalPosition(new Vector3(0.0f, Grid.HalfCellSizeInMeters, 0.0f));
      offs.SetLayerRecursively(LayerMask.NameToLayer("Overlay"));
      var spriteRenderer = offs.AddComponent<SpriteRenderer>();
      if (spriteRenderer != null && PackAnythingStaticVars.ToolIcon != null) {
        PlaserSpriteRenderer = spriteRenderer;
        float widthInM = PackAnythingStaticVars.ToolIcon.texture.width / PackAnythingStaticVars.ToolIcon.pixelsPerUnit,
          scaleWidth = Grid.CellSizeInMeters / widthInM;
        spriteRenderer.name = "MoveBeaconToolSprite";
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = PackAnythingStaticVars.ToolIcon;
        spriteRenderer.enabled = true;
        offsTransform.localScale = new Vector3(scaleWidth, scaleWidth, 1.0f);
      }

      visualizer.SetActive(false);
    }

    public override void OnLeftClickDown(Vector3 cursor_pos) {
      base.OnLeftClickDown(cursor_pos);
      if (!(waittingMoveObject != null))
        return;
      var mouseCell = DebugHandler.GetMouseCell();
      if (SurveyableCanMoveTo(mouseCell)) {
        PlaySound(GlobalAssets.GetSound("HUD_Click"));
        SetMoveBeacon(mouseCell);
        SelectTool.Instance.Activate();
      } else {
        PlaySound(GlobalAssets.GetSound("Negative"));
      }
    }

    public override void OnMouseMove(Vector3 cursor_pos) {
      base.OnMouseMove(cursor_pos);
      targetCell = Grid.PosToCell(cursor_pos);
      RefreshColor();
    }

    protected override void OnActivateTool() {
      base.OnActivateTool();
    }

    protected override void OnDeactivateTool(InterfaceTool new_tool) {
      base.OnDeactivateTool(new_tool);
      visualizer.SetActive(false);
    }

    public void Acitvate(ObjectCanMove watingMove) {
      waittingMoveObject = watingMove;
      PlayerController.Instance.ActivateTool(this);
    }

    private void SetMoveBeacon(int mouseCell) {
      if (TargetWorldModifier != null) {
        TargetWorldModifier.cell = mouseCell;
        TargetWorldModifier.StartMoving();
      }
    }

    private void RefreshColor() {
      var c = new Color(0.91f, 0.21f, 0.2f);
      if (SurveyableCanMoveTo(DebugHandler.GetMouseCell()))
        c = Color.white;
      PlaserSpriteRenderer.color = c;
    }

    private bool SurveyableCanMoveTo(int cell) {
      try {
        if (!Grid.IsValidCell(cell)) return false;
        return !Grid.Element[cell].IsSolid;
      } catch (Exception) {
        return false;
      }
    }
  }
}