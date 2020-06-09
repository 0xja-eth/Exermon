using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;
using ExerPro.EnglishModule.Data;
using UI.Common.Controls.SystemExtend.QuestionText;

namespace UI.ExerPro.EnglishPro.PlotScene.Controls
{
    public class PlotQuestionDisplay : ItemDetailDisplay<PlotQuestion>
    {
        /// <summary>
        /// 外部变量
        /// </summary>
        public Text tipName;
        public RawImage image;
        public QuestionText title, description;
        public PlotChoiceContainer choiceContainer; // 选项容器
        public RectTransform content;

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

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce()
        {
            base.initializeOnce();
            choiceContainer?.addClickedCallback(onChoiceSelected);
        }

        #endregion

        #region 回调函数
        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected override void onItemChanged()
        {
            base.onItemChanged();
            content.anchoredPosition = new Vector2(0, 0);
            // result = null; showAnswer = false;
        }

        void onChoiceSelected(int index)
        {
            showAnswer = true;

            var question = choiceContainer.getItem();
            var resultChoice = question.choices[index];
            var resultText = resultChoice.resultText;
            var resultEffect = resultChoice.effects;
            description.text = resultText;

            requestRefresh(true);
        }
        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="question">题目</param>
        protected override void drawExactlyItem(PlotQuestion question)
        {
            base.drawExactlyItem(question);
            drawBaseInfo(question);
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
        /// 绘制基本信息
        /// </summary>
        void drawBaseInfo(PlotQuestion question)
        {
            if (tipName)
                tipName.text = question.eventName;
            if (image)
                image.texture = question.picture;
        }

        /// <summary>
        /// 绘制题干
        /// </summary>
        /// <param name="question">题目</param>
        void drawTitle(PlotQuestion question)
        {
            if(title)
                title.text = question.title;
        }

        /// <summary>
        /// 绘制图片和选项
        /// </summary>
        /// <param name="question">题目</param>
        void drawChoices(PlotQuestion question)
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

            if (tipName) tipName.text = "";
            if (choiceContainer) choiceContainer.gameObject.SetActive(false);
            if (description) description.text = "";
            if (description.gameObject) description.gameObject.SetActive(false);
        }

        #endregion

    }
}
