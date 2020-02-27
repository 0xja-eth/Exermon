using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HedgehogTeam.EasyTouch;

/// <summary>
/// 状态窗口艾瑟萌视图
/// </summary>
public class ExermonView : ItemDisplay<Player> {

    /// <summary>
    /// 外部组件设置
    /// </summary>

    /// <summary>
    /// 内部变量设置
    /// </summary>

    #region 初始化

    protected override void initializeOnce() {
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 物品改变回调
    /// </summary>
    protected override void onItemChanged() {
    }

    #endregion

    #region 界面绘制
    
    /// <summary>
    /// 清空视图
    /// </summary>
    protected override void clear() {
        base.clear();
    }

    #endregion

    #region 流程控制
    
    #endregion
}
