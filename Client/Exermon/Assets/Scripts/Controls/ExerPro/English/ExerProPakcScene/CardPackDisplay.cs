using ExerPro.EnglishModule.Data;
using UI.Common.Controls.ItemDisplays;
using UI.ExerPro.EnglishPro.ExerProPackScene.CardPackItemDetail;
using UI.ExerPro.EnglishPro.ExerProPackScene.Controls.Menu;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.ExerProPackScene.Pack {
    public class CardPackDisplay : SelectableContainerDisplay<ExerProPackCard> {
        public Text description;

        ExerProPackCard[] cards;
        public ExerProPackItemDetail itemdetail;

        public override void startView() {
            base.startView();
        }

        public override void setItems(ExerProPackCard[] items) {
            cards = items; base.setItems(items);
        }
        protected override void onSubViewCreated(SelectableItemDisplay<ExerProPackCard> sub, int index) {
            base.onSubViewCreated(sub, index);
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
