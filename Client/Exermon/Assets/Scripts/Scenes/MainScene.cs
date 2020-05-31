using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Systems;
using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;
using PlayerModule.Services;
using ExermonModule.Services;
using ExerPro.EnglishModule.Services;

namespace UI.MainScene {
    using LitJson;
    using Windows;

    /// <summary>
    /// 主场景
    /// </summary>
    public class MainScene : BaseScene {

        /// <summary>
        /// 建筑物按钮
        /// </summary>
        public enum BuildingType {
            Dormitory, // 宿舍（状态）
            Library, // 图书馆（刷题）
            BattleCenter, // 对战大厅（对战）
            Shop, // 小卖部（商城）
            Adventure, // 外出冒险（冒险）
            Playground, // 操场（社团）
            TechBuilding, // 科技楼（科技）
            ClassBuilding, // 教学楼（任务）
            HelpCenter, // 新手中心（新手教程）
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ExerciseConfigWindow exerciseWindow;

        /// <summary>
        /// 能否跟随旋转
        /// </summary>
        public bool rotatable { get; set; } = true;

        /// <summary>
        /// 内部系统声明
        /// </summary>
        GameService gameSer;
        PlayerService playerSer;
        ExermonService exermonSer;

        EnglishService engSer;

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
            engSer = EnglishService.get();
        }

        /// <summary>
        /// 处理通道数据
        /// </summary>
        /// <param name="data"></param>
        protected override void processTunnelData(JsonData data) {
            base.processTunnelData(data);
            var r = DataLoader.load<bool>(data);
            if (r) playerSer.getPlayerStatus();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
        }

        #endregion

        #region 更新控制


        #endregion

        #region 场景控制
        
        #endregion

        #region 流程控制

        /// <summary>
        /// 建筑物点击回调
        /// </summary>
        /// <param name="type"></param>
        public void onBulidingsClick(BuildingType type) {
            string sceneName = "";
            switch (type) {
                case BuildingType.Dormitory:
                    sceneName = SceneSystem.Scene.StatusScene; break;
                case BuildingType.BattleCenter:
                    sceneName = SceneSystem.Scene.BattleStartScene; break;
                case BuildingType.Shop:
                    sceneName = SceneSystem.Scene.ShopScene; break;
                case BuildingType.Library:
                    onLibraryClick(); break;
                case BuildingType.Adventure:
                    onAdventureClick(); break;
            }
            if (sceneName != "") sceneSys.pushScene(sceneName);
        }
        public void onBulidingsClick(int type) {
            onBulidingsClick((BuildingType)type);
        }

        /// <summary>
        /// 图书馆点击回调
        /// </summary>
        void onLibraryClick() {
            exerciseWindow.startWindow();
        }

        /// <summary>
        /// 冒险点击回调
        /// </summary>
        void onAdventureClick() {
            engSer.start(1);
        }

        /// <summary>
        /// 打开记录界面
        /// </summary>
        public void openRecordScene() {
            sceneSys.pushScene(SceneSystem.Scene.RecordScene);
        }
        
        /// <summary>
        /// 打开记录界面
        /// </summary>
        public void openPackScene() {
            sceneSys.pushScene(SceneSystem.Scene.PackScene);
        }

        #endregion
    }

}
