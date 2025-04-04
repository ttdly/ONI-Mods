using System.Collections.Generic;
using GeyserExpandMachine.Buildings;

namespace GeyserExpandMachine {
    public class ModData {
        public static ModData Instance;
        
        public Dictionary<int, GeyserExpand> GeyserExpands;

        public ModData() {
            this.GeyserExpands = new Dictionary<int, GeyserExpand>();
        }
    }
}