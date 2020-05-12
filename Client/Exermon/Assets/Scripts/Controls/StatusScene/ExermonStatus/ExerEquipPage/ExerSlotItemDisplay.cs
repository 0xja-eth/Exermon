
using UnityEngine.Events;

using Core.Data.Loaders;

using ItemModule.Data;
using ExermonModule.Data;
using ExermonModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExerEquipPage {

    /// <summary>
    /// 状态窗口装备页信息显示
    /// </summary>
    public class ExerSlotItemDisplay : ExermonStatusSlotItemDisplay<PackContItem>,
        IItemDetailDisplay<ExerEquipSlotItem> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ExerPackEquipDetail exerPackEquipDetail;

        public ExerEquipSlotDisplay slotDisplay;

        /// <summary>
        /// 详情显示组件
        /// </summary>
        protected override ExermonStatusExerSlotDetail<PackContItem> detail {
            get { return exerPackEquipDetail; }
            set { exerPackEquipDetail = (ExerPackEquipDetail)value; }
        }

        /// <summary>
        /// 外部系统设置
        /// </summary>
        ExermonService exermonSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            exermonSer = ExermonService.get();
        }

        #endregion

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
        public void configure(IContainerDisplay<ExerEquipSlotItem> container) {
            equipSlotDisplay = (ExerEquipSlotDisplay)container;
        }

        #endregion

        #region 开启视窗

        /// <summary>
        /// 开启视窗
        /// </summary>
        /// <param name="item">容器项</param>
        /// <param name="index">容器项索引</param>
        /// <param name="refresh">是否刷新</param>
        void IItemDetailDisplay<ExerEquipSlotItem>.startView(
            ExerEquipSlotItem item, int index = -1) {
            startView(); setItem(item, index, true);
        }
        void IItemDisplay<ExerEquipSlotItem>.startView(
            ExerEquipSlotItem item) {
            startView(); setItem(item, true);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">容器项</param>
        /// <param name="index">容器项索引</param>
        /// <param name="refresh">是否刷新</param>
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
        public ExerEquipSlotItem getEquipSlotItem() {
            return equipSlotItem;
        }

        /// <summary>
        /// 获取装备槽索引
        /// </summary>
        public int getEquipIndex() {
            return equipIndex;
        }

        #endregion

        #endregion

        #region 数据控制
        
        /// <summary>
        /// 配置装备
        /// </summary>
        protected override void setupExactlyEquip() {
            base.setupExactlyEquip();
            equip = equipSlotItem == null ? null : equipSlotItem.packEquip;
        }
        /*
        /// <summary>
        /// 获取背包容器显示组件
        /// </summary>
        /// <returns>返回背包容器显示组件</returns>
        public override PackContainerDisplay<PackContItem> getPackDisplay() {
            return slotDisplay.getPackDisplay();
        }
        */
        /// <summary>
        /// 装备改变回调
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="equipItem">装备项</param>
        protected override void onEquipChanged(PackContainerDisplay<PackContItem> container, PackContItem equipItem) {
            base.onEquipChanged(container, equipItem);
            slotDisplay.refreshItems();
            slotDisplay.select(0);
        }

        #endregion

        #region 请求控制

        /// <summary>
        /// 装备请求函数
        /// </summary>
        /// <param name="item">装备项</param>
        /// <returns>返回装备时进行的请求函数</returns>
        protected override UnityAction<UnityAction> equipRequestFunc(PackContItem item) {
            if (item.type != (int)BaseContItem.Type.ExerPackEquip) return null;
            return action => exermonSer.equipExerEquip(
                this.item.exerEquipSlot, (ExerPackEquip)item, action);
        }

        /// <summary>
        /// 卸下装备请求函数
        /// </summary>
        /// <returns>返回卸下时进行的请求函数</returns>
        protected override UnityAction<UnityAction> dequipRequestFunc() {
            return action => exermonSer.dequipExerEquip(
                item.exerEquipSlot, equipSlotItem.eType, action);
        }

        #endregion
    }
}