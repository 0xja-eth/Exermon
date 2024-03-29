﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using PlayerModule.Data;

using GameModule.Services;
using PlayerModule.Services;
using ExermonModule.Services;

using UI.StartScene.Windows;

namespace UI.StartScene {

    /// <summary>
    /// 开始场景
    /// </summary>
    public class StartScene : BaseScene {

        /// <summary>
        /// 文本定义
        /// </summary>
        const string AbnormalAlertText = "您的账号异常，请返回登陆重试！";

        /// <summary>
        /// 步骤枚举
        /// </summary>
        public enum Step {
            Character, Exermons, Gifts, Info, Finished, Abnormal
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public CharacterWindow characterWindow;
        public ExermonsWindow exermonsWindow;
        public GiftsWindow giftsWindow;
        public InfoWindow infoWindow;

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

        Step step;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.StartScene;
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
            refreshStep();
            refreshWindows();
        }

        /// <summary>
        /// 刷新当前步骤
        /// </summary>
        void refreshStep() {
            var player = playerSer.player;
            switch (player.statusEnum()) {
                case Player.Status.Uncreated:
                    step = Step.Character; break;
                case Player.Status.CharacterCreated:
                    step = Step.Exermons; break;
                case Player.Status.ExermonsCreated:
                    step = Step.Gifts; break;
                case Player.Status.GiftsCreated:
                    step = Step.Info; break;
                case Player.Status.Normal:
                    step = Step.Finished; break;
                default:
                    step = Step.Abnormal; break;
            }
        }

        /// <summary>
        /// 刷新显示的窗口
        /// </summary>
        void refreshWindows() {
            closeWindows();
            switch (step) {
                case Step.Character:
                    startCharacterWindow(); break;
                case Step.Exermons:
                    startExermonsWindow(); break;
                case Step.Gifts:
                    startGiftsWindow(); break;
                case Step.Info:
                    startInfoWindow(); break;
                case Step.Finished:
                    processFinished(); break;
                case Step.Abnormal:
                    processAbnormal(); break;
            }
        }

        /// <summary>
        /// 开始人物创建窗口
        /// </summary>
        void startCharacterWindow() {
            characterWindow.startWindow();
        }

        /// <summary>
        /// 开始艾瑟萌选择窗口
        /// </summary>
        void startExermonsWindow() {
            exermonsWindow.startWindow();
        }

        /// <summary>
        /// 开始天赋选择窗口
        /// </summary>
        void startGiftsWindow() {
            exermonSer.loadExerHub(() =>
                exermonSer.loadExerSlot(giftsWindow.startWindow)
            );
        }

        /// <summary>
        /// 开始完善资料窗口
        /// </summary>
        void startInfoWindow() {
            infoWindow.startWindow();
        }

        /// <summary>
        /// 处理创建完成
        /// </summary>
        void processFinished() {
            sceneSys.pushScene(SceneSystem.Scene.MainScene, data: true);
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        void processAbnormal() {
            gameSys.requestAlert(AbnormalAlertText);
        }

        /// <summary>
        /// 关闭所有窗口
        /// </summary>
        public void closeWindows() {
            characterWindow.terminateWindow();
            exermonsWindow.terminateWindow();
            giftsWindow.terminateWindow();
            infoWindow.terminateWindow();
        }

        #endregion
    }
}