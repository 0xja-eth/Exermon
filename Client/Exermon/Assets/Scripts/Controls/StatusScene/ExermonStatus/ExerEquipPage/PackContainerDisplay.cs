using ItemModule.Data;
using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExerEquipPage {

    /// <summary>
    /// 艾瑟萌天赋池显示
    /// </summary>
    public class PackContainerDisplay : ExerPackDisplay {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ExerPackEquipDetail detail; // 帮助界面

        /// <summary>
        /// 内部变量声明
        /// </summary>
        int eType = 0;
        ExerPackEquip equipItem = null;

        #region 数据控制

        /// <summary>
        /// 设置装备类型
        /// </summary>
        /// <param name="eType">装备类型</param>
        public void setEquipType(int eType) {
            this.eType = eType;
            refreshItems();
        }

        /// <summary>
        /// 设置装备项
        /// </summary>
        /// <param name="eType">装备类型</param>
        public void setEquipItem(ExerPackEquip equipItem) {
            this.equipItem = equipItem;
            refreshItems();
        }

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        public override IItemDetailDisplay<PackContItem> getItemDetail() {
            return detail;
        }

        /// <summary>
        /// 可接受的类型列表
        /// </summary>
        /// <returns>返回可接受的物品类型列表</returns>
        protected override BaseContItem.Type[] acceptableTypes() {
            return new BaseContItem.Type[] {
                BaseContItem.Type.ExerPackEquip
            };
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="packItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isItemIncluded(ExerPackItem packItem) {
            return false;
        }

        /// <summary>
        /// 是否包含装备
        /// </summary>
        /// <param name="packEquip">装备</param>
        /// <returns>返回指定装备能否包含在容器中</returns>
        protected override bool isEquipIncluded(ExerPackEquip packEquip) {
            return packEquip.equip().eType == eType && (
                !packEquip.equiped || equipItem == packEquip);
        }

        #endregion
    }
}