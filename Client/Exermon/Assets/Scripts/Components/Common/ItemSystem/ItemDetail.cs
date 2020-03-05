using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品显示接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IItemDetail<T> : IItemDisplay<T> where T : class {

    /// <summary>
    /// 配置窗口
    /// </summary>
    void configure(ItemContainer<T> container);

    /// <summary>
    /// 启动窗口
    /// </summary>
    void startView(T item, int index = -1, bool refresh = false);

    /// <summary>
    /// 设置物品
    /// </summary>
    void setItem(T item, int index = -1, bool refresh = false);

}

/// <summary>
/// 物品详细信息
/// </summary>
public class ItemDetail<T> : ItemDisplay<T>, IItemDetail<T> where T : class {

    /// <summary>
    /// 内部变量声明
    /// </summary>
    protected ItemContainer<T> container = null;
    
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
        if (!refresh && this.item == item && this.index == index) return;
        this.item = item; this.index = index;
        onItemChanged();
    }

    #endregion
    
}
