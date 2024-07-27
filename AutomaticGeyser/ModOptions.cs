using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace AutomaticGeyser {
  [RestartRequired]
  [JsonObject(MemberSerialization.OptIn)]
  public class ModOptions {
    [Option("WorkTime", "", Format = "F0")]
    [JsonProperty]
    public float WorkTime { get; set; } = 3600;
  }
}