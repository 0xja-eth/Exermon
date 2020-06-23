using UnityEngine;

using RecordModule.Data;

using UI.Common.Controls.ItemDisplays;
using ExerPro.EnglishModule.Data;
using QuestionModule.Data;

namespace UI.ExerPro.EnglishPro.ListenScene.Controls {
    /// <summary>
    /// 题目选项容器
    /// Class B
    /// </summary>
    public class ListenChoiceContainer :
        SelectableContainerDisplay<ListeningSubQuestion.Choice> {

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

        ListeningSubQuestion question;
        public void setItem(ListeningSubQuestion ques) {
            question = ques;
            setItems(ques.choices);
            maxCheck = 1;
        }
        public ListeningSubQuestion getItem() { return question; }
		public void startView(ListeningSubQuestion item)
		{
			base.startView();
			setItem(item);
		}
	}
}