using System.Collections.Generic;
using KSerialization;
using TUNING;

namespace TweaksPack.Tweakable {
  [SerializationConfig(MemberSerialization.OptIn)]
  public class BaseTweakable : Workable {
    private static readonly EventSystem.IntraObjectHandler<BaseTweakable> OnRefreshUserMenuDelegate =
      new EventSystem.IntraObjectHandler<BaseTweakable>((component, data) => component.OnRefreshUserMenu(data));

    [Serialize] public bool isMarkForTweak;

    private Chore chore;
    public Dictionary<Tag, float> materialNeeds;

    private CellOffset[] PlacementOffsets {
      get {
        var component1 = GetComponent<Building>();
        if (component1 != null)
          return component1.Def.PlacementOffsets;
        var component2 = GetComponent<OccupyArea>();
        if (component2 != null)
          return component2.OccupiedCellsOffsets;
        Debug.Assert(false, "Ack! We put a Tweakable on something that's neither a Building nor OccupyArea!", this);
        return null;
      }
    }

    protected override void OnStopWork(WorkerBase worker) {
      base.OnStopWork(worker);
      DestroyWork();
    }

    protected override void OnAbortWork(WorkerBase worker) {
      base.OnAbortWork(worker);
      DestroyWork();
    }

    protected override void OnCompleteWork(WorkerBase worker) {
      base.OnCompleteWork(worker);
      DestroyWork();
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      var table = OffsetGroups.InvertedStandardTable;
      CellOffset[] filter = null;
      SetOffsetTable(OffsetGroups.BuildReachabilityTable(PlacementOffsets, table, filter));
      Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
      Subscribe((int)GameHashes.StatusChange, OnRefreshUserMenuDelegate);
      Toogle();
    }

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
      workerStatusItem = Db.Get().DuplicantStatusItems.Building;
      workingStatusItem = null;
      attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
      attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
      minimumAttributeMultiplier = 0.75f;
      skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
      skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
      Prioritizable.AddRef(gameObject);
      requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
      shouldShowSkillPerkStatusItem = false;
      synchronizeAnims = false;
      faceTargetWhenWorking = true;
      multitoolContext = (HashedString)"build";
      multitoolHitEffectTag = (Tag)EffectConfigs.BuildSplashId;
    }

    private void OnRefreshUserMenu(object _) {
      if (gameObject.HasTag(TweakableStaticVars.Tags.DontTweak)) return;
      Game.Instance.userMenu.AddButton(gameObject, isMarkForTweak
        ? new KIconButtonMenu.ButtonInfo("action_cancel", TweaksPackStrings.UI.BUTTON.OFF.NAME, () => {
          isMarkForTweak = !isMarkForTweak;
          Toogle();
        }, tooltipText: TweaksPackStrings.UI.BUTTON.OFF.TOOL_TIP)
        : new KIconButtonMenu.ButtonInfo("action_repair", TweaksPackStrings.UI.BUTTON.ON.NAME, () => {
          isMarkForTweak = !isMarkForTweak;
          Toogle();
        }, tooltipText: TweaksPackStrings.UI.BUTTON.ON.TOOLTIP));
    }

    private void ActiveWork() {
      chore = new WorkChore<BaseTweakable>(Db.Get().ChoreTypes.Build, this, only_when_operational: false);
    }

    private void DestroyWork() {
      if (chore != null) {
        chore.Cancel("Cancel Tweak");
        chore = null;
      }

      if (isMarkForTweak) isMarkForTweak = false;
    }

    private void Toogle() {
      if (isMarkForTweak) {
        if (chore != null) return;
        ActiveWork();
      } else {
        DestroyWork();
      }
    }

    public void Refresh() {
      isMarkForTweak = false;
      if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(gameObject))
        DetailsScreen.Instance.Show(false);
      OnRefreshUserMenu(null);
    }
  }
}