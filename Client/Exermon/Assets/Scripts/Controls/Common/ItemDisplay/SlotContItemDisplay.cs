
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Core.Data.Loaders;

using Core.UI.Utils;

using ItemModule.Data;
using ItemModule.Services;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 槽容器项展示组件
    /// </summary>
    /// <typeparam name="T">物品类型</typeparam>
    /// <typeparam name="E">装备类型</typeparam>
    public class SlotContItemDisplay<T, E> : SlotItemDisplay<T, E>
        where T : SlotContItem, new() where E : PackContItem, new() {

        /// <summary>
        /// 内部变量声明
        /// </summary>
        SlotContainer<T> slotData;

        int slotIndex;

        /// <summary>
        /// 内部系统定义
        /// </summary>
        ItemService itemSer;

        #region 初始化

        /// <summary>
        /// 初始化系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            itemSer = ItemService.get();
        }

        /// <summary>
        /// 配置
        /// </summary>
        public virtual void configure(ContainerDisplay<T> container, int index,
            SlotContainer<T> slotContainer, int slotIndex) {
            base.configure(container, index);
            setSlotData(slotContainer, slotIndex);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置容器
        /// </summary>
        public void setSlotData(SlotContainer<T> slotData, 
            int slotIndex, bool refresh = false) {
            this.slotData = slotData;
            setSlotIndex(slotIndex, refresh);
        }

        /// <summary>
        /// 设置槽索引
        /// </summary>
        /// <param name="slotIndex">槽索引</param>
        public void setSlotIndex(int slotIndex, bool refresh = false) {
            this.slotIndex = slotIndex;
            refreshSlotItem(refresh);
        }
        /*
        /// <summary>
        /// 设置槽项
        /// </summary>
        /// <param name="slotItem">槽项</param>
        public void setSlotItemData(SlotContainer<T> slotData, 
            T slotItem, bool refresh = false) {
            if (slotItem == null) setSlotIndex(slotData, 0, refresh);
            else setSlotIndex(slotData, slotItem.index, refresh);
        }
        */
        /// <summary>
        /// 获取槽容器
        /// </summary>
        /// <returns>返回对应的容器</returns>
        public SlotContainer<T> getSlotData() {
            var slotDisplay = getSlotDisplay();
            if (slotDisplay != null) return slotDisplay.getSlotData();
            return slotData;
        }

        /// <summary>
        /// 获取槽项
        /// </summary>
        /// <returns>返回槽项</returns>
        public T getSlotItemData() {
            return getSlotData().getSlotItem(slotIndex);
        }

        /// <summary>
        /// 获取槽索引
        /// </summary>
        /// <returns>返回对应的容器</returns>
        public int getSlotIndex() {
            return slotIndex;
        }

        /// <summary>
        /// 获取背包容器显示组件
        /// </summary>
        /// <returns>返回背包容器显示组件</returns>
        public virtual PackContainerDisplay<E> getPackDisplay() {
            var slotDisplay = getSlotDisplay();
            if (slotDisplay == null) return null;
            return slotDisplay.getPackDisplay();
        }
        public PackContainerDisplay<E> getPackDisplay(
            ContainerDisplay<E> container) {
            if (container == null) return getPackDisplay();
            return DataLoader.cast<PackContainerDisplay<E>>(container);
        }

        /// <summary>
        /// 获取槽容器显示组件（直接查找其 container）
        /// </summary>
        /// <returns>返回槽容器显示组件</returns>
        public virtual SlotContainerDisplay<T, E> getSlotDisplay() {
            var container = this.container;
            return DataLoader.cast<SlotContainerDisplay<T, E>>(container);
        }
        public SlotContainerDisplay<T, E> getSlotDisplay(
            ContainerDisplay<T> container) {
            if (container == null) return getSlotDisplay();
            return DataLoader.cast<SlotContainerDisplay<T, E>>(container);
        }

        /// <summary>
        /// 能否装备
        /// </summary>
        /// <param name="item">装备项</param>
        /// <returns></returns>
        public override bool isEquippable(E item) {
            return equipChangedRequestFunc(item) != null;
        }

        /// <summary>
        /// 装备
        /// </summary>
        /// <param name="equipItem">装备项</param>
        public override void setEquip(E equipItem, bool force = false) {
            if (!force && (!isEquippable(equipItem) || equip == equipItem)) return;

            var action = equipChangedRequestFunc(equipItem);
            // force 为 true 时不执行网络请求
            if (force || action == null) base.setEquip(equipItem, force);
            // 如果有 action 的话，即需要执行装备/卸下的网络请求
            else if (action != null) {
                // 查看默认的 packDisplay 是否可用
                var packContainer = getPackDisplay();
                // 如果可用，调用对应的装备函数
                if (packContainer != null) setEquip(packContainer, equipItem);
                // 否则将会自动更新 slotContainer 的内容，故直接 refreshSlotItem 即可
                else action.Invoke(() => refreshSlotItem(true));
            }
        }
        /// <param name="container">容器</param>
        public override void setEquip(ContainerDisplay<E> container, E equipItem) {
            if (!isEquippable(equipItem)) return;
            // 判断传入的 container 是否为 packContainer
            var packContainer = getPackDisplay(container);
            // 如果可用，调用对应的装备函数
            if (packContainer != null) setEquip(packContainer, equipItem);
            else setEquip(equipItem);
        }
        public void setEquip(PackContainerDisplay<E> container, E equipItem) {
            if (!isEquippable(equipItem)) return;
            if (container == null) setEquip(equipItem);
            else {
                var action = equipChangedRequestFunc(equipItem);
                if (action == null) base.setEquip(equipItem);
                else action.Invoke(() => onEquipChanged(container, equipItem));
            }
        }

        /// <summary>
        /// 装备改变回调
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="equipItem">装备项</param>
        protected virtual void onEquipChanged(PackContainerDisplay<E> container, E equipItem) {
            // 如果有对应的 slotDisplay 则更新其
            var slotDisplay = getSlotDisplay();
            Debug.Log("onEquipChanged: " + slotDisplay);
            if(slotDisplay != null) slotDisplay.refreshItems();
            else refreshSlotItem(true);

            // 如果可见则执行更新
            if (container.gameObject.activeInHierarchy)
                container.refreshItems();
        }

        /// <summary>
        /// 刷新槽项
        /// </summary>
        void refreshSlotItem(bool refresh = false) {
            if (getSlotData() != null)
                setItem(getSlotItemData(), refresh);
            else setItem(null);
        }

        #endregion

        #region 请求控制

        /// <summary>
        /// 装备改变请求函数
        /// </summary>
        /// <param name="item">装备项</param>
        /// <returns>返回装备变更时进行的请求函数</returns>
        protected virtual UnityAction<UnityAction> equipChangedRequestFunc(E item) {
            /*
            var slotDisplay = getSlotDisplay();
            var action = slotDisplay?.equipChangedRequestFunc(item, slotIndex);
            if (action != null) return action;
            */
            if (item == null) return dequipRequestFunc();
            return equipRequestFunc(item);
        }

        /// <summary>
        /// 装备请求函数
        /// </summary>
        /// <returns>返回装备时进行的请求函数</returns>
        protected virtual UnityAction<UnityAction> equipRequestFunc(E item) {
            return null;
        }

        /// <summary>
        /// 卸下装备请求函数
        /// </summary>
        /// <returns>返回卸下时进行的请求函数</returns>
        protected virtual UnityAction<UnityAction> dequipRequestFunc() {
            return null;
        }

        #endregion

    }
}