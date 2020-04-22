using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using GameModule.Services;
using PlayerModule.Services;
using ExermonModule.Services;

namespace UI.MainScene {

    /// <summary>
    /// 主场景
    /// </summary>
    public class MainScene : BaseScene {

        /// <summary>
        /// 文本定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>

        /// <summary>
        /// 能否跟随旋转
        /// </summary>
        public bool rotatable { get; set; } = true;

        /// <summary>
        /// 内部系统声明
        /// </summary>
        PlayerService playerSer;
        ExermonService exermonSer;
        GameService gameSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName() {
            return SceneSystem.Scene.MainScene;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSer = GameService.get();
            playerSer = PlayerService.get();
            exermonSer = ExermonService.get();
        }
        
        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            refresh();
        }

        #endregion

        #region 更新控制


        #endregion

        #region 场景控制

        /// <summary>
        /// 刷新场景
        /// </summary>
        public void refresh() {

        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 建筑物点击回调
        /// </summary>
        /// <param name="index"></param>
        public void onBulidingsClick(int index) {
            string sceneName = this.sceneName();
            switch (index) {
                case 1: sceneName = SceneSystem.Scene.StatusScene; break;
                case 2: sceneName = SceneSystem.Scene.BattleStartScene; break;
            }
            sceneSys.pushScene(sceneName);
        }

        #endregion
    }

}
