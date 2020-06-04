using UnityEngine;

using Core.UI;
using Core.UI.Utils;

using PlayerModule.Services;

using UI.StatusScene.Controls;
using PlayerStatus = UI.StatusScene.Controls.PlayerStatus;
using ExermonStatus = UI.StatusScene.Controls.ExermonStatus;

namespace UI.StatusScene.Windows {

    /// <summary>
    /// 状态窗口
    /// </summary>
    public class StatusWindow : BaseWindow {

        /// <summary>
        /// 视图枚举
        /// </summary>
        public enum View {
            HumanView, ExermonView,
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public StatusTabController tabController;

        public PlayerStatus.StatusDisplay playerStatusDisplay;
        public ExermonStatus.StatusDisplay exermonStatusDisplay;

        public GameObject buttons;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        View view;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        StatusScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        PlayerService playerSer = null;

        #region 初始化
        
        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = SceneUtils.getCurrentScene<StatusScene>();
        }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
        }
        
        #endregion

        #region 开启控制

        /// <summary>
        /// 开始窗口
        /// </summary>
        public override void startWindow() {
            startWindow(View.HumanView);
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
            buttons.SetActive(false);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        public void refreshView() {
            switch (view) {
                case View.HumanView:
                    playerStatusDisplay.requestRefresh(true); break;
                case View.ExermonView:
                    exermonStatusDisplay.requestRefresh(true); break;
            }
        }

        /// <summary>
        /// 清除视图
        /// </summary>
        public void clearView() {
            playerStatusDisplay.terminateView();
            exermonStatusDisplay.terminateView();
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            buttons.SetActive(true);
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
            clearView();
            this.view = view;
            switch (view) {
                case View.HumanView:
                    playerStatusDisplay.startView(playerSer.player); break;
                case View.ExermonView:
                    exermonStatusDisplay.startView(playerSer.player); break;
            }
        }

        #endregion
    }
}