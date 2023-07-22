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
            MixStatusItem.PackingItem = this.CreateDuplicantStatus("PackingItem",Duplicants,"",StatusItem.IconType.Info,NotificationType.Neutral,false,OverlayModes.None.ID);
            MixStatusItem.UnpackingItem = this.CreateDuplicantStatus("UnpackingItem", Duplicants, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            MixStatusItem.WaitingPack = this.CreateDuplicantStatus("WaitingPack", Misc, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            MixStatusItem.WaitingUnpack = this.CreateDuplicantStatus("WaitingUnpack", Misc, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
        }
    }

}
