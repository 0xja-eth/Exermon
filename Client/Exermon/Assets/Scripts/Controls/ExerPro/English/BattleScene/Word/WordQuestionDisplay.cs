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
	/// 单词题目
	/// </summary>
	public class WordQuestionDisplay : ItemDetailDisplay<Word> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text english;

		public WordChoiceContainer choiceContainer;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		List<string> choices = new List<string>();

		/// <summary>
		/// 外部系统设置
		/// </summary>
		EnglishService engSer;

		#region 初始化

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			engSer = EnglishService.get();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 物品改变回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			generateChoices();
		}

		/// <summary>
		/// 生成选项（后期需要移到后台执行）
		/// </summary>
		void generateChoices() {
			choices = engSer.generateWordChoices(item);
			choiceContainer.setItems(choices);
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(Word item) {
			base.drawExactlyItem(item);
			english.text = item.english;
		}

		#endregion

	}
}
