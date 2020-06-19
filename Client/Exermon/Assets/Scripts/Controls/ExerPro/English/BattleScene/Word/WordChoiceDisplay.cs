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

		public Color normalFontColor = new Color(0, 0, 0);
		public Color correctFontColor = new Color(0.2705882f, 0.6078432f, 0.372549f);
		public Color wrongFontColor = new Color(0.9372549f, 0.2666667f, 0.1137255f);

		public CanvasGroup canvasGroup;
		public float noAnswerAlpha = 0.4f; // 非正确答案时的透明度

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
			drawText(item);
			drawAnswer();
		}

		/// <summary>
		/// 绘制文本
		/// </summary>
		/// <param name="text"></param>
		void drawText(string text) {
			var color = normalFontColor;
			if (showAnswer()) {
				var correct = isCorrect();
				if (correct) color = correctFontColor;
				if (!isSelected() && !correct) color = wrongFontColor;
			}

			chinese.color = color;
			chinese.text = item;
		}

		/// <summary>
		/// 绘制答案
		/// </summary>
		void drawAnswer() {
			if (!showAnswer()) clearAnswer();
			else {
				var correct = isCorrect();

				if (canvasGroup)
					canvasGroup.alpha = correct ? 1 : noAnswerAlpha;

				if (correctFlag) correctFlag.SetActive(correct);
				if (isSelected() && wrongFlag) wrongFlag.SetActive(!correct);
			} 
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();

			chinese.text = "";
			clearAnswer();
		}

		/// <summary>
		/// 清除答案显示
		/// </summary>
		void clearAnswer() {
			canvasGroup.alpha = 1;
			correctFlag.SetActive(false);
			wrongFlag.SetActive(false);
		}

		#endregion

	}
}
