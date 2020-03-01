using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌仓库显示
/// </summary>
public class ExerHubDisplay : ItemContainer<PlayerExermon> {

    /// <summary>
    /// 常量设置
    /// </summary>

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public RectTransform draggingParent; // 拖拽时的父变换

    public PlayerExermonDetail detail; // 帮助界面

    public ExerSlotExermonDisplay slotDisplay;

    /// <summary>
    /// 内部变量声明
    /// </summary>

    #region 数据控制

    /// <summary>
    /// 获取物品帮助组件
    /// </summary>
    /// <returns>帮助组件</returns>
    protected override ItemDetail<PlayerExermon> getItemDetail() {
        return detail;
    }
    
    #endregion

    #region 界面绘制

    /// <summary>
    /// ItemDisplay 创建回调
    /// </summary>
    /// <param name="item">ItemDisplay</param>
    protected override void onSubViewCreated(
        SelectableItemDisplay<PlayerExermon> sub, int index) {
        base.onSubViewCreated(sub, index);
        if (sub.GetType().IsSubclassOf(
            typeof(DraggableItemDisplay<PlayerExermon>)))
            ((DraggableItemDisplay<PlayerExermon>)sub).draggingParent = draggingParent;
    }

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 装备
    /// </summary>
    public void equip() {
        slotDisplay.setEquip(selectedItem());
    }

    #endregion
}
