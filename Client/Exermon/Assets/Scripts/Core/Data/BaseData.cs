using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 游戏数据父类
/// </summary>
public class BaseData {
    int id; // ID（只读）
    public int getID() { return id; }

    /// <summary>
    /// 是否需要ID
    /// </summary>
    protected virtual bool idEnable() { return true; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public virtual void load(JsonData json) {
        id = idEnable() ? DataLoader.loadInt(json, "id") : -1;
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public virtual JsonData toJson() {
        var json = new JsonData();
        json.SetJsonType(JsonType.Object);
        if (idEnable()) json["id"] = id;
        return json;
    }
}

/// <summary>
/// 配置数据组父类
/// </summary>
public class TypeData : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    public string name { get; private set; }
    public string description { get; private set; }

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
}

/// <summary>
/// 属性数据
/// </summary>
public class ParamData : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    public int paramId { get; private set; }
    public double value { get; private set; } // 真实值

    /// <summary>
    /// 获取基本属性
    /// </summary>
    /// <returns>基本属性</returns>
    public BaseParam param() {
        return DataService.get().baseParam(paramId);
    }

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
    }
}

/// <summary>
/// 属性范围数据
/// </summary>
public class ParamRangeData : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    public int paramId { get; private set; }
    public double minValue { get; private set; } // 真实值
    public double maxValue { get; private set; } // 真实值

    /// <summary>
    /// 获取基本属性
    /// </summary>
    /// <returns>基本属性</returns>
    public BaseParam param() {
        return DataService.get().baseParam(paramId);
    }

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
    }
}