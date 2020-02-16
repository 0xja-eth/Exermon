using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 日期选择器
/// </summary>
public class DateTimePicker : BaseView, 
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler {

    /// <summary>
    /// 日期类型
    /// </summary>
    public enum Type {
        Year, Month, Day, Hour, Minute, Second
    }

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Transform container; // 容器物体

    /// <summary>
    /// 外部变量设置
    /// </summary>
    public Type type; // 日期类型
    public int itemCount = 2; // 备选数量（上下对称）
    public int threshold = 15; // 灵敏度
    public int fontSize = 18; // 每个数值的字体大小

    public Color currentTextColor = new Color(1, 1, 1); // 当前项的文本颜色
    public Color minTextColor = new Color(0.2f, 0.2f, 0.2f); // 最远项的文本颜色

    /// <summary>
    /// 预制件设置
    /// </summary>
    public GameObject itemPerfab; // 项预制件

    /// <summary>
    /// 内部变量声明
    /// </summary>
    DateTimeField field;

    Text[] items; // 项数组

    Vector2 lastDragPos;

    int value, minValue = 1, maxValue = 12; // 当前值/最大值/最小值

    bool enter = false;

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        setupItems();
    }

    /// <summary>
    /// 配置项
    /// </summary>
    void setupItems() {
        var cnt = itemsCount();
        items = new Text[cnt];
        for (int i = 0; i < cnt; ++i) {
            float dist = Math.Abs(i - currentItemIndex());
            var go = Instantiate(itemPerfab, container);
            var text = SceneUtils.text(go);
            var rate = 1 - dist / itemCount;
            text.color = minTextColor + 
                (currentTextColor - minTextColor) * rate;
            text.fontSize = fontSize;
            items[i] = text;
        }
    }

    /// <summary>
    /// 配置组件
    /// </summary>
    public void configure(DateTimeField field) {
        this.field = field;
        base.configure();
    }

    #endregion

    #region 更新控制

    /// <summary>
    /// 更新
    /// </summary>
    protected override void update() {
        base.update();
        updateScroll();
    }

    /// <summary>
    /// 更新滚轮事件
    /// </summary>
    void updateScroll() {
        if (!enter) return;
        var delta = Input.GetAxis("Mouse ScrollWheel");
        if (delta < 0) nextValue();
        else if (delta > 0) prevValue();
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 项数量
    /// </summary>
    /// <returns></returns>
    int itemsCount() {
        return itemCount * 2 + 1;
    }

    /// <summary>
    /// 获取当前选中项的索引
    /// </summary>
    /// <returns>索引</returns>
    int currentItemIndex() {
        return itemCount; // 项最大值/2 向下取整
    }

    /// <summary>
    /// 值是否有效
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>是否有效</returns>
    bool isValidValue(int value) {
        return value >= minValue && value <= maxValue;
    }

    /// <summary>
    /// 设置最大最小值
    /// </summary>
    public void setMinMaxValues(int min, int max) {
        minValue = min; maxValue = max;
        setValue(value);
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="val">值</param>
    public void setValue(int val, bool force=false) {
        Debug.Log(type + " setValue: " + val);
        val = Mathf.Clamp(val, minValue, maxValue);
        if (!force && value == val) return;
        value = val;
        onValueChanged();
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="dateTime">日期</param>
    public void setValue(DateTime dateTime, bool force = false) {
        switch(type) {
            case Type.Year:
                setValue(dateTime.Year, force); break;
            case Type.Month:
                setValue(dateTime.Month, force); break;
            case Type.Day:
                setValue(dateTime.Day, force); break;
            case Type.Hour:
                setValue(dateTime.Hour, force); break;
            case Type.Minute:
                setValue(dateTime.Minute, force); break;
            case Type.Second:
                setValue(dateTime.Second, force); break;
        }
    }

    /// <summary>
    /// 值改变回调
    /// </summary>
    void onValueChanged() {
        requestRefresh();
    }

    /// <summary>
    /// 下一个值
    /// </summary>
    public void nextValue() {
        setValue(value + 1);
        field?.updateValue(1, type);
    }

    /// <summary>
    /// 上一个值
    /// </summary>
    public void prevValue() {
        setValue(value - 1);
        field?.updateValue(-1, type);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 刷新项
    /// </summary>
    void refreshItems() {
        for (int i = 0; i < items.Length; ++i) {
            var dist = i - currentItemIndex();
            var value = this.value + dist;
            setItemValue(i, value);
        }
    }

    /// <summary>
    /// 设置项的值
    /// </summary>
    /// <param name="value"></param>
    void setItemValue(int index, int value) {
        var text = items[index];
        text.text = isValidValue(value) ? value.ToString() : "";
    }

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
        refreshItems();
    }

    #endregion

    #region 事件控制

    /// <summary>
    /// 指针进入回调
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData) {
        enter = true;
    }

    /// <summary>
    /// 指针退出回调
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData) {
        enter = false;
    }

    /// <summary>
    /// 开始拖拽事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnBeginDrag(PointerEventData eventData) {
        lastDragPos = eventData.position;
    }

    /// <summary>
    /// 拖拽中回调事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnDrag(PointerEventData eventData) {
        updateDragging(eventData);
    }

    /// <summary>
    /// 更新拖拽
    /// </summary>
    /// <param name="eventData">事件数据</param>
    void updateDragging(PointerEventData eventData) {
        var delta = eventData.position.y - lastDragPos.y;
        if(Math.Abs(delta) >= threshold) {
            if (delta > 0) nextValue();
            else prevValue();
            lastDragPos = eventData.position;
        }
    }

    /// <summary>
    /// 拖拽结束事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnEndDrag(PointerEventData eventData) {
        //_itemParent.localPosition = Vector3.zero;
    }

    #endregion
}