
using UnityEngine;
using UnityEngine.UI;

using RecordModule.Data;
using QuestionModule.Data;

using UI.Common.Controls.ItemDisplays;

using UI.Common.Controls.SystemExtend.QuestionText;

namespace UI.BattleScene.Controls.Question {

    using Question = QuestionModule.Data.Question;

    /// <summary>
    /// 题目选项显示
    /// </summary
    public class QuesChoiceDisplay :
        SelectableItemDisplay<Question.Choice> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string TextFormat = "{0}." + QuestionText.SpaceEncode + "{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Color normalFontColor = new Color(0, 0, 0);
        public Color correctFontColor = new Color(0.2705882f, 0.6078432f, 0.372549f);
        public Color wrongFontColor = new Color(0.9372549f, 0.2666667f, 0.1137255f);

        public GameObject correctFlag; // 正确答案标志
        public GameObject wrongFlag; // 错误答案标志

        public QuestionText text;

        #region 初始化

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="index">索引</param>
        public override void configure(ContainerDisplay<Question.Choice> container, int index) {
            base.configure(container, index);
            if (text) text.imageContainer = getContainer().pictureContaienr;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new QuesChoiceContainer getContainer() {
            return (QuesChoiceContainer)container;
        }

        /// <summary>
        /// 获取答题结果
        /// </summary>
        public QuestionSetRecord.IQuestionResult getResult() {
            return getContainer().result;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="choice">选项</param>
        protected override void drawExactlyItem(Question.Choice choice) {
            base.drawExactlyItem(choice);
            var result = getResult();
            var correct = result != null && choice.answer;
            var wrong = result != null && !correct && choice.isInSelection(result);

            if (correctFlag) correctFlag.SetActive(correct);
            if (wrongFlag) wrongFlag.SetActive(wrong);

            if (text) {
                var color = normalFontColor;
                if (correct) color = correctFontColor;
                if (wrong) color = wrongFontColor;
                text.text = generateChoiceText(choice);
            }
        }

        /// <summary>
        /// 生成选项文本
        /// </summary>
        /// <param name="choice">选项</param>
        /// <returns></returns>
        string generateChoiceText(Question.Choice choice) {
            var alph = ((char)('A' + index)).ToString();
            return string.Format(TextFormat, alph, choice.text);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            if (correctFlag) correctFlag.SetActive(false);
            if (wrongFlag) wrongFlag.SetActive(false);
            if (text) text.text = "";
        }

        #endregion

    }
}