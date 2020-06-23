
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 据点显示控件
	/// </summary
	public class StateDisplay : SelectableItemDisplay<RuntimeState> {
		
		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Image icon;
		public Text turns;

		/// <summary>
		/// 内部组件定义
		/// </summary>
		BattlerDisplay battler;

		#region 初始化
		
		/// <summary>
		/// 配置
		/// </summary>
		public void configure(BattlerDisplay battler) {
			this.battler = battler;
			configure();
		}

		#endregion

		#region 更新控制

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否为空物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isNullItem(RuntimeState item) {
			return base.isNullItem(item) || item.state() == null;
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

			turns.text = item.turns.ToString();
			drawState(state);
		}

		/// <summary>
		/// 绘制状态
		/// </summary>
		/// <param name="state"></param>
		void drawState(ExerProState state) {
			icon.gameObject.SetActive(true);
			icon.overrideSprite = state.icon;
		}
		
		/// <summary>
		/// 清除物品
		/// </summary>
		protected override void drawEmptyItem() {
			icon.gameObject.SetActive(false);
		}

        #endregion

    }
}