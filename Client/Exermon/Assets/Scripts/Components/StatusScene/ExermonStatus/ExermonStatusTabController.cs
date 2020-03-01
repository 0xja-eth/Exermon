
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LitJson;

/// <summary>
/// 艾瑟萌页控制器
/// </summary>
public class ExermonStatusTabController : TabView<ExermonStatusPageInfo> {

    /// <summary>
    /// 外部变量设置
    /// </summary>

    /// <summary>
    /// 内部变量定义
    /// </summary>
    public ExerSlotItem slotItem { get; private set; }
    public Player player { get; private set; }
    public Subject subject { get; private set; }

    #region 数据控制

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="player">玩家</param>
    /// <param name="subject">科目</param>
    public void setup(Player player, Subject subject) {
        this.player = player; this.subject = subject;
        if (player == null || subject == null) slotItem = null;
        else slotItem = player.getExerSlotItem(subject);
        requestRefresh();
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 显示内容页
    /// </summary>
    /// <param name="content"></param>
    protected override void showContent(ExermonStatusPageInfo content, int index) {
        content.startView(slotItem, true);
    }

    /// <summary>
    /// 显示内容页
    /// </summary>
    /// <param name="content"></param>
    protected override void hideContent(ExermonStatusPageInfo content, int index) {
        content.terminateView();
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 装备回调
    /// </summary>
    public void onEquip() {
        currentContent().equipCurrentItem();
    }

    #endregion

}

/// <summary>
/// Exermon 状态中每页总控制组件的基类
/// </summary>
public class ExermonStatusPageInfo : ItemDisplay<ExerSlotItem> {
    
    /// <summary>
    /// 装备当前装备物品（子类继承）
    /// </summary>
    public virtual void equipCurrentItem() { }

}

/// <summary>
/// Exermon 状态中每页总控制组件的基类
/// </summary>
public class ExermonStatusPageInfo<T> : ExermonStatusPageInfo where T : PackContItem {

    /// <summary>
    /// 获取容器组件
    /// </summary>
    protected virtual ItemContainer<T> getContainer() { return null; }

    /// <summary>
    /// 获取艾瑟萌槽项显示组件
    /// </summary>
    protected virtual SlotItemDisplay<ExerSlotItem, T> getSlotItemDisplay() { return null; }

    /// <summary>
    /// 装备当前装备物品
    /// </summary>
    public override void equipCurrentItem() {
        var container = getContainer();
        var slotDisplay = getSlotItemDisplay();
        if (container == null || slotDisplay == null) return;
        slotDisplay.setEquip(container.selectedItem());
    }

}
