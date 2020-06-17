using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using WordData = ExerPro.EnglishModule.Data.Word;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Word {

	/// <summary>
	/// 单词选项显示控件
	/// </summary>
	public class WordChoiceDisplay : SelectableItemDisplay<string> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text chinese;

		public GameObject correctFlag, wrongFlag;

		#region 数据控制

		/// <summary>
		/// 获取容器
		/// </summary>
		/// <returns></returns>
		public new WordChoiceContainer getContainer() {
			return container as WordChoiceContainer;
		}

		/// <summary>
		/// 获取单词
		/// </summary>
		/// <returns></returns>
		WordData getWord() {
			return getContainer()?.getItem();
		}

		/// <summary>
		/// 是否为正确答案
		/// </summary>
		/// <returns></returns>
		bool isCorrect() {
			var word = getWord();
			if (word == null) return false;
			return word.chinese == item;
		}

		/// <summary>
		/// 是否显示答案
		/// </summary>
		/// <returns></returns>
		bool showAnswer() {
			var container = getContainer();
			if (container == null) return false;
			return container.showAnswer;
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(string item) {
			base.drawExactlyItem(item);
			chinese.text = item;
			drawAnswer();
		}

		/// <summary>
		/// 绘制答案
		/// </summary>
		void drawAnswer() {
			if (!showAnswer()) {
				correctFlag.SetActive(false);
				wrongFlag.SetActive(false);
			} else {
				var correct = isCorrect();
				correctFlag.SetActive(correct);
				wrongFlag.SetActive(!correct);
			}
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();

			chinese.text = "";
			correctFlag.SetActive(false);
			wrongFlag.SetActive(false);
		}

		#endregion

	}
}
