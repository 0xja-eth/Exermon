
using Core.Systems;
using Core.UI;

using RecordModule.Services;

using UI.RecordScene.Windows;

/// <summary>
/// 记录场景
/// </summary>
namespace UI.RecordScene {

    /// <summary>
    /// 状态场景
    /// </summary>
    public class RecordScene : BaseScene {

        /// <summary>
        /// 文本定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public RecordWindow recordWindow;

        /// <summary>
        /// 内部系统声明
        /// </summary>
        RecordService recordSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName() {
            return SceneSystem.Scene.RecordScene;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            recordSer = RecordService.get();
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
            recordSer.get(recordWindow.startWindow);
        }

        #endregion

    }
}