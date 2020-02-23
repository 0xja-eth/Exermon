using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 天赋卡片
/// </summary>
public class ExerGiftCard : DraggableItemDisplay<ExerGift> {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image icon; // 图标
    public GameObject detail; // 详细

    /// <summary>
    /// 内部变量声明
    /// </summary>
    public bool bigIcon = false;

    #region 初始化

    #endregion

    #region 界面控制

    /// <summary>
    /// 刷新状态
    /// </summary>
    protected override void refreshStatus() {
        base.refreshStatus();
        if (detail) detail.SetActive(!isDragging() && isSelected());
    }

    /// <summary>
    /// 绘制物品
    /// </summary>
    protected override void drawExactlyItem(ExerGift exerGift) {
        var icon = bigIcon ? exerGift.bigIcon : exerGift.icon;
        var rect = new Rect(0, 0, icon.width, icon.height);
        this.icon.sprite = this.icon.overrideSprite = Sprite.Create(
            icon, rect, new Vector2(0.5f, 0.5f));
        this.icon.sprite.name = this.icon.overrideSprite.name = icon.name;
        this.icon.gameObject.SetActive(true);
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        icon.gameObject.SetActive(false);
    }

    #endregion

    #region 拖拽控制

    /// <summary>
    /// 开始拖拽
    /// </summary>
    protected override void onBeforeDrag() {
        if (detail) detail.SetActive(false);
    }

    #endregion

}