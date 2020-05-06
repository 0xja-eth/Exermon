using System;

using UnityEngine;

using Core.UI;
using Core.UI.Utils;

using PlayerModule.Data;
using PlayerModule.Services;
using RecordModule.Services;

using UI.PackScene.Controls;
using UI.PackScene.Controls.GeneralPack;

namespace UI.PackScene.Windows {

    /// <summary>
    /// 状态窗口
    /// </summary>
    public class PackWindow : BaseWindow {

        /// <summary>
        /// 视图枚举
        /// </summary>
        public enum View {
            Human, Exermon, QuesSugar,
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackTabController tabController;

        public HumanPackDisplay humanPack;
        public ExerPackDisplay exerPack;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        View view;
        Player player;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        PackScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        PlayerService playerSer = null;
        RecordService recordSer = null;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            player = playerSer.player;
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (PackScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
            recordSer = RecordService.get();
        }
        
        #endregion

        #region 开启控制

        /// <summary>
        /// 开始窗口
        /// </summary>
        public override void startWindow() {
            startWindow(View.Human);
        }

        /// <summary>
        /// 开始窗口
        /// </summary>
        public void startWindow(View view) {
            base.startWindow();
            tabController.startView((int)view);
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            base.terminateView();
        }

        #endregion
        
        #region 界面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        public void refreshView() {
            clearView();
            switch (view) {
                case View.Human: onHumanPack(); break;
                case View.Exermon: onExerPack(); break;
                case View.QuesSugar: onQuesSugar(); break;
            }
        }

        /// <summary>
        /// 人类背包
        /// </summary>
        void onHumanPack() {
            var container = player.packContainers.humanPack;
            humanPack.startView(); humanPack.setPackData(container);
        }

        /// <summary>
        /// 艾瑟萌背包
        /// </summary>
        void onExerPack() {
            var container = player.packContainers.exerPack;
            exerPack.startView(); exerPack.setPackData(container);
        }

        /// <summary>
        /// 题目糖背包
        /// </summary>
        void onQuesSugar() {

        }

        /// <summary>
        /// 清除视图
        /// </summary>
        public void clearView() {
            humanPack.terminateView();
            exerPack.terminateView();
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshView();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            clearView();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 切换视图
        /// </summary>
        public void switchView(int view) {
            switchView((View)view);
        }
        public void switchView(View view) {
            this.view = view;
            requestRefresh(true);
        }

        #endregion
    }
}