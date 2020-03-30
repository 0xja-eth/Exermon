
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using PlayerModule.Services;
using SeasonModule.Services;
using BattleModule.Services;

using UI.BattleStartScene.Controls;
using UI.BattleStartScene.Windows;

/// <summary>
/// 对战开始场景
/// </summary>
namespace UI.BattleStartScene {

    /// <summary>
    /// 对战开始场景
    /// </summary>
    public class BattleStartScene : BaseScene {

        /// <summary>
        /// 文本定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public TopInfoDisplay topInfoDisplay;
        public MainWindow mainWindow;
        public RightWindow rightWindow;

        /// <summary>
        /// 按钮回调
        /// </summary>

        /// <summary>
        /// 内部系统声明
        /// </summary>
        PlayerService playerSer;
        BattleService battleSer;
        SeasonService seasonSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName() {
            return SceneUtils.GameScene.BattleStartScene;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
            battleSer = BattleService.get();
            seasonSer = SeasonService.get();
        }

        /// <summary>
        /// 初始化其他
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            SceneUtils.depositSceneObject("Scene", this);
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            refresh();
        }

        #endregion

        #region 请求项控制
        

        #endregion

        #region 场景控制

        /// <summary>
        /// 刷新场景
        /// </summary>
        public void refresh() {
            playerSer.getPlayerBattle(
                () => seasonSer.getCurrentSeasonRank(onSuccess: startWindows)
            );
        }

        /// <summary>
        /// 启动窗口
        /// </summary>
        void startWindows() {
            topInfoDisplay.startView(playerSer.player);
            mainWindow.startWindow();
            rightWindow.startWindow();
        }

        #endregion

        #region 流程控制
        

        #endregion
    }
}