
using System;

using Core.Data;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;

using QuestionModule.Data;
using QuestionModule.Services;

/// <summary>
/// 记录模块
/// </summary>
namespace RecordModule { }

/// <summary>
/// 记录模块数据
/// </summary>
namespace RecordModule.Data {

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
    }

    /// <summary>
    /// 题目集记录数据
    /// </summary>
    public class QuestionSetRecord : BaseData {

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
            public int[] selection { get; protected set; }
            [AutoConvert]
            public int timespan { get; protected set; }
            [AutoConvert]
            public int expIncr { get; protected set; }
            [AutoConvert]
            public int slotExpIncr { get; protected set; }
            [AutoConvert]
            public int goldIncr { get; protected set; }
            [AutoConvert]
            public bool isNew { get; protected set; }

            /// <summary>
            /// 获取题目对象
            /// </summary>
            /// <returns>返回题目对象</returns>
            public Question question() {
                return QuestionService.get().getQuestion(questionId);
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
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string name { get; protected set; }
        [AutoConvert]
        public int seasonId { get; protected set; }
        [AutoConvert]
        public int expIncr { get; protected set; }
        [AutoConvert]
        public int slotExpIncr { get; protected set; }
        [AutoConvert]
        public int goldIncr { get; protected set; }

        [AutoConvert]
        public bool finished { get; protected set; }

        [AutoConvert]
        public DateTime createTime { get; protected set; }

        [AutoConvert]
        public PlayerQuestion[] questions { get; protected set; }
        [AutoConvert]
        public Reward[] rewards { get; protected set; }

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
    }

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