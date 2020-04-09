using System;

using UnityEngine;
using UnityEngine.UI;

using RecordModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.SystemExtend.QuestionText;

namespace UI.BattleScene.Controls.Question {
    using Question = QuestionModule.Data.Question;

    /// <summary>
    /// 题目图片容器
    /// </summary>
    public class QuestionDisplay : ItemDetailDisplay<Question> {

        /// <summary>
        /// 常量设置
        /// </summary>
        const string AnswerTextFormat = "正确答案：{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>        
        public Color normalFontColor = new Color(0, 0, 0);
        public Color descriptionColor = new Color(0.2627451f, 0.3960784f, 0.7294118f);

        public QuestionText title, description;
        public QuesPictureContainer pictureContaienr; // 图片容器
        public QuesChoiceContainer choiceContainer; // 选项容器
        public ChoiceButtonContainer buttonContainer; // 按钮容器

        public Text answerText;
        public GameObject resultObj;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        bool started = false;
        DateTime startTime;

        QuestionSetRecord.IQuestionResult _result = null; // 是否显示答案
        public QuestionSetRecord.IQuestionResult result {
            get { return _result; }
            set {
                choiceContainer.result = value;
                buttonContainer.result = value;
                _result = value;
                requestRefresh();
            }
        }

        #region 初始化

        /// <summary>
        /// 配置
        /// </summary>
        public override void configure() {
            base.configure();
            title.imageContainer = pictureContaienr;
            description.imageContainer = pictureContaienr;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 开始答题
        /// </summary>
        public void startQuestion() {
            started = true;
            startTime = DateTime.Now;
        }

        /// <summary>
        /// 开始答题
        /// </summary>
        public void terminateQuestion() {
            started = false;
        }

        /// <summary>
        /// 是否已经开始作答
        /// </summary>
        /// <returns></returns>
        public bool isStarted() { return started; }

        /// <summary>
        /// 获取答题时长
        /// </summary>
        /// <returns></returns>
        public TimeSpan getTimeSpan() {
            if (!started) return default;
            return DateTime.Now - startTime;
        }

        /// <summary>
        /// 获取所选选项
        /// </summary>
        /// <returns>返回所选选项对象</returns>
        public Question.Choice[] getSelection() {
            return buttonContainer.getCheckedItems();
        }

        /// <summary>
        /// 获取所选选项
        /// </summary>
        /// <returns>返回所选选项ID数组</returns>
        public int[] getSelectionIds() {
            var choices = getSelection();
            var res = new int[choices.Length];
            for (int i = 0; i < choices.Length; ++i)
                res[i] = choices[i].order;
            return res;
        }

        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();
            result = null;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="question">题目</param>
        protected override void drawExactlyItem(Question question) {
            base.drawExactlyItem(question);

            QuestionText.TexturePool.setTextures(question.textures());

            title.text = question.title;
            title.color = normalFontColor;

            if (result != null && description) {
                description.text = question.description;
                description.color = descriptionColor;
            }

            if (result != null && answerText)
                answerText.text = string.Format(
                    AnswerTextFormat, generateAnswerText());

            resultObj?.SetActive(result != null);

            pictureContaienr.setItem(question, true);
            choiceContainer.setItem(question, true);
            buttonContainer.setItem(question, true);
        }

        /// <summary>
        /// 生成答案文本
        /// </summary>
        /// <returns></returns>
        string generateAnswerText() {
            string res = "";
            var choices = choiceContainer.getItems();
            for (int i = 0; i < choices.Length; ++i)
                if (choices[i].answer)
                    res += (char)('A' + i);
            return res;
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            QuestionText.TexturePool.clearTextures();
            pictureContaienr.clearItems();
            choiceContainer.clearItems();
            buttonContainer.clearItems();
            title.text = "";

            resultObj?.SetActive(false);
            if (description) description.text = "";
            if (answerText) answerText.text = "";
        }

        #endregion
    }
}