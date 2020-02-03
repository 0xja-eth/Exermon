using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌卡片
/// </summary>
public class InitExerSlotDisplay : SlotItemDisplay<ExerSlotItem, ExerGift> {

    /// <summary>
    /// 常量定义
    /// </summary>

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image icon; // 图片
    public ExerGiftCard gift; // 天赋卡片（装备的）
    public Text name, subject;

    /// <summary>
    /// 内部变量声明
    /// </summary>

    #region 数据控制

    /// <summary>
    /// 获取容器
    /// </summary>
    /// <returns></returns>
    public new ExerSlotsContainer getContainer() {
        return base.getContainer() as ExerSlotsContainer;
    }

    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns></returns>
    public Exermon getExermon(ExerSlotItem item=null) {
        if (item == null) item = this.item;
        if (item == null) return null;
        return item.playerExer.exermon();
    }

    /// <summary>
    /// 配置装备
    /// </summary>
    protected override void setupEquip() {
        equip = item.exerGift();
    }

    /// <summary>
    /// 装备变更回调
    /// </summary>
    protected override void onEquipChanged() {
        base.onEquipChanged();
        item.setExerGift(equip);
        select();
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 绘制物品
    /// </summary>
    protected override void drawExactlyItem(ExerSlotItem item) {
        var exermon = getExermon(item);
        var icon = exermon.icon;
        var rect = new Rect(0, 0, icon.width, icon.height);
        this.icon.overrideSprite = Sprite.Create(
            icon, rect, new Vector2(0.5f, 0.5f));
        this.icon.overrideSprite.name = icon.name;
        name.text = item.playerExer.name();
        subject.text = exermon.subject().name;
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        name.text = subject.text = "";
        icon.overrideSprite = null;
    }

    /// <summary>
    /// 绘制装备
    /// </summary>
    /// <param name="exerGift">装备</param>
    protected override void drawExactlyEquip(ExerGift exerGift) {
        Debug.Log("drawExactlyEquip: " + exerGift);
        gift.setItem(exerGift);
    }

    /// <summary>
    /// 清除装备
    /// </summary>
    protected override void clearEquip() {
        gift.requestClear();
    }

    #endregion

    #region 事件控制

    #endregion
}
