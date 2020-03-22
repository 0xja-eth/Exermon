
using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExermonPage {

    /// <summary>
    /// 艾瑟萌仓库显示
    /// </summary>
    public class PackContainerDisplay : ItemContainer<PlayerExermon> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PlayerExermonDetail detail; // 帮助界面

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        protected override IItemDetail<PlayerExermon> getItemDetail() {
            return detail;
        }

        #endregion
    }
}