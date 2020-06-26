
using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

namespace UI.ExerPro.EnglishPro.Common.Controls {

	/// <summary>
	/// 卡牌显示
	/// </summary>
	public class CardRewardDisplay : SelectableContainerDisplay<ExerProCard> {

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public AnimationController controller;
		
		#region 界面控制

		/// <summary>
		/// 创建所有子视图
		/// </summary>
		protected override void createSubViews() {
			base.createSubViews();
			addToController();
		}

		/// <summary>
		/// 添加所有子视图到动画控制器
		/// </summary>
		void addToController() {
			foreach (var sub in subViews) {
				var display = sub as CardDisplay;
				if (display == null) continue;
				controller.add(display.animation);
				display.isOpen = false;
			}
		}
		
		#endregion

	}
}