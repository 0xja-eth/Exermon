﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌卡片容器（开始场景）
/// </summary>
public class ExerSlotsContainer : ItemContainer<ExerSlotItem> {

    /// <summary>
    /// 常量设置
    /// </summary>

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public ExerSlotDetail detail; // 帮助界面
    public ExerGiftDetail giftDetail; // 天赋帮助界面
    public ExerGiftsContainer exerGifts; // 艾瑟萌天赋

    public GameObject tips;

    /// <summary>
    /// 外部变量设置
    /// </summary>
    public bool inStartScene = false; // 是否在开始场景中

    #region 数据控制

    /// <summary>
    /// 获取物品帮助组件
    /// </summary>
    /// <returns>帮助组件</returns>
    protected override ItemInfo<ExerSlotItem> getItemDetail() {
        return detail;
    }

    /// <summary>
    /// 校验选择数目
    /// </summary>
    /// <returns>选择数目是否正确</returns>
    public bool checkSelection() {
        var cnt = itemDisplaysCount();
        for (int i = 0; i < cnt; ++i)
            if (items[i].exerGift() == null) return false;
        return true;
    }

    /// <summary>
    /// 获取装备的天赋ID数组
    /// </summary>
    /// <returns>天赋ID数组</returns>
    public int[] getGiftIds() {
        var cnt = itemDisplaysCount();
        var gids = new List<int>(cnt);
        for (int i = 0; i < cnt; ++i) {
            var gift = items[i].exerGift();
            if (gift == null) continue;
            gids.Add(gift.getID());
        }
        return gids.ToArray();
    }

    #endregion

    #region 物品帮助绘制

    /// <summary>
    /// 绘制实际物品帮助
    /// </summary>
    /// <param name="item">物品</param>
    protected override void drawExactlyItemHelp(ExerSlotItem item) {
        if (exerGifts) exerGifts.deselect();
        if (giftDetail) giftDetail.terminateView();
        if (tips) tips.SetActive(false);
        base.drawExactlyItemHelp(item);
    }

    #endregion

}