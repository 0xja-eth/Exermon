using UI.Common.Controls.ItemDisplays;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.PhraseScene.Controls {
    class OptionDisplay :
        DraggableItemDisplay<string> {

        public Text option;
        public Text drag;

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(string item) {
            base.drawExactlyItem(item);
            option.text = item; drag.text = item;
        }
        #endregion
    }
}
