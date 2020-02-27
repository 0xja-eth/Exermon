﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 赛季数据
/// </summary>
public class CompSeason : TypeData {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public DateTime startTime { get; protected set; }
    [AutoConvert]
    public DateTime endTime { get; protected set; }
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        startTime = DataLoader.loadDateTime(json, "start_time");
        endTime = DataLoader.loadDateTime(json, "end_time");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["start_time"] = DataLoader.convertDateTime(startTime);
        json["end_time"] = DataLoader.convertDateTime(endTime);

        return json;
    }*/
}

/// <summary>
/// 段位数据
/// </summary>
public class CompRank : TypeData {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public Color color { get; protected set; }
    [AutoConvert]
    public int subRankNum { get; protected set; }
    [AutoConvert]
    public int scoreFactor { get; protected set; }
    [AutoConvert]
    public int offsetFactor { get; protected set; }
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        color = DataLoader.loadColor(json, "color");
        subRankNum = DataLoader.loadInt(json, "sub_rank_num");
        scoreFactor = DataLoader.loadInt(json, "score_factor");
        offsetFactor = DataLoader.loadInt(json, "offset_factor");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["color"] = DataLoader.convertColor(color);
        json["sub_rank_num"] = subRankNum;
        json["score_factor"] = scoreFactor;
        json["offset_factor"] = offsetFactor;

        return json;
    }*/
}
