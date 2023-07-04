
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace FixPack {
    [ConfigFile("fix_pack.json", true, true)]
    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    public class Option {
        [Option("STRINGS.UI.FIX_PACK.GENERAL.ACTIVE", "STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.DESC", "STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.TITLE")]
        [JsonProperty]
        public bool ActiveDestructibleFeatures { get; set; } = false;

        [Option("STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.OPTION1", "", "STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.TITLE")]
        [JsonProperty]
        public bool RemoveNeutronium { get; set; } = true;

        [Option("STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.OPTION2", "", "STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.TITLE")]
        [JsonProperty]
        public bool ReplaceNeutroniumWithObsidian { get; set; } = true;

        [Option("STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.OPTION3", "", "STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.TITLE", Format = "F0")]
        [Limit(100.0, 3600.0)]
        [JsonProperty]
        public float AnaylsisTime { get; set; } = 3600;

        [Option("STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.OPTION4", "", "STRINGS.UI.FIX_PACK.DESTRUCTIBLE_FEATURES.TITLE", Format = "F0")]
        [Limit(100.0, 3600.0)]
        [JsonProperty]
        public float DeconstructTime { get; set; } = 1800;

        //------------------------------------------------
        
        [Option("STRINGS.UI.FIX_PACK.GENERAL.ACTIVE", "STRINGS.UI.FIX_PACK.DECONSTRUCTABLE_PROPS.DESC", "STRINGS.UI.FIX_PACK.DECONSTRUCTABLE_PROPS.TITLE")]
        [JsonProperty]
        public bool ActiveDeconstructableProps { get; set; } = false;

        //------------------------------------------------

        [Option("STRINGS.UI.FIX_PACK.GENERAL.ACTIVE", "STRINGS.UI.FIX_PACK.FLUID_SHIPPING.DESC", "STRINGS.UI.FIX_PACK.FLUID_SHIPPING.TITLE")]
        [JsonProperty]
        public bool ActiveFluidShipping { get; set; } = false;

        [Option("STRINGS.UI.FIX_PACK.FLUID_SHIPPING.OPTION1", "Internal storage volume of Canister Inserter (Kg).", "STRINGS.UI.FIX_PACK.FLUID_SHIPPING.TITLE")]
        [JsonProperty]
        public float CanisterVolume { get; set; } = 10;

        [Option("STRINGS.UI.FIX_PACK.FLUID_SHIPPING.OPTION2", "Internal storage volume of Bottle Inserter (Kg).", "STRINGS.UI.FIX_PACK.FLUID_SHIPPING.TITLE")]
        [JsonProperty]
        public float BottleVolume { get; set; } = 200;

        [Option("STRINGS.UI.FIX_PACK.FLUID_SHIPPING.OPTION3", "Internal storage volume of Bottle Filler(Kg).", "STRINGS.UI.FIX_PACK.FLUID_SHIPPING.TITLE")]
        [JsonProperty]
        public float BottleFillerVolume { get; set; } = 200;

        //------------------------------------------------

        [Option("STRINGS.UI.FIX_PACK.GENERAL.ACTIVE", "STRINGS.UI.FIX_PACK.STORAGE_POD.DESC", "STRINGS.UI.FIX_PACK.STORAGE_POD.TITLE")]
        [JsonProperty]
        public bool ActiveStoragePod { get; set; } = false;

        [Option("STRINGS.UI.FIX_PACK.STORAGE_POD.OPTION1", "How many kg of Solids a Storage Pod can store.", "STRINGS.UI.FIX_PACK.STORAGE_POD.TITLE", Format = "F0")]
        [JsonProperty]
        public float podCapacity { get; set; } = 5000f;

        [Option("STRINGS.UI.FIX_PACK.STORAGE_POD.OPTION2", "How many kg of Solids a Cool Pod can store.", "STRINGS.UI.FIX_PACK.STORAGE_POD.TITLE", Format = "F0")]
        [JsonProperty]
        public float coolPodCapacity { get; set; } = 50f;

        [Option("STRINGS.UI.FIX_PACK.STORAGE_POD.OPTION3", "Can you store food in a Storage Pod?", "STRINGS.UI.FIX_PACK.STORAGE_POD.TITLE")]
        [JsonProperty]
        public bool podStoresFood { get; set; } = false;
    }
}
