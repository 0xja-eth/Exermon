using UI.Common.Controls.ItemDisplays;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.PhraseScene.Controls {

	/// <summary>
	/// 选项显示控件
	/// </summary>
    public class OptionDisplay : DraggableItemDisplay<string> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
        public Text option;
        public Text drag;

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(string item) {
            base.drawExactlyItem(item);
            option.text = item;
			if (drag) drag.text = item;
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			option.text = "";
			if (drag) drag.text = "";
		}

		#endregion
	}
}
