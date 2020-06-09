
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UI.Common.Controls.ItemDisplays;

using UI.Common.Controls.SystemExtend.QuestionText;
using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.PlotScene.Controls
{
    /// <summary>
    /// 题目选项显示
    /// </summary
    public class PlotChoiceDisplay :
        SelectableItemDisplay<PlotQuestion.Choice>
    {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string TextFormat = "{0}." + QuestionText.SpaceEncode + "{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Color normalFontColor = new Color(0, 0, 0);

        public CanvasGroup canvasGroup;
        public float noAnswerAlpha = 0.4f; // 非正确答案时的透明度

        public QuestionText text;

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="choice">选项</param>
        protected override void drawExactlyItem(PlotQuestion.Choice choice)
        {
            base.drawExactlyItem(choice);
            //if (canvasGroup)
            //    canvasGroup.alpha = isHighlighting() ? 1 : noAnswerAlpha;

            if (text)
            {
                text.text = generateChoiceText(choice);
            }
        }

        /// <summary>
        /// 生成选项文本
        /// </summary>
        /// <param name="choice">选项</param>
        /// <returns></returns>
        string generateChoiceText(PlotQuestion.Choice choice)
        {
            return choice.text;
            //var alph = ((char)('A' + index)).ToString();
            //return string.Format(TextFormat, alph, choice.text);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem()
        {
            //if (canvasGroup) canvasGroup.alpha = 1;
            if (text) text.text = "";
        }

        #endregion

        #region 事件处理
        /// <summary>
        /// 处理点击事件回调
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            Debug.Log("Plot Choice Clicked!");

        }
        #endregion
    }
}