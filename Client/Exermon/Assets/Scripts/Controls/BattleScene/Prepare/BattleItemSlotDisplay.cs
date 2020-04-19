using ItemModule.Data;
using BattleModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.Prepare {
    
    /// <summary>
    /// 对战物资槽显示
    /// </summary>
    public class BattleItemSlotDisplay : 
        SelectableContainerDisplay<RuntimeBattleItem>, IPrepareContainerDisplay {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BattleItemDetail detail; // 帮助界面

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            registerUpdateLayout(container);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        protected override IItemDetailDisplay<RuntimeBattleItem> getItemDetail() {
            return detail;
        }

        /// <summary>
        /// 即将使用的物品
        /// </summary>
        /// <returns></returns>
        public BaseContItem itemToUse() {
            if (selectedItem() == null) return null;
            return selectedItem().battleItemSlotItem();
        }

        #endregion
    }
}