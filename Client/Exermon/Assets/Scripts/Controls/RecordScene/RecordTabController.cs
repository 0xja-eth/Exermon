
using Core.UI;

using UI.RecordScene.Windows;

namespace UI.RecordScene.Controls {

    /// <summary>
    /// 状态页控制组
    /// </summary>
    public class RecordTabController : TabView<RecordWindow> {

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(RecordWindow content, int index) {
            content.switchView(index);
        }

        /// <summary>
        /// 隐藏内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(RecordWindow content, int index) {
            content.clearView();
        }

        #endregion

    }

}
