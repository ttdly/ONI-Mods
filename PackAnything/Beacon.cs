using Klei.AI;
using KSerialization;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using TUNING;
using UnityEngine;

namespace PackAnything {
    [SerializationConfig(MemberSerialization.OptIn)]
    public class Beacon : Workable {
        private Chore chore;
        [Serialize]
        private bool isMarkForActive;
        [Serialize]
        public bool isGeyser = false;
        [Serialize]
        public int originCell;
        [Serialize]
        private int unoCount;
        private Guid statusItemGuid;
        public bool MarkFroPack => isMarkForActive;

        private static readonly EventSystem.IntraObjectHandler<Beacon> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Beacon>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            workerStatusItem = PackAnythingStaticVars.ActivingBecaon;
            faceTargetWhenWorking = true;
            synchronizeAnims = false;
            requiredSkillPerk = PackAnythingStaticVars.CanPack.Id;
            attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
            attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
            skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
            skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
            overrideAnims = new KAnimFile[1]{
                Assets.GetAnim((HashedString) "anim_use_remote_kanim")
            };
            faceTargetWhenWorking = true;
            SetWorkTime(50f);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            Subscribe((int)GameHashes.StatusChange, OnRefreshUserMenuDelegate);
            if (isMarkForActive) {
                OnClickActive();
            }
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            if (gameObject.HasTag(GameTags.Stored)) return;
            ActiveIt(worker);
            OnClickCancel();
        }

        protected override void OnAbortWork(Worker worker) {
            base.OnAbortWork(worker);
            if (isMarkForActive) {
                AddStatus();
            }
            LightActive(false);
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
            progressBar.barColor = new Color(0.5f, 0.7f, 1.0f, 1f);
            RemoveStatus();
            LightActive(true);
        }

        // 自定义的方法
        public void OnRefreshUserMenu(object _) {
            if (gameObject.HasTag(GameTags.Stored)) return;
            Game.Instance.userMenu.AddButton(gameObject, isMarkForActive ? new KIconButtonMenu.ButtonInfo("action_empty_contents", PackAnythingString.UI.ACTIVATE.NAME_OFF, new System.Action(OnClickCancel), tooltipText: PackAnythingString.UI.ACTIVATE.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_empty_contents", PackAnythingString.UI.ACTIVATE.NAME, new System.Action(OnClickActive), tooltipText: PackAnythingString.UI.ACTIVATE.TOOLTIP));
        }

        public void OnClickCancel() {
            if (chore == null && !isMarkForActive) {
                return;
            }
            isMarkForActive = false;
            chore.Cancel("Active.CancelChore");
            chore = null;
            RemoveStatus();
            Prioritizable.RemoveRef(gameObject);
            LightActive(false);
        }

        public void OnClickActive() {
            Prioritizable.AddRef(gameObject);
            isMarkForActive = true;
            if (chore != null) return;
            chore = new WorkChore<Beacon>(PackAnythingStaticVars.Active, this, only_when_operational: false);
            AddStatus();
            // 代码可用，但是考虑到激活的时候最有可能出现毛病的地方是这里，所以先弃用
            //Pickupable pickupable = gameObject.GetComponent<Pickupable>();
            //pickupable.OnTake += (_, amount) => Take(pickupable, amount);
            //gameObject.GetComponent<Pickupable>().CanAbsorb = (Pickupable other) => true;
        }

        public void ActiveIt(Worker worker) {
            GameObject originObject = null;
            foreach (Surveyable surveyable in PackAnythingStaticVars.SurveableCmps){
                if (Grid.PosToCell(surveyable) == originCell) {
                    originObject = surveyable.gameObject;
                }
            }
            if (originObject != null) {
                int cell = Grid.PosToCell(gameObject);
                Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
                KSelectable selectable = originObject.GetComponent<KSelectable>();
                OccupyArea occupyArea = originObject.GetComponent<OccupyArea>();
                Building building = originObject.GetComponent<Building>();
                if (isGeyser) {
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
                originObject.GetComponent<Surveyable>().hasBacon = false;
            }
            KBatchedAnimController kBatchedAnimController = gameObject.GetComponent<KBatchedAnimController>();
            kBatchedAnimController.Play("destroy");
            kBatchedAnimController.destroyOnAnimComplete = true;
            if (worker.HasTag(GameTags.Minion)) {
                RegisterReactEmotePair("ActiveBecaon", Db.Get().Emotes.Minion.ResearchComplete, 3f, worker);
            }
        }

        //public Pickupable Take(Pickupable pickupable, float _) {
        //    OnClickCancel();
        //    return pickupable;
        //}

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

        private void RegisterReactEmotePair(string reactable_id, Emote emote, float max_trigger_time, Worker worker) {
            if (worker.gameObject == null)
                return;
            ReactionMonitor.Instance smi = worker.gameObject.GetSMI<ReactionMonitor.Instance>();
            if (smi == null)
                return;
            EmoteChore emoteChore = new EmoteChore(worker.gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteIdle, emote);
            SelfEmoteReactable reactable = new SelfEmoteReactable(worker.gameObject, (HashedString)reactable_id, Db.Get().ChoreTypes.Emote, max_trigger_time);
            emoteChore.PairReactable(reactable);
            reactable.SetEmote(emote);
            reactable.PairEmote(emoteChore);
            smi.AddOneshotReactable(reactable);
        }

        private void AddStatus() {
            KSelectable kSelectable = GetComponent<KSelectable>();
            if (kSelectable != null) statusItemGuid = kSelectable.ReplaceStatusItem(statusItemGuid, PackAnythingStaticVars.WaitingActive);
        }

        private void RemoveStatus() {
            KSelectable kSelectable = GetComponent<KSelectable>();
            if (kSelectable != null) statusItemGuid = kSelectable.RemoveStatusItem(statusItemGuid);
        }

        private void LightActive(bool on) {
            if (on) {
                Light2D light2D = gameObject.AddOrGet<Light2D>();
                light2D.Color = new Color(0.6f, 0f, 0.6f, 1f);
                light2D.Range = 5f;
                light2D.Offset = new Vector2(0, 1);
                light2D.overlayColour = new Color(0.8f, 0f, 0.8f, 1f);
                light2D.shape = LightShape.Circle;
                light2D.drawOverlay = true;
            } else {
                Destroy(gameObject.AddOrGet<Light2D>());
            }
        }
    }
}
