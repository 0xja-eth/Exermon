
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

using QuestionModule.Data;

using BattleModule.Data;
using BattleModule.Services;

using UI.BattleScene.Controls;
using UI.BattleScene.Controls.Question;
using UI.BattleScene.Controls.Storyboards;

using BaseBattleWindow = UI.BattleScene.Windows.BaseBattleWindow;

/// <summary>
/// 对战解析场景窗口
/// </summary>
namespace UI.BattleAnswerScene.Windows {

    /// <summary>
    /// 题目窗口
    /// </summary>
    public class QuestionWindow : BaseBattleWindow {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string IndexTextFormat = "{0}/{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionDisplay questionDisplay;

        public Text indexText;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        protected new BattleAnswerScene scene;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        int index = 0; // 回合索引

        BattleRecord record;
        Question question;

        #region 初始化

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (BattleAnswerScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            record = battle.record;
        }

        #endregion

        #region 启动/结束窗口

        /// <summary>
        /// 启动窗口
        /// </summary>
        public override void startWindow() {
            startWindow(0);
        }
        /// <param name="index">索引</param>
        public void startWindow(int index) {
            base.startWindow();
            setIndex(index);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 己方分镜
        /// </summary>
        /// <returns></returns>
        protected override BattlerPrepareStoryboard selfStoryboard() { return null; }

        /// <summary>
        /// 对方分镜
        /// </summary>
        /// <returns></returns>
        protected override BattlerPrepareStoryboard oppoStoryboard() { return null; }

        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="index">索引</param>
        public void setIndex(int index) {
            var len = roundCount();
            this.index = (index + len) % len;
            requestRefresh();
        }

        /// <summary>
        /// 题目数量
        /// </summary>
        /// <returns>返回题目数量</returns>
        public int roundCount() {
            return record.self().questions.Length;
        }

        /// <summary>
        /// 当前回合
        /// </summary>
        /// <returns>返回当前回合</returns>
        public override BattleRound round() {
            return record.rounds[index];
        }

        /// <summary>
        /// 当前题目
        /// </summary>
        /// <returns></returns>
        public BattleRoundResult roundResult() {
            return record.self().questions[index];
        }

        /// <summary>
        /// 等待时间
        /// </summary>
        /// <returns></returns>
        protected override int waitSeconds() {
            return question.star().stdTime;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 重置状态
        /// </summary>
        protected override void resetStatus() {
            base.resetStatus();
            record = battle.record;
            question = roundResult().question();
        }

        /// <summary>
        /// 刷新题目
        /// </summary>
        protected override void refreshQuestion() {
            base.refreshQuestion();
            questionDisplay.setItem(question);
            questionDisplay.result = roundResult();
            questionDisplay.showAnswer = true;
        }

        /// <summary>
        /// 刷新对战时钟
        /// </summary>
        /// <param name="seconds"></param>
        protected override void refreshBattleClock(int seconds) {
            var timespan = roundResult().timespan / 1000;
            battleClock.setDuration(seconds);
            battleClock.setTimer(timespan, true);
        }

        /// <summary>
        /// 刷新索引文本
        /// </summary>
        void refreshIndexText() {
            indexText.text = string.Format(
                IndexTextFormat, index, roundCount());
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshIndexText();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            questionDisplay.requestClear(true);
            indexText.text = "";
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 下一题
        /// </summary>
        public void next() {
            setIndex(index + 1);
        }

        /// <summary>
        /// 上一题
        /// </summary>
        public void prev() {
            setIndex(index - 1);
        }

        #endregion

    }
}