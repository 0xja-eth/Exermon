﻿
using UnityEngine;
using UnityEngine.EventSystems;

using Core.UI.Utils;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 槽展示接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISlotItemDisplay<T, E> : ISelectableItemDisplay<T>,
    IDropHandler where T : class where E : class {

        /// <summary>
        /// 装备
        /// </summary>
        /// <param name="item">物品</param>
        void setEquip(E item, bool force = false);
        /// <param name="container">容器</param>
        void setEquip(SelectableContainerDisplay<E> container, E item);

        /// <summary>
        /// 获取装备
        /// </summary>
        E getEquip();
    }

    /// <summary>
    /// 槽展示组件
    /// </summary>
    /// <typeparam name="T">物品类型</typeparam>
    /// <typeparam name="E">装备类型</typeparam>
    public class SlotItemDisplay<T, E> : SelectableItemDisplay<T>, ISlotItemDisplay<T, E>
        where T : class where E : class {

        /// <summary>
        /// 是否提供预览功能
        /// </summary>
        public bool previewable = false;

        /// <summary>
        /// 是否单击卸下装备（如果可以选择，则在选择状态下单击卸下装备）
        /// </summary>
        public bool onClickDequip = false;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        protected E equip; // 装备
        protected E lastEquip; // 上个装备
        protected E previewingEquip = null; // 装备

        // protected ContainerDisplay<E> packDisplay = null; // 装备容器

        #region 初始化
        /*
        /// <summary>
        /// 配置组件
        /// </summary>
        public override void configure(ContainerDisplay<T> container, int index) {
            base.configure(container, index+1);
        }
        */
        #endregion

        #region 数据控制
        
        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();
            if (item == null) setupEmptyEquip();
            else setupExactlyEquip();
        }

        /// <summary>
        /// 配置确切物品的装备（原始装备）
        /// </summary>
        protected virtual void setupExactlyEquip() {
            lastEquip = null;
        }

        /// <summary>
        /// 配置空物品装备（原始装备）
        /// </summary>
        protected virtual void setupEmptyEquip() {
            lastEquip = equip = null;
        }

        /// <summary>
        /// 是否为空装备
        /// </summary>
        /// <param name="equip"></param>
        /// <returns></returns>
        public virtual bool isNullEquip(E equip) {
            return equip == null;
        }

        /// <summary>
        /// 装备
        /// </summary>
        /// <param name="item">物品</param>
        public virtual void setEquip(E item, bool force = false) {
            if (!force && (!isEquippable(item) || equip == item)) return;
            lastEquip = equip; equip = item;
            onEquipChanged();
        }
        /// <param name="container">容器</param>
        public virtual void setEquip(SelectableContainerDisplay<E> container, E item) {
            setEquip(item);
        }

        /// <summary>
        /// 清除装备
        /// </summary>
        public virtual void clearEquip() {
            setEquip(null, true);
        }

        /// <summary>
        /// 能否装备
        /// </summary>
        /// <param name="item">装备项</param>
        /// <returns></returns>
        public virtual bool isEquippable(E item) { return true; }

        /// <summary>
        /// 设置预览
        /// </summary>
        /// <param name="item">物品</param>
        public virtual void setPreview(E item) {
            if (previewingEquip == item) return;
            previewingEquip = item;
            onPreviewChanged();
        }

        /// <summary>
        /// 清除预览
        /// </summary>
        public void clearPreview() {
            setPreview(null);
        }

        /// <summary>
        /// 获取装备
        /// </summary>
        public E getEquip() {
            return equip;
        }

        /// <summary>
        /// 获取装备
        /// </summary>
        public E getPreview() {
            return previewingEquip;
        }

        /// <summary>
        /// 预览变更回调
        /// </summary>
        protected virtual void onPreviewChanged() {
            if (container != null)
                container.updateItemHelp();
            requestRefresh();
        }

        /// <summary>
        /// 装备变更回调
        /// </summary>
        protected virtual void onEquipChanged() {
            if (container != null)
                container.updateItemHelp();
            requestRefresh();
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 刷新装备
        /// </summary>
        void refreshEquip() {
            drawEquip(previewingEquip ?? equip);
        }

        /// <summary>
        /// 绘制装备
        /// </summary>
        /// <param name="equip">装备</param>
        void drawEquip(E equip) {
            if (isNullEquip(equip)) drawEmptyEquip();
            else drawExactlyEquip(equip);
        }

        /// <summary>
        /// 绘制装备
        /// </summary>
        /// <param name="equip">装备</param>
        protected virtual void drawExactlyEquip(E equip) { }

        /// <summary>
        /// 绘制空装备
        /// </summary>
        protected virtual void drawEmptyEquip() {}

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshEquip();
        }

        /// <summary>
        /// 清除描述
        /// </summary>
        protected override void clear() {
            base.clear();
            drawEmptyEquip();
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 指针进入回调
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public override void OnPointerEnter(PointerEventData data) {
            base.OnPointerEnter(data);
            if (previewable)
                processItemPreview(getDraggingItemDisplay(data));
        }

        /// <summary>
        /// 指针离开回调
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public override void OnPointerExit(PointerEventData data) {
            base.OnPointerExit(data);
            clearPreview();
        }

        /// <summary>
        /// 处理点击事件回调
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public override void OnPointerClick(PointerEventData data) {
            if (onClickDequip) 
                if (!isSelectable()) setEquip(null);
                else if (isSelected()) setEquip(null);
            base.OnPointerClick(data);
        }

        /// <summary>
        /// 拖拽物品放下回调
        /// </summary>
        /// <param name="data">事件数据</param>
        public void OnDrop(PointerEventData data) {
            Debug.Log("OnDrop: " + data.pointerDrag);

            processItemDrop(getDraggingItemDisplay(data), data);
        }

        /// <summary>
        /// 获取拖拽中的物品显示项
        /// </summary>
        /// <param name="data">事件数据</param>
        /// <returns>物品显示项</returns>
        DraggableItemDisplay<E> getDraggingItemDisplay(
            PointerEventData data) {
            var obj = data.pointerDrag;
            if (obj == null) return null;
            return SceneUtils.get<DraggableItemDisplay<E>>(obj);
        }

        /// <summary>
        /// 处理物品放下
        /// </summary>
        protected virtual void processItemPreview(
            DraggableItemDisplay<E> display) {
            if (display != null && display.isDraggable())
                setPreview(display.getItem());
        }

        /// <summary>
        /// 处理物品放下
        /// </summary>
        protected virtual void processItemDrop(
            DraggableItemDisplay<E> display, PointerEventData data) {
            if (display == null && !display.isDraggable()) return;
            var container = display.getContainer();
            var item = display.getItem();
            container.transferItem(this, item);
            display.OnEndDrag(data);
        }

        #endregion

    }

    /*
    /// <summary>
    /// 槽展示组件
    /// </summary>
    /// <typeparam name="T">物品类型</typeparam>
    /// <typeparam name="E1">装备类型1</typeparam>
    /// <typeparam name="E2">装备类型2</typeparam>
    public class SlotItemDisplay<T, E1, E2> : SlotItemDisplay<T, E1>
        where T : class where E1 : class where E2 : class {

        /// <summary>
        /// 内部变量定义
        /// </summary>
        protected E2 equip2; // 装备2

        #region 数据控制

        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();
            setupEquips();
        }

        /// <summary>
        /// 配置装备
        /// </summary>
        protected virtual void setupEquips() { }

        /// <summary>
        /// 装备
        /// </summary>
        /// <param name="item">物品</param>
        public void setEquip(E2 item) {
            if (equip2 == item) return;
            equip2 = item;
            onEquipChanged();
        }

        /// <summary>
        /// 获取装备
        /// </summary>
        /// <param name="equip1">装备1</param>
        /// <param name="equip2">装备2</param>
        public void getEquip(out E1 equip1, out E2 equip2) {
            equip1 = equip; equip2 = this.equip2;
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 刷新装备
        /// </summary>
        void refreshEquips() {
            drawEquips(equip, equip2);
        }

        /// <summary>
        /// 绘制装备
        /// </summary>
        /// <param name="equip">装备</param>
        void drawEquips(E1 equip1, E2 equip2) {
            if (equip1 == null) drawEmptyEquip();
            else drawExactlyEquip(equip1);
            if (equip2 == null) drawEmptyEquip2();
            else drawExactlyEquip2(equip2);
        }

        /// <summary>
        /// 绘制装备
        /// </summary>
        /// <param name="equip">装备</param>
        protected virtual void drawExactlyEquip2(E2 equip) { }

        /// <summary>
        /// 绘制空装备
        /// </summary>
        protected virtual void drawEmptyEquip2() { }

        /// <summary>
        /// 清除装备
        /// </summary>
        protected virtual void clearEquip2() { }

        /// <summary>
        /// 清除装备
        /// </summary>
        protected virtual void clearEquips() {
            clearEquip(); clearEquip2();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshEquips();
        }

        /// <summary>
        /// 清除描述
        /// </summary>
        protected override void clear() {
            base.clear();
            clearEquips();
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 拖拽物品放下回调
        /// </summary>
        /// <param name="data">事件数据</param>
        public void OnDrop(PointerEventData data) {
            DraggableItemDisplay<E1> display1;
            DraggableItemDisplay<E2> display2;

            display1 = getDraggingItemDisplay<E1>(data);
            display2 = getDraggingItemDisplay<E2>(data);

            processItemDrop(display1, display2);
        }

        /// <summary>
        /// 获取拖拽中的物品显示项
        /// </summary>
        /// <param name="data">事件数据</param>
        /// <returns>物品显示项</returns>
        DraggableItemDisplay<E> getDraggingItemDisplay<E>(
            PointerEventData data) where E : class {
            var obj = data.pointerDrag;
            if (obj == null) return null;
            return SceneUtils.get<DraggableItemDisplay<E>>(obj);
        }

        /// <summary>
        /// 处理物品放下
        /// </summary>
        protected virtual void processItemDrop(
            DraggableItemDisplay<E1> display1,
            DraggableItemDisplay<E2> display2) {

            if (display1 != null) {
                var container = display1.getContainer();
                var item = display1.getItem();
                container.removeItem(item);
                setEquip(item);
            }
            if (display2 != null) {
                var container = display2.getContainer();
                var item = display2.getItem();
                container.removeItem(item);
                setEquip(item);
            }
        }

        #endregion

    }
    */
}