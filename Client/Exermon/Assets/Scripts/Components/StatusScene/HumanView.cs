using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HedgehogTeam.EasyTouch;

/// <summary>
/// 状态窗口人物视图
/// </summary>
public class HumanView : ItemDisplay<Player> {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public BaseInfoView baseInfoView; // 人物基本信息视图
    public DetailInfoView detailInfoView; // 人物详细信息视图

    public Toggle[] toggles; // 切换按钮 

    /// <summary>
    /// 内部变量设置
    /// </summary>

    #region 初始化

    protected override void initializeOnce() {
        base.initializeOnce();
        clearTogglesIsOn();
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 物品改变回调
    /// </summary>
    protected override void onItemChanged() {
        base.onItemChanged();

        baseInfoView.setItem(item);
        detailInfoView.setItem(item);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 清除所有IsOn
    /// </summary>
    public void clearTogglesIsOn() {
        foreach (var t in toggles) t.isOn = false;
    }

    /// <summary>
    /// 清空视图
    /// </summary>
    protected override void clear() {
        base.clear();
        clearTogglesIsOn();
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 唤起信息窗口
    /// </summary>
    public void startInfoWindow() {

    }

    #endregion
}
