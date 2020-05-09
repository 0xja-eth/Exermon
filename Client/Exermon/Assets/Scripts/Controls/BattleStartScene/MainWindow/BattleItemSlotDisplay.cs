using ItemModule.Data;
using BattleModule.Data;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 对战准备场景
/// </summary>
namespace UI.BattleStartScene { }

/// <summary>
/// 对战准备场景控件
/// </summary>
namespace UI.BattleStartScene.Controls { }

/// <summary>
/// 主窗口控件
/// </summary>
namespace UI.BattleStartScene.Controls.Main {

    using Right.ItemContent;

    /// <summary>
    /// 对战物资槽显示
    /// </summary>
    public class BattleItemSlotDisplay : SlotContainerDisplay<BattleItemSlotItem, PackContItem> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackItemDetail detail; // 帮助界面

        public ItemPackDisplay packDisplay; // 背包显示

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        protected override IItemDetailDisplay<BattleItemSlotItem> getItemDetail() {
            return detail;
        }

        /// <summary>
        /// 获取背包容器
        /// </summary>
        /// <returns></returns>
        public override PackContainerDisplay<PackContItem> getPackDisplay() {
            return packDisplay;
        }

        #endregion
    }
}