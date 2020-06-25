using ExerPro.EnglishModule.Data;
using UI.Common.Controls.ItemDisplays;
using UI.ExerPro.EnglishPro.ExerProPackScene.CardPackItemDetail;
using UI.ExerPro.EnglishPro.ExerProPackScene.Controls.Menu;
using UI.ExerPro.EnglishPro.ExerProPackScene.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.ExerProPackScene.Pack {
    /// <summary>
    /// 特训药水背包
    /// </summary>
    public class PotionPackDisplay : SelectableContainerDisplay<ExerProPackPotion> {
        public Text description;

        ExerProPackPotion[] potions;
        public ExerProPackItemDetail itemdetail;
        public ExerProPackWindow packWindow;

        public override void startView() {
            base.startView();
        }

        public override void setItems(ExerProPackPotion[] items) {
            potions = items; base.setItems(items);
        }
        protected override void onSubViewCreated(SelectableItemDisplay<ExerProPackPotion> sub, int index) {
            base.onSubViewCreated(sub, index);
        }
        protected override void onSelectChanged() {
            base.onSelectChanged();
            int index = getSelectedIndex();
            if (index == -1) {
                itemdetail.equip.SetActive(false);
                return;
            }
            else {
                itemdetail.setValue(items[index]);
            }
        }
    }
}
