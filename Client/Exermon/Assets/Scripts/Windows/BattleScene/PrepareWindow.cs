using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using PlayerModule.Data;

using GameModule.Services;

using PlayerModule.Services;
using BattleModule.Services;

using UI.Common.Controls.ParamDisplays;

using UI.BattleScene.Controls;
using UI.BattleScene.Controls.Prepare;

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
        public BattleItemSlotDisplay battleItemSlotDisplay;

        /// <summary>
        /// 内部变量声明
        /// </summary>

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

        }

        /// <summary>
        /// 更新准备时间
        /// </summary>
        void updateBattleClock() {
            if (battleClock.isTimeUp()) pass();
        }

        #endregion

        #region 数据控制

        #endregion

        #region 界面控制

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            var battle = battleSer.battle;
            questionInfo.setItem(battle.round);
            battleClock.startTimer(PrepareTime);

            selfStatus.setItem(battle.self());
            oppoStatus.setItem(battle.oppo());

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
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 跳过
        /// </summary>
        public void pass() {

        }

        /// <summary>
        /// 确认
        /// </summary>
        public void confirm() {

        }

        #endregion

    }
}