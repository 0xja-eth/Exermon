using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌天赋详细信息
/// </summary>
public class ExerGiftDetail : ItemInfo<ExerGift> {

    /// <summary>
    /// 常量设置
    /// </summary>
    const string EmptyGiftText = "空";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image frame, background; // 边框、背景
    public Text name, description;
    public ParamDisplaysGroup paramsView;

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

    #endregion

    #region 界面控制

    /// <summary>
    /// 绘制物品
    /// </summary>
    protected override void drawExactlyItem(ExerGift exerGift) {
        name.text = exerGift.name;
        description.text = exerGift.description;
        paramsView.setValues(exerGift);
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        name.text = EmptyGiftText;
        description.text = "";
        paramsView.clearValues();
    }
    
    #endregion
    
}
