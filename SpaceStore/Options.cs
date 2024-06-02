using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace SpaceStore {
  [ModInfo("https://github.com/ttdly/ONI-Mods")]
  [ConfigFile("SpaceStoreConfig.json", true, true)]
  [RestartRequired]
  [JsonObject(MemberSerialization.OptIn)]
  public class Options {
    [Option("STRINGS.OPTIONS.ITEM_WIDTH", "")]
    [JsonProperty]
    public int ItemWidth { get; set; } = 130;

    [Option("STRINGS.OPTIONS.COL", "")]
    [JsonProperty]
    public int Col { get; set; } = 5;

    [Option("STRINGS.OPTIONS.ITEM_WIDTH", "")]
    [JsonProperty]
    public int DialogHeight { get; set; } = 400;
  }
}