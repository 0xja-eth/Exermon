
using UnityEngine.UI;

using Core.UI;

using ItemModule.Data;

using UI.BattleStartScene.Windows;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.Prepare {

    /// <summary>
    /// 状态页控制组
    /// </summary>
    public class ItemTabController : 
        TabView<IContainerDisplay> {

        /// <summary>
        /// 常量定义
        /// </summary>
        static readonly string[] TypeName =
            new string[] { "人类物品", "题目糖" };

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text type; 

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(
            IContainerDisplay content, int index) {
            content.startView(0);
        }

        /// <summary>
        /// 隐藏内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(
            IContainerDisplay content, int index) {
            content.terminateView();
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            type.text = TypeName[getIndex()];
        }

        /// <summary>
        /// 清除视窗
        /// </summary>
        protected override void clear() {
            base.clear();
            type.text = "";
        }

        #endregion
    }
}
