
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 题目数据
/// </summary>
public class Question : BaseData {

    /// <summary>
    /// 题目选项数据
    /// </summary>
    public class Choice : BaseData {

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int order { get; protected set; }
        [AutoConvert]
        public string text { get; protected set; }
        [AutoConvert]
        public bool answer { get; protected set; }
    }

    /// <summary>
    /// 图片
    /// </summary>
    public class Picture : BaseData {

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int number { get; protected set; }
        [AutoConvert]
        public Texture2D data { get; protected set; }
    }

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int number { get; protected set; }
    [AutoConvert]
    public string title { get; protected set; }
    [AutoConvert]
    public string description { get; protected set; }
    [AutoConvert]
    public string source { get; protected set; }
    [AutoConvert]
    public int starId { get; protected set; }
    [AutoConvert]
    public int level { get; protected set; }
    [AutoConvert]
    public int score { get; protected set; }
    [AutoConvert]
    public int subjectId { get; protected set; }
    [AutoConvert]
    public int type { get; protected set; }
    [AutoConvert]
    public int status { get; protected set; }

    [AutoConvert]
    public DateTime createTime { get; protected set; }

    [AutoConvert]
    public Choice[] choices { get; protected set; }
    [AutoConvert]
    public Picture[] pictures { get; protected set; }

    #region 数据操作

    /// <summary>
    /// 星级实例
    /// </summary>
    /// <returns></returns>
    public QuesStar star() {
        return DataService.get().quesStar(starId);
    }

    /// <summary>
    /// 科目实例
    /// </summary>
    /// <returns></returns>
    public Subject subject() {
        return DataService.get().subject(subjectId);
    }

    /// <summary>
    /// 类型文本
    /// </summary>
    /// <returns></returns>
    public string typeText() {
        return DataService.get().questionType(type).Item2;
    }

    /// <summary>
    /// 状态文本
    /// </summary>
    /// <returns></returns>
    public string statusText() {
        return DataService.get().questionStatus(status).Item2;
    }

    #endregion

}

/// <summary>
/// 有限物品数据
/// </summary>
public class QuesSugar : BaseItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int questionId { get; protected set; }
    [AutoConvert]
    public ItemPrice buyPrice { get; protected set; }
    [AutoConvert]
    public int sellPrice { get; protected set; }
    [AutoConvert]
    public int getRate { get; protected set; }
    [AutoConvert]
    public int getCount { get; protected set; }

    [AutoConvert("params")]
    public ParamData[] params_ { get; protected set; }

    /// <summary>
    /// 获取装备的属性
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData getParam(int paramId) {
        foreach (var param in params_)
            if (param.paramId == paramId) return param;
        return new ParamData(paramId);
    }
}

/// <summary>
/// 题目糖背包项
/// </summary>
public class QuesSugarPackItem : PackContItem<QuesSugar> {
    
    /// <summary>
    /// 物品
    /// </summary>
    /// <returns></returns>
    public QuesSugar sugar() { return item(); }
    
}

/// <summary>
/// 题目反馈数据
/// </summary>
public class QuesReport : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int questionId { get; protected set; }
    [AutoConvert]
    public int type { get; protected set; }
    [AutoConvert]
    public string description { get; protected set; }
    [AutoConvert]
    public DateTime createTime { get; protected set; }
    [AutoConvert]
    public string result { get; protected set; }
    [AutoConvert]
    public DateTime resultTime { get; protected set; }
}