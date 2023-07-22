using System.Collections.Generic;

namespace PackAnything {
    public class PackAnythingChoreTypes : ResourceSet<ChoreType> {
        public static ChoreType Pack;
        public static ChoreType Unpack;
        private int nextImplicitPriority = 10000;
        private const int INVALID_PRIORITY = -1;

        private ChoreType Add(
            string id,
            string[] chore_groups,
            string urge,
            string[] interrupt_exclusion,
            string name,
            string status_message,
            string tooltip,
            bool skip_implicit_priority_change,
            int explicit_priority = -1,
            string report_name = null
            ) {
            ListPool<Tag, PackAnythingChoreTypes>.PooledList interrupt_exclusion1 = ListPool<Tag, PackAnythingChoreTypes>.Allocate();
            for (int index = 0; index < interrupt_exclusion.Length; ++index)
                interrupt_exclusion1.Add(TagManager.Create(interrupt_exclusion[index]));
            if (explicit_priority == -1)
                explicit_priority = this.nextImplicitPriority;
            ChoreType choreType = new ChoreType(id, (ResourceSet)this, chore_groups, urge, name, status_message, tooltip, (IEnumerable<Tag>)interrupt_exclusion1, this.nextImplicitPriority, explicit_priority);
            interrupt_exclusion1.Recycle();
            if (!skip_implicit_priority_change)
                this.nextImplicitPriority -= 50;
            if (report_name != null)
                choreType.reportName = report_name;
            return choreType;
        }

        public PackAnythingChoreTypes(ResourceSet parent) : base(nameof(PackAnythingChoreTypes), parent) {
            PackAnythingChoreTypes.Pack = this.Add(nameof(PackAnythingChoreTypes.Pack), new string[1] { "Storage" }, "", new string[0], STRINGS.DUPLICANTS.CHORES.PACK.NAME, STRINGS.DUPLICANTS.CHORES.PACK.STATUS, STRINGS.DUPLICANTS.CHORES.PACK.TOOLTIP, true, 5000, STRINGS.DUPLICANTS.CHORES.PACK.REPORT_NAME);
            PackAnythingChoreTypes.Unpack = this.Add(nameof(PackAnythingChoreTypes.Unpack), new string[1] { "Hauling" }, "", new string[0], STRINGS.DUPLICANTS.CHORES.UNPACK.NAME, STRINGS.DUPLICANTS.CHORES.UNPACK.STATUS, STRINGS.DUPLICANTS.CHORES.UNPACK.TOOLTIP, true, 5000, STRINGS.DUPLICANTS.CHORES.UNPACK.REPORT_NAME);
        }
    }
}
