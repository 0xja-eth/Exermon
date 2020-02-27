using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;
using System.Reflection;

/// <summary>
/// 可自动转化的属性特性
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AutoConvertAttribute : Attribute {

    /// <summary>
    /// 键名
    /// </summary>
    public string keyName;

    /// <summary>
    /// 防止覆盖
    /// </summary>
    public bool preventCover;

    /// <summary>
    /// 忽略空值
    /// </summary>
    public bool ignoreNull;

    /// <summary>
    /// 转换格式
    /// </summary>
    public string format;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AutoConvertAttribute(string keyName = null, 
        bool preventCover = true, bool ignoreNull = false, string format = "") {
        this.keyName = keyName;
        this.preventCover = preventCover;
        this.ignoreNull = ignoreNull;
        this.format = format;
    }
}

/// <summary>
/// 游戏数据父类
/// </summary>
public class BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    int id; // ID（只读）
    //public int id { get; protected set; }
    public int getID() { return id; }

    /// <summary>
    /// 原始数据
    /// </summary>
    JsonData rawData;

    /// <summary>
    /// 是否需要ID
    /// </summary>
    protected virtual bool idEnable() { return true; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public void load(JsonData json) {
        rawData = json;
        Debug.Log("load: " + json.ToJson());
        loadAutoAttributes(json);
        loadCustomAttributes(json);
    }

    /// <summary>
    /// 读取自定义属性
    /// </summary>
    /// <param name="json"></param>
    protected virtual void loadCustomAttributes(JsonData json) {
        id = idEnable() ? DataLoader.load<int>(json, "id") : -1;
    }

    /// <summary>
    /// 读取自动转换属性
    /// </summary>
    void loadAutoAttributes(JsonData json) {
        var type = GetType();

        foreach (var p in type.GetProperties())
            foreach (Attribute a in p.GetCustomAttributes(false))
                if (a.GetType() == typeof(AutoConvertAttribute)) {
                    var attr = (AutoConvertAttribute)a;
                    var pType = p.PropertyType; var pName = p.Name;
                    var key = attr.keyName ?? DataLoader.hump2Underline(pName);
                    var val = p.GetValue(this);

                    var debug = string.Format("Loading {0} {1} {2} in {3} " +
                        "(ori:{4})", p, pType, pName, type, val);
                    Debug.Log(debug);

                    val = attr.preventCover ? DataLoader.load(
                        pType, val, json, key, attr.ignoreNull) : 
                        DataLoader.load(pType, json, key);
                    p.SetValue(this, val, BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
                }
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public JsonData toJson() {
        var json = new JsonData();
        json.SetJsonType(JsonType.Object);
        convertAutoAttributes(ref json);
        convertCustomAttributes(ref json);
        return json;
    }

    /// <summary>
    /// 转换自定义属性
    /// </summary>
    /// <param name="json"></param>
    protected virtual void convertCustomAttributes(ref JsonData json) {
        if (idEnable()) json["id"] = id;
    }

    /// <summary>
    /// 转换自动转换属性
    /// </summary>
    void convertAutoAttributes(ref JsonData json) {
        var type = GetType();

        foreach (var p in type.GetProperties())
            foreach (Attribute a in p.GetCustomAttributes(false))
                if (a.GetType() == typeof(AutoConvertAttribute)) {
                    var attr = (AutoConvertAttribute)a;
                    var pType = p.PropertyType; var pName = p.Name;
                    var key = attr.keyName ?? DataLoader.hump2Underline(pName);
                    var val = p.GetValue(this);

                    json[key] = DataLoader.convert(pType, val, attr.format);

                    var debug = string.Format("Converting {0} {1} in {2} (val:{3}) " +
                        "to key: {4}, res: {5}", pType, pName, type, val, key, json[key]);
                    Debug.Log(debug);
                }
    }

    /// <summary>
    /// 获取原始数据
    /// </summary>
    /// <returns>JSON字符串</returns>
    public string rawJson() { return rawData.ToJson(); }

    /// <summary>
    /// 类型转化（需要读取时候执行，因为 rawData 不会同步改变）
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <returns>目标类型对象</returns>
    public T convert<T>() where T: BaseData, new() {
        return DataLoader.load<T>(rawData);
    }
}

/// <summary>
/// 配置数据组父类
/// </summary>
public class TypeData : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public string name { get; protected set; }
    [AutoConvert]
    public string description { get; protected set; }
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        name = DataLoader.loadString(json, "name");
        description = DataLoader.loadString(json, "description");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["name"] = name;
        json["description"] = description;
        return json;
    }
    */
}

/// <summary>
/// 属性数据
/// </summary>
public class ParamData : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int paramId { get; protected set; }
    [AutoConvert]
    public double value { get; protected set; } // 真实值
    
    /// <summary>
    /// 是否需要ID
    /// </summary>
    protected override bool idEnable() { return false; }

    /// <summary>
    /// 获取基本属性
    /// </summary>
    /// <returns>基本属性</returns>
    public BaseParam param() {
        return DataService.get().baseParam(paramId);
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="val">值</param>
    public void setValue(double val) {
        value = val;
    }

    /// <summary>
    /// 增加值
    /// </summary>
    /// <param name="val">值</param>
    public void addValue(double val) {
        value += val;
    }

    /// <summary>
    /// 乘值
    /// </summary>
    /// <param name="val">值</param>
    public void timesValue(double val) {
        value *= val;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public ParamData() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public ParamData(int paramId, double value = 0) {
        this.paramId = paramId;
        this.value = value;
    }
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        paramId = DataLoader.loadInt(json, "param_id");
        value = DataLoader.loadDouble(json, "value");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["param_id"] = paramId;
        json["value"] = value;
        return json;
    }*/
}

/// <summary>
/// 属性范围数据
/// </summary>
public class ParamRangeData : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int paramId { get; protected set; }
    [AutoConvert]
    public double minValue { get; protected set; } // 真实值
    [AutoConvert]
    public double maxValue { get; protected set; } // 真实值

    /// <summary>
    /// 是否需要ID
    /// </summary>
    protected override bool idEnable() { return false; }

    /// <summary>
    /// 获取基本属性
    /// </summary>
    /// <returns>基本属性</returns>
    public BaseParam param() {
        return DataService.get().baseParam(paramId);
    }
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        paramId = DataLoader.loadInt(json, "param_id");
        minValue = DataLoader.loadDouble(json, "min_value");
        maxValue = DataLoader.loadDouble(json, "max_value");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["param_id"] = paramId;
        json["min_value"] = minValue;
        json["max_value"] = maxValue;
        return json;
    }*/
}