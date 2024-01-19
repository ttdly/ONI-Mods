using Newtonsoft.Json;
using PeterHan.PLib.Options;


namespace PackAnything {
    [RestartRequired]
    [JsonObject(MemberSerialization.OptIn)]
    public class Options {
        [Option("STRINGS.GENERATE_UNOBTANIUM", "STRINGS.GENERATE_UNOBTANIUM_DESC", null)]
        [JsonProperty]
        public bool GenerateUnobtanium { get; set; } = true;
        [Option("STRINGS.NO_NEED_DUPLICANTS_OPERATE", "", null)]
        [JsonProperty]
        public bool NoDuplicateOperation { get; set; } = false;
    }
}
