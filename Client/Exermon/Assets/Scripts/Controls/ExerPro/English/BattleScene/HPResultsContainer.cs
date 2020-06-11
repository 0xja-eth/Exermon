
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
	public class HPResultsContainer : ContainerDisplay<RuntimeBattler.DeltaHP> {

		/// <summary>
		/// 位置偏移量设置
		/// </summary>
		const float MaxXOffset = 32;
		const float MaxYOffset = 32;

		/// <summary>
		/// 外部组件设置
		/// </summary>

		#region 界面绘制

		/// <summary>
		/// 子视窗创建回调
		/// </summary>
		protected override void onSubViewCreated(ItemDisplay<RuntimeBattler.DeltaHP> sub, int index) {
			//var hpRes = sub as HPResultDisplay;
			//if (hpRes != null) hpRes.configure(battler);

			base.onSubViewCreated(sub, index);

			var rt = sub.transform as RectTransform;
			if (rt == null) return;
			var x = Random.Range(-MaxXOffset, MaxXOffset);
			var y = Random.Range(-MaxYOffset, MaxYOffset);
			rt.anchoredPosition = new Vector2(x, y);
		}

		#endregion

	}
}