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
        /// <param name="type"></param>
        public void onBulidingsClick(BuildingType type) {
            string sceneName = "";
            switch (type) {
                case BuildingType.Dormitory:
                    sceneName = SceneSystem.Scene.StatusScene; break;
                case BuildingType.BattleCenter:
                    sceneName = SceneSystem.Scene.BattleStartScene; break;
                case BuildingType.Library:
                    onLibraryClick(); break;
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
            var recordSer = RecordModule.Services.RecordService.get();
            recordSer.generateExercise(2, 1, 10, () =>
                sceneSys.pushScene(SceneSystem.Scene.ExerciseScene));
        }

        #endregion
    }

}
