﻿using System;
using UnityEngine;

namespace PackAnything {
  public class MoveTargetTool : InterfaceTool {
    public static MoveTargetTool Instance;
    private static SpriteRenderer PlacerSpriteRenderer;
    private ObjectCanMove waitingMoveObject;
    

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
        PlacerSpriteRenderer = spriteRenderer;
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
      if (!(waitingMoveObject != null))
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
 
      RefreshColor();
    }

    protected override void OnActivateTool() {
      base.OnActivateTool();
    }

    protected override void OnDeactivateTool(InterfaceTool new_tool) {
      base.OnDeactivateTool(new_tool);
      visualizer.SetActive(false);
    }

    public void Acitvate(ObjectCanMove waitingMove) {
      waitingMoveObject = waitingMove;
      PlayerController.Instance.ActivateTool(this);
    }

    private void SetMoveBeacon(int mouseCell) {

    }

    private void RefreshColor() {
      var c = new Color(0.91f, 0.21f, 0.2f);
      if (SurveyableCanMoveTo(DebugHandler.GetMouseCell()))
        c = Color.white;
      PlacerSpriteRenderer.color = c;
    }

    private static bool SurveyableCanMoveTo(int cell) {
      try {
        if (!Grid.IsValidCell(cell)) return false;
        return !Grid.Element[cell].IsSolid;
      } catch (Exception) {
        return false;
      }
    }
  }
}