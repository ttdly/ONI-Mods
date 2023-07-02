// Decompiled with JetBrains decompiler
// Type: StorageLockerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C4FA4E6F-758D-4D97-B8F6-20B31F856ED3
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using PeterHan.PLib.Options;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace FixPack.FluidShipping {
    public class BottleInserterConfig : IBuildingConfig {
        public override BuildingDef CreateBuildingDef() {
            string id = S_BI_ID;
            int width = 1;
            int height = 2;
            string anim = "stormshark_bottleinserter_kanim";
            int hitpoints = 30;
            float construction_time = 10f;
            float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] rawMinerals = MATERIALS.RAW_MINERALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tieR4, rawMinerals, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Overheatable = false;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.allowItemRemoval = false;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.alwaysDispense = true;
            storage.capacityKg = SingletonOptions<Option>.Instance.BottleVolume; //200 kg default
            go.AddOrGet<TreeFilterable>();
            go.AddOrGet<VesselInserter>();
        }

        public override void DoPostConfigureComplete(GameObject go) {
            //go.AddOrGetDef<StorageController.Def>();
        }

        public const string S_BI_ID = "StormShark.BottleInserter";
        static readonly string Name = "Bottle Inserter";
        static readonly string Description = "Bottle Inserters allow contained liquids to be inserted directly into a pipe network.";
        static readonly string Effect = "Loads " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " bottles into " + UI.FormatAsLink("Pipes", "LIQUIDPIPING") + " for transport.\n\nMust be loaded by Duplicants.";
        public static void Setup() {
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{S_BI_ID.ToUpperInvariant()}.NAME", "<link=\"" + S_BI_ID + "\">" + Name + "</link>");
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{S_BI_ID.ToUpperInvariant()}.DESC", Description);
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{S_BI_ID.ToUpperInvariant()}.EFFECT", Effect);

            ModUtil.AddBuildingToPlanScreen("Plumbing", S_BI_ID);
        }
    }
}