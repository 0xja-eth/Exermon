
using UnityEngine;
using UnityEngine.Events;

using Core.Data.Loaders;

using PlayerModule.Data;
using ItemModule.Data;

using ItemModule.Services;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 槽容器显示控件
    /// </summary>
    /// <remarks>
    /// 专门用于显示 SlotContainer 的组件
    /// </remarks>
    public class SlotContainerDisplay<T, E> : DroppableContainerDisplay<T, E> 
        where T : SlotContItem, new() where E : PackContItem, new() {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 内部变量声明
        /// </summary>
        //PackContainer<E> packData;
        SlotContainer<T> slotData;

        #region 初始化

        /// <summary>
        /// 配置
        /// </summary>
        public void configure(SlotContainer<T> slotData) {
            base.configure();
            setSlotData(slotData);
        }

        #endregion

        #region 数据控制

        #region 数据源操作
        
        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns>返回对应的容器</returns>
        public virtual PackContainerDisplay<E> getPackDisplay() {
            return null;
        }
        public PackContainerDisplay<E> getPackDisplay(
            ContainerDisplay<E> container) {
            if (container == null) return getPackDisplay();
            return DataLoader.cast<PackContainerDisplay<E>>(container);
        }


        /// <summary>
        /// 设置容器
        /// </summary>
        /// <param name="slotData">容器</param>
        public void setSlotData(SlotContainer<T> slotData) {
            this.slotData = slotData;
            refreshItems();
        }

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns>返回对应的容器</returns>
        public SlotContainer<T> getSlotData() {
            return slotData;
        }

        /// <summary>
        /// 获取槽项数据
        /// </summary>
        /// <param name="slotIndex">槽索引</param>
        /// <returns>返回对应索引的<装备槽项/returns>
        public T getSlotItemData(int slotIndex) {
            var slotData = getSlotData();
            if (slotData == null) return null;
            return slotData.getSlotItem(slotIndex);
        }

        /// <summary>
        /// 通过下标获取槽项数据
        /// </summary>
        /// <param name="slotIndex">下标</param>
        /// <returns>返回对应索引的装备槽项</returns>
        public T getSlotItemDataByIndex(int index) {
            var slotData = getSlotData();
            if (slotData == null) return null;
            return slotData.items[index];
        }
        /*
        /// <summary>
        /// 设置容器数据源
        /// </summary>
        /// <param name="packData">设置容器数据源</param>
        public void setPackData(PackContainer<E> packData) {
            var packDisplay = getPackDisplay();
            if (packDisplay == null) this.packData = packData;
            packDisplay.setPackData(packData);
        }

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns>返回对应的容器</returns>
        public PackContainer<E> getPackData() {
            var packDisplay = getPackDisplay();
            if (packDisplay == null) return packData;
            return packDisplay.getPackData();
        }
        */
        /// <summary>
        /// 获取槽项数据
        /// </summary>
        /// <param name="slotIndex">槽索引</param>
        /// <returns>返回对应索引的<装备槽项/returns>
        public SlotContItemDisplay<T, E> getSlotItemDisplay(int slotIndex) {
            return (SlotContItemDisplay<T, E>)subViews.Find(
                sub => DataLoader.castPredicate<SlotContItemDisplay<T, E>>(
                    sub, obj => obj.getSlotIndex() == slotIndex)
            );
        }

        /// <summary>
        /// 通过下标获取槽项数据
        /// </summary>
        /// <param name="slotIndex">下标</param>
        /// <returns>返回对应索引的装备槽项</returns>
        public SlotContItemDisplay<T, E> getSlotItemDisplayByIndex(int index) {
            return DataLoader.cast<SlotContItemDisplay<T, E>>(subViews[index]);
        }

        #endregion

        #region 装备槽操作

        /// <summary>
        /// 接受转移
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="item">物品</param>
        public override void acceptTransfer(ContainerDisplay<E> container, E item) {
            setEquip(container, item);
        }

        /// <summary>
        /// 能否装备
        /// </summary>
        /// <param name="item">装备项</param>
        /// <returns></returns>
        public virtual bool isEquippable(E item, int slotIndex) {
            return equipChangedRequestFunc(item, slotIndex) != null;
        }

        /// <summary>
        /// 设置装备
        /// </summary>
        /// <param name="equipItem">装备项</param>
        public void setEquip(E equipItem) {
            var slotData = getSlotData();
            if (slotData == null) return;
            var slotItem = slotData.getSlotItemByEquipItem(equipItem);
            if (slotItem == null) return;
            setEquip(equipItem, slotItem.slotIndex);
        }
        /// <param name="container">背包容器</param>
        public void setEquip(ContainerDisplay<E> container, E equipItem) {
            var slotData = getSlotData();
            if (slotData == null) return;
            var slotItem = slotData.getSlotItemByEquipItem(equipItem);
            setEquip(container, equipItem, slotItem.slotIndex);
        }
        /// <param name="slotIndex">槽索引</param>
        public void setEquip(E equipItem, int slotIndex) {
            // 判断有没有对应的 SlotContItemDisplay 项
            var slotItemDisplay = getSlotItemDisplay(slotIndex);
            if (slotItemDisplay != null) slotItemDisplay.setEquip(equipItem);
            else _setEquip(equipItem, slotIndex);
        }
        public void setEquip(ContainerDisplay<E> container, E equipItem, int slotIndex) {
            // 判断有没有对应的 SlotContItemDisplay 项
            var slotItemDisplay = getSlotItemDisplay(slotIndex);
            if (slotItemDisplay != null)
                slotItemDisplay.setEquip(container, equipItem);
            else _setEquip(container, equipItem, slotIndex);
        }
        public void setEquip(PackContainerDisplay<E> container, E equipItem, int slotIndex) {
            // 判断有没有对应的 SlotContItemDisplay 项
            var slotItemDisplay = getSlotItemDisplay(slotIndex);
            if (slotItemDisplay != null)
                slotItemDisplay.setEquip(container, equipItem);
            else _setEquip(container, equipItem, slotIndex);
        }
        void _setEquip(E equipItem, int slotIndex) {
            if (!isEquippable(equipItem, slotIndex)) return;

            var packContainer = getPackDisplay();
            if (packContainer != null) _setEquip(packContainer, equipItem, slotIndex);
            else {
                var action = equipChangedRequestFunc(equipItem, slotIndex);
                action.Invoke(refreshItems);
            }
        }
        void _setEquip(ContainerDisplay<E> container, E equipItem, int slotIndex) {
            if (!isEquippable(equipItem, slotIndex)) return;

            // 判断传入的 container 是否为 packContainer
            var packContainer = getPackDisplay(container);
            // 如果可用，调用对应的装备函数
            if (packContainer != null) _setEquip(packContainer, equipItem, slotIndex);
            else _setEquip(equipItem, slotIndex);
        }
        void _setEquip(PackContainerDisplay<E> container, E equipItem, int slotIndex) {
            if (!isEquippable(equipItem, slotIndex)) return;

            if (container == null) _setEquip(equipItem, slotIndex);
            else {
                var action = equipChangedRequestFunc(equipItem, slotIndex);
                action.Invoke(() => onEquipChanged(container, equipItem, slotIndex));
            }
        }

        /// <summary>
        /// 装备改变回调
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="equipItem">装备项</param>
        protected virtual void onEquipChanged(PackContainerDisplay<E> container, 
            E equipItem, int slotIndex) {
            // 如果可见则执行更新
            if (container.gameObject.activeInHierarchy)
                container.refreshItems();
            refreshItems();
        }

        #endregion

        /// <summary>
        /// 刷新物品
        /// </summary>
        public override void refreshItems() {
            Debug.Log("slotDisplay.refreshItems: " + slotData);
            if (slotData == null) clearItems();
            else setItems(slotData.items);
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 子视图创建回调
        /// </summary>
        /// <param name="sub">子视图</param>
        /// <param name="index">索引</param>
        protected override void onSubViewCreated(SelectableItemDisplay<T> sub, int index) {
            base.onSubViewCreated(sub, index);
            var slotItemDisplay = DataLoader.cast<SlotContItemDisplay<T, E>>(sub);
            if (slotItemDisplay != null) {
                var slotItem = getSlotItemDataByIndex(index);
                slotItemDisplay.setSlotIndex(slotItem.slotIndex);
            }
        }

        #endregion

        #region 请求控制

        /// <summary>
        /// 装备改变请求函数
        /// </summary>
        /// <param name="item">装备项</param>
        /// <returns>返回装备变更时进行的请求函数</returns>
        public virtual UnityAction<UnityAction> equipChangedRequestFunc(E item, int slotIndex) {
            if (item == null) return dequipRequestFunc(slotIndex);
            return equipRequestFunc(item, slotIndex);
        }

        /// <summary>
        /// 装备请求函数
        /// </summary>
        /// <returns>返回装备时进行的请求函数</returns>
        protected virtual UnityAction<UnityAction> equipRequestFunc(E item, int slotIndex) {
            return null;
        }

        /// <summary>
        /// 卸下装备请求函数
        /// </summary>
        /// <returns>返回卸下时进行的请求函数</returns>
        protected virtual UnityAction<UnityAction> dequipRequestFunc(int slotIndex) {
            return null;
        }

        #endregion

    }
}