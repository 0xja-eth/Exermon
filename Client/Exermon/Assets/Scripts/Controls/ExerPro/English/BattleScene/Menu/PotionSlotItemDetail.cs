
using UnityEngine.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Windows;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	/// <summary>
	/// 特训药品槽显示
	/// </summary>
	public class PotionSlotItemDetail : 
        ItemDetailDisplay<ExerProSlotPotion> {

		/// <summary>
		/// 常量设置
		/// </summary>

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text name, description; 

		public ToggleWindow window; // 帮助界面

		#region 启动/关闭控制

		/// <summary>
		/// 启动视窗
		/// </summary>
		public override void startView() {
			base.startView();
			window.startWindow();
		}

		/// <summary>
		/// 关闭视窗
		/// </summary>
		public override void terminateView() {
			base.terminateView();
			window.terminateWindow();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否为空物品
		/// </summary>
		/// <param name="item">物品</param>
		/// <returns></returns>
		public override bool isNullItem(ExerProSlotPotion item) {
			return base.isNullItem(item) || item.isNullItem();
		}

		#endregion

		#region 画面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(ExerProSlotPotion item) {
			base.drawExactlyItem(item);
			var potion = item.packPotion.item();

			name.text = potion.name;
			description.text = potion.description;
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			name.text = description.text = "";
		}

		#endregion
	}
}