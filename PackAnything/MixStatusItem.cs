using Database;


namespace PackAnything {
    public class MixStatusItem : StatusItems {
        public static StatusItem PackingItem;
        public static StatusItem UnpackingItem;
        public static StatusItem WaitingPack;
        public static StatusItem WaitingUnpack;
        public string Duplicants = "DUPLICANTS";
        public string Misc = "MISC";


        public MixStatusItem(ResourceSet parent) : base(nameof(MixStatusItem), parent) {
            this.CreateStatusItems();
        }

        private StatusItem CreateDuplicantStatus(
            string id,
            string prefix,
            string icon,
            StatusItem.IconType icon_type,
            NotificationType notification_type,
            bool allow_multiples,
            HashedString render_overlay,
            bool showWorldIcon = true,
            int status_overlays = 2) {
            return this.Add(new StatusItem(id, prefix, icon, icon_type, notification_type, allow_multiples, render_overlay, showWorldIcon, status_overlays));
        }
        private StatusItem CreateMiscStatus(
            string id,
            string prefix,
            string icon,
            StatusItem.IconType icon_type,
            NotificationType notification_type,
            bool allow_multiples,
            HashedString render_overlay,
            bool showWorldIcon = true,
            int status_overlays = 129022) {
            return this.Add(new StatusItem(id, prefix, icon, icon_type, notification_type, allow_multiples, render_overlay, showWorldIcon, status_overlays));
        }

        private void CreateStatusItems() {
            MixStatusItem.PackingItem = this.CreateDuplicantStatus("SurveyItem",Duplicants,"",StatusItem.IconType.Info,NotificationType.Neutral,false,OverlayModes.None.ID);
            MixStatusItem.UnpackingItem = this.CreateDuplicantStatus("ActiveBeacon", Duplicants, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            MixStatusItem.WaitingPack = this.CreateDuplicantStatus("WaitingSurvey", Misc, "status_item_needs_furniture", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
            MixStatusItem.WaitingUnpack = this.CreateDuplicantStatus("WaitingActive", Misc, "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
        }
    }

}
