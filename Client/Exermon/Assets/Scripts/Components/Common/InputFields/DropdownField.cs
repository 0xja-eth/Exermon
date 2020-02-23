using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 下拉列表域
/// </summary>
public class DropdownField : BaseInputField<Tuple<int, string>> {

    /// <summary>
    /// 无效值提示语
    /// </summary>
    const string InvalidValueTips = "所选值无效！";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Dropdown dropdown;

    /// <summary>
    /// 内部变量声明
    /// </summary>
    Tuple<int, string>[] options = new Tuple<int, string>[0];

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        //check = check ?? defaultCheckFunc;
        processPresetedOptions();
        dropdown?.onValueChanged.AddListener(
            (index) => {
                Debug.Log("Listener: " + index);
                value = options[index];
                onValueChanged();
            });
    }

    /// <summary>
    /// 处理预先设置好的选项
    /// </summary>
    void processPresetedOptions() {
        if (options.Length > 0) return;
        var cnt = dropdown.options.Count;
        options = new Tuple<int, string>[cnt];
        for(int i = 0; i < cnt; i++) {
            var option = dropdown.options[i];
            options[i] = new Tuple<int, string>(i, option.text);
        }
    }

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
        value = options[0];
    }

    /// <summary>
    /// 新增选项
    /// </summary>
    /// <param name="opt">选项文本</param>
    void createOption(string opt) {
        dropdown.options.Add(new Dropdown.OptionData(opt));
    }

    /// <summary>
    /// 清除所有选项
    /// </summary>
    void clearOptions() {
        dropdown.ClearOptions();
    }
    
    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public void startView(int index = 0) {
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
        return dropdown.value;
    }

    /// <summary>
    /// 设置当前索引
    /// </summary>
    /// <param name="index">索引</param>
    public void setIndex(int index, bool check = true, bool emit = true) {
        setValue(options[index], check, emit);
    }

    /// <summary>
    /// 获取选中项
    /// </summary>
    /// <returns>项</returns>
    public string getValueText() {
        return getValue().Item2;
    }

    /// <summary>
    /// 获取选中项
    /// </summary>
    /// <returns>项</returns>
    public int getValueId() {
        return getValue().Item1;
    }

    /// <summary>
    /// 设置选中项
    /// </summary>
    /// <returns>项</returns>
    public void setValue(int id, bool check = true, bool emit = true) {
        for (int i = 0; i < options.Length; i++)
            if (options[i].Item1 == id) {
                setValue(options[i], check, emit); break;
            }
        base.setValue(null, check, emit);
    }

    /// <summary>
    /// 设置选中项
    /// </summary>
    /// <returns>项</returns>
    public void setValue(string text, bool check = true, bool emit = true) {
        for (int i = 0; i < options.Length; i++)
            if (options[i].Item2 == text) {
                setValue(options[i], check, emit); break;
            }
        base.setValue(null, check, emit);
    }
    /*
    /// <summary>
    /// 默认检查函数
    /// </summary>
    public string defaultCheckFunc(Tuple<int, string> value) {
        foreach (var item in options)
            if (item.Item1 == value.Item1) return "";
        return InvalidValueTips;
    }
    */
    #endregion

    #region 界面绘制

    /// <summary>
    /// 绘制值
    /// </summary>
    /// <param name="text">值</param>
    protected override void drawValue(Tuple<int, string> value) {
        Debug.Log("drawValue: " + value);
        dropdown.itemText.text = value.Item2;
        Debug.Log("drowdown.value = " + dropdown.value + 
            "\ndrowdown.itemText.text = " + dropdown.itemText.text);
    }

    #endregion

    #region 事件控制

    /// <summary>
    /// 是否实际有焦点
    /// </summary>
    public override bool isRealFocused() {
        return dropdown && 
            dropdown.gameObject == EventSystem.current.currentSelectedGameObject;
    }

    #endregion
}
