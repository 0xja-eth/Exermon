using System;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

using RecordModule.Data;
using RecordModule.Services;

using UI.Common.Controls.InputFields;
using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

using UI.Common.Controls.SystemExtend.QuestionText;

namespace UI.Common.Controls.QuestionDisplay {
    using Question = QuestionModule.Data.Question;

    using QuestionDrawView = DrawView.DrawView;

    /// <summary>
    /// 题目显示组件
    /// </summary>
    public class QuestionDisplay : ItemDetailDisplay<Question> {

        /// <summary>
        /// 常量设置
        /// </summary>
        const string AnswerTextFormat = "正确答案：{0}";
        const string DescriptionFormat = "解析：{0}";
        const string EmptyDescriptionText = "暂无";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text num, subject, type, quesNum;
        public StarsDisplay star;

        public QuestionText title, description;
        public QuesPictureContainer pictureContaienr; // 图片容器
        public QuesChoiceContainer choiceContainer; // 选项容器
        public ChoiceButtonContainer buttonContainer; // 按钮容器

        public CollectButton collectButton;

        public Text answerText;
        public GameObject resultObj;

        public GameObject confirmBtn;

        public RectTransform content;

        public DropdownField showTypeSelect;
        
        public GameObject rightView; // 右侧视图

        public GameObject[] rightViews;

        /// <summary>
        /// 外部变量设定
        /// </summary>
        public int pictureViewIndex = 0;
        public int noPictureViewIndex = 1; // 没有图片时显示的右视图索引

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public string quesNumFormat = "题目编号 #{0}";

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
                if (choiceContainer)
                    choiceContainer.result = value;
                if (buttonContainer)
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
                if (choiceContainer)
                    choiceContainer.showAnswer = value;
                if (buttonContainer)
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
            if (showTypeSelect) showTypeSelect.onChanged = onShowTypeChanged;
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
            if (!buttonContainer) return new Question.Choice[0];
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
            content.anchoredPosition = new Vector2(0, 0);
            // result = null; showAnswer = false;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="question">题目</param>
        protected override void drawExactlyItem(Question question) {
            base.drawExactlyItem(question);

            drawBaseInfo(question);
            drawTitle(question);
            drawCollect(question);
            drawPictruesAndChoices(question);

            if (showAnswer) drawResult(question);
            else resultObj?.SetActive(false);

            rightView.SetActive(true);
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        void drawBaseInfo(Question question) {
            if (quesNum) quesNum.text = string.Format(
                quesNumFormat, question.number);
            if (num) num.text = (index+1).ToString();
            if (star) star.setValue(question.starId);
            if (type) type.text = question.typeText();
            if (subject) subject.text = question.subject().name;
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
            pictureContaienr?.startView(question);
            choiceContainer?.startView(question);
            buttonContainer?.startView(question);

            if (question.pictures.Length <= 0) processNoPicture();
            else {
                showTypeSelect.setIndex(pictureViewIndex);
                pictureContaienr?.select(0);
            }
        }

        /// <summary>
        /// 处理无图片情况
        /// </summary>
        void processNoPicture() {
            if (showTypeSelect && showTypeSelect.getIndex() == pictureViewIndex)
                showTypeSelect.setIndex(noPictureViewIndex);
        }

        /// <summary>
        /// 绘制结果
        /// </summary>
        /// <param name="question">题目</param>
        void drawResult(Question question) {
            if (description) description.text = string.Format(
                DescriptionFormat, generateDescription(question));
            if (answerText) answerText.text = string.Format(
                AnswerTextFormat, generateAnswerText(question));
            if (resultObj) resultObj.SetActive(true);
        }

        /// <summary>
        /// 绘制收藏情况
        /// </summary>
        /// <param name="question"></param>
        void drawCollect(Question question) {
            if (collectButton) collectButton.setItem(question);
        }

        /// <summary>
        /// 生成题解文本
        /// </summary>
        /// <returns></returns>
        string generateDescription(Question question) {
            if (question.description == "")
                return EmptyDescriptionText;
            return question.description;
        }

        /// <summary>
        /// 生成答案文本
        /// </summary>
        /// <returns></returns>
        string generateAnswerText(Question question) {
            string res = "";
            var choices = question.shuffleChoices();
            for (int i = 0; i < choices.Length; ++i)
                if (choices[i].answer)
                    res += (char)('A' + i);
            return res;
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            QuestionText.TexturePool.clearTextures();

            pictureContaienr?.clearItems();
            choiceContainer?.clearItems();
            buttonContainer?.clearItems();

            title.text = description.text = "";

            if (quesNum) quesNum.text = "";
            if (num) num.text = "";
            if (type) type.text = "";
            if (subject) subject.text = "";
            if (star) star.clearValue();

            if (resultObj) resultObj.SetActive(false);

            if (description) description.text = "";
            if (answerText) answerText.text = "";

            if (collectButton) collectButton.requestClear(true);

            rightView.SetActive(false);
        }

        /// <summary>
        /// 清除右方视图
        /// </summary>
        void clearRightView() {
            for (int i = 0; i < rightViews.Length; ++i)
                rightViews[i].SetActive(false);
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 显示类型改变回调
        /// </summary>
        public void onShowTypeChanged(Tuple<int, string> obj) {
            if (!showTypeSelect) return;
            var index = showTypeSelect.getIndex();
            for (int i = 0; i < rightViews.Length; ++i) {
                var go = rightViews[i];
                var view = SceneUtils.get<BaseView>(go);
                if (view != null) 
                    if (i == index) view.startView();
                    else view.terminateView();
                else go.SetActive(i == index);
            }
        }

        #endregion
    }
}