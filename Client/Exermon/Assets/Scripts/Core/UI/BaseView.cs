
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

    bool refreshRequested = true;
    bool clearRequested = false;

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
        shown = gameObject.activeSelf;
        initialize();
    }

    /// <summary>
    /// 唤醒
    /// </summary>
    protected override void awake() {
        base.awake();
        // 如果还没有初始化，则自动进行配置
        if (!initialized) configure();
    }

    #endregion

    #region 更新控制

    /// <summary>
    /// 更新
    /// </summary>
    protected override void update() {
        base.update();
        updateRefresh();
    }

    /// <summary>
    /// 更新刷新
    /// </summary>
    void updateRefresh() {
        if (isRefreshRequested()) refresh();
        else if (isClearRequested()) clear();
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
        requestRefresh();
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
        requestClear();
        gameObject.SetActive(shown = false);
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 请求刷新
    /// </summary>
    public void requestRefresh(bool force = false) {
        if (force) refresh();
        else refreshRequested = true;
    }

    /// <summary>
    /// 请求清除
    /// </summary>
    public void requestClear(bool force = false) {
        if (force) clear();
        else clearRequested = true;
    }

    /// <summary>
    /// 重置所有请求
    /// </summary>
    void resetRequests() {
        clearRequested = refreshRequested = false;
    }

    /// <summary>
    /// 是否需要刷新
    /// </summary>
    /// <returns>需要刷新</returns>
    protected virtual bool isRefreshRequested() {
        return shown && refreshRequested;
    }

    /// <summary>
    /// 是否需要清除
    /// </summary>
    /// <returns>需要清除</returns>
    protected virtual bool isClearRequested() {
        return clearRequested;
    }

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected virtual void refresh() {
        Debug.Log(name + " refresh");
        resetRequests();
    }

    /// <summary>
    /// 清除视窗
    /// </summary>
    protected virtual void clear() {
        Debug.Log(name + " clear");
        resetRequests();
    }

    #endregion
}
