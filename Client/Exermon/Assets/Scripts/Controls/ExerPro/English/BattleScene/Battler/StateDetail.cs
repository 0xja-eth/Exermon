
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 状态详情控件
	/// </summary
	public class StateDetail : ItemDetailDisplay<RuntimeState> {

		/// <summary>
		/// 文本常量定义
		/// </summary>
		const string TurnFormat = "剩余回合：{0}";

		const float XOffset = 24;

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text name, description, turns;
	
		#region 数据控制

		/// <summary>
		/// 是否为空物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isNullItem(RuntimeState item) {
			return base.isNullItem(item) && item.state() != null;
		}

		#endregion
		
		#region 界面控制

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="item">结果</param>
		protected override void drawExactlyItem(RuntimeState item) {
            base.drawExactlyItem(item);
			var state = item.state();

			turns.text = string.Format(TurnFormat, item.turns);
			drawState(state);
		}

		/// <summary>
		/// 绘制状态
		/// </summary>
		/// <param name="state"></param>
		void drawState(ExerProState state) {
			name.text = state.name;
			description.text = state.description;
		}
		
		/// <summary>
		/// 清除物品
		/// </summary>
		protected override void drawEmptyItem() {
			name.text = description.text = turns.text = "";
		}
		
		#endregion

	}
}