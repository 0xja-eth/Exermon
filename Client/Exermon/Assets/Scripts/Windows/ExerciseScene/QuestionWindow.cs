
using UnityEngine.UI;
using UnityEngine.Events;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using QuestionModule.Data;

using RecordModule.Data;
using RecordModule.Services;

using UI.Common.Controls.QuestionDisplay;

/// <summary>
/// 刷题场景窗口
/// </summary>
namespace UI.ExerciseScene.Windows {

    /// <summary>
    /// 题目窗口
    /// </summary>
    public class QuestionWindow : BaseWindow {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string EmptyAlertText = "未选择答案！";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionNavigation questionNav;

        public QuestionDisplay questionDisplay;
        public ChoiceButtonContainer buttonContainer; // 按钮容器

        public QuestionTimer timer;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        protected ExerciseScene scene;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        public bool starting { get; private set; } = true; // 回合索引

        ExerciseRecord record;
        Question question;

        /// <summary>
        /// 外部系统
        /// </summary>
        GameSystem gameSys;
        RecordService recordSer;

        #region 初始化

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (ExerciseScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            recordSer = RecordService.get();
            gameSys = GameSystem.get();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            record = recordSer.exerciseRecord;

        }

        /// <summary>
        /// 配置题目
        /// </summary>
        void configureQuestions() {
            questionNav.configure(record.getQuestions());
        }

        #endregion

        #region 启动/结束窗口
        
        #endregion

        #region 数据控制

        #endregion

        #region 界面控制

        #endregion

        #region 流程控制

        /// <summary>
        /// 下一题
        /// </summary>
        public void next() {
            questionNav.next();
        }

        /// <summary>
        /// 上一题
        /// </summary>
        public void prev() {
            questionNav.prev();
        }

        /// <summary>
        /// 题目改变回调
        /// </summary>
        void startQuestion() {
            var qid = questionDisplay.getItem().getID();
            recordSer.startQuestion(qid, onQuestionStarted);
        }

        /// <summary>
        /// 回答
        /// </summary>
        public void answer() {
            answer(false);
        }
        /// <param name="terminate">是否结束</param>
        public void answer(bool terminate, bool force = false) {
            var selection = questionDisplay.getSelectionIds();
            if (selection.Length <= 0 && !force)
                gameSys.requestAlert(EmptyAlertText);
            else doAnswer(selection, terminate);

        }

        /// <summary>
        /// 回答问题
        /// </summary>
        /// <param name="selection"></param>
        void doAnswer(int[] selection, bool terminate) {
            var qid = questionDisplay.getItem().getID();
            var timespan = questionDisplay.getTimeSpan();
            terminate = terminate || questionNav.isLastQuestion();

            var action = terminate ? (UnityAction)
                onExerciseTerminated : onAnswered;
            recordSer.answerQuestion(qid, selection,
                timespan, terminate, action);
        }

        /// <summary>
        /// 结束刷题
        /// </summary>
        public void terminate() {
            answer(true, true);
        }

        #endregion

        #region 回调控制

        /// <summary>
        /// 答题完毕回调
        /// </summary>
        void onAnswered() {
            next(); startQuestion();
        }

        /// <summary>
        /// 题目开始回调
        /// </summary>
        void onQuestionStarted() {
            var question = questionDisplay.getItem();
            timer.startTimer(question.star().stdTime);
            questionDisplay.startQuestion();
        }

        /// <summary>
        /// 刷题结束回调
        /// </summary>
        void onExerciseTerminated() {
            starting = false;

            timer.stopTimer(true);
            questionDisplay.terminateQuestion();

            questionNav.select(0);
            questionNav.results = record.questions;
            questionNav.showAnswer = true;

            scene.onExerciseTerminated();
        }

        #endregion

    }
}