﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏所有窗口的基类（实际上是带动画功能的View）
/// </summary>
public class BaseWindow : BaseView {

    /// <summary>
    /// 窗口状态
    /// </summary>
    public enum State {
        None, // 未设置
        Showing, // 开启中
        Shown, // 已开启
        Hiding, // 关闭中
        Hidden, // 已关闭
    }

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public GameObject background; // 窗口背景

    public new Animation animation; // 动画组件

    public string showWindowAniText = "ShowWindow"; // 显示窗口动画名
    public string hideWindowAniText = "HideWindow"; // 隐藏窗口动画名

    /// <summary>
    /// 内部变量声明
    /// </summary>
    protected State state = State.None; // 窗口状态（可以在子类自定义）
    
    #region 更新控制

    /// <summary>
    /// 更新
    /// </summary>
    protected override void update() {
        base.update();
        updateBackground();
        updateWindowState();
    }

    /// <summary>
    /// 更新窗口背景
    /// </summary>
    void updateBackground() {
        if (background != null) background.SetActive(isVisibleState());
    }

    /// <summary>
    /// 更新窗口状态
    /// </summary>
    void updateWindowState() {
        switch(state) {
            case State.Showing: if (!isPlaying(showWindowAniText)) onWindowShown(); break;
            case State.Hiding: if (!isPlaying(hideWindowAniText)) onWindowHidden(); break;
        }
    }

    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动窗口
    /// </summary>
    public virtual void startWindow() {
        base.startView();
    }

    /// <summary>
    /// 显示窗口（视窗）
    /// </summary>
    protected override void showView() {
        base.showView();
        changeState(State.Showing);
        if(animation) {
            animation.Play(showWindowAniText);
            animation.wrapMode = WrapMode.Once;
        }
    }

    /// <summary>
    /// 结束窗口
    /// </summary>
    public virtual void terminateWindow() {
        base.terminateView();
    }

    /// <summary>
    /// 隐藏窗口（视窗）
    /// </summary>
    protected override void hideView() {
        changeState(State.Hiding);
        if (animation) {
            animation.Play(hideWindowAniText);
            animation.wrapMode = WrapMode.Once;
        }
    }
    
    /// <summary>
    /// 窗口完全显示回调
    /// </summary>
    void onWindowShown() {
        changeState(State.Shown);
        refresh();
    }

    /// <summary>
    /// 窗口完全隐藏回调
    /// </summary>
    void onWindowHidden() {
        base.hideView();
        changeState(State.Hidden);
        updateBackground();
    }

    #endregion

    #region 状态控制

    /// <summary>
    /// 改变状态
    /// </summary>
    /// <param name="state">新状态</param>
    public void changeState(State state) {
        this.state = state;
    }

    /// <summary>
    /// 获取状态
    /// </summary>
    /// <returns>当前状态</returns>
    public State getState() { return state; }

    /// <summary>
    /// 判断是否处于可视状态
    /// </summary>
    /// <returns>是否可视状态</returns>
    bool isVisibleState() {
        return state != State.None && state != State.Hidden;
    }

    /// <summary>
    /// 是否在播放动画
    /// </summary>
    /// <param name="aniName">动画名称</param>
    /// <returns>是否播放</returns>
    bool isPlaying(string aniName) {
        return animation && animation.IsPlaying(aniName);
    }

    #endregion

    #region 界面控制
    /*
    /// <summary>
    /// 刷新窗口
    /// </summary>
    public new virtual void refresh() {
        clear(); Debug.Log(name + " refresh");
    }

    /// <summary>
    /// 清除窗口
    /// </summary>
    public new virtual void clear() {
        Debug.Log(name + " clear");
    }
    */
    #endregion
}
