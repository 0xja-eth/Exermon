
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 槽展示组件
/// </summary>
/// <typeparam name="T">物品类型</typeparam>
/// <typeparam name="E">装备类型</typeparam>
public class SlotItemDisplay<T, E> : ItemDisplay<T>, IDropHandler
    where T : class where E : class {

    /// <summary>
    /// 内部变量定义
    /// </summary>
    protected E equip; // 装备1

    #region 数据控制

    /// <summary>
    /// 物品变更回调
    /// </summary>
    protected override void onItemChanged() {
        base.onItemChanged();
        setupEquip();
    }

    /// <summary>
    /// 配置装备
    /// </summary>
    protected virtual void setupEquip() { }

    /// <summary>
    /// 装备
    /// </summary>
    /// <param name="item">物品</param>
    public void setEquip(E item) {
        if (equip == item) return;
        equip = item;
        onEquipChanged();
    }

    /// <summary>
    /// 获取装备
    /// </summary>
    public E getEquip() {
        return equip;
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
        drawEquip(equip);
    }

    /// <summary>
    /// 绘制装备
    /// </summary>
    /// <param name="equip">装备</param>
    void drawEquip(E equip) {
        if (equip == null) drawEmptyEquip();
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
    protected virtual void drawEmptyEquip() {
        clearEquip();
    }

    /// <summary>
    /// 清除装备
    /// </summary>
    protected virtual void clearEquip() { }

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
        clearEquip();
    }

    #endregion

    #region 事件控制

    /// <summary>
    /// 拖拽物品放下回调
    /// </summary>
    /// <param name="data">事件数据</param>
    public void OnDrop(PointerEventData data) {
        Debug.Log(name + " OnDrop: " + data);
        Debug.Log(name + " data.pointerDrag: " + data.pointerDrag);
        processItemDrop(getDraggingItemDisplay(data));
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
    protected virtual void processItemDrop(
        DraggableItemDisplay<E> display) {
        Debug.Log(name + " processItemDrop: " + display);
        if (display != null && display.isDraggable()) {
            var container = display.getContainer();
            var item = display.getItem();
            container.transferItem(this, item);
        }
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