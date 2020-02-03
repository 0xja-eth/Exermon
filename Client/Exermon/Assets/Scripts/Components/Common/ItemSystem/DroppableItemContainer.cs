using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 物品容器（实现IDropHandler）
/// </summary>
public class DroppableItemContainer<T> : ItemContainer<T>, IDropHandler where T : class {
    
    #region 事件控制

    /// <summary>
    /// 拖拽物品放下回调
    /// </summary>
    /// <param name="data">事件数据</param>
    public void OnDrop(PointerEventData data) {
        processItemDrop(getDraggingItemDisplay(data));
    }

    /// <summary>
    /// 获取拖拽中的物品显示项
    /// </summary>
    /// <param name="data">事件数据</param>
    /// <returns>物品显示项</returns>
    DraggableItemDisplay<T> getDraggingItemDisplay(PointerEventData data) {
        var obj = data.pointerDrag;
        if (obj == null) return null;
        return SceneUtils.get<DraggableItemDisplay<T>>(obj);
    }

    /// <summary>
    /// 处理物品放下
    /// </summary>
    protected virtual void processItemDrop(DraggableItemDisplay<T> display) {
        if (display == null) return;
        var container = display.getContainer();
        container.transferItem(this, display.getItem());
    }

    #endregion

}
