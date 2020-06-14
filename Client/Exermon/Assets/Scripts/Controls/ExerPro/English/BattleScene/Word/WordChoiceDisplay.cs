using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using ExerPro.EnglishModule.Services;
using ExerPro.EnglishModule.Data;

using Core.UI;
using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	/// <summary>
	/// 单词选项显示控件
	/// </summary>
	public class WordChoiceDisplay : SelectableItemDisplay<string> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text chinese;
		
		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(string item) {
			base.drawExactlyItem(item);
			chinese.text = item;
		}

		#endregion

	}
}
