using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using RecordModule.Data;

using QuestionModule.Services;

/// <summary>
/// 记录模块服务
/// </summary>
namespace RecordModule.Services {

    /// <summary>
    /// 记录服务
    /// </summary>
    public class RecordService : BaseService<RecordService> {

        /// <summary>
        /// 操作文本设定
        /// </summary>
        const string Get = "查询记录";

        const string QuestionCollect = "收藏/解除收藏题目";
        const string QuestionUnwrong = "解除错题";
        const string QuestionNote = "添加备注";

        const string ExerciseGenerate = "配置刷题";
        const string ExerciseStart = "开始答题";
        const string ExerciseAnswer = "上传答案";

        /// <summary>
        /// 记录数据
        /// </summary>
        public class RecordData : BaseData {

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public List<QuestionRecord> questionRecords { get; protected set; }
            [AutoConvert]
            public List<ExerciseRecord> exerciseRecords { get; protected set; }

            #region 数据操作

            #region 题目记录操作

            /// <summary>
            /// 获取题目记录
            /// </summary>
            /// <param name="qid">题目ID</param>
            /// <param name="create">若找不到是否新建一个记录</param>
            /// <returns>题目记录数据</returns>
            public QuestionRecord getQuestionRecord(int qid, bool create = false) {
                var res = questionRecords.Find((q) => q.questionId == qid);
                if (res == null && create)
                    res = createQuestionRecord(qid);
                return res;
            }

            /// <summary>
            /// 根据记录ID获取题目记录
            /// </summary>
            /// <param name="id">题目记录ID</param>
            /// <returns>题目记录数据</returns>
            public QuestionRecord getQuestionRecordById(int id) {
                return questionRecords.Find((q) => q.getID() == id);
            }

            /// <summary>
            /// 新建题目记录
            /// </summary>
            /// <param name="qid">题目ID</param>
            /// <returns>题目记录数据</returns>
            public QuestionRecord createQuestionRecord(int qid) {
                var rec = new QuestionRecord(qid);
                questionRecords.Add(rec);
                return rec;
            }

            /// <summary>
            /// 题目是否已收藏
            /// </summary>
            /// <param name="qid">题目ID</param>
            /// <returns></returns>
            public bool isQuestionCollected(int qid) {
                var rec = getQuestionRecord(qid);
                return rec != null && rec.collected;
            }

            /// <summary>
            /// 题目是否在错题本中
            /// </summary>
            /// <param name="qid">题目ID</param>
            /// <returns></returns>
            public bool isQuestionWrong(int qid) {
                var rec = getQuestionRecord(qid);
                return rec != null && rec.wrong;
            }

            /// <summary>
            /// 收藏/解除收藏题目
            /// </summary>
            /// <param name="qid">题目ID</param>
            public void collect(int qid) {
                var rec = getQuestionRecord(qid);
                rec.collect();
            }

            /// <summary>
            /// 设置错题标志
            /// </summary>
            /// <param name="qid">题目ID</param>
            /// <param name="val">值</param>
            public void setWrong(int qid, bool val) {
                var rec = getQuestionRecord(qid);
                rec.setWrong(val);
            }

            /// <summary>
            /// 设置错题标志
            /// </summary>
            /// <param name="qid">题目ID</param>
            /// <param name="note">备注</param>
            public void setNote(int qid, string note) {
                var rec = getQuestionRecord(qid);
                rec.setNote(note);
            }

            #endregion

            #region 刷题记录操作

            #endregion

            #endregion
            /*
            /// <summary>
            /// 数据加载
            /// </summary>
            /// <param name="json">数据</param>
            public override void load(JsonData json) {
                base.load(json);

                questionRecords = DataLoader.loadDataList<QuestionRecord>(json, "question_records");
                exerciseRecords = DataLoader.loadDataList<ExerciseRecord>(json, "exercise_records");
            }

            /// <summary>
            /// 获取JSON数据
            /// </summary>
            /// <returns>JsonData</returns>
            public override JsonData toJson() {
                var json = base.toJson();

                json["question_records"] = DataLoader.convertDataArray(questionRecords);
                json["exercise_records"] = DataLoader.convertDataArray(exerciseRecords);

                return json;
            }*/
        }

        /// <summary>
        /// 业务操作
        /// </summary>
        public enum Oper {
            Get,
            QuestionCollect, QuestionUnwrong, QuestionNote,
            ExerciseGenerate, ExerciseStart, ExerciseAnswer,
        }

        /// <summary>
        /// 状态（以刷题为例）
        /// </summary>
        public enum State {
            Closed, // 不在刷题场景
            Generated, // 题目已经生成完毕，准备开始答题
            Started, // 已经开始答题
            Answered, // 已经提交作答，等待下一题
            Terminated // 刷题完毕，退出刷题场景前的状态
        }
        /* 状态机：
         * Closed → 配置刷题、开始刷题 → Generated → 读取、下载题目 → Started
         *     ↑                                           开始新题目 ↑ ↓ 作答题目
         *   退出场景 ← Terminated ← 所有题目作答完毕或玩家终止答题 ← Answered
         */

        /// <summary>
        /// 当前题目集记录（刷题时用）
        /// </summary>
        public QuestionSetRecord currentRecord { get; protected set; } = null;

        /// <summary>
        /// 记录数据
        /// </summary>
        public RecordData recordData { get; protected set; }

        /// <summary>
        /// 外部系统
        /// </summary>
        QuestionService quesSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            quesSer = QuestionService.get();
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected override void initializeStateDict() {
            base.initializeStateDict();
            addStateDict(State.Closed);
            addStateDict(State.Generated);
            addStateDict(State.Started);
            addStateDict(State.Answered);
            addStateDict(State.Terminated);
        }

        /// <summary>
        /// 初始化操作字典
        /// </summary>
        protected override void initializeOperDict() {
            base.initializeOperDict();
            addOperDict(Oper.Get, Get, NetworkSystem.Interfaces.RecordGet);

            addOperDict(Oper.QuestionCollect, QuestionCollect,
                NetworkSystem.Interfaces.RecordQuestionCollect);
            addOperDict(Oper.QuestionUnwrong, QuestionUnwrong,
                NetworkSystem.Interfaces.RecordQeestionUnwrong);
            addOperDict(Oper.QuestionNote, QuestionNote,
                NetworkSystem.Interfaces.RecordQeestionNote);

            addOperDict(Oper.ExerciseGenerate, ExerciseGenerate,
                NetworkSystem.Interfaces.RecordExerciseGenerate);
            addOperDict(Oper.ExerciseStart, ExerciseStart,
                NetworkSystem.Interfaces.RecordExerciseStart);
            addOperDict(Oper.ExerciseAnswer, ExerciseAnswer,
                NetworkSystem.Interfaces.RecordExerciseAnswer);

        }

        /// <summary>
        /// 其他初始化工作
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            changeState(State.Closed);
        }

        #endregion

        #region 操作控制

        #region 题目记录操作

        /// <summary>
        /// 获取记录数据
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void get(UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                recordData = DataLoader.load<RecordData>(res);
                onSuccess?.Invoke();
            };
            JsonData data = new JsonData();
            sendRequest(Oper.Get, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 题目收藏/解除收藏
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void collect(int qid, UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                recordData.collect(qid);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData(); data["qid"] = qid;

            sendRequest(Oper.QuestionCollect, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 错题解除
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void unwrong(int qid, UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                recordData.setWrong(qid, false);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData(); data["qid"] = qid;

            sendRequest(Oper.QuestionUnwrong, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 备注
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <param name="note">备注内容</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void note(int qid, string note, UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                recordData.setNote(qid, note);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["qid"] = qid; data["note"] = note;

            sendRequest(Oper.QuestionNote, data, _onSuccess, onError, uid: true);
        }

        #endregion

        #region 刷题操作

        /// <summary>
        /// 生成刷题
        /// </summary>
        /// <param name="sid">科目</param>
        /// <param name="genType">刷题模式</param>
        /// <param name="count">题量</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void generateExercise(int sid, int genType, int count,
            UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess =
                onExerciseGeneratedFunc(onSuccess);

            JsonData data = new JsonData();
            data["sid"] = sid; data["gen_type"] = genType; data["count"] = count;

            sendRequest(Oper.ExerciseGenerate, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 刷题生成完毕回调
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        NetworkSystem.RequestObject.SuccessAction onExerciseGeneratedFunc(
            UnityAction onSuccess, UnityAction onError = null) {
            return (res) => {
                changeState(State.Generated);
                currentRecord = DataLoader.load(currentRecord, res, "record");
                var qids = currentRecord.getQuestionIds();
                quesSer.loadQuestions(qids, onSuccess, onError);
            };
        }

        /// <summary>
        /// 开始答题
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void startQuestion(int qid,
            UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                changeState(State.Started);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["qid"] = qid;

            sendRequest(Oper.ExerciseStart, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 回答题目
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <param name="selection">选择</param>
        /// <param name="timespan">用时（毫秒）</param>
        /// <param name="terminate">是否提交</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void answerQuestion(int qid, int[] selection, int timespan,
            bool terminate, UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (terminate) {
                    changeState(State.Terminated);
                    currentRecord = DataLoader.load(currentRecord, res, "record");
                } else
                    changeState(State.Answered);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["qid"] = qid;
            data["timespan"] = timespan; data["terminate"] = terminate;
            data["selection"] = DataLoader.convert(selection);

            sendRequest(Oper.ExerciseAnswer, data, _onSuccess, onError, uid: true);
        }

        #endregion

        #endregion
    }
}