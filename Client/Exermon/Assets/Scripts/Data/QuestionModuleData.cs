
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
        /*
        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        public override void load(JsonData json) {
            base.load(json);

            order = DataLoader.loadInt(json, "order");
            text = DataLoader.loadString(json, "text");
            answer = DataLoader.loadBool(json, "answer");
        }

        /// <summary>
        /// 获取JSON数据
        /// </summary>
        /// <returns>JsonData</returns>
        public override JsonData toJson() {
            var json = base.toJson();

            json["order"] = order;
            json["text"] = text;
            json["answer"] = answer;

            return json;
        }
        */
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
        /*
        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        public override void load(JsonData json) {
            base.load(json);

            number = DataLoader.loadInt(json, "number");
            data = DataLoader.loadTexture2D(json, "data");
        }

        /// <summary>
        /// 获取JSON数据
        /// </summary>
        /// <returns>JsonData</returns>
        public override JsonData toJson() {
            var json = base.toJson();

            json["number"] = number;
            json["data"] = DataLoader.convertTexture2D(data);

            return json;
        }*/
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        number = DataLoader.loadInt(json, "number");
        title = DataLoader.loadString(json, "title");
        description = DataLoader.loadString(json, "description");
        source = DataLoader.loadString(json, "source");
        starId = DataLoader.loadInt(json, "star_id");
        level = DataLoader.loadInt(json, "level");
        subjectId = DataLoader.loadInt(json, "subject_id");
        type = DataLoader.loadInt(json, "type");
        status = DataLoader.loadInt(json, "status");
        createTime = DataLoader.loadDateTime(json, "create_time");

        choices = DataLoader.loadDataArray<Choice>(json, "choices");
        pictures = DataLoader.loadDataArray<Picture>(json, "pictures");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["number"] = number;
        json["title"] = title;
        json["description"] = description;
        json["source"] = source;
        json["star_id"] = starId;
        json["level"] = level;
        json["subject_id"] = subjectId;
        json["type"] = type;
        json["status"] = status;
        json["create_time"] = DataLoader.convertDateTime(createTime);

        json["choices"] = DataLoader.convertDataArray(choices);
        json["pictures"] = DataLoader.convertDataArray(pictures);

        return json;
    }*/
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        questionId = DataLoader.loadInt(json, "question_id");

        buyPrice = DataLoader.loadData<ItemPrice>(json, "buy_price");
        sellPrice = DataLoader.loadInt(json, "sell_price");
        getRate = DataLoader.loadInt(json, "get_rate");
        getCount = DataLoader.loadInt(json, "get_count");

        params_ = DataLoader.loadDataArray<ParamData>(json, "params");

    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["question_id"] = questionId;

        json["buy_price"] = DataLoader.convertData(buyPrice);
        json["sell_price"] = sellPrice;
        json["get_rate"] = getRate;
        json["get_count"] = getCount;

        json["params"] = DataLoader.convertDataArray(params_);

        return json;
    }*/
}

/// <summary>
/// 题目糖背包项
/// </summary>
public class QuesSugarPackItem : PackContItem {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.QuesSugarPackItem; }

    /// <summary>
    /// 物品
    /// </summary>
    /// <returns></returns>
    public QuesSugar sugar() {
        return DataService.get().quesSugar(itemId);
    }
    
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        questionId = DataLoader.loadInt(json, "question_id");
        type = DataLoader.loadInt(json, "type");
        description = DataLoader.loadString(json, "description");
        createTime = DataLoader.loadDateTime(json, "create_time");
        result = DataLoader.loadString(json, "result");
        resultTime = DataLoader.loadDateTime(json, "result_time");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["question_id"] = questionId;
        json["type"] = type;
        json["description"] = description;
        json["create_time"] = DataLoader.convertDateTime(createTime);
        json["result"] = result;
        json["result_time"] = DataLoader.convertDateTime(resultTime);

        return json;
    }*/
}