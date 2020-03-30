
using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExermonPage {

    /// <summary>
    /// 艾瑟萌仓库显示
    /// </summary>
    public class PackContainerDisplay : PackContainerDisplay<PlayerExermon> {

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
        int subjectId = 0;

        #region 数据控制

        /// <summary>
        /// 设置装备类型
        /// </summary>
        /// <param name="eType">装备类型</param>
        public void setSubjectId(int subjectId) {
            this.subjectId = subjectId;
            refreshItems();
        }

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        protected override IItemDetailDisplay<PlayerExermon> getItemDetail() {
            return detail;
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="playerExer">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isIncluded(PlayerExermon playerExer) {
            if (!base.isIncluded(playerExer)) return false;
            return playerExer.exermon().subjectId == subjectId;
        }

        #endregion
    }
}