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
public class PlayerStatusDisplay : ItemDisplay<Player> {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public PlayerBaseInfoDisplay baseInfo; // 人物基本信息视图
    public PlayerDetailInfoDisplay detailInfo; // 人物详细信息视图

    public InfoEditWindow infoEditWindow;

    /// <summary>
    /// 内部变量设置
    /// </summary>

    #region 初始化
        
    #endregion

    #region 数据控制

    /// <summary>
    /// 物品改变回调
    /// </summary>
    protected override void onItemChanged() {
        base.onItemChanged();

        baseInfo.setItem(item);
        detailInfo.setItem(item);
    }

    #endregion

    #region 界面绘制
    
    /// <summary>
    /// 刷新视图
    /// </summary>
    protected override void refresh() {
        base.clear();

        baseInfo.requestRefresh(true);
        detailInfo.requestRefresh(true);
    }
    
    #endregion
    
}
