using UI.Common.Controls.ItemDisplays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExerPro.EnglishModule.Data;
using UI.Common.Controls.SystemExtend.QuestionText;
using UI.ExerPro.EnglishPro.ListenScene.Controls;
using UnityEngine;

namespace Assets.Scripts.Controls.ExerPro.English.ListenScene
{
	/// <summary>
	/// 听力小题容器
	/// 利大佬说的Class D
	/// </summary>
	class ListenSubQuestionContainer :
				ContainerDisplay<ListeningSubQuestion>
	{
		public QuestionText title, description;
		public ListenChoiceContainer choiceContainer; // 选项容器
		ListeningSubQuestion question;
		/// <summary>
		/// 获取与设置物品
		/// </summary>
		/// <param name="ques"></param>
		public void setItem(ListeningSubQuestion ques) {
			question = ques;
		}

		public ListeningSubQuestion getItem() { return question; }

		public void startView(ListeningSubQuestion item)
		{
			base.startView();
			setItem(item);
		}


	}
}
