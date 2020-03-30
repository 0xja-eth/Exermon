
using UnityEngine;

using PlayerModule.Data;
using ItemModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleStartScene.Controls.Right.ItemContent {

    /// <summary>
    /// 物品背包显示
    /// </summary>
    public class ItemPackDisplay : HumanPackDisplay {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackItemDetail detail; // 帮助界面

        public RectTransform draggingParent; // 拖拽父结点

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        protected override IItemDetailDisplay<PackContItem> getItemDetail() {
            return detail;
        }
        
        /// <summary>
        /// 可接受的类型列表
        /// </summary>
        /// <returns>返回可接受的物品类型列表</returns>
        protected override BaseContItem.Type[] acceptableTypes() {
            return new BaseContItem.Type[] {
                BaseContItem.Type.HumanPackItem
            };
        }
        
        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="packItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isItemIncluded(HumanPackItem packItem) {
            return !packItem.equiped && packItem.item().battleUse;
        }

        /// <summary>
        /// 是否包含装备
        /// </summary>
        /// <param name="packEquip">装备</param>
        /// <returns>返回指定装备能否包含在容器中</returns>
        protected override bool isEquipIncluded(HumanPackEquip packEquip) {
            return false;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// ItemDisplay 创建回调
        /// </summary>
        /// <param name="item">ItemDisplay</param>
        protected override void onSubViewCreated(
            SelectableItemDisplay<PackContItem> item, int index) {
            base.onSubViewCreated(item, index);
            var giftCard = item as PackItemDisplay;
            giftCard.draggingParent = draggingParent;
        }

        #endregion
    }
}