using System.Collections.Generic;

namespace PackAnything {
    public class PackAnythingChoreTypes : ResourceSet<ChoreType> {
        public static ChoreType Survey;
        public static ChoreType Active;
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
            PackAnythingChoreTypes.Survey = this.Add(nameof(PackAnythingChoreTypes.Survey), new string[1] { "Storage" }, "", new string[0], STRINGS.DUPLICANTS.CHORES.SURVEY.NAME, STRINGS.DUPLICANTS.CHORES.SURVEY.STATUS, STRINGS.DUPLICANTS.CHORES.SURVEY.TOOLTIP, true, 5000, STRINGS.DUPLICANTS.CHORES.SURVEY.REPORT_NAME);
            PackAnythingChoreTypes.Active = this.Add(nameof(PackAnythingChoreTypes.Active), new string[1] { "Hauling" }, "", new string[0], STRINGS.DUPLICANTS.CHORES.ACTIVE.NAME, STRINGS.DUPLICANTS.CHORES.ACTIVE.STATUS, STRINGS.DUPLICANTS.CHORES.ACTIVE.TOOLTIP, true, 5000, STRINGS.DUPLICANTS.CHORES.ACTIVE.REPORT_NAME);
        }
    }
}
