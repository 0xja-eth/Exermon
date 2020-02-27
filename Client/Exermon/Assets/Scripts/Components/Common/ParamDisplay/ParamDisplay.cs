using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LitJson;
using UnityEngine.EventSystems;

/// <summary>
/// 属性展示
/// </summary>
public class ParamDisplay : BaseView {

    /// <summary>
    /// 常量设置
    /// </summary>
    const string DefaultTextFormat = "{0}";
    const string DefaultDateFormat = DataLoader.DisplayDateFormat;
    const string DefaultDateTimeFormat = DataLoader.DisplayDateTimeFormat;

    const string DefaultTrueText = "是";
    const string DefaultFalseText = "否";

    static readonly Color DefaultTrueColor = new Color(0, 1, 0);
    static readonly Color DefaultFalseColor = new Color(0.92549f, 0.42353f, 0.42353f);
    static readonly Color DefaultNormalColor = new Color(1, 1, 1);

    public const string TrueColorKey = "_true_color";
    public const string FalseColorKey = "_false_color";
    public const string NormalColorKey = "_normal_color";

    const string DefaultTrueSign = "+";
    const string DefaultFalseSign = "-";

    /// <summary>
    /// 能转化为属性显示数据的接口
    /// </summary>
    public interface DisplayDataConvertable {

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        JsonData convertToDisplayData(string type = "");
    }

    /// <summary>
    /// 能转化为属性显示数据的接口
    /// </summary>
    public interface DisplayDataArrayConvertable {

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        JsonData[] convertToDisplayDataArray(string type = "");
    }

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum Type {

    }

    /// <summary>
    /// 显示项
    /// </summary>
    [Serializable]
    public struct DisplayItem {
        // 键名
        public string key;
        // 对应显示的 GameObject
        public GameObject obj;
        // 对应显示的类型   Text, Sign, Percent, TimeSpan, TimeSpanWithHour, 
        //                 Date, DateTime, Color, ScaleX, ScaleY
        public string type;
        // 是否有动画效果
        public bool animated;
        // 是否为配置数据（不进行清除操作）
        public bool configData;
        // 格式
        public string format;
        // 触发模式         Click, Hover, HOC (HoverOrClick)
        public string trigger;
    }

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public EventTrigger trigger;
    
    /// <summary>
    /// 外部变量定义
    /// </summary>
    public DisplayItem[] displayItems;
    
    /// <summary>
    /// 内部变量声明
    /// </summary>
    JsonData displayData; // 要显示的数据
    JsonData rawData; // 原始数据

    bool force = false;

    #region 初始化
    
    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        processDisplayItems();
        initializeDisplayData();
    }

    /// <summary>
    /// 补充处理显示项（自动尝试对未设置的 obj 进行设置）
    /// </summary>
    void processDisplayItems() {
        for (int i = 0; i < displayItems.Length; i++) {
            var item = displayItems[i];
            bindDisplayItemEvent(item);
            if (item.obj == null) {
                var name = DataLoader.underline2UpperHump(item.key);
                displayItems[i].obj = SceneUtils.find(transform, name);
            }
        }
    }

    /// <summary>
    /// 绑定显示项事件
    /// </summary>
    /// <param name="item">显示项</param>
    void bindDisplayItemEvent(DisplayItem item) {
        if (trigger == null) return;
        if (item.trigger == "") return;
        EventTrigger.Entry entry;

        switch (item.trigger.ToLower()) {
            case "click":
                entry = new EventTrigger.Entry();
                entry.callback = new EventTrigger.TriggerEvent();
                entry.callback.AddListener((_) => refreshKey(item));
                entry.eventID = EventTriggerType.PointerClick;
                trigger.triggers.Add(entry);
                break;
            case "hover":
                entry = new EventTrigger.Entry();
                entry.callback = new EventTrigger.TriggerEvent();
                entry.callback.AddListener((_) => refreshKey(item));
                entry.eventID = EventTriggerType.PointerEnter;
                trigger.triggers.Add(entry);
                break;
            case "hoc":
                entry = new EventTrigger.Entry();
                entry.callback = new EventTrigger.TriggerEvent();
                entry.callback.AddListener((_) => refreshKey(item));
                entry.eventID = EventTriggerType.PointerEnter;
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.callback = new EventTrigger.TriggerEvent();
                entry.callback.AddListener((_) => refreshKey(item));
                entry.eventID = EventTriggerType.PointerClick;
                trigger.triggers.Add(entry);
                break;
        }
    }

    /// <summary>
    /// 初始化显示数据
    /// </summary>
    void initializeDisplayData() {
        displayData = new JsonData();
        displayData.SetJsonType(JsonType.Object);
        foreach (var item in displayItems)
            displayData[item.key] = null;
        rawData = displayData;
    }

    /// <summary>
    /// 配置组件
    /// </summary>
    public override void configure() {
        configure(new JsonData());
    }
    /// <param name="initData">初始数据</param>
    public void configure(JsonData initData) {
        base.configure();
        setValue(initData, true);
    }
    /// <param name="obj">对象</param>
    public void configure(DisplayDataConvertable obj, string type = "") {
        configure(obj.convertToDisplayData(type));
    }

    #endregion

    #region 数据控制
    
    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="value">值</param>
    public void setValue(JsonData value, bool force = false) {
        this.force = force;
        foreach (var item in displayItems)
            if (DataLoader.contains(value, item.key))
                setKey(item.key, value[item.key]);
        rawData = value;
    }
    /// <param name="obj">值对象</param>
    public void setValue(DisplayDataConvertable obj, string type = "", bool force = false) {
        setValue(obj.convertToDisplayData(type), force);
    }

    /// <summary>
    /// 设置键
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    void setKey(string key, JsonData value, bool refresh = false) {
        displayData[key] = value;
        requestRefresh(refresh);
    }

    /// <summary>
    /// 清除值
    /// </summary>
    public void clearValue() {
        force = true;
        foreach (var item in displayItems) 
            if(!item.configData) clearKey(item.key);
    }

    /// <summary>
    /// 清除键
    /// </summary>
    /// <param name="key">键</param>
    void clearKey(string key) {
        setKey(key, null, true);
    }
    
    #endregion

    #region 界面绘制

    /// <summary>
    /// 刷新所有键
    /// </summary>
    void refreshKeys() {
        foreach (var item in displayItems)
            if (item.trigger == "") refreshKey(item);
    }

    /// <summary>
    /// 刷新键
    /// </summary>
    /// <param name="item">显示项</param>
    void refreshKey(DisplayItem item) {
        var value = DataLoader.load(displayData, item.key);
        refreshKey(item, value);
    }
    /// <param name="value">值</param>
    void refreshKey(DisplayItem item, JsonData value) {
        if (item.obj == null) return;

        switch (item.type) {
            case "Text": processTextDisplayItem(item, value); break;
            case "Sign": processSignDisplayItem(item, value); break;
            case "Percent": processPercentDisplayItem(item, value); break;
            case "TimeSpan": processTimeSpanDisplayItem(item, value); break;
            case "TimeSpanWithHour": processTimeSpanDisplayItem(item, value, true); break;
            case "Date": processDateDisplayItem(item, value); break;
            case "DateTime": processDateTimeDisplayItem(item, value); break;
            case "Color": processColorDisplayItem(item, value); break;
            case "ScaleX": processScaleXDisplayItem(item, value); break;
            case "ScaleY": processScaleYDisplayItem(item, value); break;
            default: processExtended(item, value); break;
        }
    }

    #region 具体显示项处理

    /// <summary>
    /// 处理文本类型的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    void processTextDisplayItem(DisplayItem item, JsonData value) {
        var text = SceneUtils.text(item.obj);
        if (text == null) return;
        var format = item.format.Length > 0 ? item.format : DefaultTextFormat;
        if (value != null && value.IsDouble) {
            var val = DataLoader.load<double>(value);
            text.text = string.Format(format, SceneUtils.double2Str(val));
        } else if (value != null && value.IsInt)
            text.text = string.Format(format, DataLoader.load<int>(value));
        else if (value != null && value.IsBoolean) {
            var val = DataLoader.load<bool>(value);
            text.text = string.Format(format, val ? DefaultTrueText : DefaultFalseText);
        } else
            text.text = string.Format(format, value);
    }

    /// <summary>
    /// 处理符号类型的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    void processSignDisplayItem(DisplayItem item, JsonData value) {
        var text = SceneUtils.text(item.obj);
        if (text == null) return;
        var format = item.format.Length > 0 ? item.format : DefaultTextFormat;
        string signText = "";
        if (value != null && value.IsDouble) {
            var val = DataLoader.load<double>(value);
            if (val > 0) signText = DefaultTrueSign;
            text.text = signText + string.Format(format, val);
        } else if (value != null && value.IsInt) {
            var val = DataLoader.load<int>(value);
            if (val > 0) signText = DefaultTrueSign;
            text.text = signText + string.Format(format, val);
        } else if (value != null && value.IsBoolean) {
            var val = DataLoader.load<bool>(value);
            signText = val ? DefaultTrueSign : DefaultFalseSign;
            text.text = string.Format(format, signText);
        } else
            text.text = string.Format(format, value);
    }

    /// <summary>
    /// 处理百分比类型的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    void processPercentDisplayItem(DisplayItem item, JsonData value) {
        var text = SceneUtils.text(item.obj);
        if (text == null) return;
        var format = item.format.Length > 0 ? item.format : DefaultTextFormat;
        if (value != null && value.IsDouble) {
            var val = DataLoader.load<double>(value);
            text.text = string.Format(format, SceneUtils.double2Perc(val));
        } else
            text.text = string.Format(format, value + "%");
    }

    /// <summary>
    /// 处理时间段类型的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    void processTimeSpanDisplayItem(DisplayItem item, JsonData value, bool hour = false) {
        var text = SceneUtils.text(item.obj);
        if (text == null) return;
        var format = item.format.Length > 0 ? item.format : DefaultTextFormat;
        if (value != null && value.IsDouble) {
            var val = DataLoader.load<double>(value); // 单位须为秒
            var txtVal = hour ? SceneUtils.time2StrWithHour(val) : SceneUtils.time2Str(val);
            text.text = string.Format(format, txtVal);
        } else if (value != null && value.IsInt) {
            var val = DataLoader.load<int>(value); // 单位须为秒
            var txtVal = hour ? SceneUtils.time2StrWithHour(val) : SceneUtils.time2Str(val);
            text.text = string.Format(format, txtVal);
        } else
            text.text = string.Format(format, value);
    }

    /// <summary>
    /// 处理日期类型的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    void processDateDisplayItem(DisplayItem item, JsonData value) {
        var text = SceneUtils.text(item.obj);
        if (text == null) return;
        var format = item.format.Length > 0 ? item.format : DefaultDateFormat;
        var date = DataLoader.load<DateTime>(value);
        text.text = date.ToString(format);
    }

    /// <summary>
    /// 处理日期时间类型的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    void processDateTimeDisplayItem(DisplayItem item, JsonData value) {
        var text = SceneUtils.text(item.obj);
        if (text == null) return;
        var format = item.format.Length > 0 ? item.format : DefaultDateTimeFormat;
        var date = DataLoader.load<DateTime>(value);
        text.text = date.ToString(format);
    }

    /// <summary>
    /// 处理颜色类型的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    void processColorDisplayItem(DisplayItem item, JsonData value) {
        var graphic = SceneUtils.get<Graphic>(item.obj);
        if (graphic == null) return;

        var color = new Color();
        if(value != null)
            if (value.IsString) color = DataLoader.load<Color>(value);
            else if (value.IsBoolean) {
                var val = DataLoader.load<bool>(value);
                color = val ? trueColor() : falseColor();
            } else if (value.IsInt || value.IsDouble) {
                var val = DataLoader.load<Double>(value);
                if (val > 0) color = trueColor();
                else if (val < 0) color = falseColor();
                else color = normalColor();
            }

        var ani = SceneUtils.ani(item.obj);

        if (!force && item.animated && ani != null) {
            var ori = graphic.color;
            var tmpAni = AnimationUtils.createAnimation();
            tmpAni.addCurve(typeof(Graphic), "m_Color.r", ori.r, color.r);
            tmpAni.addCurve(typeof(Graphic), "m_Color.g", ori.g, color.g);
            tmpAni.addCurve(typeof(Graphic), "m_Color.b", ori.b, color.b);
            tmpAni.addCurve(typeof(Graphic), "m_Color.a", ori.a, color.a);
            tmpAni.setupAnimation(ani);
        } else graphic.color = color;
    }

    /// <summary>
    /// 获取正确颜色
    /// </summary>
    /// <returns></returns>
    Color trueColor() {
        if (DataLoader.contains(rawData, TrueColorKey))
            return DataLoader.load<Color>(rawData, TrueColorKey);
        return DefaultTrueColor;
    }

    /// <summary>
    /// 获取正确颜色
    /// </summary>
    /// <returns></returns>
    Color falseColor() {
        if (DataLoader.contains(rawData, FalseColorKey))
            return DataLoader.load<Color>(rawData, FalseColorKey);
        return DefaultFalseColor;
    }

    /// <summary>
    /// 获取常规颜色
    /// </summary>
    /// <returns></returns>
    Color normalColor() {
        if (DataLoader.contains(rawData, NormalColorKey))
            return DataLoader.load<Color>(rawData, NormalColorKey);
        return DefaultNormalColor;
    }

    /// <summary>
    /// 处理X缩放类型的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    void processScaleXDisplayItem(DisplayItem item, JsonData value) {
        var transform = item.obj.transform;
        if (transform == null) return;

        var rate = DataLoader.load<float>(value);
        var ani = SceneUtils.ani(item.obj);
        var ori = transform.localScale;

        if (!force && item.animated && ani != null) {
            var tmpAni = AnimationUtils.createAnimation();
            tmpAni.addCurve(typeof(Transform), "m_LocalScale.x", ori.x, rate);
            tmpAni.setupAnimation(ani);
        } else
            transform.localScale = new Vector3(rate, ori.y, ori.z);
    }

    /// <summary>
    /// 处理Y缩放类型的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    void processScaleYDisplayItem(DisplayItem item, JsonData value) {
        var transform = item.obj.transform;
        if (transform == null) return;

        var rate = DataLoader.load<float>(value);
        var ani = SceneUtils.ani(item.obj);
        var ori = transform.localScale;

        if (item.animated && ani != null) {
            var tmpAni = AnimationUtils.createAnimation();
            tmpAni.addCurve(typeof(Transform), "m_LocalScale.y", ori.y, rate);
            tmpAni.setupAnimation(ani);
        } else
            transform.localScale = new Vector3(ori.x, rate, ori.z);
    }

    /// <summary>
    /// 处理拓展的显示项
    /// </summary>
    /// <param name="item">显示项</param>
    /// <param name="value">值</param>
    protected virtual void processExtended(DisplayItem item, JsonData value) { }

    #endregion

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
        refreshKeys();
        force = false;
    }

    /// <summary>
    /// 清除视窗
    /// </summary>
    protected override void clear() {
        base.clear();
        clearValue();
    }

    #endregion
}