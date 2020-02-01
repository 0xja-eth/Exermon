using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 下拉列表域
/// </summary>
public class DropdownField : BaseView {

    /// <summary>
    /// 值变更回调函数类型
    /// </summary>
    public delegate string onChangeFunc(int index);

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Dropdown drowdown;

    /// <summary>
    /// 内部变量声明
    /// </summary>
    Tuple<int, string>[] options;

    /// <summary>
    /// 内部变量声明
    /// </summary>
    public onChangeFunc onChange { get; set; }

    #region 初始化
    
    /// <summary>
    /// 配置组件
    /// </summary>
    public void configure(Tuple<int, string>[] options) {
        this.options = options;
        base.configure();
        createOptions();
    }

    /// <summary>
    /// 创建所有选项
    /// </summary>
    void createOptions() {
        foreach (var opt in options)
            createOption(opt.Item2);
    }

    /// <summary>
    /// 新增选项
    /// </summary>
    /// <param name="opt">选项文本</param>
    void createOption(string opt) {
        drowdown.options.Add(new Dropdown.OptionData(opt));
    }

    /// <summary>
    /// 清除所有选项
    /// </summary>
    void clearOptions() {
        drowdown.ClearOptions();
    }

    #endregion

    #region 回调控制

    /// <summary>
    /// 值变更回调
    /// </summary>
    public void onValueChanged() {
        Debug.Log("onValueChanged: " + getIndex());
        onChange?.Invoke(getIndex());
    }

    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public void startView(int index=0) {
        base.startView();
        setIndex(index);
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 获取当前索引
    /// </summary>
    /// <returns>索引</returns>
    public int getIndex() {
        return drowdown.value;
    }

    /// <summary>
    /// 设置当前索引
    /// </summary>
    /// <param name="value">索引</param>
    public void setIndex(int index) {
        drowdown.value = index;
    }

    /// <summary>
    /// 获取选中项
    /// </summary>
    /// <returns>项</returns>
    public Tuple<int, string> getValue() {
        return options[getIndex()];
    }

    /// <summary>
    /// 设置选中项
    /// </summary>
    /// <returns>项</returns>
    public void setValue(int id) {
        for (int i = 0; i < options.Length; i++)
            if (options[i].Item1 == id) {
                setIndex(i); break;
            }
        setIndex(-1);
    }

    #endregion
    
}
