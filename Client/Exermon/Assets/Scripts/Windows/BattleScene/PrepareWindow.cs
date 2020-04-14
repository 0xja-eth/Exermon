using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using PlayerModule.Services;

using BattleModule.Data;
using BattleModule.Services;

using UI.BattleScene.Controls;
using UI.BattleScene.Controls.Prepare;
using UI.BattleScene.Controls.Waiting;

/// <summary>
/// 对战开始场景窗口
/// </summary>
namespace UI.BattleScene.Windows {

    /// <summary>
    /// 准备窗口
    /// </summary>
    public class PrepareWindow : BaseWindow {

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
        public QuestionInfo questionInfo;
        public ItemTabController tabController;
        public BattleClock battleClock;

        public BattlerStatus selfStatus, oppoStatus;

        public BattlerPrepareStatus selfPStatus;//, oppoPStatus;

        public BattleItemSlotDisplay battleItemSlotDisplay;

        public GameObject prepareControl;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        RuntimeBattle battle;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        BattleScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        DataService dataSer = null;
        PlayerService playerSer = null;
        BattleService battleSer = null;

        #region 初始化

        /// <summary>
        /// 初次初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            scene = (BattleScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            dataSer = DataService.get();
            playerSer = PlayerService.get();
            battleSer = BattleService.get();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateBattleClock();
        }

        /// <summary>
        /// 更新准备时间
        /// </summary>
        void updateBattleClock() {
            if (battleClock.isTimeUp()) pass();
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

        #endregion

        #region 界面控制

        /// <summary>
        /// 显示上个玩家的答题状态
        /// </summary>
        void showPStatus(RuntimeBattlePlayer battler) {
            if (battler == battle.self()) showSelfPStatus();
            //if (battler == battle.oppo()) showOppoPStatus();
        }

        /// <summary>
        /// 显示自身答题状态
        /// </summary>
        void showSelfPStatus(RuntimeBattlePlayer battler = null) {
            if (battler == null) battler = battle.self();
            selfPStatus.startView(battler);
        }

        ///// <summary>
        ///// 显示对方答题状态
        ///// </summary>
        //void showOppoPStatus(RuntimeBattlePlayer battler = null) {
        //    if (battler == null) battler = battle.oppo();
        //    oppoPStatus.startView(battler);
        //}

        /// <summary>
        /// 清空答题状态
        /// </summary>
        void showPStatuses() {
            showSelfPStatus();
            //showOppoPStatus();
        }

        /// <summary>
        /// 清空答题状态
        /// </summary>
        void clearPStatuses() {
            selfPStatus.terminateView();
            //oppoPStatus.terminateView();
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            battle = battleSer.battle;

            var containerDisplays = new IPrepareContainerDisplay[] {
                battleItemSlotDisplay, /* quesSugarPackDisplay */};
            tabController.configure(containerDisplays);

            questionInfo.setItem(battle.round, true);

            battleClock.startView(PrepareTime);

            selfStatus.setItem(battle.self(), true);
            oppoStatus.setItem(battle.oppo(), true);

            var battleItems = battle.self().battleItems;
            battleItemSlotDisplay.setItems(battleItems);
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            questionInfo.requestClear(true);
            battleClock.requestClear(true);

            selfStatus.requestClear(true);
            oppoStatus.requestClear(true);

            battleItemSlotDisplay.clearItems();

            clearPStatuses();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 跳过
        /// </summary>
        public void pass() {
            battleSer.prepareComplete(onPrepareCompleted);
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void confirm() {
            battleSer.prepareComplete(
                tabController.itemToUse(), onPrepareCompleted);
        }

        #endregion

        #region 回调控制

        /// <summary>
        /// 准备完成回调
        /// </summary>
        void onPrepareCompleted() {
            showSelfPStatus();
            prepareControl.SetActive(false);
            battleClock.stopTimer();
        }

        #endregion

    }
}