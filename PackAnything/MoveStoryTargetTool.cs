using System.Collections.Generic;
using UnityEngine;

namespace PackAnything {
  public class MoveStoryTargetTool : InterfaceTool {
    public static MoveStoryTargetTool Instance;
    public TemplateContainer stampTemplate;
    public GameObject PlacerPrefab;
    private bool deactivateOnStamp;
    private GameObject[] needDestory = { };
    private StampToolPreview preview;
    private bool ready = true;
    private bool selectAffected;

    public static void DestroyInstance() {
      Instance = null;
    }

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
      Instance = this;
      PlacerPrefab = StampTool.Instance.PlacerPrefab;
      preview = new StampToolPreview(this, new StampToolPreview_Placers(PlacerPrefab), new StampToolPreview_Area(),
        new StampToolPreview_SolidLiquidGas(), new StampToolPreview_Prefabs());
    }

    public void Activate(TemplateContainer template, GameObject[] objects, bool SelectAffected = false,
      bool DeactivateOnStamp = false) {
      selectAffected = SelectAffected;
      deactivateOnStamp = DeactivateOnStamp;
      if (stampTemplate == template || template == null || template.cells == null)
        return;
      stampTemplate = template;
      PlayerController.Instance.ActivateTool(this);
      StartCoroutine(preview.Setup(template));
      needDestory = objects;
    }

    public override void OnLeftClickDown(Vector3 cursor_pos) {
      base.OnLeftClickDown(cursor_pos);
      Stamp(cursor_pos);
    }

    private void Stamp(Vector2 pos) {
      if (!ready)
        return;
      var cell1 = Grid.PosToCell(pos);
      var size = stampTemplate.info.size;
      var x1 = Mathf.FloorToInt((float)(-(double)size.X / 2.0));
      var cell2 = Grid.OffsetCell(cell1, x1, 0);
      var cell3 = Grid.PosToCell(pos);
      size = stampTemplate.info.size;
      var x2 = Mathf.FloorToInt(size.X / 2f);
      var cell4 = Grid.OffsetCell(cell3, x2, 0);
      var cell5 = Grid.PosToCell(pos);
      size = stampTemplate.info.size;
      var y1 = 1 + Mathf.FloorToInt((float)(-(double)size.Y / 2.0));
      var cell6 = Grid.OffsetCell(cell5, 0, y1);
      var cell7 = Grid.PosToCell(pos);
      size = stampTemplate.info.size;
      var y2 = 1 + Mathf.FloorToInt(size.Y / 2f);
      var cell8 = Grid.OffsetCell(cell7, 0, y2);
      if (!Grid.IsValidBuildingCell(cell2) || !Grid.IsValidBuildingCell(cell4) || !Grid.IsValidBuildingCell(cell8) ||
          !Grid.IsValidBuildingCell(cell6))
        return;
      ready = false;
      var pauseOnComplete = SpeedControlScreen.Instance.IsPaused;
      if (SpeedControlScreen.Instance.IsPaused)
        SpeedControlScreen.Instance.Unpause();
      if (stampTemplate.cells != null) {
        preview.OnPlace();
        var gameObjectList = new List<GameObject>();
        for (var index = 0; index < stampTemplate.cells.Count; ++index)
        for (var layer = 0; layer < 34; ++layer) {
          var gameObject =
            Grid.Objects[
              Grid.XYToCell((int)(pos.x + (double)stampTemplate.cells[index].location_x),
                (int)(pos.y + (double)stampTemplate.cells[index].location_y)), layer];
          if (gameObject != null && !gameObjectList.Contains(gameObject))
            gameObjectList.Add(gameObject);
        }

        foreach (var original in gameObjectList)
          if (original != null)
            Util.KDestroyGameObject(original);
      }

      TemplateLoader.Stamp(stampTemplate, pos, () => CompleteStamp(pauseOnComplete));
      if (selectAffected) {
        DebugBaseTemplateButton.Instance.ClearSelection();
        if (stampTemplate.cells != null)
          for (var index = 0; index < stampTemplate.cells.Count; ++index)
            DebugBaseTemplateButton.Instance.AddToSelection(Grid.XYToCell(
              (int)(pos.x + (double)stampTemplate.cells[index].location_x),
              (int)(pos.y + (double)stampTemplate.cells[index].location_y)));
      }

      if (!deactivateOnStamp)
        return;
      DestroyObjects();
      DeactivateTool();
    }

    private void CompleteStamp(bool pause) {
      if (pause)
        SpeedControlScreen.Instance.Pause();
      ready = true;
      OnDeactivateTool(null);
    }

    private void DestroyObjects() {
      if (needDestory == null || needDestory.Length == 0) return;
      foreach (var go in needDestory)
        if (go != null)
          Destroy(go);
    }

    protected override void OnDeactivateTool(InterfaceTool new_tool) {
      base.OnDeactivateTool(new_tool);
      if (gameObject.activeSelf)
        return;
      preview.Cleanup();
      stampTemplate = null;
    }
  }
}