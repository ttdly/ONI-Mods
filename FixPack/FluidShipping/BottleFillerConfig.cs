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
    public class BottleFillerConfig : IBuildingConfig {

        public override BuildingDef CreateBuildingDef() {
            string id = S_BF_ID;
            int width = 3;
            int height = 2;
            string anim = "gas_bottler_kanim";
            int hitpoints = 100;
            float construction_time = 120f;
            float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] rawMinerals = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues none = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tieR4, rawMinerals, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.Overheatable = false;
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            ////Bottle Filler as storage
            //Storage storage = go.AddOrGet<Storage>();
            //storage.showInUI = true;
            //storage.allowItemRemoval = true;
            //storage.showDescriptor = true;
            //storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);

            //storage.capacityKg = BuildingGenerationPatches.Options.BottleFillerVolume; //1000 kg default

            //ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            //conduitConsumer.conduitType = ConduitType.Liquid;
            //conduitConsumer.ignoreMinMassCheck = true;
            //conduitConsumer.forceAlwaysSatisfied = true;
            //conduitConsumer.alwaysConsume = true;
            //conduitConsumer.capacityKG = storage.capacityKg;

            //go.AddOrGet<BottleFiller>();
            //go.AddOrGet<BuildingComplete>().isManuallyOperated = true;

            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go, false);
            defaultStorage.showDescriptor = true;
            defaultStorage.storageFilters = STORAGEFILTERS.LIQUIDS;
            defaultStorage.capacityKg = SingletonOptions<Option>.Instance.BottleFillerVolume;
            defaultStorage.allowItemRemoval = false;
            defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);

            go.AddOrGet<DropAllWorkable>();
            LiquidBottler liquidBottler = go.AddOrGet<LiquidBottler>();
            liquidBottler.storage = defaultStorage;
            liquidBottler.workTime = 9f;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.storage = defaultStorage;
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = defaultStorage.capacityKg;
            conduitConsumer.keepZeroMassObject = false;

        }

        public override void DoPostConfigureComplete(GameObject go) {
            //go.AddOrGetDef<StorageController.Def>();
        }

        public const string S_BF_ID = "StormShark.BottleFiller";
        static readonly string Name = "Bottle Filler";
        static readonly string Description = "Bottle Fillers allow liquids piped to their internal storage to be hand-collected by Duplicants.";
        static readonly string Effect = "Fills " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " bottles from internal storage. \n\nMust be filled via plumbing network.";
        public static void Setup() {
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{S_BF_ID.ToUpperInvariant()}.NAME", "<link=\"" + S_BF_ID + "\">" + Name + "</link>");
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{S_BF_ID.ToUpperInvariant()}.DESC", Description);
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{S_BF_ID.ToUpperInvariant()}.EFFECT", Effect);

            ModUtil.AddBuildingToPlanScreen("Plumbing", S_BF_ID);
        }
    }
}