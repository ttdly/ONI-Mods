using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using UnityEngine;

namespace PackAnything {
    public class DelayMove : KMonoBehaviour, ISim1000ms {
        private ProgressBar m_Progress;
        [SerializeField]
        public float orderProgress;
        public float delay = 0.5f;
        public int Handler;
        [SerializeField]
        public int cell;
        [SerializeField]
        public int unoCount;
        public virtual float GetProgress() => orderProgress;

        protected override void OnSpawn() {
            base.OnSpawn();
            PackAnythingStaticVars.SetMoving(true);
            PackAnythingStaticVars.SetTargetMove(this);
            LightActive(true);
        }

        public void Sim1000ms(float dt) {
            orderProgress += delay;
            ShowProgressBar();
            MoveObject();
        }

        private void MoveObject() {
            if (orderProgress < 0.5) return;
            MoveStatus moveStatus = PackAnythingStaticVars.MoveStatus;
            if (!moveStatus.HaveAnObjectMoving) return;
            bool isGeyser = false;
            GameObject originObject = moveStatus.surveyable.gameObject;
            Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
            if (originObject.gameObject.HasTag(GameTags.GeyserFeature)) {
                isGeyser = true;
                DeleteNeutronium(Grid.PosToCell(originObject));
                if (SingletonOptions<Options>.Instance.GenerateUnobtanium && unoCount > 0) {
                    CreateNeutronium(cell);
                    cell = Grid.CellAbove(cell);
                }
                posCbc = Grid.CellToPosCBC(cell, originObject.FindOrAddComponent<KBatchedAnimController>().sceneLayer);
                float num = -0.15f;
                posCbc.z += num;
            }
            GameObject cloneObject;
            if (isGeyser) {
                cloneObject = Util.KInstantiate(Assets.GetPrefab(originObject.GetComponent<KPrefabID>().PrefabTag), posCbc);
                cloneObject.AddOrGet<Surveyable>().isSurveyed = true;
            } else {
                cloneObject = GameUtil.KInstantiate(originObject, posCbc, Grid.SceneLayer.Building);
            }
            cloneObject.transform.SetPosition(posCbc);
            cloneObject.SetActive(false);
            if (orderProgress < 1) return;
            cloneObject.SetActive(true);
            Destroy(originObject);
            PackAnythingStaticVars.SurveableCmps.RemoveAll(data => data == null);
            LightActive(false);
            PackAnythingStaticVars.SetMoving(false);
            CancelAll();
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
            WorldModifier worldModifier = PackAnythingStaticVars.MoveStatus.worldModifier;
            if (worldModifier == null) return;
            if (on) {
                Light2D light2D = worldModifier.gameObject.AddOrGet<Light2D>();
                light2D.Color = new Color(0.6f, 0f, 0.6f, 1f);
                light2D.Range = 3f;
                light2D.Offset = new Vector2(0, 1);
                light2D.overlayColour = new Color(0.8f, 0f, 0.8f, 1f);
                light2D.shape = LightShape.Circle;
                light2D.drawOverlay = true;
            } else {
                DestroyImmediate(worldModifier.gameObject.AddOrGet<Light2D>());
            }
        }

        private void CancelAll() {
            LightActive(false);
            PackAnythingStaticVars.SetMoving(false);
            if (m_Progress != null) {
                m_Progress.gameObject.DeleteObject();
                m_Progress = null;
            }
            DestroyImmediate(gameObject);
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
    }
}
