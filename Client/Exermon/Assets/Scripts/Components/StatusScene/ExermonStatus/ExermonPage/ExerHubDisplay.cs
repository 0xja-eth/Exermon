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

    public ItemInfo<PlayerExermon> detail; // 帮助界面

    /// <summary>
    /// 内部变量声明
    /// </summary>

    #region 数据控制
    
    /// <summary>
    /// 获取物品帮助组件
    /// </summary>
    /// <returns>帮助组件</returns>
    protected override ItemInfo<PlayerExermon> getItemDetail() {
        return detail;
    }

    /// <summary>
    /// 最大选中数量
    /// </summary>
    /// <returns>最大选中数</returns>
    public override int maxCheckCount() {
        return DataService.get().staticData.configure.maxSubject;
    }
    
    /// <summary>
    /// 物品变更回调
    /// </summary>
    protected override void onItemsChanged() {
        base.onItemsChanged();
    }

    /// <summary>
    /// 校验选择数目
    /// </summary>
    /// <returns>选择数目是否正确</returns>
    public bool checkSelection() {
        return checkedIndices.Count == maxCheckCount();
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// ItemDisplay 创建回调
    /// </summary>
    /// <param name="item">ItemDisplay</param>
    protected override void onSubViewCreated(SelectableItemInfo<PlayerExermon> sub, int index) {
        base.onSubViewCreated(sub, index);
        if (sub.GetType().IsSubclassOf(typeof(DraggableItemDisplay<PlayerExermon>)))
            ((DraggableItemDisplay<PlayerExermon>)sub).draggingParent = draggingParent;
    }

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
    }

    #endregion

}
