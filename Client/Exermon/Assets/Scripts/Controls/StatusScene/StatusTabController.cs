
using Core.UI;

using UI.StatusScene.Windows;

namespace UI.StatusScene.Controls {

    /// <summary>
    /// 状态页控制组
    /// </summary>
    public class StatusTabController : TabView<StatusWindow> {

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(StatusWindow content, int index) {
            content.switchView(index);
        }

        /// <summary>
        /// 隐藏内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(StatusWindow content, int index) {
            content.clearView();
        }

        #endregion

    }

}
