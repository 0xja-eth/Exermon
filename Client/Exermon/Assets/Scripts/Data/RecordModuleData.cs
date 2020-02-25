
using System;

using LitJson;

using UnityEditor;

/// <summary>
/// 题目记录数据
/// </summary>
public class QuestionRecord : BaseData {
    
    /// <summary>
    /// 属性
    /// </summary>
    public int questionId { get; private set; }
    public int count { get; private set; }
    public int correct { get; private set; }
    public DateTime firstDate { get; private set; }
    public DateTime lastDate { get; private set; }
    public int firstTime { get; private set; }
    public double avgTime { get; private set; }
    public double corrTime { get; private set; }
    public int sumExp { get; private set; }
    public int sumGold { get; private set; }
    public int source { get; private set; }
    public bool collected { get; private set; }
    public bool wrong { get; private set; }
    public string note { get; private set; }

    #region 数据操作

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

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        questionId = DataLoader.loadInt(json, "question_id");
        count = DataLoader.loadInt(json, "count");
        correct = DataLoader.loadInt(json, "correct");
        firstDate = DataLoader.loadDateTime(json, "first_date");
        lastDate = DataLoader.loadDateTime(json, "last_date");
        firstTime = DataLoader.loadInt(json, "first_time");
        avgTime = DataLoader.loadDouble(json, "avg_time");
        corrTime = DataLoader.loadDouble(json, "corr_time");
        sumExp = DataLoader.loadInt(json, "sum_exp");
        sumGold = DataLoader.loadInt(json, "sum_gold");
        source = DataLoader.loadInt(json, "source");
        collected = DataLoader.loadBool(json, "collected");
        wrong = DataLoader.loadBool(json, "wrong");
        note = DataLoader.loadString(json, "note");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["question_id"] = questionId;
        json["count"] = count;
        json["correct"] = correct;
        json["first_date"] = DataLoader.convertDateTime(firstDate);
        json["last_date"] = DataLoader.convertDateTime(lastDate);
        json["first_time"] = firstTime;
        json["avg_time"] = avgTime;
        json["corr_time"] = corrTime;
        json["sum_exp"] = sumExp;
        json["sum_gold"] = sumGold;
        json["source"] = source;
        json["collected"] = collected;
        json["wrong"] = wrong;
        json["note"] = note;

        return json;
    }
}

/// <summary>
/// 题目集记录数据
/// </summary>
public class QuestionSetRecord : BaseData {

    /// <summary>
    /// 玩家题目记录数据
    /// </summary>
    public class PlayerQuestion : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        public int questionId { get; private set; }
        public int[] selection { get; private set; }
        public int timespan { get; private set; }
        public int expIncr { get; private set; }
        public int slotExpIncr { get; private set; }
        public int goldIncr { get; private set; }
        public bool isNew { get; private set; }

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        public override void load(JsonData json) {
            base.load(json);

            questionId = DataLoader.loadInt(json, "question_id");
            selection = DataLoader.loadIntArray(json, "selection");
            timespan = DataLoader.loadInt(json, "timespan");
            expIncr = DataLoader.loadInt(json, "exp_incr");
            slotExpIncr = DataLoader.loadInt(json, "slot_exp_incr");
            goldIncr = DataLoader.loadInt(json, "gold_incr");
            isNew = DataLoader.loadBool(json, "is_new");
        }

        /// <summary>
        /// 获取JSON数据
        /// </summary>
        /// <returns>JsonData</returns>
        public override JsonData toJson() {
            var json = base.toJson();

            json["question_id"] = questionId;

            json["selection"] = DataLoader.convertArray(selection);
            json["timespan"] = timespan;
            json["exp_incr"] = expIncr;
            json["slot_exp_incr"] = slotExpIncr;
            json["gold_incr"] = goldIncr;
            json["is_new"] = isNew;

            return json;
        }
    }

    /// <summary>
    /// 题目集奖励数据
    /// </summary>
    public class Reward : BaseData {

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        /// <summary>
        /// 属性
        /// </summary>
        public int type { get; private set; }
        public int itemId { get; private set; }
        public int count { get; private set; }

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

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        public override void load(JsonData json) {
            base.load(json);

            type = DataLoader.loadInt(json, "type");
            itemId = DataLoader.loadInt(json, "item_id");
            count = DataLoader.loadInt(json, "count");
        }

        /// <summary>
        /// 获取JSON数据
        /// </summary>
        /// <returns>JsonData</returns>
        public override JsonData toJson() {
            var json = base.toJson();

            json["type"] = type;
            json["item_id"] = itemId;
            json["count"] = count;

            return json;
        }
    }

    /// <summary>
    /// 属性
    /// </summary>
    public string name { get; private set; }
    public int seasonId { get; private set; }
    public int expIncr { get; private set; }
    public int slotExpIncr { get; private set; }
    public int goldIncr { get; private set; }

    public bool finished { get; private set; }

    public DateTime createTime { get; private set; }

    public PlayerQuestion[] questions { get; private set; }
    public Reward[] rewards { get; private set; }

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
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        name = DataLoader.loadString(json, "name");
        seasonId = DataLoader.loadInt(json, "season_id");
        expIncr = DataLoader.loadInt(json, "exp_incr");
        slotExpIncr = DataLoader.loadInt(json, "slot_exp_incr");
        goldIncr = DataLoader.loadInt(json, "gold_incr");

        finished = DataLoader.loadBool(json, "finished");

        createTime = DataLoader.loadDateTime(json, "create_time");

        questions = DataLoader.loadDataArray<PlayerQuestion>(json, "questions");
        rewards = DataLoader.loadDataArray<Reward>(json, "rewards");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["name"] = name;
        json["season_id"] = seasonId;
        json["exp_incr"] = expIncr;
        json["slot_exp_incr"] = slotExpIncr;
        json["gold_incr"] = goldIncr;
        json["finished"] = finished;

        json["create_time"] = DataLoader.convertDateTime(createTime);

        json["questions"] = DataLoader.convertDataArray(questions);
        json["rewards"] = DataLoader.convertDataArray(rewards);

        return json;
    }
}

/// <summary>
/// 刷题记录数据
/// </summary>
public class ExerciseRecord : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    public int subjectId { get; private set; }
    public int genType { get; private set; }
    public int count { get; private set; }

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

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        subjectId = DataLoader.loadInt(json, "subject_id");
        genType = DataLoader.loadInt(json, "gen_type");
        count = DataLoader.loadInt(json, "count");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["subject_id"] = subjectId;
        json["gen_type"] = genType;
        json["count"] = count;

        return json;
    }
}
