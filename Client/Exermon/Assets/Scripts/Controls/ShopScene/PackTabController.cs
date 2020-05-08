
using Core.UI;

using UI.ShopScene.Windows;

namespace UI.ShopScene.Controls {

    /// <summary>
    /// 状态页控制组
    /// </summary>
    public class PackTabController : TabView<ShopWindow> {

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(ShopWindow content, int index) {
            content.switchView(index);
        }

        /// <summary>
        /// 隐藏内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(ShopWindow content, int index) {
            content.clearView();
        }

        #endregion

    }

}
