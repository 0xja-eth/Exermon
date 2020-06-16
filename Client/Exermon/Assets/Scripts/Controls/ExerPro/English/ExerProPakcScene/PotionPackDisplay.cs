using ExerPro.EnglishModule.Data;
using UI.Common.Controls.ItemDisplays;
using UI.ExerPro.EnglishPro.ExerProPackScene.CardPackItemDetail;
using UI.ExerPro.EnglishPro.ExerProPackScene.Controls.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.ExerProPackScene.Pack {
    /// <summary>
    /// 特训药水背包
    /// </summary>
    public class PotionPackDisplay : SelectableContainerDisplay<ExerProPackPotion> {
        public Text description;

        ExerProPackPotion[] packitems;
        public ExerProPackItemDetail itemdetail;

        public override void startView() {
            base.startView();
        }

        public override void setItems(ExerProPackPotion[] items) {
            packitems = items; base.setItems(items);
        }
        protected override void onSubViewCreated(SelectableItemDisplay<ExerProPackPotion> sub, int index) {
            base.onSubViewCreated(sub, index);
            ((PackPotionDisplay)sub).setItem(items[index]);
            ((PackPotionDisplay)sub).description = description;
        }
        protected override void onSelectChanged() {
            base.onSelectChanged();
            int index = getSelectedIndex();
            if (index == -1) {
                return;
            }
            else {
                itemdetail.setValue(items[index]);
            }
        }
    }
}
