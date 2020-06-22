using ExerPro.EnglishModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.SystemExtend.QuestionText;
using UI.ExerPro.EnglishPro.ListenScene.Controls;
using UnityEngine;

namespace Assets.Scripts.Controls.ExerPro.English.ListenScene
{
	/// <summary>
	/// 题目选项显示
	/// 利大佬说的Class C
	/// </summary>
	public class ListeningSubQuestionDisplay :
		ItemDisplay<ListeningSubQuestion>
	{
		/// <summary>
		/// 子题目容器
		/// </summary>
		public QuestionText title, description;
		public ListenChoiceContainer choiceContainer; // 选项容器
		public RectTransform content;
		ListenSubQuestionContainer container;
		#region 初始化
		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce()
		{
			base.initializeOnce();
			choiceContainer?.addClickedCallback(onChoiceSelected);

		}
		/// <summary>
		/// 显示答案解析
		/// </summary>
		bool _showAnswer = false;
		public bool showAnswer
		{
			get { return _showAnswer; }
			set
			{
				_showAnswer = value;
				requestRefresh();
			}
		}
		#endregion


		// 		protected override void drawExactlyItem(ListeningSubQuestion ques)
		// 		{
		// 			base.drawExactlyItem(ques);
		// 			container?.startView(ques);
		// 		}


		#region 回调函数
		/// <summary>
		/// 物品变更回调
		/// </summary>
		protected override void onItemChanged()
		{
			base.onItemChanged();
			//content.anchoredPosition = new Vector2(0, 0);
			// result = null; showAnswer = false;
		}

		void onChoiceSelected(int index)
		{
			showAnswer = true;

			var question = choiceContainer.getItem();
			//var resultChoice = question.choices[index];
			var resultText = "";
			//var resultEffect = resultChoice.effects;
			description.text = resultText;

			requestRefresh(true);
		}
		#endregion
		#region 界面控制

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="question">题目</param>
		protected override void drawExactlyItem(ListeningSubQuestion question)
		{
			base.drawExactlyItem(question);
			if (showAnswer)
			{
				if (title) title.gameObject.SetActive(false);
				if (choiceContainer) choiceContainer.gameObject.SetActive(false);
				if (description) description.gameObject.SetActive(true);
			}
			else
			{
				if (title) title.gameObject.SetActive(true);
				if (choiceContainer) choiceContainer.gameObject.SetActive(true);
				if (description) description.gameObject.SetActive(false);
				drawTitle(question);
				drawChoices(question);
			}

		}

		/// <summary>
		/// 绘制题干
		/// </summary>
		/// <param name="question">题目</param>
		void drawTitle(ListeningSubQuestion question)
		{
			if (question == null)
				Debug.Log("danteding no question");
			if (title)
				title.text = question.title;
		}

		/// <summary>
		/// 绘制图片和选项
		/// </summary>
		/// <param name="question">题目</param>
		void drawChoices(ListeningSubQuestion question)
		{
			choiceContainer?.startView(question);
		}

		/// <summary>
		/// 清除物品
		/// </summary>
		protected override void drawEmptyItem()
		{
			base.drawEmptyItem();
			title.text = description.text = "";
			if (choiceContainer) choiceContainer.gameObject.SetActive(false);
			if (description) description.text = "";
			if (description.gameObject) description.gameObject.SetActive(false);
		}

		#endregion
	}
}
