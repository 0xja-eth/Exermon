﻿
using System.Collections.Generic;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using QuestionModule.Data;

/// <summary>
/// 题目模块服务
/// </summary>
namespace QuestionModule.Services {

    /// <summary>
    /// 题目服务
    /// </summary>
    public class QuestionService : BaseService<QuestionService> {

        /// <summary>
        /// 操作文本设定
        /// </summary>
        const string Get = "查询题目";

        const string GetReports = "查询反馈记录";
        const string PushReport = "提交反馈";

        const string GetDetail = "获取题目详情";

        /// <summary>
        /// 题目缓存
        /// </summary>
        public class QuestionCache : BaseData {

            /// <summary>
            /// 缓存的题目
            /// </summary>
            [AutoConvert]
            public List<Question> questions { get; protected set; } = new List<Question>();

            /// <summary>
            /// 添加题目
            /// </summary>
            /// <param name="json">题目数据</param>
            public void addQuestions(Question[] questions) {
                this.questions.AddRange(questions);
            }

            /// <summary>
            /// 读取自定义属性
            /// </summary>
            /// <param name="json">数据</param>
            protected override void loadCustomAttributes(JsonData json) {
                base.loadCustomAttributes(json);
                if (questions == null) questions = new List<Question>();
            }
        }

        /// <summary>
        /// 业务操作
        /// </summary>
        public enum Oper {
            Get,
            GetReports, PushReport,
            GetDetail,
        }

        /// <summary>
        /// 缓存题目数据
        /// </summary>
        public QuestionCache questionCache { get; protected set; } = new QuestionCache();

        /// <summary>
        /// 反馈记录
        /// </summary>
        public QuesReport[] quesReports { get; protected set; }

        /// <summary>
        /// 外部系统
        /// </summary>
        StorageSystem storageSys;

        #region 初始化

        /// <summary>
        /// 初始化操作字典
        /// </summary>
        protected override void initializeOperDict() {
            base.initializeOperDict();
            addOperDict(Oper.Get, Get, NetworkSystem.Interfaces.QuestionGet);

            addOperDict(Oper.GetReports, GetReports,
                NetworkSystem.Interfaces.QuestionReportGet);
            addOperDict(Oper.PushReport, PushReport,
                NetworkSystem.Interfaces.QuestionReportPush);

            addOperDict(Oper.GetDetail, GetDetail,
                NetworkSystem.Interfaces.QuestionDetailGet);
        }

        #endregion

        #region 操作控制

        /// <summary>
        /// 获取记录数据
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void get(int[] qids, UnityAction onSuccess = null, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var questions = DataLoader.load<Question[]>(res, "questions");
                questionCache.addQuestions(questions);
                onSuccess?.Invoke();
            };
            JsonData data = new JsonData();
            data["qids"] = DataLoader.convert(qids);
            sendRequest(Oper.Get, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取反馈记录
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getReports(UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                quesReports = DataLoader.load(quesReports, res, "reports");
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();

            sendRequest(Oper.GetReports, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 提交反馈
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <param name="type">类型</param>
        /// <param name="description">描述</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void pushReport(int qid, int type, string description, UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["qid"] = qid; data["type"] = type;
            data["description"] = description;

            sendRequest(Oper.PushReport, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取题目详情
        /// </summary>
        /// <param name="question">题目对象</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getDetail(Question question, UnityAction onSuccess, UnityAction onError = null) {
            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                DataLoader.load(question, res);
                onSuccess?.Invoke();
            };
            getDetail(question.id, _onSuccess, onError);
        }
        /// <param name="qid">题目ID</param>
        public void getDetail(int qid, 
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData(); data["qid"] = qid;
            sendRequest(Oper.GetDetail, data, onSuccess, onError, uid: true);
        }

        #endregion

        #region 题目操作

        /// <summary>
        /// 读取题目（先判断缓存）
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void loadQuestion(int qid, UnityAction onSuccess = null, UnityAction onError = null) {
            loadQuestions(new int[] { qid }, onSuccess, onError);
        }
        /// <param name="qids">题目ID集</param>
        public void loadQuestions(int[] qids, UnityAction onSuccess = null, UnityAction onError = null) {
            var cnt = qids.Length;
            var reqIds = new List<int>(); // 需要请求的题目ID数组
            for (int i = 0; i < cnt; ++i)
                // 如果没有缓存
                if (!isQuestionCached(qids[i])) reqIds.Add(qids[i]);

            if (reqIds.Count > 0) // 如果需要请求
                get(reqIds.ToArray(), onSuccess, onError);
            else onSuccess?.Invoke();
        }

        /// <summary>
        /// 读取题目详情（先判断缓存）
        /// </summary>
        /// <param name="question">题目对象</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void loadQuestionDetail(Question question, UnityAction onSuccess = null, UnityAction onError = null) {
            if (question.hasDetail()) onSuccess?.Invoke();
            else getDetail(question, onSuccess, onError);
        }
        /// <param name="qid">题目ID</param>
        public void loadQuestionDetail(int qid, UnityAction onSuccess = null, UnityAction onError = null) {
            loadQuestion(qid, () => {
                var question = getQuestion(qid);
                loadQuestionDetail(question, onSuccess, onError);
            }, onError);
        }

        /// <summary>
        /// 题目是否已缓存
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <returns>是否缓存</returns>
        public bool isQuestionCached(int qid) {
            return questionCache.questions.Exists(q => q.id == qid);
        }

        /// <summary>
        /// 是否存在详情
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <returns>返回是否有题目详情缓存</returns>
        public bool isQuestionDetailCached(int qid) {
            var question = getQuestion(qid);
            return question != null && question.hasDetail();
        }

        /// <summary>
        /// 获取单个题目（需确保已经缓存）
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <returns>题目实例</returns>
        public Question getQuestion(int qid) {
            return questionCache.questions.Find(q => q.id == qid);
        }

        /// <summary>
        /// 获取单个题目（需确保已经缓存）
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <returns>题目实例</returns>
        public Question.Detail getQuestionDetail(int qid) {
            var question = getQuestion(qid);
            if (question == null) return null;
            return question.detail;
        }

        #endregion

    }
}