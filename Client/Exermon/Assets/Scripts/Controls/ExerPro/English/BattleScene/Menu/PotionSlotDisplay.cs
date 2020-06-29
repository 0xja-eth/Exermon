
using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	using Windows;

	/// <summary>
	/// 特训药品槽显示
	/// </summary>
	public class PotionSlotDisplay : 
        SelectableContainerDisplay<ExerProSlotPotion> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PotionSlotItemDetail detail; // 帮助界面

		public MenuWindow menu;

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        public override IItemDetailDisplay<ExerProSlotPotion> getItemDetail() {
            return detail;
        }
		
		/// <summary>
		/// 使用药水
		/// </summary>
		/// <param name="slotItem"></param>
		public void use(PotionSlotItemDisplay slotItem) {
			if (menu.usePotion(slotItem))
				removeItem(slotItem.getItem());
		}
		/*
		/// <summary>
		/// 点击回调
		/// </summary>
		/// <param name="index"></param>
		public override void click(int index) {
			base.click(index);
			var display = subViews[index] as PotionSlotItemDisplay;
			if (display.isEnabled()) use(display);
		}
		*/
		#endregion
	}
}