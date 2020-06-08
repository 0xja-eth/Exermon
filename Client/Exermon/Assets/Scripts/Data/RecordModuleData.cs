
using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;

using QuestionModule.Data;
using QuestionModule.Services;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 记录模块
/// </summary>
namespace RecordModule { }

/// <summary>
/// 记录模块数据
/// </summary>
namespace RecordModule.Data {

    using Services;

    /// <summary>
    /// 题目记录数据
    /// </summary>
    public class QuestionRecord : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int questionId { get; protected set; }
        [AutoConvert]
        public int count { get; protected set; }
        [AutoConvert]
        public int correct { get; protected set; }
        [AutoConvert]
        public DateTime firstDate { get; protected set; }
        [AutoConvert]
        public DateTime lastDate { get; protected set; }
        [AutoConvert]
        public int firstTime { get; protected set; }
        [AutoConvert]
        public double avgTime { get; protected set; }
        [AutoConvert]
        public double corrTime { get; protected set; }
        [AutoConvert]
        public int sumExp { get; protected set; }
        [AutoConvert]
        public int sumGold { get; protected set; }
        [AutoConvert]
        public int source { get; protected set; }
        [AutoConvert]
        public bool collected { get; protected set; }
        [AutoConvert]
        public bool wrong { get; protected set; }
        [AutoConvert]
        public string note { get; protected set; }

        /// <summary>
        /// 题目缓存
        /// </summary>
        Question _question = null;

        #region 数据操作

        /// <summary>
        /// 获取题目对象
        /// </summary>
        /// <returns>返回题目对象</returns>
        public Question question() {
            if (_question != null) return _question;
            return _question = QuestionService.get().getQuestion(questionId);
        }

        /// <summary>
        /// 星级实例
        /// </summary>
        /// <returns></returns>
        public string sourceText() {
            return DataService.get().recordSource(source).Item2;
        }

        /// <summary>
        /// 收藏/解除收藏
        /// </summary>
        public void collect() {
            collected = !collected;
        }

        /// <summary>
        /// 设置错题标志
        /// </summary>
        /// <param name="val">值</param>
        public void setWrong(bool val) {
            wrong = val;
        }

        /// <summary>
        /// 设置备注
        /// </summary>
        /// <param name="note">备注</param>
        public void setNote(string note) {
            this.note = note;
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public QuestionRecord() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="qid">题目ID</param>
        public QuestionRecord(int qid) {
            questionId = qid;
        }
    }

    /// <summary>
    /// 玩家题目记录数据
    /// </summary>
    public interface IQuestionResult {

        /// <summary>
        /// 当前选择
        /// </summary>
        /// <returns>返回当前选择</returns>
        int[] getSelection();

        /// <summary>
        /// 做题用时（毫秒）
        /// </summary>
        /// <returns>返回做题用时</returns>
        int getTimespan();

        /// <summary>
        /// 是否作答
        /// </summary>
        /// <returns></returns>
        bool isAnswered();
    }

    /// <summary>
    /// 玩家题目记录数据
    /// </summary>
    public class PlayerQuestion : BaseData, IQuestionResult {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int questionId { get; protected set; }
        [AutoConvert]
        public int[] selection { get; protected set; } = null;
        [AutoConvert]
        public int timespan { get; protected set; }
        [AutoConvert]
        public int expIncr { get; protected set; }
        [AutoConvert]
        public int slotExpIncr { get; protected set; }
        [AutoConvert]
        public int goldIncr { get; protected set; }
        [AutoConvert]
        public bool correct { get; protected set; }
        [AutoConvert]
        public int score { get; protected set; }
        [AutoConvert]
        public bool isNew { get; protected set; }

        /// <summary>
        /// 题目缓存
        /// </summary>
        Question _question = null;

        #region 数据操作
        
        /// <summary>
        /// 获取题目对象
        /// </summary>
        /// <returns>返回题目对象</returns>
        public Question question() {
            if (_question != null) return _question;
            return _question = QuestionService.get().getQuestion(questionId);
        }

        /// <summary>
        /// 当前选择
        /// </summary>
        /// <returns>返回当前选择</returns>
        public int[] getSelection() { return selection; }

        /// <summary>
        /// 做题用时（毫秒）
        /// </summary>
        /// <returns>返回做题用时</returns>
        public int getTimespan() { return timespan; }

        /// <summary>
        /// 当前选择
        /// </summary>
        /// <returns>返回当前选择</returns>
        public bool isAnswered() { return selection != null; }

        #endregion
    }

    /// <summary>
    /// 题目集奖励数据
    /// </summary>
    public class QuestionSetReward : BaseData {

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int type { get; protected set; }
        [AutoConvert]
        public int itemId { get; protected set; }
        [AutoConvert]
        public int count { get; protected set; }

        /// <summary>
        /// 获取物品实例
        /// </summary>
        /// <returns></returns>
        public BaseItem item() {
            if (this.type == 0 || !Enum.IsDefined(
                typeof(BaseItem.Type), this.type)) return null;
            var name = ((BaseItem.Type)this.type).ToString();
            var type = Type.GetType(name);
            return (BaseItem)DataService.get().get(type, itemId);
        }
        public T item<T>() where T : BaseItem {
            if (this.type == 0 || !Enum.IsDefined(
                typeof(BaseItem.Type), this.type)) return null;
            var name = ((BaseItem.Type)this.type).ToString();
            var type = Type.GetType(name);
            return (T)DataService.get().get(type, itemId);
        }
    }

    /// <summary>
    /// 题目集记录数据
    /// </summary>
    public class QuestionSetRecord<Q, R> : BaseData,
        ParamDisplay.IDisplayDataConvertable, ParamDisplay.IDisplayDataArrayConvertable
        where Q : PlayerQuestion where R : QuestionSetReward {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string name { get; protected set; }
        [AutoConvert]
        public int seasonId { get; protected set; }
        [AutoConvert]
        public int goldIncr { get; protected set; }

        //[AutoConvert]
        public Dictionary<int, int> exerExpIncrs { get; protected set; } = new Dictionary<int, int>();
        //[AutoConvert]
        public Dictionary<int, int> slotExpIncrs { get; protected set; } = new Dictionary<int, int>();

        [AutoConvert]
        public bool finished { get; protected set; }

        [AutoConvert]
        public DateTime createTime { get; protected set; }

        [AutoConvert]
        public Q[] questions { get; protected set; } = null;
        [AutoConvert]
        public R[] rewards { get; protected set; } = null;

        #region 数据转换

        /// <summary>
        /// 转化为显示数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual JsonData convertToDisplayData(string type = "") {
            switch (type) {
                case "result": return convertResultData();
                default: return toJson();
            }
        }

        /// <summary>
        /// 转化为显示数据数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonData[] convertToDisplayDataArray(string type = "") {
            switch (type) {
                case "exer_exp": return generateExerExpDataArray();
                case "slot_exp": return generateSlotExpDataArray();
                default: return null;
            }
        }

        /// <summary>
        /// 转换为统计数据
        /// </summary>
        /// <returns></returns>
        protected virtual JsonData convertResultData() {
            var res = toJson();

            res["name"] = name;

            res["exp_incr"] = playerExpIncr();
            res["gold_incr"] = goldIncr;

            res["finished"] = finished;
            res["create_time"] = DataLoader.convert(createTime);

            res["count"] = questions.Length;

            res["sum_time"] = sumTime()/1000.0;

            res["sum_score"] = sumScore();
            res["max_score"] = maxScore();

            res["corr_cnt"] = corrCnt();
            res["corr_rate"] = corrRate();
            res["new_cnt"] = newCnt();
            res["new_rate"] = newRate();

            return res;
        }

        /// <summary>
        /// 生成经验数据数组
        /// </summary>
        protected virtual JsonData[] generateExerExpDataArray() {
            int cnt = exerExpIncrs.Count, index = 0;
            var res = new JsonData[cnt];

            foreach (var pair in exerExpIncrs) {
                var data = new JsonData();

                data["subject_id"] = pair.Key;
                data["exp_incr"] = pair.Value;

                res[index++] = data;
            }

            return res;
        }

        /// <summary>
        /// 生成槽经验数据数组
        /// </summary>
        protected virtual JsonData[] generateSlotExpDataArray() {
            int cnt = slotExpIncrs.Count, index = 0;
            var res = new JsonData[cnt];

            foreach (var pair in slotExpIncrs) {
                var data = new JsonData();

                data["subject_id"] = pair.Key;
                data["exp_incr"] = pair.Value;

                res[index++] = data;
            }

            return res;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取所有题目的ID
        /// </summary>
        /// <returns>题目ID集</returns>
        public int[] getQuestionIds() {
            var cnt = questions.Length;
            var ids = new int[cnt];
            for (int i = 0; i < cnt; ++i)
                ids[i] = questions[i].questionId;
            return ids;
        }

        /// <summary>
        /// 获取题目对象集
        /// </summary>
        /// <returns>返回题目集</returns>
        public Question[] getQuestions() {
            var cnt = this.questions.Length;
            var questions = new Question[cnt];
            for (int i = 0; i < cnt; ++i)
                questions[i] = this.questions[i].question();
            return questions;
        }

        /// <summary>
        /// 是否包含某题目
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <returns></returns>
        public bool hasQuestion(int qid) {
            var cnt = questions.Length;
            for (int i = 0; i < cnt; ++i)
                if (questions[i].questionId == qid)
                    return true;
            return false;
        }

        /// <summary>
        /// 获取题目对象集
        /// </summary>
        /// <returns>返回题目集</returns>
        public QuestionRecord[] getQuestionRecords() {
            var cnt = questions.Length;
            var records = new QuestionRecord[cnt];
            for (int i = 0; i < cnt; ++i)
                records[i] = RecordService.get().
                    recordData.getQuestionRecordById(
                    questions[i].questionId
                );
            return records;
        }

        /// <summary>
        /// 总用时
        /// </summary>
        /// <returns></returns>
        public int sumTime() {
            var res = 0;
            foreach (var ques in questions)
                res += ques.timespan;
            return res;
        }

        /// <summary>
        /// 总分数
        /// </summary>
        /// <returns></returns>
        public int sumScore() {
            var res = 0;
            foreach (var ques in questions)
                res += ques.score;
            return res;
        }

        /// <summary>
        /// 最大分数
        /// </summary>
        /// <returns></returns>
        public int maxScore() {
            var res = 0;
            foreach (var ques in questions)
                res += ques.question().score;
            return res;
        }

        /// <summary>
        /// 正确数
        /// </summary>
        /// <returns></returns>
        public int corrCnt() {
            var res = 0;
            foreach (var ques in questions)
                if (ques.correct) ++res;
            return res;
        }

        /// <summary>
        /// 新题目数
        /// </summary>
        /// <returns></returns>
        public int newCnt() {
            var res = 0;
            foreach (var ques in questions)
                if (ques.isNew) ++res;
            return res;
        }

        /// <summary>
        /// 正确率
        /// </summary>
        /// <returns></returns>
        public double corrRate() {
            return corrCnt()*1.0 / questions.Length;
        }

        /// <summary>
        /// 新题目率
        /// </summary>
        /// <returns></returns>
        public int newRate() {
            return newCnt() / questions.Length;
        }

        /// <summary>
        /// 玩家经验增量
        /// </summary>
        /// <returns>返回玩家经验获得量</returns>
        public int playerExpIncr() {
            var res = 0;
            foreach (var pair in slotExpIncrs)
                res += pair.Value;
            return res;
        }

        #endregion
        
        #region 数据读取

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);

            Debug.Log("loadCustomAttributes: " + json.ToJson());

            this.exerExpIncrs.Clear();
            this.slotExpIncrs.Clear();

            var exerExpIncrs = DataLoader.load(json, "exer_exp_incrs");
            var slotExpIncrs = DataLoader.load(json, "slot_exp_incrs");

            if (exerExpIncrs != null) {
                Debug.Log("Load exerExpIncrs: " + exerExpIncrs.ToJson());
                foreach (KeyValuePair<string, JsonData> pair in exerExpIncrs) {
                    var key = int.Parse(pair.Key);
                    var data = DataLoader.load<int>(pair.Value);
                    Debug.Log("Load exerExpIncrs: " + key + ", " + data);
                    this.exerExpIncrs.Add(key, data);
                }
            }
            if (slotExpIncrs != null) {
                Debug.Log("Load slotExpIncrs: " + slotExpIncrs.ToJson());
                foreach (KeyValuePair<string, JsonData> pair in slotExpIncrs) {
                    var key = int.Parse(pair.Key);
                    var data = DataLoader.load<int>(pair.Value);
                    Debug.Log("Load slotExpIncrs: " + key + ", " + data);
                    this.slotExpIncrs.Add(key, data);
                }
            }

        }

        /// <summary>
        /// 转化自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void convertCustomAttributes(ref JsonData json) {
            base.convertCustomAttributes(ref json);
            var exer = new JsonData();
            var slot = new JsonData();

            foreach (var pair in exerExpIncrs) {
                exer[pair.Key.ToString()] = pair.Value;
            }
            foreach (var pair in slotExpIncrs) {
                exer[pair.Key.ToString()] = pair.Value;
            }

            json["exer_exp_incrs"] = exer;
            json["slot_exp_incrs"] = slot;
        }

        #endregion

    }

    /// <summary>
    /// 题目集记录数据
    /// </summary>
    public class QuestionSetRecord : QuestionSetRecord
        <PlayerQuestion, QuestionSetReward> { }
    
    /// <summary>
    /// 刷题记录数据
    /// </summary>
    public class ExerciseRecord : QuestionSetRecord {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int subjectId { get; protected set; }
        [AutoConvert]
        public int genType { get; protected set; }
        [AutoConvert]
        public int count { get; protected set; }

        #region 数据转换

        /// <summary>
        /// 转换为统计数据
        /// </summary>
        /// <returns></returns>
        protected override JsonData convertResultData() {
            var res = base.convertResultData();

            Debug.Log("convertResultData: " + toJson().ToJson());
            Debug.Log(exerExpIncrs);
            Debug.Log(subjectId);

            res["subject"] = subject().name;
            res["mode"] = genTypeText();
            res["count"] = count;

            res["exer_exp_incr"] = exerExpIncrs[subjectId];
            res["slot_exp_incr"] = slotExpIncrs[subjectId];

            return res;
        }

        #endregion

        /// <summary>
        /// 科目实例
        /// </summary>
        /// <returns></returns>
        public Subject subject() {
            return DataService.get().subject(subjectId);
        }

        /// <summary>
        /// 刷题模式
        /// </summary>
        /// <returns></returns>
        public string genTypeText() {
            return DataService.get().exerciseGenType(genType).Item2;
        }
    }
}