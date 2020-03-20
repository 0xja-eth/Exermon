
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
}
