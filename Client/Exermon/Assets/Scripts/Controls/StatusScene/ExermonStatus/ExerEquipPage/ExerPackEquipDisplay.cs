
using UnityEngine;
using UnityEngine.UI;

using ItemModule.Data;
using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExerEquipPage {

    /// <summary>
    /// 装备背包显示
    /// </summary
    public class ExerPackEquipDisplay : SelectableItemDisplay<PackContItem> {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon; // 图片
        public Text count;

        public GameObject equipedFlag;

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new PackContainerDisplay getContainer() {
            return base.getContainer() as PackContainerDisplay;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 是否为空物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool isNullItem(PackContItem item) {
            return base.isNullItem(item) || item.isNullItem() ||
                item.type != (int)BaseContItem.Type.ExerPackEquip;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制物品
        /// </summary>
        protected override void drawExactlyItem(PackContItem contItem) {
            base.drawExactlyItem(contItem);
            drawExerPackEquip((ExerPackEquip)contItem);
        }

        /// <summary>
        /// 绘制艾瑟萌背包装备
        /// </summary>
        /// <param name="packEquip">艾瑟萌背包装备</param>
        void drawExerPackEquip(ExerPackEquip packEquip) {
            var equip = packEquip.equip();
            icon.gameObject.SetActive(true);
            icon.overrideSprite = equip.icon;

            if (count) count.text = packEquip.count > 1 ?
                    packEquip.count.ToString() : "";
            equipedFlag?.SetActive(packEquip.equiped);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            if (count) count.text = "";
            icon.overrideSprite = null;
            icon.gameObject.SetActive(false);
            equipedFlag?.SetActive(false);
        }

        #endregion

        #region 事件控制

        #endregion
    }
}