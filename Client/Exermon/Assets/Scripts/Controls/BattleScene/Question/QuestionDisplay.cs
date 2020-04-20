using System;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using RecordModule.Data;
using RecordModule.Services;

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
        const string CollectOnText = "已收藏";
        const string CollectOffText = "收藏";

        /// <summary>
        /// 外部组件设置
        /// </summary>        
        public QuestionText title, description;
        public QuesPictureContainer pictureContaienr; // 图片容器
        public QuesChoiceContainer choiceContainer; // 选项容器
        public ChoiceButtonContainer buttonContainer; // 按钮容器

        public Text answerText, collectText;
        public Image collectImg;
        public GameObject resultObj;

        public GameObject confirmBtn;

        public RectTransform content;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public Texture2D collectOn, collectOff;

        /// <summary>
        /// 外部系统定义
        /// </summary>
        RecordService recordSer;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        bool quesStarted = false;
        DateTime startTime;

        /// <summary>
        /// 显示结果
        /// </summary>
        IQuestionResult _result = null;
        public IQuestionResult result {
            get { return _result; }
            set {
                choiceContainer.result = value;
                buttonContainer.result = value;
                _result = value;
                requestRefresh();
            }
        }

        /// <summary>
        /// 显示答案解析
        /// </summary>
        bool _showAnswer = false;
        public bool showAnswer {
            get { return _showAnswer; }
            set {
                choiceContainer.showAnswer = value;
                buttonContainer.showAnswer = value;
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
            //if (content) registerUpdateLayout(content);
        }
        
        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            recordSer = RecordService.get();
        }

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
            confirmBtn.SetActive(quesStarted = true);
            startTime = DateTime.Now;
        }

        /// <summary>
        /// 开始答题
        /// </summary>
        public void terminateQuestion() {
            confirmBtn.SetActive(quesStarted = false);
        }

        /// <summary>
        /// 是否已经开始作答
        /// </summary>
        /// <returns></returns>
        public bool isStarted() { return quesStarted; }

        /// <summary>
        /// 获取答题时长
        /// </summary>
        /// <returns></returns>
        public TimeSpan getTimeSpan() {
            if (!quesStarted) return default;
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
            result = null; showAnswer = false;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="question">题目</param>
        protected override void drawExactlyItem(Question question) {
            base.drawExactlyItem(question);

            drawTitle(question);
            drawPictruesAndChoices(question);
            if (showAnswer) drawResult(question);
            else resultObj?.SetActive(false);


        }

        /// <summary>
        /// 绘制题干
        /// </summary>
        /// <param name="question">题目</param>
        void drawTitle(Question question) {
            QuestionText.TexturePool.setTextures(question.textures());
            title.text = question.title;
        }

        /// <summary>
        /// 绘制图片和选项
        /// </summary>
        /// <param name="question">题目</param>
        void drawPictruesAndChoices(Question question) { 
            pictureContaienr.setItem(question, false);
            choiceContainer.setItem(question, false);
            buttonContainer.setItem(question, false);
        }

        /// <summary>
        /// 绘制结果
        /// </summary>
        /// <param name="question">题目</param>
        void drawResult(Question question) {
            if (description) 
                description.text = question.description;
            if (answerText)
                answerText.text = string.Format(
                    AnswerTextFormat, generateAnswerText());

            resultObj?.SetActive(true);
        }

        /// <summary>
        /// 绘制收藏情况
        /// </summary>
        /// <param name="question"></param>
        void drawCollect(Question question) {
            var record = recordSer.recordData.
                getQuestionRecord(question.getID());
            var collected = record.collected;
            var texture = collected ? collectOn : collectOff;
            var text = collected ? CollectOnText : CollectOffText;
            collectImg.overrideSprite = AssetLoader.generateSprite(texture);
            collectText.text = text;
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