
using Core.UI;

using UI.StatusScene.Windows;

namespace UI.StatusScene.Controls.PlayerStatus {

    /// <summary>
    /// 修改按钮控制器
    /// </summary>
    public class EditButtonsController : TabView<InfoEditWindow> {

        /// <summary>
        /// 外部组件设置
        /// </summary>

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(InfoEditWindow content, int index) {
            content.startWindow((InfoEditWindow.Type)index);
        }

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(InfoEditWindow content, int index) {
            content.terminateWindow();
        }

        #endregion

    }
}