

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	/// <summary>
	/// 手牌控件
	/// </summary
	public class DrawCardGroupDisplay : 
		SelectableContainerDisplay<ExerProPackCard> {

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public AnimationController controller;

		#region 界面控制

		/// <summary>
		/// 子视图创建回调
		/// </summary>
		/// <param name="sub">子视图</param>
		/// <param name="index">索引</param>
		protected override void onSubViewCreated(SelectableItemDisplay<ExerProPackCard> sub, int index) {
			base.onSubViewCreated(sub, index);
			var display = sub as CardDisplay;
			if (display == null) return;

			controller.add(display.animation);
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 关闭
		/// </summary>
		public void next() {
			// TODO: 添加跳过逻辑
		}

		#endregion

	}
}