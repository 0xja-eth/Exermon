﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 所有输入域基类
/// </summary>
public class BaseInputField<T> : BaseView {

    /// <summary>
    /// 检查函数类型
    /// </summary>
    public delegate string checkFunc(T value);
    public delegate void onChangeFunc(T value);

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Text explainer; // 错误提示
    public GameObject correct, wrong; // 图标提示

    /// <summary>
    /// 内部变量声明
    /// </summary>
    public checkFunc check { get; set; }
    public onChangeFunc onChanged { get; set; }

    protected T value = default; // 值

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
    }

    #endregion

    #region 监控输入

    /// <summary>
    /// 内容变化回调
    /// </summary>
    public virtual void onValueChanged(bool check = true, bool emit = true) {
        Debug.Log(name + " onValueChanged");
        requestRefresh();
        if (check) doCheck();
        if (emit) onChanged?.Invoke(getValue());
    }

    /// <summary>
    /// 执行校验
    /// </summary>
    /// <param name="display">判断后是否显示校验信息</param>
    public string doCheck(bool display = true) {
        if (check == null) return "";
        var res = check.Invoke(getValue());
        if (display) displayCheckResult(res);
        return res;
    }

    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public void startView(T value) {
        base.startView();
        setValue(value);
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 获取内容值
    /// </summary>
    /// <returns>内容</returns>
    public virtual T getValue() { return value; }

    /// <summary>
    /// 设置内容值
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="check">是否更新状态</param>
    /// <param name="emit">是否回调</param>
    public void setValue(T value, bool check = true, bool emit = true) {
        if (assignValue(value))
            onValueChanged(check, emit);
    }

    /// <summary>
    /// 赋值
    /// </summary>
    /// <param name="value"></param>
    protected virtual bool assignValue(T value) {
        this.value = value;
        return true;
    }

    /// <summary>
    /// 判断是否正确
    /// </summary>
    /// <param name="display">判断后是否显示校验信息</param>
    /// <returns>是否正确</returns>
    public bool isCorrect(bool display = true) {
        return doCheck(display) == "";
    }

    #endregion

    #region 画面绘制

    /// <summary>
    /// 刷新值
    /// </summary>
    void refreshValue() {
        drawValue(value);
    }

    /// <summary>
    /// 绘制值
    /// </summary>
    /// <param name="value">值</param>
    protected virtual void drawValue(T value) { }

    /// <summary>
    /// 清除值
    /// </summary>
    protected virtual void clearValue() {
        setValue(default, false, false);
        requestRefresh(true);
    }

    #region 状态绘制

    /// <summary>
    /// 设置状态
    /// </summary>
    /// <param name="corr">正误</param>
    public void setStatus(bool corr) {
        if (corr) showCorrect();
        else showWrong();
    }

    /// <summary>
    /// 清空状态
    /// </summary>
    public void clearStatus() {
        correct?.SetActive(false);
        wrong?.SetActive(false);
    }

    /// <summary>
    /// 显示正确
    /// </summary>
    public void showCorrect() {
        correct?.SetActive(true);
        wrong?.SetActive(false);
    }

    /// <summary>
    /// 显示正错误
    /// </summary>
    public void showWrong() {
        correct?.SetActive(false);
        wrong?.SetActive(true);
    }

    /// <summary>
    /// 显示校验结果
    /// </summary>
    /// <param name="res">校验结果</param>
    public void displayCheckResult(string res) {
        setStatus(res == "");
        setExplainerText(res);
    }

    /// <summary>
    /// 设置提示文本
    /// </summary>
    /// <param name="text">提示文本</param>
    public void setExplainerText(string text) {
        if (explainer != null) explainer.text = text;
    }

    /// <summary>
    /// 清空提示文本
    /// </summary>
    public void clearExplainerText() {
        setExplainerText("");
    }

    #endregion

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
        refreshValue();
    }

    /// <summary>
    /// 清空内容
    /// </summary>
    protected override void clear() {
        base.clear();
        clearValue();
        clearStatus();
        clearExplainerText();
    }

    #endregion

}