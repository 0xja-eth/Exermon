
using ItemModule.Data;
using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

namespace UI.StatusScene.Controls.ExermonStatus {
    
    /// <summary>
    /// 状态窗口艾瑟萌槽信息显示
    /// </summary>
    public class ExermonStatusSlotItemDisplay<T> : 
        SlotContItemDisplay<ExerSlotItem, T>
        where T : PackContItem, new() {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public MultParamsDisplay expBar;

        /// <summary>
        /// 详情显示组件
        /// </summary>
        protected virtual ExermonStatusExerSlotDetail<T> detail { get; set; }

        /// <summary>
        /// 页显示组件
        /// </summary>
        protected ExermonStatusPageDisplay<T> pageDisplay;

        #region 数据控制

        /// <summary>
        /// 设置页显示组件
        /// </summary>
        /// <param name="pageDisplay">页显示组件</param>
        public void setPageDisplay(ExermonStatusPageDisplay<T> pageDisplay) {
            this.pageDisplay = pageDisplay;
        }

        /// <summary>
        /// 配置装备
        /// </summary>
        protected override void setupExactlyEquip() {
            base.setupExactlyEquip();
            equip = item.getEquip<T>();
            // equip = (item == null ? equip : item.getEquip<T>());
        }

        /// <summary>
        /// 配置空装备
        /// </summary>
        protected override void setupEmptyEquip() {
            lastEquip = null;
        }

        /// <summary>
        /// 装备变更回调
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();
            detail.setSlotItem(item);
        }
            
        /// <summary>
        /// 预览变更回调
        /// </summary>
        protected override void onPreviewChanged() {
            base.onPreviewChanged();
            detail.setItem(previewingEquip);
        }

        /// <summary>
        /// 获取背包显示组件
        /// </summary>
        /// <returns></returns>
        public override PackContainerDisplay<T> getPackDisplay() {
            return pageDisplay.getPackDisplay();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(ExerSlotItem item) {
            base.drawExactlyItem(item);
            expBar?.setValue(item, "slot_exp");
        }

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        protected override void drawExactlyEquip(T equipItem) {
            base.drawExactlyEquip(equipItem);
            detail.setItem(equipItem);
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            expBar?.clearValue();
        }

        #endregion

    }
}