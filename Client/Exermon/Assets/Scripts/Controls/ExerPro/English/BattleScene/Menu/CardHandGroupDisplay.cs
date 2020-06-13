
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.Data.Loaders;
using Core.UI.Utils;

using GameModule.Services;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 战场控件
	/// </summary
	public class CardHandGroupDisplay : 
		SelectableContainerDisplay<ExerProPackCard> {

		/// <summary>
		/// 常量定义
		/// </summary>

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public int MaxDeltaAngle = 12; // 最大相差角度
		public Vector2 cardPivot = new Vector2(0.5f, -2);
		
		#region 界面控制

		/// <summary>
		/// 子视图创建回调
		/// </summary>
		/// <param name="sub">子视图</param>
		/// <param name="index">索引</param>
		protected override void onSubViewCreated(
			SelectableItemDisplay<ExerProPackCard> sub, int index) {
			base.onSubViewCreated(sub, index);
			var rt = sub.transform as RectTransform;
			if (rt == null) return;

			rt.pivot = cardPivot;
		}
		
		#endregion
	}
}