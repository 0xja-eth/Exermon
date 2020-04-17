
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
        public BattlerAttackActionStoryboard selfAAction, oppoAAction;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        int stage = 0; // 0 => 准备, 1 => 攻击, 2 => 结束

        #region 数据控制

        /// <summary>
        /// 获取自身分镜
        /// </summary>
        /// <returns>返回自身分镜</returns>
        protected override BattlerPrepareStoryboard selfStoryboard() {
            return stage == 0 ? selfPAction : (BattlerPrepareStoryboard)selfAAction;
        }

        /// <summary>
        /// 获取失败分镜
        /// </summary>
        /// <returns>返回对方分镜</returns>
        protected override BattlerPrepareStoryboard oppoStoryboard() {
            return stage == 0 ? oppoPAction : (BattlerPrepareStoryboard)oppoAAction;
        }

        /// <summary>
        /// 清空所有准备行动
        /// </summary>
        public void clearPActions() {
            var lastStage = stage;
            stage = 0; clearStoryboards();
            stage = lastStage;
        }

        /// <summary>
        /// 清空所有攻击行动
        /// </summary>
        public void clearAActions() {
            var lastStage = stage;
            stage = 1; clearStoryboards();
            stage = lastStage;
        }

        #endregion

        #region 界面控制

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
        const float PActionDeltaSeconds = 1;

        /// <summary>
        /// 攻击行动动画显示时间
        /// </summary>
        const float AActionShowSeconds = 1.5f;

        /// <summary>
        /// 显示正确方的题目状态动画
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        IEnumerator actionAnimation() {
            CoroutineUtils.resetActions();
            setupPerpareAction(battle.self());
            setupPerpareAction(battle.oppo());
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
            if (battler.attackAction() != null)
                CoroutineUtils.addAction(() =>
                    showStoryboard(battler), AActionShowSeconds);
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