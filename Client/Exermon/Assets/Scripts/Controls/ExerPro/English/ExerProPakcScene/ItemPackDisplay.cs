using ExerPro.EnglishModule.Data;
using UI.Common.Controls.ItemDisplays;
using UI.ExerPro.EnglishPro.ExerProPackScene.CardPackItemDetail;
using UI.ExerPro.EnglishPro.ExerProPackScene.Controls.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.ExerProPackScene.Pack {
    /// <summary>
    /// 特训物品背包
    /// </summary>
    public class ItemPackDisplay : SelectableContainerDisplay<ExerProPackItem> {
        public Text description;

        ExerProPackItem[] packitems;
        public ExerProPackItemDetail itemdetail;

        public override void startView() {
            base.startView();
        }

        public override void setItems(ExerProPackItem[] items) {
            packitems = items; base.setItems(items);
        }
        protected override void onSubViewCreated(SelectableItemDisplay<ExerProPackItem> sub, int index) {
            base.onSubViewCreated(sub, index);
            //((PackItemDisplay)sub).setItem(items[index]);
            //((PackItemDisplay)sub).description = description;
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
