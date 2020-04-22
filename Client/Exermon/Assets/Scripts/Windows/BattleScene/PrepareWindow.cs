using UnityEngine;

using UI.BattleScene.Controls;
using UI.BattleScene.Controls.Prepare;
using UI.BattleScene.Controls.Storyboards;

namespace UI.BattleScene.Windows {

    /// <summary>
    /// 准备窗口
    /// </summary>
    public class PrepareWindow : BaseBattleWindow {

        /// <summary>
        /// 半常量定义
        /// </summary>
        public const int PrepareTime = 30; // 准备时间（秒）

        /// <summary>
        /// 文本常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ItemTabController tabController;

        public BattlerPrepareStoryboard selfPStatus;//, oppoPStatus;

        public BattleItemSlotDisplay battleItemSlotDisplay;

        public GameObject prepareControl;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            configureContainers();
        }

        /// <summary>
        /// 配置容器
        /// </summary>
        void configureContainers() {
            var containerDisplays = new IPrepareContainerDisplay[] {
                battleItemSlotDisplay, /* quesSugarPackDisplay */};
            tabController.configure(containerDisplays);
        }

        #endregion

        #region 启动/结束窗口

        /// <summary>
        /// 启动窗口
        /// </summary>
        public override void startWindow() {
            base.startWindow();
            prepareControl.SetActive(true);
            selfStatus.startView();
            oppoStatus.startView();
        }

        /// <summary>
        /// 结束窗口
        /// </summary>
        public override void terminateWindow() {
            base.terminateWindow();
            prepareControl.SetActive(false);
            battleClock.terminateView();
            selfStatus.terminateView();
            oppoStatus.terminateView();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取自身分镜
        /// </summary>
        /// <returns>返回自身分镜</returns>
        protected override BattlerPrepareStoryboard selfStoryboard() {
            return selfPStatus;
        }

        /// <summary>
        /// 获取对方分镜
        /// </summary>
        /// <returns>返回对方分镜</returns>
        protected override BattlerPrepareStoryboard oppoStoryboard() {
            return null;
        }

        /// <summary>
        /// 等待秒数
        /// </summary>
        /// <returns></returns>
        protected override int waitSeconds() {
            return PrepareTime;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 刷新容器
        /// </summary>
        void refreshContainers() {
            var battleItems = battle.self().battleItems;
            battleItemSlotDisplay.setItems(battleItems);
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshContainers();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            battleItemSlotDisplay.clearItems();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 跳过
        /// </summary>
        public override void pass() {
            base.pass();
            battleSer.prepareComplete(onPrepareCompleted);
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void confirm() {
            passed = true;
            battleSer.prepareComplete(
                tabController.itemToUse(), onPrepareCompleted);
        }

        #endregion

        #region 回调控制

        /// <summary>
        /// 准备完成回调
        /// </summary>
        void onPrepareCompleted() {
            showSelfStoryboard();
            prepareControl.SetActive(false);
            battleClock.stopTimer();
        }

        #endregion

    }
}