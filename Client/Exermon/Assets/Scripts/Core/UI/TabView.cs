
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LitJson;

/// <summary>
/// Tab视图
/// </summary>
public abstract class TabView<T> : GroupView<Toggle> {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public T[] contents;

    public bool switchContent = true; // 是否通过切换内容页方式进行tab

    /// <summary>
    /// 外部变量设置
    /// </summary>
    public int defaultIndex = 0;

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        initializeToggles();
    }

    /// <summary>
    /// 初始化选项
    /// </summary>
    void initializeToggles() {
        foreach(var t in subViews)
            t.onValueChanged.AddListener(onTabChanged);
    }

    #endregion

    #region 启动控制

    /// <summary>
    /// 启动组件
    /// </summary>
    public override void startView() {
        startView(defaultIndex);
    }

    /// <summary>
    /// 启动组件
    /// </summary>
    /// <param name="index">初始索引</param>
    public void startView(int index) {
        base.startView();
        setIndex(index);
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 获取当前选中的索引
    /// </summary>
    /// <returns></returns>
    public int getIndex() {
        for (int i = 0; i < subViews.Count; i++)
            if (subViews[i].isOn) return i;
        return -1;
    }

    /// <summary>
    /// 设置选中索引
    /// </summary>
    /// <param name="index"></param>
    public void setIndex(int index) {
        for (int i = 0; i < subViews.Count; i++)
            subViews[i].isOn = (index == i);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 刷新科目
    /// </summary>
    void refreshContents() {
        if (switchContent) refreshSwitchContent();
        else refreshChangeContent();
    }

    /// <summary>
    /// 刷新内容切换
    /// </summary>
    void refreshSwitchContent() {
        for (int i = 0; i < contents.Length; i++)
            if (subViews[i].isOn)
                showContent(contents[i], i);
            else
                hideContent(contents[i], i);
    }

    /// <summary>
    /// 刷新内容变更
    /// </summary>
    void refreshChangeContent() {
        if (contents.Length <= 0) return;
        var index = getIndex();
        foreach(var content in contents)
            if (index >= 0)
                showContent(content, index);
            else
                hideContent(content, index);
    }

    /// <summary>
    /// 显示内容页
    /// </summary>
    /// <param name="content"></param>
    protected abstract void showContent(T content, int index);

    /// <summary>
    /// 显示内容页
    /// </summary>
    /// <param name="content"></param>
    protected abstract void hideContent(T content, int index);

    /// <summary>
    /// 清空内容页
    /// </summary>
    void clearContents() {
        for (int i = 0; i < contents.Length; i++)
            hideContent(contents[i], i);
    }

    /// <summary>
    /// 清空Tab
    /// </summary>
    void clearTabs() {
        foreach (var toggle in subViews) toggle.isOn = false;
    }

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
        refreshContents();
    }

    /// <summary>
    /// 清空视窗
    /// </summary>
    protected override void clear() {
        base.clear();
        clearTabs();
        clearContents();
    }

    #endregion

    #region 事件控制

    /// <summary>
    /// 页变更回调
    /// </summary>
    protected virtual void onTabChanged(bool val) {
        requestRefresh();
    }

    #endregion

}
