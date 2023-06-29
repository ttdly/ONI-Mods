
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
    }
}
