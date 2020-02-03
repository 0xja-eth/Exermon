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
    /// 常量定义
    /// </summary>
    const string EmptyGiftText = "空";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image frame; // 边框
    public Text name;  // 名称

    /// <summary>
    /// 内部变量声明
    /// </summary>

    #region 初始化

    #endregion

    #region 界面控制

    /// <summary>
    /// 绘制物品
    /// </summary>
    protected override void drawExactlyItem(ExerGift exerGift) {
        name.text = exerGift.name;
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        name.text = EmptyGiftText;
    }
    
    #endregion
    
}