
using Core.UI;

using UI.ExerPro.BusinessManScene.Windows;

namespace UI.ExerPro.BusinessManScene.Controls
{

    /// <summary>
    /// 状态页控制组
    /// </summary>
    public class PackTabController : TabView<BusinessManWindow>
    {

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(BusinessManWindow content, int index)
        {
            content.switchView(index);
        }

        /// <summary>
        /// 隐藏内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(BusinessManWindow content, int index)
        {
            content.clearView();
        }

        #endregion

    }

}
