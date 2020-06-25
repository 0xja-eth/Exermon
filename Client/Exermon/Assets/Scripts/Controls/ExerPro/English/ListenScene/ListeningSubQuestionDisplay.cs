
using UnityEngine.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.SystemExtend.QuestionText;

namespace UI.ExerPro.EnglishPro.ListenScene.Controls {

	/// <summary>
	/// 题目选项显示
	/// 利大佬说的Class C
	/// </summary>
	public class ListeningSubQuestionDisplay :
		ItemDisplay<ListeningSubQuestion> {

		/// <summary>
		/// 子题目容器
		/// </summary>
		public Text title, description;
		public ListenChoiceContainer choiceContainer; // 选项容器
		
		/// <summary>
		/// 是否已选择（答案）
		/// </summary>
		//public bool isSelected { get; protected set; }

		///// <summary>
		///// 容器
		///// </summary>
		//public ListenSubQuestionContainer container { get; set; }

		/// <summary>
		/// 显示答案解析
		/// </summary>
		bool _showAnswer = false;
		public bool showAnswer {
			get { return _showAnswer; }
			set {
				_showAnswer = value;
				requestRefresh();
			}
		}

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			//choiceContainer?.addClickedCallback(onChoiceClicked);
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否显示答案解析
		/// </summary>
		/// <returns></returns>
		public bool isSelected() {
			return choiceContainer.isSelected();
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="question">题目</param>
		protected override void drawExactlyItem(ListeningSubQuestion question) {
			base.drawExactlyItem(question);

			if (title) title.gameObject.SetActive(true);
			if (choiceContainer) choiceContainer.gameObject.SetActive(true);
			if (description) description.gameObject.SetActive(false);

			drawTitle(question);
			drawChoices(question);
		}

		/// <summary>
		/// 绘制题干
		/// </summary>
		/// <param name="question">题目</param>
		void drawTitle(ListeningSubQuestion question) {
			if (title) title.text = question.title;
		}

		/// <summary>
		/// 绘制图片和选项
		/// </summary>
		/// <param name="question">题目</param>
		void drawChoices(ListeningSubQuestion question) {
			choiceContainer?.setItems(question.choices);
		}

		/// <summary>
		/// 清除物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			title.text = description.text = "";
			if (choiceContainer) choiceContainer.gameObject.SetActive(false);
			if (description) description.text = "";
			if (description.gameObject) description.gameObject.SetActive(false);
		}

		#endregion
	}
}
