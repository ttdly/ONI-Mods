using HarmonyLib;
using KMod;

namespace BlueprintsButtonTooltip {
    public class Mod: UserMod2 {
        
    }

    [HarmonyPatch(typeof(TopLeftControlScreen), "RefreshKleiItemDropButton")]
    public class TopLeftControlScreenRefreshKleiItemDropButtonPatch {
        private static bool Prefix(MultiToggle ___kleiItemDropButton) {
            if (!KleiItemDropScreen.HasItemsToShow()) {
                ___kleiItemDropButton.GetComponent<ToolTip>()
                    .SetSimpleTooltip(STRINGS.UI.ITEM_DROP_SCREEN.IN_GAME_BUTTON.TOOLTIP_ERROR_NO_ITEMS);
                ___kleiItemDropButton.ChangeState(1);
            }
            else {
                var showCount = "";
                int num = 0;
                foreach (KleiItems.ItemData itemData in PermitItems.IterateInventory()) {
                    if (!itemData.IsOpened)
                        ++num;
                }

                if (num > 1) {
                    showCount = STRINGS.UI.ITEM_DROP_SCREEN.UNOPENED_ITEM_COUNT
                        .Replace("{0}", num.ToString());

                }
                else if (num == 1) {
                    showCount = STRINGS.UI.ITEM_DROP_SCREEN.UNOPENED_ITEM
                        .Replace("{0}", num.ToString());
                }

                ___kleiItemDropButton.GetComponent<ToolTip>().SetSimpleTooltip(
                    $"{STRINGS.UI.ITEM_DROP_SCREEN.IN_GAME_BUTTON.TOOLTIP_ITEMS_AVAILABLE} \n\n {showCount}");
                ___kleiItemDropButton.ChangeState(2);
            }

            return false;
        }
    }
}