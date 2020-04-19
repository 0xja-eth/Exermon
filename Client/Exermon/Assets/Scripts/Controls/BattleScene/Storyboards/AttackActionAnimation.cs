using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using PlayerModule.Data;
using BattleModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 对战场景分镜控件
/// </summary>
namespace UI.BattleScene.Controls.Storyboards {

    /// <summary>
    /// 攻击行动动画显示
    /// </summary>
    public class AttackActionAnimation : ItemDisplay<RuntimeBattle>{

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BaseWindow selfWindow;

        public BattlerAttackActionStoryboard 
            selfStoryboard, oppoStoryboard;

        /// <summary>
        /// 内部变量
        /// </summary>
        protected bool answered = false, correct = false;
        
        #region 启动/结束控制

        /// <summary>
        /// 启动窗口
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="refresh">强制刷新</param>
        public override void startView(
            RuntimeBattle item, bool refresh = false) {
            base.startView(item, refresh);
            //selfStoryboard.startView(item.self(), true);
            //oppoStoryboard.startView(item.oppo(), true);
        }

        /// <summary>
        /// 启动视窗
        /// </summary>
        public override void startView() {
            base.startView();
            if (selfWindow) selfWindow.startWindow();
        }
        /// <param name="force">强制（无过渡动画）</param>
        public void startView(bool force) {
            if (force) base.startView(); else startView();
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            shown = false;
            if (selfWindow) selfWindow.terminateWindow();
            else base.terminateView();
        }
        /// <param name="force">强制（无过渡动画）</param>
        public void terminateView(bool force) {
            if (force) base.terminateView(); else terminateView();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 物品改变回调
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="battle">对战</param>
        protected override void drawExactlyItem(RuntimeBattle battle) {
            base.drawExactlyItem(battle);

            selfStoryboard.startView(item.self(), true);
            oppoStoryboard.startView(item.oppo(), true);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            selfStoryboard.requestClear(true);
            oppoStoryboard.requestClear(true);
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 处理伤害
        /// </summary>
        public void onAttack() {
            var action = item.attackAction();
            if (action == null) return;
            CalcService.AttackActionProceessor.process(action);
            requestRefresh();
        }

        #endregion
    }
}
