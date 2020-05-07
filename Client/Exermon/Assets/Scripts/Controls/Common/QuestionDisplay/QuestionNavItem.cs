
using UnityEngine.UI;

using QuestionModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.QuestionDisplay {

    /// <summary>
    /// 题目导航栏项
    /// </summary
    public class QuestionNavItem :
        SelectableItemDisplay<Question> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text number, selectedNumber;

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">题目</param>
        protected override void drawExactlyItem(Question item) {
            base.drawExactlyItem(item);
            number.text = (index+1).ToString();
            selectedNumber.text = (index+1).ToString();
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            number.text = "";
            selectedNumber.text = "";
        }

        #endregion

    }
}