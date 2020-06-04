
using System;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using GameModule.Services;
using PlayerModule.Services;

using UI.Common.Controls.InputFields;

namespace UI.StartScene.Windows {

    /// <summary>
    /// 补全信息窗口
    /// </summary>
    public class InfoWindow : BaseWindow {

        /// <summary>
        /// 常量定义
        /// </summary>
        static readonly TimeSpan DeltaDate = 
            new TimeSpan(18 * 365, 0, 0, 0, 0); // 18 年

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
        DataService dataSer = null;
        PlayerService playerSer = null;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            configureInputs();
        }

        /// <summary>
        /// 配置输入域
        /// </summary>
        void configureInputs() {
            var now = DateTime.Now;
            var min = dataSer.staticData.configure.minBirth;
            birthInput.configure(min, now, now - DeltaDate);
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = SceneUtils.getCurrentScene<StartScene>();
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            dataSer = DataService.get();
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