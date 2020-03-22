
using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExerEquipPage {

    /// <summary>
    /// 状态窗口装备页信息显示
    /// </summary>
    public class ExerSlotItemDisplay : ExermonStatusSlotItemDisplay<ExerPackEquip>, 
        IItemDetail<ExerEquipSlotItem> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ExerPackEquipDetail exerPackEquipDetail;

        /// <summary>
        /// 详情显示组件
        /// </summary>
        protected override ExermonStatusExerSlotDetail<ExerPackEquip> detail {
            get { return exerPackEquipDetail; }
            set { exerPackEquipDetail = (ExerPackEquipDetail)value; }
        }

        #region 接口实现

        /// <summary>
        /// 装备槽容器
        /// </summary>
        ExerEquipSlotDisplay equipSlotDisplay;

        /// <summary>
        /// 装备槽项
        /// </summary>
        ExerEquipSlotItem equipSlotItem;

        /// <summary>
        /// 装备槽索引
        /// </summary>
        int equipIndex = 0;

        #region 初始化

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="container"></param>
        public void configure(ItemContainer<ExerEquipSlotItem> container) {
            equipSlotDisplay = (ExerEquipSlotDisplay)container;
        }

        #endregion

        #region 开启视窗

        /// <summary>
        /// 开启视窗
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        void IItemDetail<ExerEquipSlotItem>.startView(ExerEquipSlotItem item, int index = -1, bool refresh = false) {
            startView(); setItem(item, index, refresh);
        }
        void IItemDisplay<ExerEquipSlotItem>.startView(ExerEquipSlotItem item, bool refresh = false) {
            startView(); setItem(item, refresh);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        public void setItem(ExerEquipSlotItem item, int index = -1, bool refresh = false) {
            if (!refresh && equipSlotItem == item && equipIndex == index) return;
            equipSlotItem = item; equipIndex = index;
            onEquipItemChanged();
        }
        public void setItem(ExerEquipSlotItem item, bool refresh = false) {
            if (!refresh && equipSlotItem == item) return;
            equipSlotItem = item;
            onEquipItemChanged();
        }

        /// <summary>
        /// 装备物品改变回调
        /// </summary>
        void onEquipItemChanged() {
            base.onItemChanged();
            exerPackEquipDetail.setEquipSlotItem(
                equipIndex, equipSlotItem);
            pageDisplay.requestRefresh();
        }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns></returns>
        ExerEquipSlotItem IItemDisplay<ExerEquipSlotItem>.getItem() {
            return equipSlotItem;
        }

        /// <summary>
        /// 获取装备槽项
        /// </summary>
        /// <param name="slotItem"></param>
        public ExerEquipSlotItem getEquipSlotItem() {
            return equipSlotItem;
        }

        #endregion

        #endregion

        #region 数据控制

        /// <summary>
        /// 配置装备
        /// </summary>
        protected override void setupEquip() {
            equip = equipSlotItem == null ? null : equipSlotItem.packEquip;
        }
        /*
        /// <summary>
        /// 装备变更回调
        /// </summary>
        protected override void onItemChanged() {
            updateEquipSlotItem();
            base.onItemChanged();
        }

        /// <summary>
        /// 更新装备槽物品
        /// </summary>
        void updateEquipSlotItem() {
        }
        */
        #endregion
    }
}