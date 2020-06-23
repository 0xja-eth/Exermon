﻿using UnityEngine;

using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;
using QuestionModule.Data;

namespace UI.ExerPro.EnglishPro.ListenScene.Controls {

    /// <summary>
    /// 题目选项容器
    /// Class B
    /// </summary>
    public class ListenChoiceContainer :
        SelectableContainerDisplay<BaseQuestion.Choice> {

		/// <summary>
		///外部组件设置
		/// </summary>
		public ListeningSubQuestionDisplay questionDisplay;

		/// <summary>
		/// 显示答案解析
		/// </summary>
		public bool showAnswer() {
			return questionDisplay.showAnswer;
		}
		
	}
}