﻿
using UnityEngine;
using UnityEngine.UI;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using GameModule.Data;
using GameModule.Services;
using RecordModule.Services;

using UI.TitleScene.Windows;

namespace UI.TitleScene {

    /// <summary>
    /// 标题场景
    /// </summary>
    public class TitleScene : BaseScene {

        /// <summary>
        /// 常量设定
        /// </summary>
        const float MaxCameraRotateX = 10; // 摄像机最大横向旋转程度
        const float MaxCameraRotateY = 5; // 摄像机最大纵向旋转程度
        const float MoveSpeed = 0.4f; // 旋转速度

        const string VersionFormat = "版本：{0}.{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public LoginWindow loginWindow;
        public Text version;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        float sumX = 0, sumY = 0;

        /// <summary>
        /// 能否跟随旋转
        /// </summary>
        public bool rotatable { get; set; } = true;

        /// <summary>
        /// 内部系统声明
        /// </summary>
        GameService gameSer;
        RecordService recordSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.TitleScene;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSer = GameService.get();
            recordSer = RecordService.get();

            gameSys.start();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            if (!version) return;
            var mainVersion = GameStaticData.LocalMainVersion;
            var subVersion = GameStaticData.LocalSubVersion;
            version.text = string.Format(VersionFormat, mainVersion, subVersion);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            if (rotatable)
                updateCameraRotating();
            if (GameSystem.initialized)
                updateLoading();
        }

        /// <summary>
        /// 更新摄像机跟随
        /// </summary>
        void updateCameraRotating() {
            // 获得鼠标当前位置的X和Y
            float mouseX = Input.GetAxis("Mouse X") * MoveSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * MoveSpeed;

            sumX = Mathf.Clamp(sumX + mouseX, -MaxCameraRotateX, MaxCameraRotateX);
            sumY = Mathf.Clamp(sumY + mouseY, -MaxCameraRotateY, MaxCameraRotateY);

            if (Mathf.Abs(sumY) < MaxCameraRotateY)
                // 鼠标在Y轴上的移动转为摄像机上下的运动，即是绕着X轴反向旋转
                transform.localRotation = transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);

            if (Mathf.Abs(sumX) < MaxCameraRotateX)
                // 鼠标在X轴上的移动转为摄像机左右的移动，同时带动其子物体摄像机的左右移动
                transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);
        }

        /// <summary>
        /// 更新加载
        /// </summary>
        void updateLoading() {
            /*
            if (gameSys.isLoaded() && !loginWindow.shown)
                loginWindow.startWindow();
            */
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 登陆成功回调
        /// </summary>
        public void startGame() {
            recordSer.get(gameSer.startGame);
        }

        #endregion
    }
}