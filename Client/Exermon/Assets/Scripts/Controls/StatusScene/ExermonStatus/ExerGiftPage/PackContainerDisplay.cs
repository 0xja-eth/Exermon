
using UnityEngine;

using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExerGiftPage {

    /// <summary>
    /// 艾瑟萌天赋池显示
    /// </summary>
    public class PackContainerDisplay : PackContainerDisplay<PlayerExerGift> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PlayerExerGiftDetail detail; // 帮助界面

        /// <summary>
        /// 内部变量声明
        /// </summary>
        PlayerExerGift equipItem = null;

        #region 数据控制

        /// <summary>
        /// 设置装备项
        /// </summary>
        /// <param name="eType">装备类型</param>
        public void setEquipItem(PlayerExerGift equipItem) {
            this.equipItem = equipItem;
            refreshItems();
        }

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        public override IItemDetailDisplay<PlayerExerGift> getItemDetail() {
            return detail;
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="playerExer">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isIncluded(PlayerExerGift playerGift) {
            if (!base.isIncluded(playerGift)) return false;
            return !playerGift.equiped || playerGift == equipItem;
        }

        #endregion
    }
}