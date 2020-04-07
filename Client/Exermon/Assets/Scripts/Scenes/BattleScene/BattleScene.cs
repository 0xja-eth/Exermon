
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

    /// <summary>
    /// 对战场景
    /// </summary>
    public class BattleScene : BaseScene {

        /// <summary>
        /// 外部组件设置
        /// </summary>

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

        }

        /// <summary>
        /// 答题状态
        /// </summary>
        void onQuesting() {

        }

        /// <summary>
        /// 答题完毕
        /// </summary>
        void onQuested() {

        }

        /// <summary>
        /// 开始行动完毕
        /// </summary>
        void onActing() {

        }

        /// <summary>
        /// 回合结算
        /// </summary>
        void onResulting() {

        }

        /// <summary>
        /// 对战结束
        /// </summary>
        void onTerminating() {

        }

        #endregion

        #endregion

    }
}