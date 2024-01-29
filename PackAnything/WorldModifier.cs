using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using PeterHan.PLib.Options;
using System.Reflection;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace PackAnything {
    public class WorldModifier : KMonoBehaviour {
        [MyCmpReq]
        Storage storage;
        [SerializeField]
        public int cell;
        [SerializeField]
        public int unoCount;
        private Tag consumeTag = SimHashes.Steel.CreateTag();
        private readonly Dictionary<Element, float> createElementList = new Dictionary<Element, float>();
        public bool CanStartMove {
            get {
                if (SingletonOptions<Options>.Instance.DontConsumeAnything) return true;
                return storage.GetMassAvailable(consumeTag) >= 10f;
            }
        }

        ObjectCanMove OriginCanMoveCompent {
            get {
                return PackAnythingStaticVars.MoveStatus.watingMoveObject;
            }
        }

        GameObject OriginObject {
            get {
                return PackAnythingStaticVars.MoveStatus.watingMoveObject.gameObject;
            }
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            createElementList.Clear();
            createElementList.Add(ElementLoader.FindElementByHash(SimHashes.Iron), 7f);
            createElementList.Add(ElementLoader.FindElementByHash(SimHashes.RefinedCarbon), 2f);
            if (!SingletonOptions<Options>.Instance.DontConsumeAnything) {
                ManualDeliveryKG manualDeliveryKg = gameObject.AddOrGet<ManualDeliveryKG>();
                manualDeliveryKg.SetStorage(storage);
                manualDeliveryKg.RequestedItemTag = SimHashes.Steel.CreateTag();
                manualDeliveryKg.capacity = 200f;
                manualDeliveryKg.refillMass = 10f;
                manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
            }
        }

        private void ConsumeElement() {
            storage.ConsumeIgnoringDisease(consumeTag, 10f);
            foreach(KeyValuePair<Element, float> pair in createElementList) {
                Fall(pair.Key.substance.SpawnResource(gameObject.transform.position, pair.Value, 0f, byte.MaxValue, 0));
            }
        }

        private void Fall(GameObject go) {
            if (GameComps.Fallers.Has(go)) GameComps.Fallers.Remove(go);
            Vector2 initial_velocity = new Vector2(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(4f, 8f));
            GameComps.Fallers.Add(go, initial_velocity);
        }

        public void StartMoving() {
            PackAnythingStaticVars.SetMoving(true);
            OriginCanMoveCompent.RefershObjectType();
            if (OriginCanMoveCompent.objectType == ObjectType.GravitasCreatureManipulator) {
                SetFinalPosition(OriginObject);
            } else {
                CloneOriginObject(out GameObject clonedObject);
                SetFinalPosition(clonedObject);
                clonedObject.SetActive(false);
                clonedObject.SetActive(true);
                OriginCanMoveCompent.DestoryOriginObject();
            }
            PackAnythingStaticVars.SetMoving(false);
            if (!SingletonOptions<Options>.Instance.DontConsumeAnything) {
                ConsumeElement();
            }
        }

        public void SetFinalPosition(GameObject final) {
            int originCell = Grid.PosToCell(OriginObject.transform.position);
            Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
            if (OriginCanMoveCompent.objectType == ObjectType.Geyser) {
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
            if (OriginCanMoveCompent.objectType == ObjectType.Geyser || OriginCanMoveCompent.objectType == ObjectType.RadiationEmitter) {
                clonedObject = Util.KInstantiate(Assets.GetPrefab(OriginObject.GetComponent<KPrefabID>().PrefabTag));
            } else {
                clonedObject = GameUtil.KInstantiate(OriginObject, OriginObject.transform.position, Grid.SceneLayer.Building);
            }
            ObjectCanMove surveyable = clonedObject.GetComponent<ObjectCanMove>();
            surveyable.objectType = OriginCanMoveCompent.objectType;
            surveyable.isSurveyed = true;
            PackAnythingStaticVars.SurveableCmps.Add(surveyable);
            switch (OriginCanMoveCompent.objectType) {
                case ObjectType.Geyser:
                    if (OriginObject.GetComponent<Studyable>().Studied) {
                        IDetouredField<Studyable, bool> detoured = PDetours.DetourField<Studyable, bool>("studied");
                        detoured.Set(clonedObject.AddOrGet<Studyable>(), true);
                    }
                    break;
                case ObjectType.RadiationEmitter:
                    ToogleSetLockerr(clonedObject);
                    ToogleLoreBearer(clonedObject);
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

        public void ToogleSetLockerr(GameObject needToogle) {
            if (OriginObject.GetComponent<SetLocker>() != null) {
                if (!OriginObject.GetComponent<SetLocker>().SidescreenButtonInteractable()) {
                    PDetours.DetourField<SetLocker, bool>("used").Set(needToogle.AddOrGet<SetLocker>(), true);
                }
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
