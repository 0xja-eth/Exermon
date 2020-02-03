using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 滚动列表项（可自行拓展）
/// </summary>
public class ScrollListItem : BaseView {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Button button; // 按钮组件
    public Image image; // 背景图片
    public Text text; // 文字组件
    public Toggle toggle; // toggle组件

    public Color selectedColor = new Color(1, 1, 1, 1); // 当前选中颜色
    public Color checkedColor = new Color(1, 1, 1, 1); // 选择颜色

    public string itemTextFormat = "{0}. {1}"; // 项目文本格式

    public string itemText { get; set; } // 文本
    public string itemTag { get; set; } // 标签
    public int index { get; set; } // 标签

    /// <summary>
    /// 按钮点击回调函数
    /// </summary>
    /// <param name="index">下标/索引（从0开始）</param>
    public delegate void onClick(int index);

    /// <summary>
    /// 内部变量声明
    /// </summary>
    Color normalColor; // 默认颜色

    bool selected = false; // 是否当前选中
    bool _checked = false; // 是否选择

    ScrollList list; // 列表组件

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        if (image) normalColor = image.color;
        onSelectChange();
    }

    #endregion

    #region 更新控制

    /// <summary>
    /// 更新
    /// </summary>
    protected override void update() {
        updateIndex(); base.update();
    }

    /// <summary>
    /// 更新当前索引
    /// </summary>
    public void updateIndex() {
        if (!list.isDirty) return;
        refresh();
    }

    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public virtual void startView(ScrollList list, string text, string tag, onClick callback) {
        this.list = list; base.startView(); setItem(text, tag); setButtonCallBack(callback); 
    }

    #endregion

    #region 回调控制

    /// <summary>
    /// 设置回调
    /// </summary>
    public void setButtonCallBack(onClick func) {
        if (!button) return;
        clearButtonCallBack();
        button.onClick.AddListener(() => func.Invoke(index));
    }

    /// <summary>
    /// 清空回调
    /// </summary>
    public void clearButtonCallBack() {
        if (!button) return;
        button.onClick.RemoveAllListeners();
    }

    #endregion

    #region 状态控制

    /// <summary>
    /// 设置选中状态
    /// </summary>
    /// <param name="_checked">是否选中</param>
    public void setChecked(bool _checked) {
        if (this._checked != _checked) {
            if (toggle) toggle.isOn = _checked;
            this._checked = _checked;
            onSelectChange();
        }
    }

    /// <summary>
    /// 设置当前选择状态
    /// </summary>
    /// <param name="selected">是否当前选择</param>
    public void setSelected(bool selected) {
        if (this.selected != selected) {
            this.selected = selected;
            onSelectChange();
        }
    }

    /// <summary>
    /// 当选中情况改变
    /// </summary>
    protected virtual void onSelectChange() {
        if (!image) return;
        if (selected) image.color = selectedColor;
        else if (_checked) image.color = checkedColor;
        else image.color = normalColor;
    }

    #endregion

    #region 内容控制

    /// <summary>
    /// 设置项目
    /// </summary>
    /// <param name="item">项文本</param>
    /// <param name="tag">项标签</param>
    public void setItem(string item, string tag) {
        itemText = item; itemTag = tag;
        requestRefresh();
    }

    /// <summary>
    /// 刷新
    /// </summary>
    protected override void refresh() {
        index = list.getListItemId(itemText, itemTag);
        text.text = generateItemText();
    }

    /// <summary>
    /// 生成最终项目文本
    /// </summary>
    /// <returns>项目文本</returns>
    protected virtual string generateItemText() {
        return string.Format(itemTextFormat, index+1, itemText, itemTag);
    }

    #endregion
}
