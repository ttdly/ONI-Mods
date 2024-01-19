// Decompiled with JetBrains decompiler
// Type: StampTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
using System.Collections.Generic;
using UnityEngine;
namespace PackAnything {
    public class MoveStoryTargetTool : InterfaceTool {
        public static MoveStoryTargetTool Instance;
        private StampToolPreview preview;
        public TemplateContainer stampTemplate;
        public GameObject PlacerPrefab;
        private bool ready = true;
        private bool selectAffected;
        private bool deactivateOnStamp;
        private GameObject[] needDestory = { };

        public static void DestroyInstance() => Instance = null;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            Instance = this;
            PlacerPrefab = StampTool.Instance.PlacerPrefab;
            preview = new StampToolPreview(this, new IStampToolPreviewPlugin[4]{
            new StampToolPreview_Placers(PlacerPrefab),
            new StampToolPreview_Area(),
            new StampToolPreview_SolidLiquidGas(),
            new StampToolPreview_Prefabs()
        });
        }

        private void Update() => preview.Refresh(Grid.PosToCell(GetCursorPos()));

        public void Activate(TemplateContainer template, GameObject[] objects, bool SelectAffected = false, bool DeactivateOnStamp = false) {
            selectAffected = SelectAffected;
            deactivateOnStamp = DeactivateOnStamp;
            if (stampTemplate == template || template == null || template.cells == null)
                return;
            stampTemplate = template;
            PlayerController.Instance.ActivateTool(this);
            StartCoroutine(preview.Setup(template));
            needDestory = objects;
        }

        private Vector3 GetCursorPos() => PlayerController.GetCursorPos(KInputManager.GetMousePos());

        public override void OnLeftClickDown(Vector3 cursor_pos) {
            base.OnLeftClickDown(cursor_pos);
            Stamp((Vector2)cursor_pos);
        }

        private void Stamp(Vector2 pos) {
            if (!ready)
                return;
            int cell1 = Grid.PosToCell(pos);
            Vector2f size = stampTemplate.info.size;
            int x1 = Mathf.FloorToInt((float)(-(double)size.X / 2.0));
            int cell2 = Grid.OffsetCell(cell1, x1, 0);
            int cell3 = Grid.PosToCell(pos);
            size = stampTemplate.info.size;
            int x2 = Mathf.FloorToInt(size.X / 2f);
            int cell4 = Grid.OffsetCell(cell3, x2, 0);
            int cell5 = Grid.PosToCell(pos);
            size = stampTemplate.info.size;
            int y1 = 1 + Mathf.FloorToInt((float)(-(double)size.Y / 2.0));
            int cell6 = Grid.OffsetCell(cell5, 0, y1);
            int cell7 = Grid.PosToCell(pos);
            size = stampTemplate.info.size;
            int y2 = 1 + Mathf.FloorToInt(size.Y / 2f);
            int cell8 = Grid.OffsetCell(cell7, 0, y2);
            if (!Grid.IsValidBuildingCell(cell2) || !Grid.IsValidBuildingCell(cell4) || !Grid.IsValidBuildingCell(cell8) || !Grid.IsValidBuildingCell(cell6))
                return;
            ready = false;
            bool pauseOnComplete = SpeedControlScreen.Instance.IsPaused;
            if (SpeedControlScreen.Instance.IsPaused)
                SpeedControlScreen.Instance.Unpause();
            if (stampTemplate.cells != null) {
                preview.OnPlace();
                List<GameObject> gameObjectList = new List<GameObject>();
                for (int index = 0; index < stampTemplate.cells.Count; ++index) {
                    for (int layer = 0; layer < 34; ++layer) {
                        GameObject gameObject = Grid.Objects[Grid.XYToCell((int)(pos.x + (double)stampTemplate.cells[index].location_x), (int)(pos.y + (double)stampTemplate.cells[index].location_y)), layer];
                        if (gameObject != null && !gameObjectList.Contains(gameObject))
                            gameObjectList.Add(gameObject);
                    }
                }
                foreach (GameObject original in gameObjectList) {
                    if (original != null)
                        Util.KDestroyGameObject(original);
                }
            }
            TemplateLoader.Stamp(stampTemplate, pos, () => CompleteStamp(pauseOnComplete));
            if (selectAffected) {
                DebugBaseTemplateButton.Instance.ClearSelection();
                if (stampTemplate.cells != null) {
                    for (int index = 0; index < stampTemplate.cells.Count; ++index)
                        DebugBaseTemplateButton.Instance.AddToSelection(Grid.XYToCell((int)(pos.x + (double)stampTemplate.cells[index].location_x), (int)(pos.y + (double)stampTemplate.cells[index].location_y)));
                }
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
            foreach (GameObject go in needDestory) {
                if (go != null) {
                    Destroy(go);
                }
            }
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
