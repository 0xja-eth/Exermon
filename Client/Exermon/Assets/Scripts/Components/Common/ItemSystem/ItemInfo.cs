using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品详细信息
/// </summary>
public class ItemInfo<T> : BaseView where T : class {

    /// <summary>
    /// 内部变量声明
    /// </summary>
    protected ItemContainer<T> container = null;

    protected T item = null;
    protected int index = -1;

    #region 初始化

    /// <summary>
    /// 配置组件
    /// </summary>
    /// <param name="window">父窗口</param>
    public void configure(ItemContainer<T> container) {
        this.container = container;
        configure();
    }

    #endregion

    #region 启动控制

    /// <summary>
    /// 启动窗口
    /// </summary>
    /// <param name="item">物品</param>
    /// <param name="index">所在索引</param>
    /// <param name="refresh">强制刷新</param>
    public void startView(T item, int index = -1, bool refresh = false) {
        startView();
        setItem(item, index, refresh);
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 设置物品
    /// </summary>
    /// <param name="item">物品</param>
    /// <param name="index">所在索引</param>
    /// <param name="refresh">强制刷新</param>
    public void setItem(T item, int index = -1, bool refresh = false) {
        if (!refresh && this.item == item && 
            this.index == index) return;
        this.item = item; this.index = index;
        onItemChanged();
    }

    /// <summary>
    /// 获取物品
    /// </summary>
    /// <returns>物品</returns>
    public T getItem() {
        return item;
    }

    /// <summary>
    /// 物品变更回调
    /// </summary>
    protected virtual void onItemChanged() {
        requestRefresh();
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 刷新物品
    /// </summary>
    protected virtual void refreshItem() {
        drawItem(item);
    }

    /// <summary>
    /// 绘制物品
    /// </summary>
    /// <param name="item">物品</param>
    void drawItem(T item) {
        if (item == null) drawEmptyItem();
        else drawExactlyItem(item);
    }

    /// <summary>
    /// 绘制空物品
    /// </summary>
    protected virtual void drawEmptyItem() {
        clearItem();
    }

    /// <summary>
    /// 绘制确切的物品
    /// </summary>
    /// <param name="item">物品</param>
    protected virtual void drawExactlyItem(T item) { }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected virtual void clearItem() { }

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
        refreshItem();
    }

    /// <summary>
    /// 清除视窗
    /// </summary>
    protected override void clear() {
        base.clear();
        clearItem();
    }

    #endregion
}
