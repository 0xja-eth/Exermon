
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
    /// 对战窗口基类（动画分镜控制）
    /// </summary>
    public abstract class BaseBattleWindow : BaseWindow {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BattleClock battleClock;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        protected RuntimeBattle battle;
        protected bool passed = false;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        protected BattleScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        protected GameSystem gameSys = null;
        protected DataService dataSer = null;
        protected PlayerService playerSer = null;
        protected BattleService battleSer = null;

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
            if (battleClock) updateBattleClock();
        }

        /// <summary>
        /// 更新准备时间
        /// </summary>
        protected virtual void updateBattleClock() {
            if (!passed && battleClock.isTimeUp()) pass();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取自身分镜
        /// </summary>
        /// <returns>返回自身分镜</returns>
        protected abstract BattlerPrepareStoryboard selfStoryboard();

        /// <summary>
        /// 获取对方分镜
        /// </summary>
        /// <returns>返回对方分镜</returns>
        protected abstract BattlerPrepareStoryboard oppoStoryboard();

        #endregion

        #region 界面控制

        #region 分镜控制

        /// <summary>
        /// 显示分镜
        /// </summary>
        /// <param name="battler">要显示的战斗者</param>
        /// <param name="setLast">是否置顶</param>
        /// <param name="force">是否强制显示（忽略动画）</param>
        public void showStoryboard(RuntimeBattlePlayer battler, 
            bool setLast = true, bool force = false) {
            if (battler == battle.self())
                showSelfStoryboard(setLast, force);
            if (battler == battle.oppo())
                showOppoStoryboard(setLast, force);
        }
        /// <param name="display">用于显示的控件</param>
        void showStoryboard(BattlerPrepareStoryboard display, 
            RuntimeBattlePlayer battler = null,
            bool setLast = true, bool force = false) {
            if (display == null || display.shown) return;
            if (force) display.startView(force);
            else display.startView(battler);
            if (setLast) display.transform.SetAsLastSibling();
        }

        /// <summary>
        /// 显示自身分镜
        /// </summary>
        /// <param name="setLast">是否置顶</param>
        /// <param name="force">是否强制显示（忽略动画）</param>
        public void showSelfStoryboard(bool setLast = true, bool force = false) {
            showStoryboard(selfStoryboard(), battle.self(), setLast, force);
        }

        /// <summary>
        /// 显示对方分镜
        /// </summary>
        /// <param name="setLast">是否置顶</param>
        /// <param name="force">是否强制显示（忽略动画）</param>
        public void showOppoStoryboard(bool setLast = true, bool force = false) {
            showStoryboard(oppoStoryboard(), battle.oppo(), setLast, force);
        }

        /// <summary>
        /// 显示所有分镜
        /// </summary>
        public void showStoryboards() {
            showStoryboards(false);
        }
        /// <param name="force">是否强制显示（忽略动画）</param>
        public void showStoryboards(bool force = false) {
            showSelfStoryboard(force: force);
            showOppoStoryboard(force: force);
        }

        /// <summary>
        /// 清空所有分镜
        /// </summary>
        public void clearStoryboards() {
            clearStoryboards(false);
        }
        /// <param name="force">是否强制显示（忽略动画）</param>
        public void clearStoryboards(bool force) {
            selfStoryboard()?.terminateView(force);
            oppoStoryboard()?.terminateView(force);
        }

        #endregion

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();

            passed = false;
            battle = battleSer.battle;
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();

            clearStoryboards();
        }

        #endregion
        
        #region 流程控制

        /// <summary>
        /// 跳过
        /// </summary>
        public virtual void pass() {
            passed = true;
        }

        #endregion
    }
}