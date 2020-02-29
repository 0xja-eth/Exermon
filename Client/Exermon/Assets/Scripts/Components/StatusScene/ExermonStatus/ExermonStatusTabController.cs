
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LitJson;

/// <summary>
/// 艾瑟萌页控制器
/// </summary>
public class ExermonStatusTabController : TabView<ExermonPageInfoBase> {

    /// <summary>
    /// 外部变量设置
    /// </summary>
    public ExerHubDisplay exerHubDisplay;

    /// <summary>
    /// 内部变量定义
    /// </summary>
    ExerSlotItem slotItem;

    #region 数据控制

    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="player">玩家</param>
    /// <param name="subject">科目</param>
    public void setup(Player player, Subject subject) {
        if (player == null || subject == null) slotItem = null;
        else slotItem = player.getExerSlotItem(subject);
        updateExerHubItems(player, subject);
        requestRefresh();
    }

    /// <summary>
    /// 更新艾瑟萌仓库物品
    /// </summary>
    void updateExerHubItems(Player player, Subject subject) {
        var items = new List<PlayerExermon>();
        if (player != null && subject != null) { 
            var exerHub = player.packContainers.exerHub;
            items.Add(slotItem.playerExer);
            items.AddRange(exerHub.items.FindAll(item =>
                item.exermon().subjectId == subject.getID()));
        }
        exerHubDisplay.setItems(items);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 显示内容页
    /// </summary>
    /// <param name="content"></param>
    protected override void showContent(ExermonPageInfoBase content, int index) {
        content.startView(slotItem, true);
    }

    /// <summary>
    /// 显示内容页
    /// </summary>
    /// <param name="content"></param>
    protected override void hideContent(ExermonPageInfoBase content, int index) {
        content.terminateView();
    }

    #endregion

}

/// <summary>
/// Exermon页的基类
/// </summary>
public class ExermonPageInfoBase : ItemDisplay<ExerSlotItem> {

}