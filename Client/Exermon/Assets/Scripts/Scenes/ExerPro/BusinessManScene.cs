
using Core.Systems;
using Core.UI;

using UI.ExerPro.BusinessManScene.Windows;

/// <summary>
/// 背包场景
/// </summary>
namespace UI.ExerPro.BusinessManScene
{

    /// <summary>
    /// 商店场景
    /// </summary>
    public class BusinessManScene : BaseScene
    {

        /// <summary>
        /// 文本定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BusinessManWindow shopWindow;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName()
        {
            return SceneSystem.Scene.BusinessManScene;
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
            shopWindow.startWindow();
        }

        #endregion

    }
}