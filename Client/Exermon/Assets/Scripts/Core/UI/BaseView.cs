
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏所有视图的基类
/// </summary>
public class BaseView : BaseComponent {

    /// <summary>
    /// 外部组件设置
    /// </summary>

    /// <summary>
    /// 内部变量声明
    /// </summary>
    public bool initialized { get; protected set; } = false; // 初始化标志
    public bool shown { get; protected set; } = false;

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    void initialize() {
        initializeEvery();
        if (initialized) return;
        initialized = true;
        initializeOnce();
    }

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected virtual void initializeOnce() { }

    /// <summary>
    /// 每次打开时初始化（子类中重载）
    /// </summary>
    protected virtual void initializeEvery() { }

    /// <summary>
    /// 配置组件
    /// </summary>
    public virtual void configure() {
        initialize();
    }
    
    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public virtual void startView() {
        initialize(); showView();
    }

    /// <summary>
    /// 显示视窗
    /// </summary>
    protected virtual void showView() {
        gameObject.SetActive(shown = true);
        refresh();
    }

    /// <summary>
    /// 结束视窗
    /// </summary>
    public virtual void terminateView() {
        hideView();
    }

    /// <summary>
    /// 隐藏视窗
    /// </summary>
    protected virtual void hideView() {
        clear();
        gameObject.SetActive(shown = false);
    }

    #endregion
    
    #region 界面控制

    /// <summary>
    /// 刷新视窗（clear后重绘）
    /// </summary>
    public virtual void refresh() {
        clear(); Debug.Log(name + " refresh");
    }

    /// <summary>
    /// 清除视窗
    /// </summary>
    public virtual void clear() {
        Debug.Log(name + " clear");
    }

    #endregion
}
