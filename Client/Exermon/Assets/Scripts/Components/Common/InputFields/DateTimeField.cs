
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 日期选择域
/// </summary>
public class DateTimeField : BaseView {

    /// <summary>
    /// 值变更回调函数类型
    /// </summary>
    public delegate string onChangeFunc(DateTime dateTime);

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Text valueText;
    public Transform pickersParent;
    public GameObject pickersContaienr;
    public DateTimePicker[] pickers;

    /// <summary>
    /// 外部变量设置
    /// </summary>
    public DateTime defaultDateTime = DateTime.Now;
    public DateTime minDateTime = new DateTime(1900, 1, 1);
    public DateTime maxDateTime = DateTime.Now;

    public string dateFormat = "yyyy 年 MM 月 dd 日";

    /// <summary>
    /// 内部变量声明
    /// </summary>
    DateTime dateTime;

    DateTimePicker year, month, day, hour, minute, second;

    Transform oriParent;

    /// <summary>
    /// 内部变量声明
    /// </summary>
    public onChangeFunc onChange { get; set; }

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        setupPickers();
    }

    /// <summary>
    /// 配置日期选择项
    /// </summary>
    void setupPickers() {
        year = getPicker(DateTimePicker.Type.Year);
        month = getPicker(DateTimePicker.Type.Month);
        day = getPicker(DateTimePicker.Type.Day);
        hour = getPicker(DateTimePicker.Type.Hour);
        minute = getPicker(DateTimePicker.Type.Minute);
        second = getPicker(DateTimePicker.Type.Second);

        dateTime = defaultDateTime;
        updatePickersRange();

        foreach (var picker in pickers) 
            picker.configure(this);
    }

    #endregion

    #region 回调控制

    /// <summary>
    /// 值变更回调
    /// </summary>
    public void onValueChanged(bool emit=true) {
        Debug.Log("onValueChanged: " + dateTime);
        requestRefresh();
        updatePickersRange();
        if(emit) onChange?.Invoke(getValue());
    }

    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public void startView(DateTime dateTime) {
        base.startView();
        setValue(dateTime);
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 获取日期选择器
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>日期选择器</returns>
    DateTimePicker getPicker(DateTimePicker.Type type) {
        foreach (var picker in pickers)
            if (picker.type == type) return picker;
        return null;
    }

    /// <summary>
    /// 是否选择中
    /// </summary>
    /// <returns>选择中</returns>
    bool isSelecting() {
        return pickersContaienr.activeSelf;
    }

    /// <summary>
    /// 获取当前值
    /// </summary>
    /// <returns>当前值</returns>
    public DateTime getValue() {
        return dateTime;
    }

    /// <summary>
    /// 设置当前值
    /// </summary>
    /// <param name="value">值</param>
    public void setValue(DateTime dateTime, bool emit = true) {
        if (this.dateTime == dateTime) return;
        this.dateTime = dateTime;
        if (this.dateTime > maxDateTime) this.dateTime = maxDateTime;
        if (this.dateTime < minDateTime) this.dateTime = minDateTime;
        onValueChanged(emit);
    }

    /// <summary>
    /// 更新值
    /// </summary>
    public void updateValue(int delta, DateTimePicker.Type type) {
        Debug.Log(type + " updateValue: " + delta);
        switch (type) {
            case DateTimePicker.Type.Year:
                setValue(dateTime.AddYears(delta), false); break;
            case DateTimePicker.Type.Month:
                setValue(dateTime.AddMonths(delta), false); break;
            case DateTimePicker.Type.Day:
                setValue(dateTime.AddDays(delta), false); break;
            case DateTimePicker.Type.Hour:
                setValue(dateTime.AddHours(delta), false); break;
            case DateTimePicker.Type.Minute:
                setValue(dateTime.AddMinutes(delta), false); break;
            case DateTimePicker.Type.Second:
                setValue(dateTime.AddSeconds(delta), false); break;
        }
    }

    /// <summary>
    /// 更新日期选择器的范围
    /// </summary>
    void updatePickersRange() {
        var min = minDateTime;
        var max = maxDateTime;
        var year = dateTime.Year;
        var month = dateTime.Month;
        var minMonth = (year == min.Year) ? min.Month : 1;
        var maxMonth = (year == max.Year) ? max.Month : 12;
        var minDay = (year == min.Year && month == min.Month) ? min.Day : 1;
        var maxDay = (year == max.Year && month == max.Month) ? 
            max.Day : DateTime.DaysInMonth(year, month);

        this.year?.setMinMaxValues(min.Year, max.Year);
        this.month?.setMinMaxValues(minMonth, maxMonth);
        day?.setMinMaxValues(minDay, maxDay);
        hour?.setMinMaxValues(0, 23);
        minute?.setMinMaxValues(0, 59);
        second?.setMinMaxValues(0, 59);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 刷新日期选择器
    /// </summary>
    void refreshPickers() {
        foreach(var picker in pickers) 
            picker.setValue(dateTime, true);
    }

    /// <summary>
    /// 绘制时间文本
    /// </summary>
    void drawDateTime() {
        valueText.text = dateTime.ToString(dateFormat);
    }

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
        drawDateTime();
        if (pickersContaienr.activeSelf)
            refreshPickers();
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 开始选择
    /// </summary>
    public void startSelect() {
        pickersContaienr.SetActive(true);
        oriParent = pickersContaienr.transform.parent;
        pickersContaienr.transform.SetParent(pickersParent);
        refreshPickers();
    }

    /// <summary>
    /// 结束选择
    /// </summary>
    public void endSelect() {
        pickersContaienr.SetActive(false);
        pickersContaienr.transform.SetParent(oriParent);
        onValueChanged();
    }

    /// <summary>
    /// 反转选择
    /// </summary>
    public void toggleSelect() {
        if (isSelecting()) endSelect();
        else startSelect();
    }

    #endregion

}
