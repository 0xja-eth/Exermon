
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Data;
using ItemModule.Data;
using PlayerModule.Data;
using ExermonModule.Data;

using GameModule.Services;
using PlayerModule.Services;
using ExermonModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

namespace UI.StatusScene.Controls.ExermonStatus {

    /// <summary>
    /// Exermon 状态中每页总控制组件的基类
    /// </summary>
    public class ExermonStatusPageDisplay : ItemDisplay<ExerSlotItem> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PageTabController tabController;
            
        public GameObject equip, dequip;

        /// <summary>
        /// 内部组件设置
        /// </summary>
        Button equipBtn, dequipBtn;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        protected PlayerService playerSer;
        protected ExermonService exerSer;

        #region 初始化

        protected override void initializeOnce() {
            base.initializeOnce();
            equipBtn = SceneUtils.button(equip);
            dequipBtn = SceneUtils.button(dequip);
        }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
            exerSer = ExermonService.get();
        }

        #endregion

        #region 启动控制

        /// <summary>
        /// 加载函数
        /// </summary>
        /// <returns></returns>
        public virtual UnityAction<UnityAction> getLoadFunction() { return null; }

        #endregion

        #region 数据控制

        /// <summary>
        /// 装备当前装备物品（子类继承）
        /// </summary>
        public virtual void equipCurrentItem() { }

        /// <summary>
        /// 卸下当前装备物品（子类继承）
        /// </summary>
        public virtual void dequipCurrentItem() { }

        #endregion

        #region 流程控制

        /// <summary>
        /// 状态变更回调
        /// </summary>
        public virtual void onEquipChanged() {
            requestRefresh(true);
        }

        #region 请求控制

        /// <summary>
        /// 可否装备
        /// </summary>
        /// <returns></returns>
        public virtual bool equipable() { return false; }
        public virtual bool dequipable() { return false; }

        /*
        /// <summary>
        /// 请求更改艾瑟萌，并更新整个页面
        /// </summary>
        protected void requestEquip() {
            equipRequestFunc()?.Invoke(onEquipChanged);
        }

        /// <summary>
        /// 请求更改艾瑟萌，并更新整个页面
        /// </summary>
        protected void requestDequip() {
            dequipRequestFunc()?.Invoke(onEquipChanged);
        }
        
        /// <summary>
        /// 装备请求函数
        /// </summary>
        /// <returns></returns>
        protected virtual UnityAction<UnityAction> equipRequestFunc() {
            return null;
        }

        /// <summary>
        /// 装备请求函数
        /// </summary>
        /// <returns></returns>
        protected virtual UnityAction<UnityAction> dequipRequestFunc() {
            return null;
        }
        */
        
        #endregion

        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新按钮可用性
        /// </summary>
        public virtual void refreshButtons() {
            equip.SetActive(equipable());
            dequip.SetActive(!equip.activeSelf && dequipable());
            equipBtn.interactable = equip.activeSelf;
            dequipBtn.interactable = dequip.activeSelf;
            if (!equip.activeSelf && !dequip.activeSelf) {
                var obj = defaultShownObject();
                if (obj == null) return;
                var btn = SceneUtils.button(obj);
                obj.SetActive(true);
            }
        }

        /// <summary>
        /// 默认显示按钮
        /// </summary>
        public virtual GameObject defaultShownObject() {
            return equip;
        }
        /*
        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshButtons();
        }
        */
        #endregion
    }

    /// <summary>
    /// Exermon 状态中每页总控制组件的基类
    /// </summary>
    public class ExermonStatusPageDisplay<T> : ExermonStatusPageDisplay where T : PackContItem, new() {

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            var slotDisplay = getSlotItemDisplay();
            var packDisplay = getPackDisplay();

            slotDisplay?.setPageDisplay(this);
            //slotDisplay?.setPackDisplay(packDisplay);
            packDisplay?.addCallback(refreshButtons, 0);
            packDisplay?.addCallback(refreshButtons, 1);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取容器组件
        /// </summary>
        public virtual PackContainerDisplay<T> getPackDisplay() { return null; }

        /// <summary>
        /// 获取艾瑟萌槽项显示组件
        /// </summary>
        public virtual ExermonStatusSlotItemDisplay<T> getSlotItemDisplay() { return null; }

        /// <summary>
        /// 装备当前装备物品
        /// </summary>
        public override void equipCurrentItem() {
            var container = getPackDisplay();
            var slotDisplay = getSlotItemDisplay();
            if (container == null || slotDisplay == null) return;
            slotDisplay.setEquip(container, container.selectedItem());
        }

        /// <summary>
        /// 卸下当前装备物品
        /// </summary>
        public override void dequipCurrentItem() {
            var container = getPackDisplay();
            var slotDisplay = getSlotItemDisplay();
            if (container == null || slotDisplay == null) return;
            slotDisplay.setEquip(container, null);
        }

        /*
        /// <summary>
        /// 获取容器物品数据
        /// </summary>
        /// <returns></returns>
        protected virtual T getFirstItem() {
            return item.getEquip<T>();
        }

        /// <summary>
        /// 获取容器物品数据
        /// </summary>
        /// <returns></returns>
        protected virtual List<T> getContainerItems() {
            var containerData = getPackContainerData();
            if (containerData == null) return new List<T>();
            return containerData.getItems(packCondition());
        }
        /// <summary>
        /// 背包条件
        /// </summary>
        /// <returns></returns>
        protected virtual Predicate<T> packCondition() { return (item) => true; }
        */

        /// <summary>
        /// 获取容器物品数据
        /// </summary>
        /// <returns></returns>
        protected virtual PackContainer<T> getPackContainer() {
            return playerSer.player.packContainers.getContainer<T>();
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 刷新槽显示
        /// </summary>
        void refreshSlotDisplay() {
            var slotDisplay = getSlotItemDisplay();
            slotDisplay?.setSlotData(
                item.exerSlot, item.subjectId, true);
        }

        /// <summary>
        /// 刷新背包容器
        /// </summary>
        void refreshPackContainer() {
            var packDisplay = getPackDisplay();
            if (packDisplay == null) return;
            packDisplay.configure(getPackContainer());
            packDisplay.select(0);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            if (item != null) {
                refreshSlotDisplay();
                refreshPackContainer();
            }
        }

        #endregion

        #region 请求控制

        /// <summary>
        /// 可否装备
        /// </summary>
        /// <returns></returns>
        public override bool equipable() {
            var packDisplay = getPackDisplay();
            var slotItemDisplay = getSlotItemDisplay();
            var selectedItem = packDisplay.selectedItem();
            var equipedItem = slotItemDisplay.getEquip();
            if (selectedItem == null || selectedItem.equiped) return false;
            if (!slotItemDisplay.isEquippable(selectedItem)) return false;
            return !selectedItem.isNullItem();
        }
        public override bool dequipable() {
            var packDisplay = getPackDisplay();
            var slotItemDisplay = getSlotItemDisplay();
            var selectedItem = packDisplay.selectedItem();
            var equipedItem = slotItemDisplay.getEquip();
            if (!slotItemDisplay.isEquippable(null)) return false;
            return selectedItem == equipedItem && equipedItem != null && 
                !equipedItem.isNullItem();
        }

        #endregion
    }
}