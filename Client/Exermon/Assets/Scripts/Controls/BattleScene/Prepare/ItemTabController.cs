
using UnityEngine.UI;

using Core.UI;

using ItemModule.Data;

using UI.BattleStartScene.Windows;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.Prepare {
    
    /// <summary>
    /// 对战准备容器显示接口
    /// </summary>
    public interface IPrepareContainerDisplay : IContainerDisplay {

        /// <summary>
        /// 将要使用的物品
        /// </summary>
        /// <returns></returns>
        BaseContItem itemToUse();

    }

    /// <summary>
    /// 状态页控制组
    /// </summary>
    public class ItemTabController : 
        TabView<IPrepareContainerDisplay> {

        /// <summary>
        /// 常量定义
        /// </summary>
        static readonly string[] TypeName =
            new string[] { "人类物品", "题目糖" };
        static readonly string[] TitleText =
            new string[] { "选择物资", "选择题目糖" };

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text type, title; 

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(
            IPrepareContainerDisplay content, int index) {
            content.startView(0);
        }

        /// <summary>
        /// 隐藏内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(
            IPrepareContainerDisplay content, int index) {
            content.terminateView();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取使用的物品
        /// </summary>
        public BaseContItem itemToUse() {
            return currentContent().itemToUse();
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            type.text = TypeName[getIndex()];
            title.text = TitleText[getIndex()];
        }

        /// <summary>
        /// 清除视窗
        /// </summary>
        protected override void clear() {
            base.clear();
            type.text = title.text = "";
        }

        #endregion
    }
}
