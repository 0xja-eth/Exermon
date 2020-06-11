
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls {

	/// <summary>
	/// HP结果显示容器
	/// </summary
	public class StatesContainer : SelectableContainerDisplay<RuntimeState> {

		/// <summary>
		/// 位置偏移量设置
		/// </summary>
		const float MaxXOffset = 32;
		const float MaxYOffset = 32;

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public StateDetail detail;

		/// <summary>
		/// 获取物品详情
		/// </summary>
		/// <returns></returns>
		public override IItemDetailDisplay<RuntimeState> getItemDetail() {
			return detail;
		}

	}
}