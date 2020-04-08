using ItemModule.Data;
using BattleModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.Prepare {
    
    /// <summary>
    /// 对战物资槽显示
    /// </summary>
    public class BattleItemSlotDisplay : 
        ContainerDisplay<RuntimeBattleItem> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BattleItemDetail detail; // 帮助界面

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        protected override IItemDetailDisplay<RuntimeBattleItem> getItemDetail() {
            return detail;
        }

        #endregion
    }
}