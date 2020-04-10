using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using PlayerModule.Data;

using GameModule.Services;

using PlayerModule.Services;
using BattleModule.Services;

using UI.Common.Controls.ParamDisplays;

using UI.BattleScene.Controls;
using UI.BattleScene.Controls.Question;

/// <summary>
/// 对战开始场景窗口
/// </summary>
namespace UI.BattleScene.Windows {

    /// <summary>
    /// 题目窗口
    /// </summary>
    public class QuestionWindow : BaseWindow {

        /// <summary>
        /// 常量定义
        /// </summary>
        public const string NotStartedAlertText = "尚未开始作答！";
        public const string EmptyAlertText = "未选择答案！";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionInfo questionInfo;
        public QuestionDisplay questionDisplay;
        public BattleClock battleClock;
        public BattlerStatus selfStatus, oppoStatus;

        public QuesChoiceContainer choiceContainer;

        /// <summary>
        /// 内部变量声明
        /// </summary>

        /// <summary>
        /// 场景组件引用
        /// </summary>
        BattleScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        DataService dataSer = null;
        PlayerService playerSer = null;
        BattleService battleSer = null;

        #region 初始化

        /// <summary>
        /// 初次初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            scene = (BattleScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            dataSer = DataService.get();
            playerSer = PlayerService.get();
            battleSer = BattleService.get();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateBattleClock();
        }

        /// <summary>
        /// 更新准备时间
        /// </summary>
        void updateBattleClock() {
            if (battleClock.isTimeUp()) pushAnswer(true);
        }

        #endregion

        #region 启动/结束窗口

        /// <summary>
        /// 启动窗口
        /// </summary>
        public override void startWindow() {
            base.startWindow();
            choiceContainer.startView();
            battleClock.startView();
            selfStatus.startView();
            oppoStatus.startView();
        }

        /// <summary>
        /// 结束窗口
        /// </summary>
        public override void terminateWindow() {
            base.terminateWindow();
            choiceContainer.terminateView();
            battleClock.terminateView();
            selfStatus.terminateView();
            oppoStatus.terminateView();
        }

        #endregion

        #region 数据控制

        #endregion

        #region 界面控制

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            var battle = battleSer.battle;
            var question = battle.round.question();

            questionInfo.setItem(battle.round);

            questionDisplay.setItem(question);
            battleClock.startTimer(question.star().stdTime);

            selfStatus.setItem(battle.self(), true);
            oppoStatus.setItem(battle.oppo(), true);

            questionDisplay.startQuestion();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            questionInfo.requestClear(true);
            battleClock.requestClear(true);

            questionDisplay.requestClear(true);

            selfStatus.requestClear(true);
            oppoStatus.requestClear(true);
        }

        #endregion

        #region 流程控制
        
        /// <summary>
        /// 题目结果回调
        /// </summary>
        public void onQuested() {
            onQuestionTerminated(false);
            var battle = battleSer.battle;
            questionDisplay.result = battle.self();
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void confirm() {
            pushAnswer(false);
        }

        /// <summary>
        /// 上传答案
        /// </summary>
        /// <param name="force">是否强制提交</param>
        void pushAnswer(bool force = false) {
            if (!questionDisplay.isStarted())
                gameSys.requestAlert(NotStartedAlertText);
            else {
                var selection = questionDisplay.getSelectionIds();
                var timespan = questionDisplay.getTimeSpan();

                if (selection.Length <= 0 && !force)
                    gameSys.requestAlert(EmptyAlertText);
                else
                    battleSer.questionAnswer(selection, timespan,
                        onQuestionTerminated);
            }
        }

        /// <summary>
        /// 准备完成回调
        /// </summary>
        void onQuestionTerminated() {
            onQuestionTerminated(true);
        }
        void onQuestionTerminated(bool mask) {
            questionDisplay.terminateQuestion();
            battleClock.stopTimer();
            if (mask) scene.showWaitingMask();
        }

        #endregion

    }
}