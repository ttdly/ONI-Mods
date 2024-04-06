using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace SpaceStore {
    [RestartRequired]
    [JsonObject(MemberSerialization.OptIn)]
    public class Options {
        [Option("OPTIONS.ITEM_WIDTH", "", null)]
        [JsonProperty]
        public int ItemWidth { get; set; } = 130;

        [Option("OPTIONS.COL", "", null)]
        [JsonProperty]
        public int Col { get; set; } = 5;

        [Option("OPTIONS.ITEM_WIDTH", "", null)]
        [JsonProperty]
        public int DialogHeight { get; set; } = 400;
    }
}
