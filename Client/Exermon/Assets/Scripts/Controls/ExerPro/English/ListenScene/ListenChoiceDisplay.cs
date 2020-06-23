
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UI.Common.Controls.ItemDisplays;

using UI.Common.Controls.SystemExtend.QuestionText;

using QuestionModule.Data;
using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.ListenScene.Controls {
    /// <summary>
    /// 题目选项显示
    /// 利大佬说的Class A
    /// </summary>
    public class ListenChoiceDisplay :
        SelectableItemDisplay<BaseQuestion.Choice> {
        /// <summary>
        /// 常量定义
        /// </summary>
        const string TextFormat = "{0}." + QuestionText.SpaceEncode + "{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Color normalFontColor = new Color(255, 255, 255);
        public Color correctFontColor = new Color(0.2705882f, 0.6078432f, 0.372549f);
        public Color wrongFontColor = new Color(0.9372549f, 0.2666667f, 0.1137255f);

        public CanvasGroup canvasGroup;
        public float noAnswerAlpha = 0.4f; // 非正确答案时的透明度

        public GameObject correctFlag; // 正确答案标志
        public GameObject wrongFlag; // 错误答案标志

        public Text text;


        #region 数据控制

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new ListenChoiceContainer getContainer() {
            return (ListenChoiceContainer)container;
        }

        /// <summary>
        /// 是否显示答案
        /// </summary>
        /// <returns></returns>
        public bool showAnswer() {
            return getContainer().showAnswer();
        }

        #endregion

        #region 数据控制

		/// <summary>
		/// 是否有效
		/// </summary>
		/// <returns></returns>
        public override bool isActived() {
            return base.isActived() && !showAnswer();
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="choice">选项</param>
        protected override void drawExactlyItem(BaseQuestion.Choice choice) {
            base.drawExactlyItem(choice);
            bool correct = false, wrong = false;

            if (showAnswer()) {
                if (canvasGroup)
                    canvasGroup.alpha = choice.answer ? 1 : noAnswerAlpha;

                var isInSelected = getContainer().getItemDisplay(getContainer().getSelectedIndex()) == this;
                correct = choice.answer;
                wrong = !choice.answer && isInSelected;

                if (correctFlag) correctFlag.SetActive(correct);
                if (wrongFlag) wrongFlag.SetActive(wrong);

            }
            else {
                if (canvasGroup) canvasGroup.alpha = 1;
                if (correctFlag) correctFlag.SetActive(false);
                if (wrongFlag) wrongFlag.SetActive(false);
            }

            if (text) {
                var color = normalFontColor;
                if (correct) color = correctFontColor;
                if (wrong) color = wrongFontColor;

                text.color = color;
                text.text = generateChoiceText(choice);
            }
        }

        /// <summary>
        /// 生成选项文本
        /// </summary>
        /// <param name="choice">选项</param>
        /// <returns></returns>
        string generateChoiceText(ListeningSubQuestion.Choice choice) {
            var alph = ((char)('A' + index)).ToString();
            return string.Format(TextFormat, alph, choice.text);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            if (canvasGroup) canvasGroup.alpha = 1;
            if (correctFlag) correctFlag.SetActive(false);
            if (wrongFlag) wrongFlag.SetActive(false);
            if (text) text.text = "";
        }

        #endregion
    }
}