using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 初始艾瑟萌卡片
/// </summary>
public class InitExermonCard : SelectableItemDisplay<Exermon> {

    /// <summary>
    /// 常量定义
    /// </summary>

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image icon; // 图片
    public Text name, subject;

    /// <summary>
    /// 内部变量声明
    /// </summary>

    #region 数据控制

    /// <summary>
    /// 获取容器
    /// </summary>
    /// <returns></returns>
    public new ExermonsContainer getContainer() {
        return base.getContainer() as ExermonsContainer;
    }

    /// <summary>
    /// 是否强制选中
    /// </summary>
    /// <returns>强制选中</returns>
    public override bool isForceChecked() {
        if (item == null) return false;
        return isCheckable() && item.subject().force;
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 绘制物品
    /// </summary>
    protected override void drawExactlyItem(Exermon exermon) {
        var icon = exermon.icon;
        var rect = new Rect(0, 0, icon.width, icon.height);
        this.icon.overrideSprite = Sprite.Create(
            icon, rect, new Vector2(0.5f, 0.5f));
        this.icon.overrideSprite.name = icon.name;
        if (name) name.text = exermon.name;
        if (subject) subject.text = exermon.subject().name;
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        if (name) name.text = "";
        if (subject) subject.text = "";
        icon.overrideSprite = null;
    }

    #endregion

    #region 事件控制
    
    #endregion
}
