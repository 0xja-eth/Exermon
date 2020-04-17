
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using PlayerModule.Data;

using GameModule.Services;

using PlayerModule.Services;

using BattleModule.Data;
using BattleModule.Services;

using UI.BattleScene.Controls;
using UI.BattleScene.Controls.Question;
using UI.BattleScene.Controls.Storyboards;

/// <summary>
/// 对战开始场景窗口
/// </summary>
namespace UI.BattleScene.Windows {

    /// <summary>
    /// 题目窗口
    /// </summary>
    public class QuestionWindow : BaseBattleWindow {

        /// <summary>
        /// 常量定义
        /// </summary>
        public const string NotStartedAlertText = "尚未开始作答！";
        public const string EmptyAlertText = "未选择答案！";

        const string AnswerUploadWaitText = "作答中……";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionInfo questionInfo;
        public QuestionDisplay questionDisplay;
        public BattlerStatus selfStatus, oppoStatus;

        public BattlerQuestedStoryboard selfQStatus, oppoQStatus;

        public Animation oppoPromptAni;

        public QuesChoiceContainer choiceContainer;
        
        #region 启动/结束窗口

        /// <summary>
        /// 启动窗口
        /// </summary>
        public override void startWindow() {
            base.startWindow();
            choiceContainer.startView();
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

        /// <summary>
        /// 获取自身分镜
        /// </summary>
        /// <returns>返回自身分镜</returns>
        protected override BattlerPrepareStoryboard selfStoryboard() {
            return selfQStatus;
        }

        /// <summary>
        /// 获取对方分镜
        /// </summary>
        /// <returns>返回对方分镜</returns>
        protected override BattlerPrepareStoryboard oppoStoryboard() {
            return oppoQStatus;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 显示对方提示
        /// </summary>
        void showOppoPrompt() {
            oppoPromptAni.Play();
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();

            var question = battle.round.question();
            question.clearShuffleChoices();

            questionInfo.setItem(battle.round, true);

            Debug.LogError("Refresh: " + question.getID());
            questionDisplay.setItem(question);

            battleClock.startView(question.star().stdTime);

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

        #region 动画控制

        /// <summary>
        /// 题目状态动画显示时间差
        /// </summary>
        const float QStatusDeltaSeconds = 1;
        
        /// <summary>
        /// 题目状态动画显示时间
        /// </summary>
        const float QStatusShowSeconds = 1;

        /// <summary>
        /// 显示正确方的题目状态动画
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        IEnumerator correctQStatusAni(RuntimeBattlePlayer corr) {
            var oppo = battle.getPlayer(corr.getID(), true);
            CoroutineUtils.resetActions();
            CoroutineUtils.addAction(() => showStoryboard(corr), QStatusDeltaSeconds);
            CoroutineUtils.addAction(() => showStoryboard(oppo, false), QStatusShowSeconds);
            CoroutineUtils.addAction(clearStoryboards);
            return CoroutineUtils.generateCoroutine();
        }

        #endregion

        #region 回调控制

        /// <summary>
        /// 题目结果回调
        /// </summary>
        public void onQuested() {
            var lastPlayer = battle.lastPlayer;
            var selfPlayer = battle.self();

            if (lastPlayer == null || !lastPlayer.isAnswered()) return;

            if (lastPlayer == selfPlayer) onSelfQuested();

            // 如果当前答题的人正确或者双方均答题完毕
            if (lastPlayer.correct || battle.isQuestCompleted()) {
                selfQStatus.waiting = oppoQStatus.waiting = false;
                doRoutine(correctQStatusAni(lastPlayer));
                onQuestionTerminated();
            } else if (lastPlayer == selfPlayer) {
                // 如果是自己答题（答错）
                showSelfStoryboard();
                onQuestionTerminated(false, false);
            } else // 否则是对方答题（答错）
                showOppoPrompt();
        }

        /// <summary>
        /// 当前玩家答题结束
        /// </summary>
        void onSelfQuested() {
            gameSys.requestLoadEnd(); // 关闭 Loading
            questionDisplay.result = battle.self();
        }

        /// <summary>
        /// 作答提交回调
        /// </summary>
        void onAnswerPushed() {
            questionDisplay.terminateQuestion();
            // 开始 Loading
            gameSys.requestLoadStart(AnswerUploadWaitText);
        }

        /// <summary>
        /// 题目结束回调
        /// </summary>
        void onQuestionTerminated(bool stopTimer = true, bool showAnswer = true) {
            questionDisplay.terminateQuestion();
            if (stopTimer) battleClock.stopTimer();
            questionDisplay.showAnswer = showAnswer;
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 跳过
        /// </summary>
        public override void pass() {
            pushAnswer(true);
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void confirm() {
            pushAnswer();
        }

        /// <summary>
        /// 上传答案
        /// </summary>
        /// <param name="force">是否强制提交</param>
        void pushAnswer(bool force = false) {
            if (!questionDisplay.isStarted()) return;

            passed = true;

            var selection = questionDisplay.getSelectionIds();
            var timespan = questionDisplay.getTimeSpan();

            if (selection.Length <= 0 && !force)
                gameSys.requestAlert(EmptyAlertText);
            else battleSer.questionAnswer(
                selection, timespan, onAnswerPushed);
        }

        #endregion

    }
}