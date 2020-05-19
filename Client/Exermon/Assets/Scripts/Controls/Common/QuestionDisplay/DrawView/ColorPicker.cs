
using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.QuestionDisplay.DrawView {

    /// <summary>
    /// 选项按钮容器
    /// </summary>
    public class ColorPicker : SelectableItemDisplay<ColorRef> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image colorDisplay;

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(ColorRef item) {
            base.drawExactlyItem(item);
            colorDisplay.color = item.color;
        }

        /// <summary>
        /// 绘制空值
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            colorDisplay.color = new Color(0, 0, 0, 0);
        }

        #endregion
    }
}