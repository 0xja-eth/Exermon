using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌槽详细信息
/// </summary>
public class ExerSlotDetail : ItemInfo<ExerSlotItem> {
    
    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image full;
    public ExerGiftCard gift; // 天赋卡片（装备的）
    public Text name, subject, description;
    public ParamBarsGroup paramsView;

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        setupParamsView();
    }

    /// <summary>
    /// 配置属性组
    /// </summary>
    void setupParamsView() {
        var params_ = DataService.get().staticData.configure.baseParams;
        paramsView.configure(params_);
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 获取容器
    /// </summary>
    /// <returns></returns>
    public ExerSlotsContainer getContainer() {
        return container as ExerSlotsContainer;
    }

    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns></returns>
    public Exermon getExermon(ExerSlotItem item = null) {
        if (item == null) item = this.item;
        if (item == null) return null;
        return item.playerExer.exermon();
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 绘制确切的物品
    /// </summary>
    /// <param name="exermon">物品</param>
    protected override void drawExactlyItem(ExerSlotItem item) {
        drawFullView(item);
        drawInfoView(item);
        drawParamsView(item);
    }

    /// <summary>
    /// 绘制全身像卡片
    /// </summary>
    /// <param name="exermon">物品</param>
    void drawFullView(ExerSlotItem item) {
        var exermon = getExermon(item);
        var full = exermon.full;
        var rect = new Rect(0, 0, full.width, full.height);
        this.full.overrideSprite = Sprite.Create(
            full, rect, new Vector2(0.5f, 0.5f));
        this.full.overrideSprite.name = full.name;
        name.text = item.playerExer.name();
        gift.setItem(item.exerGift());
    }

    /// <summary>
    /// 绘制基本信息
    /// </summary>
    /// <param name="exermon">物品</param>
    void drawInfoView(ExerSlotItem item) {
        var exermon = getExermon(item);

        subject.text = exermon.subject().name;
        description.text = exermon.description;
    }

    /// <summary>
    /// 绘制属性信息
    /// </summary>
    /// <param name="exermon">物品</param>
    void drawParamsView(ExerSlotItem item) {
        paramsView.setValues(item);
    }

    /// <summary>
    /// 清除全身像卡片
    /// </summary>
    void clearFullView() {
        full.overrideSprite = null;
        name.text = subject.text = "";
        gift.setItem(null);
    }

    /// <summary>
    /// 清除基本信息
    /// </summary>
    void clearInfoView() {
        description.text = "";
    }

    /// <summary>
    /// 清除属性信息
    /// </summary>
    void clearParamsView() {
        paramsView.clearValues();
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        clearFullView();
        clearInfoView();
        clearParamsView();
    }

    #endregion
    
}
