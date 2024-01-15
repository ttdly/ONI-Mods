using KSerialization;
using TUNING;
using UnityEngine;
using static Operational;


namespace SaltBox {
    public class SaltBoxConfig : IBuildingConfig {
        public const string ID = "SaltBox";

        public override BuildingDef CreateBuildingDef() {
            float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] rawMinerals = MATERIALS.RAW_MINERALS;
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR0 = BUILDINGS.DECOR.BONUS.TIER0;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "rationbox_kanim", 10, 10f, tieR4, rawMinerals, 1600f, BuildLocationRule.OnFloor, tieR0, noise);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            SoundEventVolumeCache.instance.AddVolume("rationbox_kanim", "RationBox_open", NOISE_POLLUTION.NOISY.TIER1);
            SoundEventVolumeCache.instance.AddVolume("rationbox_kanim", "RationBox_close", NOISE_POLLUTION.NOISY.TIER1);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
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
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]{
                new ElementConverter.ConsumedElement(SimHashes.Salt.CreateTag(), 0.01f)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[1] {
                new ElementConverter.OutputElement(0.001f, SimHashes.Sand, 0.0f, storeOutput: true)
            };
            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.RequestedItemTag = SimHashes.Salt.CreateTag();
            manualDeliveryKg.capacity = 320f;
            manualDeliveryKg.refillMass = 32;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
            ElementDropper elementDropper = go.AddComponent<ElementDropper>();
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
        [MyCmpReq]
        private readonly Storage storage;
        [Serialize]
        private float userMaxCapacity = float.PositiveInfinity;
        private FilteredStorage filteredStorage;
        private static readonly EventSystem.IntraObjectHandler<SaltBox> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SaltBox>((component, data) => component.OnOperationalChanged(data));
        private static readonly EventSystem.IntraObjectHandler<SaltBox> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<SaltBox>((component, data) => component.OnCopySettings(data));
        private static readonly EventSystem.IntraObjectHandler<SaltBox> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<SaltBox>((component, data) => component.OnStorageChange(data));

        protected override void OnPrefabInit() {
            filteredStorage = new FilteredStorage(this, new Tag[1]{
                GameTags.Compostable
            }, this, false, Db.Get().ChoreTypes.FoodFetch);
            FoodStorage component = this.GetComponent<FoodStorage>();
            component.FilteredStorage = this.filteredStorage;
            component.SpicedFoodOnly = component.SpicedFoodOnly;
            filteredStorage.FilterChanged();
            Subscribe(-592767678, OnOperationalChangedDelegate);
            Subscribe(-905833192, OnCopySettingsDelegate);
            Subscribe((int) GameHashes.OnStorageChange, OnStorageChangeDelegate);
            DiscoveredResources.Instance.Discover("FieldRation".ToTag(), GameTags.Edible);
        }

        private void OnStorageChange(object data) {
            GameObject go = data as GameObject;
            if (go == null) return;
            if (go.HasTag(GameTags.Stored)) return;
            go.RemoveTag(StaticVars.StoredInSaltBox);
        }

        protected override void OnSpawn() {
            Operational component = GetComponent<Operational>();
            component.SetActive(component.IsOperational);
            filteredStorage.FilterChanged();
            PickupSalt();
        }

        public void PickupSalt() {
            GameObject gameObject = Grid.Objects[Grid.PosToCell(this.gameObject.transform.GetPosition()), 3];
            if (gameObject != null) {
                ObjectLayerListItem objectLayerListItem = gameObject.GetComponent<Pickupable>().objectLayerListItem;
                while (objectLayerListItem != null) {
                    Pickupable component = objectLayerListItem.gameObject.GetComponent<Pickupable>();
                    if (component != null && component.HasTag(SimHashes.Salt.CreateTag())) {
                        storage.Store(component.gameObject);
                        break;
                    }
                    objectLayerListItem = objectLayerListItem.nextItem;
                }
            }
        }

        protected override void OnCleanUp() => filteredStorage.CleanUp();

        private void OnOperationalChanged(object _) {
            Operational component = GetComponent<Operational>();
            component.SetActive(component.IsOperational);
        }

        private void OnCopySettings(object data) {
            GameObject gameObject = (GameObject)data;
            if (gameObject == null)
                return;
            RationBox component = gameObject.GetComponent<RationBox>();
            if (component == null)
                return;
            UserMaxCapacity = component.UserMaxCapacity;
        }

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

        public float RotTemperature => 277.15f;

        public float PreserveTemperature => 255.15f;
    }

    public class SaltBoxController : GameStateMachine<SaltBoxController, SaltBoxController.Instance> {

        public class Def : BaseDef {
        }

        public new class Instance : GameInstance {
            [MyCmpReq]
            public Storage storage;
            [MyCmpReq]
            public ElementConverter converter;
            public Instance(IStateMachineTarget master, Def def)
                : base(master, def) {
            }
        }

        public State off;

        public State on;

        public override void InitializeStates(out BaseState default_state) {
            default_state = off;
            off.PlayAnim("off").UpdateTransition(on, AnyUnfreshFood, UpdateRate.SIM_4000ms, load_balance: true).Enter(RemoveTag);
            on.PlayAnim("on").UpdateTransition(off, AllFoodFresh, UpdateRate.SIM_4000ms, load_balance: true).Enter(AddTag);
        }

        private void AddTag(Instance smi) {
            smi.converter.SetAllConsumedActive(true);
            foreach (GameObject item in smi.storage.items) {
                if (item == null) continue;
                item.AddTag(StaticVars.StoredInSaltBox);
            }
        }

        private void RemoveTag(Instance smi) {
            smi.converter.SetAllConsumedActive(false);
            foreach (GameObject item in smi.storage.items) {
                if (item == null) continue;
                item.RemoveTag(StaticVars.StoredInSaltBox);
            }
            
        }

        public bool HasEnoughMass(Instance smi) {
            GameObject go = smi.storage.Find((int)SimHashes.Salt);
            if (go == null) return false;
            return go.GetComponent<PrimaryElement>().Mass > 1;
        }

        private bool AnyUnfreshFood(Instance smi, float _) {
            if (!HasEnoughMass(smi)) return false;
            foreach (GameObject item in smi.storage.items) {
                if (!(item == null)) {
                    if (item.GetSMI<Rottable.Instance>()?.RotConstitutionPercentage < 0.5) {
                        smi.converter.SetAllConsumedActive(true);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool AllFoodFresh(Instance smi, float _) {
            if (!HasEnoughMass(smi)) return true;
            foreach (GameObject item in smi.storage.items) {
                if (!(item == null)) {
                    if (item.GetSMI<Rottable.Instance>()?.RotConstitutionPercentage < 1) return false;
                }
            }
            smi.converter.SetAllConsumedActive(false);
            return true;
        }
    }

}
