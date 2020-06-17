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
        ListeningSubQuestion question;
        public void setItem(ListeningSubQuestion ques) { question = ques; }
        public ListeningSubQuestion getItem() { return question; }
		public void startView(ListeningSubQuestion item)
		{
			base.startView();
			setItem(item);
		}
	}
}