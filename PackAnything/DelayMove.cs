using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using PeterHan.PLib.Options;
using System;
using System.Reflection;
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
            OriginSurvayable.objectType = CheckObjectType(OriginSurvayable.objectType);
            if (OriginSurvayable.objectType == ObjectType.GravitasCreatureManipulator) {
                SetFinalPosition(OriginObject);
            } else {
                CloneOriginObject(out GameObject clonedObject);
                SetFinalPosition(clonedObject);
                clonedObject.SetActive(false);
                clonedObject.SetActive(true);
                OriginObject.SetActive(false);
                Destroy(OriginObject);
            }
            PackAnythingStaticVars.SetMoving(false);
            DestroyImmediate(gameObject);
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

        public ObjectType CheckObjectType(ObjectType objectType) {
            if (objectType != ObjectType.None) return objectType;
            switch (OriginObject.GetComponent<KPrefabID>().PrefabTag.ToString()) {
                case "WarpReceiver":
                    return ObjectType.WarpReceiver;
                case "GravitasCreatureManipulator":
                    return ObjectType.GravitasCreatureManipulator;
            }
            if (OriginObject.GetComponent<SetLocker>() != null) return ObjectType.HaveSetLocker;
            if (OriginObject.GetComponent<LoreBearer>() != null) return ObjectType.HaveLoreBearer;
            if (OriginObject.GetComponent<Activatable>() != null) return ObjectType.Activatable;
            return ObjectType.None;
        }

        public void CloneOriginObject(out GameObject clonedObject) {
            if (OriginSurvayable.objectType == ObjectType.Geyser) {
                clonedObject = Util.KInstantiate(Assets.GetPrefab(OriginObject.GetComponent<KPrefabID>().PrefabTag));
            } else {
                clonedObject = GameUtil.KInstantiate(OriginObject, OriginObject.transform.position, Grid.SceneLayer.Building);
            }
            Surveyable surveyable = clonedObject.GetComponent<Surveyable>();
            surveyable.objectType = OriginSurvayable.objectType;
            surveyable.isSurveyed = true;
            PackAnythingStaticVars.SurveableCmps.Add(surveyable);
            switch (OriginSurvayable.objectType) {
                case ObjectType.Geyser:
                    if (OriginObject.GetComponent<Studyable>().Studied) {
                        IDetouredField<Studyable, bool> detoured = PDetours.DetourField<Studyable, bool>("studied");
                        detoured.Set(clonedObject.AddOrGet<Studyable>(), true);
                    }
                    break;
                case ObjectType.HaveSetLocker:
                    if (!OriginObject.GetComponent<SetLocker>().SidescreenButtonInteractable()) {
                        PDetours.DetourField<SetLocker, bool>("used").Set(clonedObject.AddOrGet<SetLocker>(), true);
                    }
                    ToogleLoreBearer(clonedObject);
                    break;
                
                case ObjectType.CryoTank:
                case ObjectType.HaveLoreBearer:
                    ToogleLoreBearer(clonedObject);
                    break;
                case ObjectType.WarpPortal:
                    IDetouredField<WarpPortal, bool> warp = PDetours.DetourField<WarpPortal, bool>("discovered");
                    bool origin = warp.Get(OriginObject.GetComponent<WarpPortal>());
                    warp.Set(clonedObject.AddOrGet<WarpPortal>(), origin);
                    ToogleLoreBearer(clonedObject);
                    break;
                case ObjectType.WarpReceiver:
                    clonedObject.AddOrGet<WarpReceiver>().Used = OriginObject.GetComponent<WarpReceiver>().Used;
                    ToogleLoreBearer(clonedObject);
                    break;
                case ObjectType.Activatable:
                    if (OriginObject.GetComponent<Activatable>().IsActivated) {
                        Activatable activatable = clonedObject.AddOrGet<Activatable>();
                        Type type = typeof(Activatable);
                        MethodInfo methodInfo = type.GetMethod("OnCompleteWork", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (methodInfo != null) {
                            _ = methodInfo.Invoke(activatable, new object[1] { null });
                        }
                    }
                    break;
            }
        }

        public void ToogleLoreBearer(GameObject needToogle) {
            if (OriginObject.GetComponent<LoreBearer>() != null) {
                if (!OriginObject.GetComponent<LoreBearer>().SidescreenButtonInteractable()) {
                    PDetours.DetourField<LoreBearer, bool>("BeenClicked").Set(needToogle.AddOrGet<LoreBearer>(), true);
                }
            }
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
