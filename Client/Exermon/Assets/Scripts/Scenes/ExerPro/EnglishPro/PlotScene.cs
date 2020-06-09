
using Core.Systems;
using Core.UI;

using UI.ExerPro.EnglishPro.PlotScene.Windows;
using ExerPro.EnglishModule.Services;

/// <summary>
/// 背包场景
/// </summary>
namespace UI.ExerPro.EnglishPro.PlotScene
{

    /// <summary>
    /// 商店场景
    /// </summary>
    public class PlotScene : BaseScene
    {
        /// <summary>
        /// 外部变量
        /// </summary>
        public PlotWindow plotWindow;

        /// <summary>
        /// 外部系统
        /// </summary>
        EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex()
        {
            return SceneSystem.Scene.EnglishProPlotScene;
        }

        protected override void initializeSystems()
        {
            base.initializeSystems();
            engSer = EnglishService.get();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start()
        {
            base.start();
            refresh();
        }

        #endregion

        #region 场景控制

        /// <summary>
        /// 刷新场景
        /// </summary>
        public void refresh()
        {
            plotWindow.startWindow();
        }

        public override void popScene()
        {
            engSer.exitNode();
        }

        #endregion

    }
}