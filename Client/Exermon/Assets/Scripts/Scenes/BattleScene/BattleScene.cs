
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using Core.Systems;

using PlayerModule.Services;
using SeasonModule.Services;
using BattleModule.Services;

/// <summary>
/// 对战场景
/// </summary>
namespace UI.BattleScene {

    using Windows;

    /// <summary>
    /// 对战场景
    /// </summary>
    public class BattleScene : BaseScene {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string WaitingOppoText = "等待对方行动中……";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PrepareWindow prepareWindow;
        public QuestionWindow questionWindow;

        public GameObject waitingMask;

        /// <summary>
        /// 内部组件设置
        /// </summary>
        Text waitText;

        /// <summary>
        /// 内部系统声明
        /// </summary>
        PlayerService playerSer;
        BattleService battleSer;
        SeasonService seasonSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName() {
            return SceneSystem.Scene.BattleScene;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
            battleSer = BattleService.get();
            seasonSer = SeasonService.get();
        }

        /// <summary>
        /// 初始化其他
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            SceneUtils.depositSceneObject("Scene", this);
            waitText = SceneUtils.find<Text>(waitingMask, "Text");
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            onPerparing();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            if (battleSer.isStateChanged())
                onStateChanged();
            battleSer.update();
        }

        #endregion

        #region 场景控制

        /// <summary>
        /// 关闭所有窗口
        /// </summary>
        void clearWindows() {
            prepareWindow.terminateWindow();
            questionWindow.terminateWindow();
        }

        /// <summary>
        /// 关闭所有控件
        /// </summary>
        void clearControls() {
            hideWaitingMask();
        }

        /// <summary>
        /// 清空场景
        /// </summary>
        void clear() {
            clearWindows();
            clearControls();
        }

        #endregion

        #region 流程控制
        
        #region 状态回调处理

        /// <summary>
        /// 状态改变回调
        /// </summary>
        void onStateChanged() {
            switch ((BattleService.State)battleSer.state) {
                case BattleService.State.Preparing: onPerparing(); break;
                case BattleService.State.Questing: onQuesting(); break;
                case BattleService.State.Quested: onQuested(); break;
                case BattleService.State.Acting: onActing(); break;
                case BattleService.State.Resulting: onResulting(); break;
                case BattleService.State.Terminating: onTerminating(); break;
            }
        }

        /// <summary>
        /// 准备状态
        /// </summary>
        void onPerparing() {
            clear();
            prepareWindow.startWindow();
        }

        /// <summary>
        /// 答题状态
        /// </summary>
        void onQuesting() {
            clear();
            questionWindow.startWindow();
        }

        /// <summary>
        /// 答题完毕
        /// </summary>
        void onQuested() {
            questionWindow.onQuested();
        }

        /// <summary>
        /// 开始行动完毕
        /// </summary>
        void onActing() {
            clear();
        }

        /// <summary>
        /// 回合结算
        /// </summary>
        void onResulting() {
            clear();
        }

        /// <summary>
        /// 对战结束
        /// </summary>
        void onTerminating() {

        }

        #endregion

        #region 具体流程处理

        /// <summary>
        /// 显示等待遮罩
        /// </summary>
        public void showWaitingMask(string text = WaitingOppoText) {
            waitingMask.SetActive(true);
            waitText.text = text;
        }

        /// <summary>
        /// 关闭等待遮罩
        /// </summary>
        public void hideWaitingMask() {
            waitingMask.SetActive(false);
        }

        #endregion

        #endregion

    }
}