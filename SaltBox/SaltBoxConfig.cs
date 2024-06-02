using KSerialization;
using TUNING;
using UnityEngine;

namespace SaltBox {
  public class SaltBoxConfig : IBuildingConfig {
    public const string ID = "SaltBox";

    public override BuildingDef CreateBuildingDef() {
      var tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
      var rawMinerals = MATERIALS.RAW_MINERALS;
      var none = NOISE_POLLUTION.NONE;
      var tieR0 = BUILDINGS.DECOR.BONUS.TIER0;
      var noise = none;
      var buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "rationbox_kanim", 10, 10f, tieR4, rawMinerals,
        1600f, BuildLocationRule.OnFloor, tieR0, noise);
      buildingDef.Overheatable = false;
      buildingDef.Floodable = false;
      buildingDef.AudioCategory = "Metal";
      SoundEventVolumeCache.instance.AddVolume("rationbox_kanim", "RationBox_open", NOISE_POLLUTION.NOISY.TIER1);
      SoundEventVolumeCache.instance.AddVolume("rationbox_kanim", "RationBox_close", NOISE_POLLUTION.NOISY.TIER1);
      return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
      Prioritizable.AddRef(go);
      var storage = go.AddOrGet<Storage>();
      storage.capacityKg = 500f;
      storage.showInUI = true;
      storage.showDescriptor = true;
      storage.storageFilters = STORAGEFILTERS.FOOD;
      storage.allowItemRemoval = true;
      storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
      storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
      storage.showCapacityStatusItem = true;
      storage.showCapacityAsMainStatus = true;
      go.AddOrGet<TreeFilterable>();
      go.AddOrGet<FoodStorage>();
      go.AddOrGet<SaltBox>();
      go.AddOrGetDef<RocketUsageRestriction.Def>();
      var elementConverter = go.AddOrGet<ElementConverter>();
      elementConverter.consumedElements = new ElementConverter.ConsumedElement[1] {
        new ElementConverter.ConsumedElement(SimHashes.Salt.CreateTag(), 0.002f)
      };
      elementConverter.outputElements = new ElementConverter.OutputElement[1] {
        new ElementConverter.OutputElement(0.001f, SimHashes.Sand, 0.0002f, storeOutput: true)
      };
      var manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
      manualDeliveryKg.SetStorage(storage);
      manualDeliveryKg.RequestedItemTag = SimHashes.Salt.CreateTag();
      manualDeliveryKg.capacity = 300f;
      manualDeliveryKg.refillMass = 30f;
      manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
      var elementDropper = go.AddComponent<ElementDropper>();
      elementDropper.emitMass = 10f;
      elementDropper.emitTag = SimHashes.Sand.CreateTag();
      elementDropper.emitOffset = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public override void DoPostConfigureComplete(GameObject go) {
      go.AddOrGetDef<StorageController.Def>();
      go.AddOrGetDef<SaltBoxController.Def>();
    }
  }

  public class SaltBox : KMonoBehaviour, IUserControlledCapacity {
    private static readonly EventSystem.IntraObjectHandler<SaltBox> OnOperationalChangedDelegate =
      new EventSystem.IntraObjectHandler<SaltBox>((component, data) => component.OnOperationalChanged(data));

    private static readonly EventSystem.IntraObjectHandler<SaltBox> OnCopySettingsDelegate =
      new EventSystem.IntraObjectHandler<SaltBox>((component, data) => component.OnCopySettings(data));

    private static readonly EventSystem.IntraObjectHandler<SaltBox> OnStorageChangeDelegate =
      new EventSystem.IntraObjectHandler<SaltBox>((component, data) => component.OnStorageChange(data));

    [MyCmpReq] private readonly Storage storage;

    private FilteredStorage filteredStorage;

    [Serialize] private float userMaxCapacity = float.PositiveInfinity;

    public float RotTemperature => 277.15f;

    public float PreserveTemperature => 255.15f;

    public float UserMaxCapacity {
      get => Mathf.Min(userMaxCapacity, storage.capacityKg);
      set {
        userMaxCapacity = value;
        filteredStorage.FilterChanged();
      }
    }

    public float AmountStored => storage.MassStored();

    public float MinCapacity => 0.0f;

    public float MaxCapacity => storage.capacityKg;

    public bool WholeValues => false;

    public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

    protected override void OnPrefabInit() {
      filteredStorage = new FilteredStorage(this, new Tag[1] {
        GameTags.Compostable
      }, this, false, Db.Get().ChoreTypes.FoodFetch);
      var component = GetComponent<FoodStorage>();
      component.FilteredStorage = filteredStorage;
      component.SpicedFoodOnly = component.SpicedFoodOnly;
      filteredStorage.FilterChanged();
      Subscribe(-592767678, OnOperationalChangedDelegate);
      Subscribe(-905833192, OnCopySettingsDelegate);
      Subscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
      DiscoveredResources.Instance.Discover("FieldRation".ToTag(), GameTags.Edible);
    }

    private void OnStorageChange(object data) {
      var go = data as GameObject;
      if (go == null) return;
      if (go.HasTag(GameTags.Stored)) return;
      go.RemoveTag(StaticVars.StoredInSaltBox);
    }

    protected override void OnSpawn() {
      var component = GetComponent<Operational>();
      component.SetActive(component.IsOperational);
      filteredStorage.FilterChanged();
      PickupSalt();
    }

    public void PickupSalt() {
      var gameObject = Grid.Objects[Grid.PosToCell(this.gameObject.transform.GetPosition()), 3];
      if (gameObject != null) {
        var objectLayerListItem = gameObject.GetComponent<Pickupable>().objectLayerListItem;
        while (objectLayerListItem != null) {
          var component = objectLayerListItem.gameObject.GetComponent<Pickupable>();
          if (component != null && component.HasTag(SimHashes.Salt.CreateTag())) {
            storage.Store(component.gameObject);
            break;
          }

          objectLayerListItem = objectLayerListItem.nextItem;
        }
      }
    }

    protected override void OnCleanUp() {
      filteredStorage.CleanUp();
    }

    private void OnOperationalChanged(object _) {
      var component = GetComponent<Operational>();
      component.SetActive(component.IsOperational);
    }

    private void OnCopySettings(object data) {
      var gameObject = (GameObject)data;
      if (gameObject == null)
        return;
      var component = gameObject.GetComponent<RationBox>();
      if (component == null)
        return;
      UserMaxCapacity = component.UserMaxCapacity;
    }
  }

  public class SaltBoxController : GameStateMachine<SaltBoxController, SaltBoxController.Instance> {
    public State off;

    public State on;

    public override void InitializeStates(out BaseState default_state) {
      default_state = off;
      root.Enter(smi => smi.converter.SetWorkSpeedMultiplier(0.001f));
      off.PlayAnim("off").UpdateTransition(on, AnyUnfreshFood, UpdateRate.SIM_4000ms, true).Enter(RemoveTag);
      on.PlayAnim("on").UpdateTransition(off, AllFoodFresh, UpdateRate.SIM_4000ms, true).Enter(AddTag);
    }

    private void AddTag(Instance smi) {
      smi.converter.SetAllConsumedActive(true);
      foreach (var item in smi.storage.items) {
        if (item == null) continue;
        item.AddTag(StaticVars.StoredInSaltBox);
      }
    }

    private void RemoveTag(Instance smi) {
      smi.converter.SetAllConsumedActive(false);
      foreach (var item in smi.storage.items) {
        if (item == null) continue;
        item.RemoveTag(StaticVars.StoredInSaltBox);
      }
    }

    public bool HasEnoughMass(Instance smi) {
      var go = smi.storage.Find((int)SimHashes.Salt);
      if (go == null) return false;
      return go.GetComponent<PrimaryElement>().Mass > 1;
    }

    private bool AnyUnfreshFood(Instance smi, float _) {
      if (!HasEnoughMass(smi)) return false;
      foreach (var item in smi.storage.items)
        if (!(item == null))
          if (item.GetSMI<Rottable.Instance>()?.RotConstitutionPercentage < 0.5) {
            smi.converter.SetAllConsumedActive(true);
            return true;
          }

      return false;
    }

    private bool AllFoodFresh(Instance smi, float _) {
      if (!HasEnoughMass(smi)) return true;
      foreach (var item in smi.storage.items)
        if (!(item == null))
          if (item.GetSMI<Rottable.Instance>()?.RotConstitutionPercentage < 1)
            return false;
      smi.converter.SetAllConsumedActive(false);
      return true;
    }

    public class Def : BaseDef {
    }

    public new class Instance : GameInstance {
      [MyCmpReq] public ElementConverter converter;

      [MyCmpReq] public Storage storage;

      public Instance(IStateMachineTarget master, Def def)
        : base(master, def) {
      }
    }
  }
}