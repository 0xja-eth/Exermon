using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌天赋卡片容器
/// </summary>
public class ExerGiftsContainer : ItemContainer<ExerGift> {

    /// <summary>
    /// 常量设置
    /// </summary>

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public RectTransform draggingParent; // 拖拽对象的父变换

    public ExerGiftDetail detail; // 帮助界面
    public ExerSlotDetail slotDetail; // 艾瑟萌槽帮助界面
    public ExerSlotsContainer exerSlot; // 艾瑟萌槽

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
    protected override ItemInfo<ExerGift> getItemDetail() {
        return detail;
    }

    /// <summary>
    /// 准备转移
    /// </summary>
    /// <param name="item">物品</param>
    protected override ExerGift prepareTransfer(ExerGift item) {
        return item;
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// ItemDisplay 创建回调
    /// </summary>
    /// <param name="item">ItemDisplay</param>
    protected override void onSubViewCreated(ItemDisplay<ExerGift> item, int index) {
        base.onSubViewCreated(item, index);
        if (!inStartScene) return;
        var giftCard = item as ExerGiftCard;
        giftCard.draggingParent = draggingParent;
    }

    /// <summary>
    /// 绘制实际物品帮助
    /// </summary>
    /// <param name="item">物品</param>
    protected override void drawExactlyItemHelp(ExerGift item) {
        if (exerSlot) exerSlot.deselect();
        if (slotDetail) slotDetail.terminateView();
        if (tips) tips.SetActive(false);
        base.drawExactlyItemHelp(item);
    }

    #endregion


}
