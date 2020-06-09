
using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;
using RecordModule.Data;

using UI.Common.Controls.ParamDisplays;
using System.Linq;

/// <summary>
/// 题目模块
/// </summary>
namespace QuestionModule { }

/// <summary>
/// 题目模块数据
/// </summary>
namespace QuestionModule.Data {

    /// <summary>
    /// 题目数据
    /// </summary>
    public class BaseQuestion : BaseData, ParamDisplay.IDisplayDataConvertable {

        /// <summary>
        /// 题目类型
        /// </summary>
        public enum Type {
            Single = 0,  // 单选题
            Multiple = 1,  // 多选题
            Judge = 2,  // 判断题
            Other = -1,  // 其他
        }
        
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

            /// <summary>
            /// 是否选择
            /// </summary>
            /// <param name="result">结果</param>
            /// <returns>返回该选项是否在所给的选择中</returns>
            public bool isInSelection(IQuestionResult result) {
                return isInSelection(result.getSelection());
            }
            /// <param name="selection">选择</param>
            public bool isInSelection(int[] selection) {
                for (int i = 0; i < selection.Length; ++i)
                    if (selection[i] == order) return true;
                return false;
            }
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
        public int type { get; protected set; }

        public Choice[] choices { get; protected set; }

        /// <summary>
        /// 打乱的选项
        /// </summary>
        Choice[] shuffledChoices { get; set; } = null;

        #region 数据转换

        /// <summary>
        /// 转化为显示数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>返回转化后的显示数据</returns>
        public virtual JsonData convertToDisplayData(string type = "") {
            return toJson();
        }

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json)
        {
            base.loadCustomAttributes(json);
            var data = DataLoader.load(json, "choices");
            if (data != null && data.IsArray)
            {
                List<Choice> tempChoices = new List<Choice>();
                for (int i = 0; i < data.Count; i++)
                {
                    var item = loadChoice(data[i]);
                    if (item != null)
                        tempChoices.Add(item);
                }
                choices = tempChoices.ToArray();
                int a = 5;
            }
        }

        protected virtual BaseQuestion.Choice loadChoice(JsonData data)
        {
            return DataLoader.load<Choice>(data);
        }

        #endregion

        #region 数据操作
        
        /// <summary>
        /// 类型文本
        /// </summary>
        /// <returns></returns>
        public string typeText() {
            return DataService.get().questionType(type).Item2;
        }
        
        /// <summary>
        /// 打乱的选项
        /// </summary>
        /// <returns>返回打乱的选项</returns>
        public Choice[] shuffleChoices() {
            if (shuffledChoices == null) {
                int map = 0, cnt = choices.Length;
                var res = new Choice[cnt];
                for (int i = 0; i < cnt; ++i) {
                    int index = UnityEngine.Random.Range(0, cnt);
                    while ((map & (1 << index)) != 0)
                        index = UnityEngine.Random.Range(0, cnt);
                    res[index] = choices[i];
                    map = map | (1 << index);
                }
                shuffledChoices = res;
            }
            return shuffledChoices;
        }

        /// <summary>
        /// 清空打乱题目
        /// </summary>
        public void clearShuffleChoices() {
            shuffledChoices = null;
        }

        #endregion
    }

    /// <summary>
    /// 题目组数据
    /// </summary>
    public class GroupQuestion<T> : BaseData where T: BaseQuestion, new() {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string article { get; protected set; }
        [AutoConvert]
        public string source { get; protected set; }

        [AutoConvert]
        public T[] subQuestions { get; protected set; }
        
    }

    /// <summary>
    /// 题目数据
    /// </summary>
    public class Question : BaseQuestion {
        
        /// <summary>
        /// 题目详情
        /// </summary>
        public class Detail : BaseData,
            ParamDisplay.IDisplayDataConvertable {

            /// <summary>
            /// 过期秒数（30min)
            /// </summary>
            const int OutOfDateSeconds = 60 * 30;

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public int sumPlayer { get; protected set; }
            [AutoConvert]
            public int count { get; protected set; }
            [AutoConvert]
            public int sumCollect { get; protected set; }
            [AutoConvert]
            public int sumCount { get; protected set; }
            [AutoConvert]
            public double corrRate { get; protected set; }
            [AutoConvert]
            public double allCorrRate { get; protected set; }
            [AutoConvert]
            public int firstTime { get; protected set; }
            [AutoConvert]
            public int avgTime { get; protected set; }
            [AutoConvert]
            public int allAvgTime { get; protected set; }
            [AutoConvert]
            public DateTime firstDate { get; protected set; }
            [AutoConvert]
            public DateTime updateTime { get; protected set; }

            /// <summary>
            /// 数据转换
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public JsonData convertToDisplayData(string type = "") {
                return toJson();
            }

            /// <summary>
            /// 是否过期
            /// </summary>
            /// <returns></returns>
            public bool isOutOfDate() {
                return (DateTime.Now - updateTime).TotalSeconds >= OutOfDateSeconds;
            }
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
            public bool descPic { get; protected set; }
            [AutoConvert]
            public Texture2D data { get; protected set; }
        }

        /// <summary>
        /// 属性
        /// </summary>
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
        public int status { get; protected set; }

        [AutoConvert]
        public DateTime createTime { get; protected set; }
        
        [AutoConvert]
        public Picture[] pictures { get; protected set; }

        [AutoConvert]
        public Detail detail { get; protected set; } = null;

        /// <summary>
        /// 打乱的选项
        /// </summary>
        Choice[] _shuffleChoices { get; set; } = null;

        #region 数据转换

        /// <summary>
        /// 转化为显示数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>返回转化后的显示数据</returns>
        public override JsonData convertToDisplayData(string type = "") {
            switch (type) {
                case "detail": return convertDetailData();
                default: return toJson();
            }
        }

        /// <summary>
        /// 转化为详情数据
        /// </summary>
        /// <returns></returns>
        JsonData convertDetailData() {
            if (detail == null) return new JsonData();
            var res = detail.convertToDisplayData();

            res["number"] = number;
            res["subject"] = subject().name;
            res["type"] = typeText();
            res["score"] = score;
            res["source"] = source;
            res["create_time"] = DataLoader.convert(createTime);

            return res;
        }

        #endregion

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
        /// 状态文本
        /// </summary>
        /// <returns></returns>
        public string statusText() {
            return DataService.get().questionStatus(status).Item2;
        }

        /// <summary>
        /// 是否多选
        /// </summary>
        /// <returns>返回当前题目是否多选</returns>
        public bool isMultiple() {
            return type == (int)Type.Multiple;
        }

        /// <summary>
        /// 纹理数组
        /// </summary>
        /// <returns>返回纹理数组</returns>
        public Texture2D[] textures() {
            var res = new Texture2D[pictures.Length];
            for (int i = 0; i < pictures.Length; ++i)
                res[i] = pictures[i].data;
            return res;
        }

        /// <summary>
        /// 是否有详情数据
        /// </summary>
        /// <returns></returns>
        public bool hasDetail() {
            return detail != null && !detail.isOutOfDate();
        }

        #endregion
    }

    /// <summary>
    /// 有限物品数据
    /// </summary>
    public class QuesSugar : BaseItem, IEffectsConvertable {

        /// <summary>
        /// 效果描述文本格式定义
        /// </summary>
        const string EffectDescFormat = "当前回合属性 {0} 增加 {1} %";
        const string EffectShortDescFormat = "{0} +{1}%";

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

        /// <summary>
        /// 转化为效果数据数组（用于对战中显示）
        /// </summary>
        /// <returns>返回对应的效果数据数组</returns>
        public EffectData[] convertToEffectData() {
            var res = new List<EffectData>();
            foreach(var param in params_) {
                if (param.value == 0) continue;
                res.Add(generateEffectData(param));
            }
            return res.ToArray();
        }

        /// <summary>
        /// 生成效果数据
        /// </summary>
        /// <param name="param">效果</param>
        /// <returns>返回效果数据</returns>
        EffectData generateEffectData(ParamData param) {
            var code = EffectData.Code.BattleAddParam;
            var params_ = new JsonData();
            params_.SetJsonType(JsonType.Array);
            params_.Add(param.paramId); // p
            params_.Add(1); params_.Add(0); // t, a
            params_.Add(param.value); // b

            var baseParam = param.param();
            var value = Math.Round((param.value - 1) * 100);

            var desc = string.Format(
                EffectDescFormat, baseParam.name, value);
            var shortDesc = string.Format(
                EffectShortDescFormat, baseParam.name[0], value);

            return new EffectData(code, params_, desc, shortDesc);
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
}