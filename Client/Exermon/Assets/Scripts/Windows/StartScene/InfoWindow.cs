using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using LitJson;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using PlayerModule.Services;

using UI.Common.Controls.InputFields;

namespace UI.StartScene.Windows {

    /// <summary>
    /// 补全信息窗口
    /// </summary>
    public class InfoWindow : BaseWindow {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string CreateSuccessText = "人物创建完成，点击确认进入游戏！";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public DateTimeField birthInput; // 名称
        public TextInputField schoolInput, cityInput,
            contactInput, descriptionInput; // 名称

        /// <summary>
        /// 场景组件引用
        /// </summary>
        StartScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        PlayerService playerSer = null;

        #region 初始化

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (StartScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            playerSer = PlayerService.get();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 创建角色
        /// </summary>
        public void create() {
            doCreate();
        }

        /// <summary>
        /// 跳过创建
        /// </summary>
        public void jump() {
            doCreate(true);
        }

        /// <summary>
        /// 执行创建
        /// </summary>
        void doCreate(bool jump = false) {
            if (jump)
                playerSer.createInfo(onCreateSuccess);
            else {
                var birth = birthInput.getValue();
                var school = schoolInput.getValue();
                var city = cityInput.getValue();
                var contact = contactInput.getValue();
                var description = descriptionInput.getValue();

                playerSer.createInfo(birth, school, city,
                    contact, description, onCreateSuccess);
            }
        }

        /// <summary>
        /// 完善信息成功回调
        /// </summary>
        void onCreateSuccess() {
            gameSys.requestAlert(CreateSuccessText);
            scene.refresh();
        }

        #endregion
    }
}