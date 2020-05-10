
using UnityEngine;
using UnityEngine.UI;

using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExerEquipPage {

    /// <summary>
    /// 装备背包显示
    /// </summary
    public class ExerEquipSlotItemDisplay : SelectableItemDisplay<ExerEquipSlotItem> {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon; // 图片
        public Text name, type;

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new ExerEquipSlotDisplay getContainer() {
            return base.getContainer() as ExerEquipSlotDisplay;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制物品
        /// </summary>
        protected override void drawExactlyItem(ExerEquipSlotItem slotItem) {
            if (type) type.text = slotItem.equipType().name;
            if (slotItem.isNullItem()) clearPackEquip();
            else {
                var equip = slotItem.equip();
                icon.gameObject.SetActive(true);
                icon.overrideSprite = equip.icon;

                if (name) name.text = equip.name;
            }
        }

        /// <summary>
        /// 清除背包装备绘制信息
        /// </summary>
        void clearPackEquip() {
            if (name) name.text = "";
            icon.overrideSprite = null;
            icon.gameObject.SetActive(false);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            if (type) type.text = "";
            clearPackEquip();
        }

        #endregion

        #region 事件控制

        #endregion
    }
}