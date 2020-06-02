
using Core.Systems;
using Core.UI;

using PlayerModule.Services;

using UI.PackScene.Windows;

/// <summary>
/// 背包场景
/// </summary>
namespace UI.PackScene {

    /// <summary>
    /// 背包场景
    /// </summary>
    public class PackScene : BaseScene {

        /// <summary>
        /// 文本定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackWindow packWindow;

        /// <summary>
        /// 内部系统声明
        /// </summary>
        PlayerService playerSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.PackScene;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            refresh();
        }

        #endregion
        
        #region 场景控制

        /// <summary>
        /// 刷新场景
        /// </summary>
        public void refresh() {
            playerSer.getPlayerPack(packWindow.startWindow);
        }

        #endregion

    }
}