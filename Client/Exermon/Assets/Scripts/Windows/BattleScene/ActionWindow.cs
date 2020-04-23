
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using BattleModule.Data;

using UI.BattleScene.Controls.Storyboards;

/// <summary>
/// 对战场景窗口
/// </summary>
namespace UI.BattleScene.Windows {

    /// <summary>
    /// 行动窗口（伪窗口）
    /// </summary>
    public class ActionWindow : BaseBattleWindow {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BattlerPrepareActionStoryboard selfPAction, oppoPAction;
        public AttackActionAnimation selfAAction, oppoAAction;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        AttackActionAnimation currentAction = null;

        #region 数据控制

        /// <summary>
        /// 获取自身分镜
        /// </summary>
        /// <returns>返回自身分镜</returns>
        protected override BattlerPrepareStoryboard selfStoryboard() {
            return selfPAction;
        }

        /// <summary>
        /// 获取失败分镜
        /// </summary>
        /// <returns>返回对方分镜</returns>
        protected override BattlerPrepareStoryboard oppoStoryboard() {
            return oppoPAction;
        }

        /// <summary>
        /// 清空所有准备行动
        /// </summary>
        public void clearPActions() {
            clearStoryboards();
        }

        /// <summary>
        /// 清空所有攻击行动
        /// </summary>
        public void clearAActions() {
            selfAAction.terminateView();
            oppoAAction.terminateView();
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 显示动画
        /// </summary>
        public void startAction(AttackActionAnimation action) {
            action.startView(battle);
            currentAction = action;
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();

            doRoutine(actionAnimation());
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
        }

        #endregion
        
        #region 动画控制

        /// <summary>
        /// 准备行动动画显示时间差
        /// </summary>
        const float PActionDeltaSeconds = 2;

        /// <summary>
        /// 攻击行动动画显示时间
        /// </summary>
        const float AActionShowSeconds = 2.5f;

        /// <summary>
        /// 显示正确方的题目状态动画
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        IEnumerator actionAnimation() {
            CoroutineUtils.resetActions();
            setupPerpareAction(battle.self());
            setupPerpareAction(battle.oppo());
            CoroutineUtils.addAction(clearPActions);
            setupAttackAction(battle.self());
            setupAttackAction(battle.oppo());
            CoroutineUtils.addAction(pass);
            return CoroutineUtils.generateCoroutine();
        }

        /// <summary>
        /// 配置准备行动
        /// </summary>
        /// <param name="battler">行动玩家</param>
        void setupPerpareAction(RuntimeBattlePlayer battler) {
            if (battler.perpareAction() != null)
                CoroutineUtils.addAction(() =>
                    showStoryboard(battler), PActionDeltaSeconds);
        }

        /// <summary>
        /// 配置攻击行动
        /// </summary>
        /// <param name="battler">行动玩家</param>
        void setupAttackAction(RuntimeBattlePlayer battler) {
            if (battler.attackAction() == null) return;

            AttackActionAnimation action = null;
            if (battler == battle.self()) action = selfAAction;
            if (battler == battle.oppo()) action = oppoAAction;
            if (action == null) return;
            CoroutineUtils.addAction(() => 
                startAction(action), AActionShowSeconds);
        }

        #endregion

        #region 动画回调

        /// <summary>
        /// 攻击
        /// </summary>
        public void onAttack() {
            if (currentAction == null) return;
            currentAction.onAttack();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 完成
        /// </summary>
        public override void pass() {
            base.pass();
            battleSer.actionComplete();
        }

        #endregion
    }
}