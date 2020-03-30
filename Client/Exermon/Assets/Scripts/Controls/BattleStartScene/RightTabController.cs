
using Core.UI;

using UI.BattleStartScene.Windows;

namespace UI.BattleStartScene.Controls {

    /// <summary>
    /// 状态页控制组
    /// </summary>
    public class RightTabController : TabView<RightWindow> {

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(RightWindow content, int index) {
            content.type = (RightWindow.Type)index;
        }

        /// <summary>
        /// 隐藏内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(RightWindow content, int index) {
            content.type = RightWindow.Type.Normal;
        }

        #endregion

    }

}
