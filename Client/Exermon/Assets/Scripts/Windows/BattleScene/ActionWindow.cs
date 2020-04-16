
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
using UI.BattleScene.Controls.Animators;

/// <summary>
/// 对战开始场景窗口
/// </summary>
namespace UI.BattleScene.Windows {

    /// <summary>
    /// 行动窗口（伪窗口）
    /// </summary>
    public class ActionWindow : BaseWindow {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BattlerPrepareAction selfPAction, oppoPAction;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        RuntimeBattle battle;
        bool passed = false;

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
        
        #endregion

        #region 启动/结束窗口

        /// <summary>
        /// 启动窗口
        /// </summary>
        public override void startWindow() {
            base.startWindow();
        }

        /// <summary>
        /// 结束窗口
        /// </summary>
        public override void terminateWindow() {
            base.terminateWindow();
        }

        #endregion

        #region 数据控制

        #endregion

        #region 界面控制
        
        /// <summary>
        /// 显示上个玩家的答题状态
        /// </summary>
        void showPAction(RuntimeBattlePlayer battler, bool setLast = true) {
            if (battler == battle.self()) showSelfPAction(setLast, battler);
            if (battler == battle.oppo()) showOppoPAction(setLast, battler);
        }

        /// <summary>
        /// 显示自身答题状态
        /// </summary>
        void showSelfPAction(bool setLast = true,
            RuntimeBattlePlayer battler = null) {
            if (battler == null) battler = battle.self();
            if (selfPAction.shown) return;
            selfPAction.startView(battler);
            if (setLast) selfPAction.transform.SetAsLastSibling();
        }

        /// <summary>
        /// 显示对方答题状态
        /// </summary>
        void showOppoPAction(bool setLast = true,
            RuntimeBattlePlayer battler = null) {
            if (battler == null) battler = battle.oppo();
            if (oppoPAction.shown) return;
            oppoPAction.startView(battler);
            if (setLast) oppoPAction.transform.SetAsLastSibling();
        }

        /// <summary>
        /// 清空答题状态
        /// </summary>
        void showPActions() {
            showSelfPAction();
            showOppoPAction();
        }

        /// <summary>
        /// 清空答题状态
        /// </summary>
        void clearPActions() {
            selfPAction.terminateView();
            oppoPAction.terminateView();
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            clearPActions();
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
            CoroutineUtils.addAction(() => showPAction(corr), QStatusDeltaSeconds);
            CoroutineUtils.addAction(() => showPAction(oppo, false), QStatusShowSeconds);
            CoroutineUtils.addAction(clearPActions);
            return CoroutineUtils.generateCoroutine();
        }

        #endregion
        /*
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
                showSelfPAction();
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
        */
        #region 流程控制

        /// <summary>
        /// 确认
        /// </summary>
        public void pass() {
            battleSer.actionComplete();
        }

        #endregion

    }
}