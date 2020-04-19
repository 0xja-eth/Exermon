
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;

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

using UI.BattleScene.Controls.ItemDisplays;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 对战开始场景窗口
/// </summary>
namespace UI.BattleScene.Windows {

    /// <summary>
    /// 回合结算窗口
    /// </summary>
    public class RoundResultWindow : BaseWindow {

        /// <summary>
        /// 常量定义
        /// </summary>
        const int WaitSeconds = 5;

        const string AnswerUploadWaitText = "作答中……";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ParamDisplay selfResult, oppoResult;
        public UsedItemDisplay selfItem, oppoItem;
        public Text timer;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        RuntimeBattle battle;
        DateTime endTime;
        bool timing = false;

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
            updateTimer();
        }

        /// <summary>
        /// 更新准备时间
        /// </summary>
        void updateTimer() {
            if (!timing) return;
            var now = DateTime.Now;
            if (now < endTime) drawTimer();
            else clearTimer();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置倒计时
        /// </summary>
        void setupTimer() {
            var now = DateTime.Now;
            endTime = now + new TimeSpan(0, 0, WaitSeconds);
            timing = true;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            battle = battleSer.battle;

            drawBattlerResult();
            drawUsedItems();
            setupTimer();
        }

        /// <summary>
        /// 绘制对战状态
        /// </summary>
        void drawBattlerResult() {
            selfResult.setValue(battle.self(), "round_result");
            oppoResult.setValue(battle.oppo(), "round_result");
        }

        /// <summary>
        /// 绘制使用物品
        /// </summary>
        void drawUsedItems() {
            var selfItem = battle.self().roundItem();
            var oppoItem = battle.oppo().roundItem();

            this.selfItem.setItem(selfItem);
            this.oppoItem.setItem(oppoItem);
        }

        /// <summary>
        /// 绘制倒计时
        /// </summary>
        void drawTimer() {
            var delta = endTime - DateTime.Now;
            timer.text = ((int)delta.TotalSeconds).ToString();
        }

        /// <summary>
        /// 清空倒计时
        /// </summary>
        void clearTimer() {
            pass();
            timing = false;
            timer.text = "";
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            selfResult.clearValue();
            oppoResult.clearValue();
            selfItem.requestClear(true);
            oppoItem.requestClear(true);
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 跳过
        /// </summary>
        void pass() {
            battleSer.resultComplete();
        }

        #endregion

    }
}