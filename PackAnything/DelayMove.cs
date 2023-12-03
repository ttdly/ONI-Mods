using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using STRINGS;
using System;
using UnityEngine;

namespace PackAnything {
    public class DelayMove : KMonoBehaviour, ISim1000ms {
        private ProgressBar m_Progress;
        [SerializeField]
        public float orderProgress;
#if DEBUG
        public float delay = 0.5f;
#else
        public float delay = 0.5f;
#endif
        public int Handler;
        [SerializeField]
        public int cell;
        [SerializeField]
        public int unoCount;
        private CellOffset[] cellOffsets;
        private int originCell;
        private SimHashes[] elements;
        private float[] mass;
        public virtual float GetProgress() => orderProgress;

        protected override void OnSpawn() {
            base.OnSpawn();
            Handler = Subscribe(GameHashes.RefreshUserMenu.GetHashCode(), OnRefreshUserMenu);
            PackAnythingStaticVars.targetMove = this;
            //CoverTargetWithUno();
            LightActive(true);
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            Unsubscribe(Handler);
            Handler = -1;
        }

        public void Sim1000ms(float dt) {
            orderProgress += delay;
            ShowProgressBar();
            Complete();
        }

        private void Complete() { 
            if (orderProgress >= 1) {
                if (PackAnythingStaticVars.targetSurveyable != null) {
                    GameObject originObject = PackAnythingStaticVars.targetSurveyable.gameObject;
                    Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
                    KSelectable selectable = originObject.GetComponent<KSelectable>();
                    OccupyArea occupyArea = originObject.GetComponent<OccupyArea>();
                    Building building = originObject.GetComponent<Building>();
                    if (PackAnythingStaticVars.targetSurveyable.gameObject.HasTag(GameTags.GeyserFeature)) {
                        DeleteNeutronium(Grid.PosToCell(originObject));
                        if (SingletonOptions<Options>.Instance.GenerateUnobtanium && unoCount > 0) {
                            gameObject.transform.SetPosition(Grid.CellToPosCBC(Grid.OffsetCell(cell, 1, 2), Grid.SceneLayer.Move));
                            CreateNeutronium(cell);
                            cell = Grid.CellAbove(cell);
                        }
                        posCbc = Grid.CellToPosCBC(cell, originObject.FindOrAddComponent<KBatchedAnimController>().sceneLayer);
                        float num = -0.15f;
                        posCbc.z += num;
                    }
                    selectable?.transform.SetPosition(posCbc);
                    occupyArea?.UpdateOccupiedArea();
                    building?.UpdatePosition();
                }
                LightActive(false);
                PackAnythingStaticVars.targetModifier = null;
                CancelAll();
                
            }
        }

        private void ShowProgressBar() {
            if (m_Progress == null) {
                m_Progress = ProgressBar.CreateProgressBar(gameObject, GetProgress);
                m_Progress.enabled = true;
                m_Progress.SetVisibility(true);
                m_Progress.barColor = PackAnythingStaticVars.PrimaryColor;
            }
        }

        private void LightActive(bool on) {
            if (PackAnythingStaticVars.targetModifier == null) return;
            if (on) {
                Light2D light2D = PackAnythingStaticVars.targetModifier.gameObject.AddOrGet<Light2D>();
                light2D.Color = new Color(0.6f, 0f, 0.6f, 1f);
                light2D.Range = 3f;
                light2D.Offset = new Vector2(0, 1);
                light2D.overlayColour = new Color(0.8f, 0f, 0.8f, 1f);
                light2D.shape = LightShape.Circle;
                light2D.drawOverlay = true;
            } else {
                DestroyImmediate(PackAnythingStaticVars.targetModifier.gameObject.AddOrGet<Light2D>());
            }
        }

        private void CancelAll() {
            LightActive(false);
            if (PackAnythingStaticVars.targetModifier != null) {
                PackAnythingStaticVars.targetModifier = null;
            }
            PackAnythingStaticVars.targetSurveyable = null;
            PackAnythingStaticVars.targetMove = null;
            //RemoverTargetUno();
            if(m_Progress != null) {
                m_Progress.gameObject.DeleteObject();
                m_Progress = null;
            }
            PlayDestory(gameObject);
        }

        private void PlayDestory(GameObject go) {
            KBatchedAnimController kBatchedAnimController = go.GetComponent<KBatchedAnimController>();
            if (kBatchedAnimController == null) return;
            kBatchedAnimController.Play("destroy");
            kBatchedAnimController.destroyOnAnimComplete = true;
        }

        public void CreateNeutronium(int cell) {
            int[] cells = new[]{
                Grid.CellLeft(cell),
                cell,
                Grid.CellRight(cell),
                Grid.CellRight(Grid.CellRight(cell))
            };
            foreach (int x in cells) {
                if (unoCount == 0) continue;
                if (Grid.Element.Length < x || Grid.Element[x] == null) {
                    PUtil.LogError("Out of index.");
                    new IndexOutOfRangeException();
                    return;
                }
                if (!Grid.IsValidCell(x)) continue;
                SimMessages.ReplaceElement(gameCell: x, new_element: SimHashes.Unobtanium, ev: CellEventLogger.Instance.DebugTool, mass: 100f);
                unoCount--;
            }
        }

        public void DeleteNeutronium(int cell) {
            int[] cells = new[]{
                Grid.CellDownLeft(cell),
                Grid.CellBelow(cell),
                Grid.CellDownRight(cell),
                Grid.CellRight(Grid.CellDownRight(cell))
            };
            unoCount = 0;
            foreach (int x in cells) {
                if (Grid.Element.Length < x || Grid.Element[x] == null) {
                    new IndexOutOfRangeException();
                    return;
                }
                Element e = Grid.Element[x];
                if (!e.IsSolid || !e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM")) continue;
                SimMessages.ReplaceElement(gameCell: x, new_element: SimHashes.Vacuum, ev: CellEventLogger.Instance.DebugTool, mass: 100f);
                unoCount++;
            }
        }

        //private void CoverTargetWithUno() {
        //    originCell = Grid.PosToCell(PackAnythingStaticVars.targetSurveyable);
        //    cellOffsets = PackAnythingStaticVars.targetSurveyable.PlacementOffsets;
        //    elements = new SimHashes[cellOffsets.Length];
        //    mass = new float[cellOffsets.Length];
        //    for (int i = 0; i < cellOffsets.Length; i++) {
        //        int cell = Grid.OffsetCell(originCell, cellOffsets[i]);
        //        if (Grid.Element.Length < cell || Grid.Element[cell] == null) {
        //            PUtil.LogError("Out of index.");
        //            new IndexOutOfRangeException();
        //            return;
        //        }
        //        if (!Grid.IsValidCell(cell)) continue;
        //        elements[i] = Grid.Element[cell].id;
        //        mass[i] = Grid.Mass[cell];
        //        SimMessages.ReplaceElement(gameCell: cell, new_element: SimHashes.Unobtanium, ev: CellEventLogger.Instance.DebugTool, mass: 100f);
        //    }
        //}

        //private void RemoverTargetUno() {
        //    for(int i = 0; i < cellOffsets.Length; i++) {
        //        int cell = Grid.OffsetCell(originCell, cellOffsets[i]);
        //        if (Grid.Element.Length < cell || Grid.Element[cell] == null) {
        //            PUtil.LogError("Out of index.");
        //            new IndexOutOfRangeException();
        //            return;
        //        }
        //        if (!Grid.IsValidCell(cell)) continue;
        //        SimMessages.ReplaceElement(gameCell: cell, new_element: elements[i], ev: CellEventLogger.Instance.DebugTool, mass: mass[i]);
        //    }
        //}

        private void OnRefreshUserMenu(object data) {
            Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_control", UI.USERMENUACTIONS.PICKUPABLEMOVE.NAME_OFF, CancelAll, Action.NumActions, null, null, null, UI.USERMENUACTIONS.PICKUPABLEMOVE.TOOLTIP_OFF));
        }

    }
}
