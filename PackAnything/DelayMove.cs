using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using PeterHan.PLib.Options;
using System;
using UnityEngine;

namespace PackAnything {
    public class DelayMove : KMonoBehaviour {
        [SerializeField]
        public int cell;
        [SerializeField]
        public int unoCount;

        Surveyable OriginSurvayable {
            get {
                return PackAnythingStaticVars.MoveStatus.surveyable;
            }
        }

        GameObject OriginObject {
            get {
                return PackAnythingStaticVars.MoveStatus.surveyable.gameObject;
            }
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            PackAnythingStaticVars.SetMoving(true);
            PackAnythingStaticVars.SetTargetMove(this);
            if (!PackAnythingStaticVars.MoveStatus.HaveAnObjectMoving) return;
            CloneOriginObject(out GameObject clonedObject);
            SetFinalPosition(clonedObject);
            clonedObject.SetActive(true);
            OriginObject.SetActive(false);
            UpdateSurveyStatues(clonedObject);
            Util.KDestroyGameObject(OriginObject);
            PackAnythingStaticVars.SetMoving(false);
            DestroyImmediate(gameObject);
        }

        public void UpdateSurveyStatues(GameObject needSurvey) {
            int index = PackAnythingStaticVars.SurveableCmps.IndexOf(OriginSurvayable);
            Surveyable surveyable = needSurvey.AddOrGet<Surveyable>();
            surveyable.isSurveyed = OriginSurvayable.isSurveyed;
            surveyable.objectType = OriginSurvayable.objectType;
            if (index == -1) {
                PackAnythingStaticVars.SurveableCmps.Add(surveyable);
                PackAnythingStaticVars.SurveableCmps.RemoveAll(item => item == null);
            } else {
                PackAnythingStaticVars.SurveableCmps[index] = surveyable;
            }
        }

        public void SetFinalPosition(GameObject final) {
            int originCell = Grid.PosToCell(OriginObject.transform.position);
            Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
            if (OriginSurvayable.objectType == ObjectType.Geyser) {
                DeleteNeutronium(originCell);
                if (SingletonOptions<Options>.Instance.GenerateUnobtanium && unoCount > 0) {
                    CreateNeutronium(cell);
                    cell = Grid.CellAbove(cell);
                }
                posCbc = Grid.CellToPosCBC(cell, OriginObject.FindOrAddComponent<KBatchedAnimController>().sceneLayer);
                posCbc.z -= 0.15f;
            }
            final.transform.SetPosition(posCbc);
        }

        public void CloneOriginObject(out GameObject clonedObject) {
            switch (OriginSurvayable.objectType) {
                case ObjectType.Geyser:
                    clonedObject = Util.KInstantiate(Assets.GetPrefab(OriginObject.GetComponent<KPrefabID>().PrefabTag));
                    if (OriginObject.GetComponent<Studyable>().Studied) {
                        PDetours.DetourField<Studyable, bool>("studied").Set(clonedObject.AddOrGet<Studyable>(), true);
                    }
                    break;
                default:
                    clonedObject = GameUtil.KInstantiate(OriginObject, OriginObject.transform.position, Grid.SceneLayer.Building);
                    break;
            }
            clonedObject.SetActive(false);
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
